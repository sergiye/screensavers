using System;
using System.Windows.Forms;

namespace MorphClocks
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            btnFont.Text = AppSettings.Instance.FontName;
            btnTextColor.BackColor = AppSettings.Instance.TextColor;
            btnLinesColor.BackColor = AppSettings.Instance.LineColor;

            numBackTimer.Value = AppSettings.Instance.WorkEnd;
            cbxMixPoint.Checked = AppSettings.Instance.MixPoint;
            cbxMove3D.Checked = AppSettings.Instance.Move3D;
            cbxBackTimer.Checked = AppSettings.Instance.BackTimer;
            cbxDrawCircle.Checked = AppSettings.Instance.DrawCircle;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            AppSettings.Instance.WorkEnd = (int) numBackTimer.Value;
            AppSettings.Instance.MixPoint = cbxMixPoint.Checked;
            AppSettings.Instance.Move3D = cbxMove3D.Checked;
            AppSettings.Instance.BackTimer = cbxBackTimer.Checked;
            AppSettings.Instance.DrawCircle = cbxDrawCircle.Checked;
            AppSettings.Instance.Save();
            Close();
        }

        private void btnTextColor_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            btnTextColor.BackColor = dlg.Color;
            AppSettings.Instance.TextColor = btnTextColor.BackColor;
        }

        private void btnLinesColor_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            btnLinesColor.BackColor = dlg.Color;
            AppSettings.Instance.LineColor = btnLinesColor.BackColor;
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            var dlg = new FontDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            btnFont.Text = dlg.Font.Name;
            AppSettings.Instance.FontName = btnFont.Text;
        }
    }
}
