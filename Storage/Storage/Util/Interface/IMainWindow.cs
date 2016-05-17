namespace Storage.Util.Interface
{
    public interface IMainWindow
    {
        object DataContext { get; set; }
        void Show();
    }
}
