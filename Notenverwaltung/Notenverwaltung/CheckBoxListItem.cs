namespace Notenverwaltung
{
    /// <summary>
    /// Dient der Darstellung einer Checkbox in einer Listbox.
    /// </summary>
    public class CheckBoxListItem
    {
        public string Text { get; set; }

        public bool Checked { get; set; }

        public CheckBoxListItem(string text, bool check)
        {
            Text = text;
            Checked = check;
        }
    }
}
