using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MorphClocks
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //save settings
            Close();
        }

        private void btnTextColor_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            btnTextColor.BackColor = dlg.Color;
        }

        private void btnLinesColor_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            btnLinesColor.BackColor = dlg.Color;
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            var dlg = new FontDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            btnFont.Text = dlg.Font.Name;
        }
    }
}
