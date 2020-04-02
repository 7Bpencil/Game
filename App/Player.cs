namespace App
{
    public class Player: ISubject
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

    public class Environment: ISubject
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

    public class Enemy: ISubject
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