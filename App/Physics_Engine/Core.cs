namespace App.Physics_Engine
{
    public class Core
    {
        private float currentTime;
        private float elapsedTime;
        private float prviousTime;
        private float lagTime;
        private float FPS;

        private float frameTime => 1 / FPS;
        private float MPF => 1000 * frameTime;
    }
}