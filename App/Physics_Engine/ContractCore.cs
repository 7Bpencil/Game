using System;
using System.Windows.Forms;

namespace App.Physics_Engine
{
    public abstract class ContractCore
    {
        protected abstract void GameLoop(object sender, EventArgs args);
        protected abstract void UpdateObjects();
        protected abstract void UpdatePlayer();
    }
}