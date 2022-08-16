using System;
using System.Drawing;
using System.Windows.Forms;

namespace MorphClocks {
  public partial class SettingsForm : Form {
    
    public SettingsForm() {
      
      InitializeComponent();

      btnFont.Text = AppSettings.Instance.FontName;
      btnTextColor.BackColor = AppSettings.Instance.TextColor;
      btnBackColor.BackColor = AppSettings.Instance.BackColor;
      btnLinesColor.BackColor = AppSettings.Instance.LineColor;

      cbxMixPoint.Checked = AppSettings.Instance.MixPoint;
      cbxMove3D.Checked = AppSettings.Instance.Move3D;
      cbxDrawCircle.Checked = AppSettings.Instance.DrawCircle;
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      Close();
    }

    private void btnOk_Click(object sender, EventArgs e) {
      AppSettings.Instance.MixPoint = cbxMixPoint.Checked;
      AppSettings.Instance.Move3D = cbxMove3D.Checked;
      AppSettings.Instance.DrawCircle = cbxDrawCircle.Checked;
      AppSettings.Instance.Save();
      Close();
    }

    private void btnTextColor_Click(object sender, EventArgs e) {
      var dlg = new ColorDialog {Color = AppSettings.Instance.TextColor};
      if (dlg.ShowDialog() != DialogResult.OK) return;
      btnTextColor.BackColor = dlg.Color;
      AppSettings.Instance.TextColor = dlg.Color;
    }

    private void btnBackColor_Click(object sender, EventArgs e) {
      var dlg = new ColorDialog {Color = AppSettings.Instance.BackColor};
      if (dlg.ShowDialog() != DialogResult.OK) return;
      btnBackColor.BackColor = dlg.Color;
      AppSettings.Instance.BackColor = dlg.Color;
    }

    private void btnLinesColor_Click(object sender, EventArgs e) {
      var dlg = new ColorDialog {Color = AppSettings.Instance.LineColor};
      if (dlg.ShowDialog() != DialogResult.OK) return;
      btnLinesColor.BackColor = dlg.Color;
      AppSettings.Instance.LineColor = dlg.Color;
    }

    private void btnFont_Click(object sender, EventArgs e) {
      var dlg = new FontDialog {Font = new Font(btnFont.Text, 8)};
      if (dlg.ShowDialog() != DialogResult.OK) return;
      btnFont.Text = dlg.Font.Name;
      AppSettings.Instance.FontName = dlg.Font.Name;
    }
  }
}