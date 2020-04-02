namespace App
{
    public interface ISubject
    {
        int HitPoint { get; set; }
        string View { get; }
        void Destructibility();
        void ShowView();
    }
}