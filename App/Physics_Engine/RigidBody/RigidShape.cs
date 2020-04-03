namespace App.Physics_Engine.RigidBody
{
    public class RigidShape
    {
        public Vector center { get; set; }
        public float angle { get; set; }

        public RigidShape(Vector center)
        {
            this.center = center;
            angle = 0;
        }
    }
}