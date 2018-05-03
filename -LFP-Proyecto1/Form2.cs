using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _LFP_Proyecto1
{
    public partial class Seleccion : Form
    {
        List<tab> Tabs = new List<tab>();

        public Seleccion(List<tab> Tabs)
        {
            InitializeComponent();
            this.Tabs = Tabs;
            Llenar();
        }

        public int Sel(string tab)
        {
            if(tab == "a")
            {
                tab tabSelA = Tabs.Find(x => x.nomTab.Contains(tabA.SelectedText));
                return tabSelA.numTab;
            }
            else
            {
                tab tabSelB = Tabs.Find(x => x.nomTab.Contains(tabB.SelectedText));
                return tabSelB.numTab;
            }
        }

        public void Llenar()
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                tabA.Items.Add(Tabs[i].nomTab);
                tabB.Items.Add(Tabs[i].nomTab);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
