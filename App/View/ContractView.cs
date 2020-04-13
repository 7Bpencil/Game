using System.Windows.Forms;

namespace App.View
{
    public abstract class ContractView : Form
    {
        public abstract void Render();
        public abstract void PrintDebugInfo(string[] messages);
    }
}