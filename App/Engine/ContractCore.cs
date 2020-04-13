using System;
using System.Collections.Generic;
using System.Windows.Forms;
using App.Engine.PhysicsEngine;
using App.Engine.PhysicsEngine.Collision;
using App.Engine.PhysicsEngine.RigidBody;

namespace App.Engine
{
    public abstract class ContractCore
    {
        public abstract void GameLoop(object sender, EventArgs args);
        public abstract void OnMouseMove(Vector newPosition);
        public abstract void OnKeyDown(Keys keyPressed);
        public abstract void OnKeyUp(Keys keyPressed);
        public abstract List<RigidShape> GetSceneObjects();
        public abstract List<CollisionInfo> GetCollisions();
    }
}