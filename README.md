# Notenverwaltung

Programm zur systematischen Speicherung von Noten.


## Features

 - Einfügen von (ein oder mehreren) PDF Dateien (-> File Watcher, Naming Patterns)
 - Einfügen von zusätzlichen Dateien (z.B. MP3-Dateien, Videos, Musescore, ...)
 - Kategorie, Instrument, Lied hinzufügen/bearbeiten/löschen
 - Ausgabe/Ausdruck von PDF Dateien (z.B. für ein Instrument inkl. Ausweich-Instrumenten, alle Noten eines Liedes außer Stimme XY, ...)
 - Ansehen von (Datei-) Listen (z.B. alle zusätzlichen Dateien/Stimmen eines Liedes, alle Stimmen, alle Lieder, alle Kategorien)
 - Suchfunktion für Lied mit Eingrenzung auf Kategorie
 - Standardbesetzung für ein Lied prüfen
 - Liednummerierung für alle Ordner


## Ordnerstruktur

Es gibt neben dem Programmverzeichnis ein weiteres, in welchem die PDFs und weitere Dateien liegen.
Der Pfad zu diesem Ordner wird im Programmverzeichnis hinterlegt.
Dieser Ordner ist wie folgt aufgebaut:

 - `Musikstücke`
    - `{TITEL} [# {KOMPONIST} [# {ARRANGEUR} ] ]`
    	- `{INSTRUMENT}#{STIMMUNG}[#{NUMMER}].pdf`
        - `Meta.xml`: Kategorien, Besetzung des Stücks, gibt Ausweichstimmen an
        - `Zusätzliche Dateien`: Enthält alle zusätzlichen Dateien zu entsprechendem Stück
 - `Einstellungen`
    - `Besetzungen.xml`: Enthält z.B, Besetzungen von Großem Orchester, Jugendgruppe, Quintett
    - `Kategorien.xml`: Liste aller Kategorien
    - `Mappen.xml`: Enthält Dictionaries mit Nummer und Musikstück für jede Mappe
    - `NamePatterns.xml`: Enthält weitere Namepatterns für Ordner und PDFs
    - `Aufgaben.xml`: Speichert offene Aufgaben
