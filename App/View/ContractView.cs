using System.Windows.Forms;
using App.Engine;

namespace App.View
{
    public abstract class ContractView : Form
    {
        public abstract void Render();
        protected abstract ContractCore EngineCore { get; set; }
    }
}