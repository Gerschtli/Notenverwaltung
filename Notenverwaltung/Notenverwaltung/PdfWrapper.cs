using System;
using System.IO;
using System.Windows;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Notenverwaltung
{
    enum PageFormat
    {
        Unknown,
        A4Portrait,
        A4Landscape,
        A5Portrait,
        A5Landscape
    }

    public class PdfWrapper        // todo: Nummer auf Seiten schreiben
    {

        #region Variablen

        private static XSize _A4Size = PageSizeConverter.ToSize(PageSize.A4);
        private static XSize _A5Size = PageSizeConverter.ToSize(PageSize.A5);

        private string _tmpPath = Path.GetTempFileName();

        #endregion

        #region Methoden

        /// <summary>
        /// Fügt eine Liste von PDF Dateien zusammen. Der Parameter scale legt einen von zwei Modi wie folgt fest:
        /// scale == true: Alle Seiten werden auf A4 abgebildet.
        ///   A5 Portrait  -> auf A4 Portrait hochskalieren
        ///   A5 Landscape -> drehen und auf A4 Portrait hochskalieren
        /// scale == false: Alle Seiten bleiben in Originalgröße.
        ///   2x A5        -> wenn nötig drehen und auf A4 Portrait legen
        ///   A5 Portrait  -> drehen und auf A4 Portrait legen
        ///   A5 Landscape -> auf A4 Portrait legen
        /// </summary>
        /// <param name="sourcePdfPaths">Liste der zusammenzufügenden PDFs</param>
        /// <param name="outputPdfPath">Pfad zur Output-Datei</param>
        /// <param name="scale">Legt wie beschrieben fest, ob hochskaliert werden soll.</param>
        public void Merge(string[] sourcePdfPaths, string outputPdfPath, bool scale)
        {
            try
            {
                _MergeAndRotate(sourcePdfPaths, scale);

                _CombineAndScale(outputPdfPath, scale);

                File.Delete(_tmpPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Extrahiert aus gegebener PDF-Datei weitere PDFs, die die als Pattern angegebene Anzahl Seiten besitzen.
        /// Die Seiten werden durchnummeriert gespeichert.
        /// </summary>
        /// <param name="sourcePdfPath">Pfad zur Quell-PDF-Datei</param>
        /// <param name="outputPdfPath">Pfad zu einem Ordner, in welchem die neuen PDFs gespeichert werden sollen (ohne Backslash am Ende).</param>
        /// <param name="pattern">Anzahl Seiten pro Datei</param>
        public void ExtractFiles(string sourcePdfPath, string outputPdfPath, int pattern)
        {
            try
            {
                PdfDocument inputDocument = PdfReader.Open(sourcePdfPath, PdfDocumentOpenMode.Import);
                PdfDocument outputDocument = null;
                int j = 1;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i % pattern == 0)
                        outputDocument = new PdfDocument();

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i % pattern == pattern - 1 || i == inputDocument.PageCount - 1)
                    {
                        outputDocument.Save(String.Format("{0}\\{1}.pdf", outputPdfPath, j++));
                        outputDocument.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Hilfsfunktionen

        /// <summary>
        /// Gibt das entsprechende Element aus der Enumeration PageFormat zurück.
        /// </summary>
        private PageFormat _GetPageFormat(double width, double height)
        {
            /*
             * A4Portrait   -> height = a4.height, width = a4.width
             * A4Landscape  -> height = a4.width , width = a4.height
             * A5Portrait   -> height = a5.height, width = a5.width
             * A5Landscape  -> height = a5.width , width = a5.height
             */

            if (height == _A4Size.Height && width == _A4Size.Width)
                return PageFormat.A4Portrait;
            else if (height == _A4Size.Width && width == _A4Size.Height)
                return PageFormat.A4Landscape;
            else if (height == _A5Size.Height && width == _A5Size.Width)
                return PageFormat.A5Portrait;
            else if (height == _A5Size.Width && width == _A5Size.Height)
                return PageFormat.A5Landscape;

            return PageFormat.Unknown;
        }

        /// <summary>
        /// Übernimmt das Zusammenfügen und Rotieren, je nach gewähltem Modus (scale).
        /// </summary>
        private void _MergeAndRotate(string[] sourcePdfPaths, bool scale)
        {
            PdfDocument outputDocument = new PdfDocument();
            PdfDocument inputDocument;
            PageFormat pf;
            PdfPage page;

            foreach (string path in sourcePdfPaths)
            {
                inputDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import);

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    page = inputDocument.Pages[i];
                    pf = _GetPageFormat(page.Width, page.Height);

                    if (pf == PageFormat.Unknown)
                        continue;

                    if (pf == PageFormat.A4Landscape || scale && pf == PageFormat.A5Landscape)
                    {
                        page.Rotate = (page.Rotate - 90) % 360;
                    }
                    else if (!scale && pf == PageFormat.A5Portrait)
                    {
                        page.Rotate = (page.Rotate + 90) % 360;
                    }

                    outputDocument.AddPage(page);
                }
            }
            outputDocument.Save(_tmpPath);
        }

        /// <summary>
        /// Übernimmt das Kominieren von je zwei A5 Seiten zu einer A4 Seite und das Skalieren, je nach gewähltem Modus (scale).
        /// </summary>
        private void _CombineAndScale(string outputPdfPath, bool scale)
        {
            XPdfForm form = XPdfForm.FromFile(_tmpPath);
            XGraphics gfx;
            XRect rect;
            PdfPage page;
            PageFormat pf;
            PdfDocument outputDocument = new PdfDocument();

            for (int i = 0; i < form.PageCount; i++)
            {
                page = outputDocument.Pages.Add();
                page.Size = PageSize.A4;
                form.PageNumber = i + 1;

                gfx = XGraphics.FromPdfPage(page);

                if (!scale)
                {
                    pf = _GetPageFormat(form.PointWidth, form.PointHeight);
                    Console.WriteLine(pf);

                    if (pf == PageFormat.A5Landscape)
                    {
                        rect = new XRect(0, 0, _A4Size.Width, _A4Size.Height / 2);
                        gfx.DrawImage(form, rect);

                        if (i + 1 < form.PageCount)
                        {
                            form.PageNumber = i + 2;
                            pf = _GetPageFormat(form.PointWidth, form.PointHeight);

                            if (pf == PageFormat.A5Landscape)
                            {
                                rect = new XRect(0, _A4Size.Height / 2, _A4Size.Width, _A4Size.Height / 2);
                                gfx.DrawImage(form, rect);
                                i++;
                            }
                        }

                        continue;
                    }
                }

                rect = new XRect(0, 0, _A4Size.Width, _A4Size.Height);

                gfx.DrawImage(form, rect);
            }

            outputDocument.Save(outputPdfPath);
        }

        #endregion

    }
}
