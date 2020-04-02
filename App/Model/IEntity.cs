namespace App.Model
{
    public interface IEntity
    {
        int HitPoint { get; set; }
        string View { get; }
        void Destructibility();
        void ShowView();
    }
}