namespace App.Model
{
    public class Player : IEntity
    {
        public int HitPoint { get; set; }
        public string View { get; }

        public void Destructibility()
        {
            throw new System.NotImplementedException();
        }

        public void ShowView()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Environment : IEntity
    {
        public int HitPoint { get; set; }
        public string View { get; }

        public void Destructibility()
        {
            throw new System.NotImplementedException();
        }

        public void ShowView()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Enemy : IEntity
    {
        public int HitPoint { get; set; }
        public string View { get; }

        public void Destructibility()
        {
            throw new System.NotImplementedException();
        }

        public void ShowView()
        {
            throw new System.NotImplementedException();
        }
    }
}