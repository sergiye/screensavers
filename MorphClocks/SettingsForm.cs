using System;
using System.Drawing;
using System.Windows.Forms;

namespace MorphClocks {
  public partial class SettingsForm : Form {

    private readonly int transparentColor = Color.Transparent.ToArgb();

    public SettingsForm() {
      
      InitializeComponent();
      
      btnFont.Text = $"{AppSettings.Instance.FontName} / {AppSettings.Instance.FontSize}";

      btnTextColor.BackColor = AppSettings.Instance.TextColor;
      cbxTextColor.CheckedChanged += (s, e) => {
        btnTextColor.Enabled = cbxTextColor.Checked;
        if (!cbxTextColor.Checked)
          btnTextColor.BackColor = AppSettings.Instance.TextColor = Color.Transparent;
      };
      btnTextColor.Enabled = cbxTextColor.Checked = !IsTransparent(AppSettings.Instance.TextColor);

      btnBackColor.BackColor = AppSettings.Instance.BackColor;
      cbxBackColor.CheckedChanged += (s, e) => { 
        btnBackColor.Enabled = cbxBackColor.Checked;
        if (!cbxBackColor.Checked)
          btnBackColor.BackColor = AppSettings.Instance.BackColor = Color.Transparent;
      };
      btnBackColor.Enabled = cbxBackColor.Checked = !IsTransparent(AppSettings.Instance.BackColor);

      btnLinesColor.BackColor = AppSettings.Instance.LineColor;

      cbxMixPoint.Checked = AppSettings.Instance.MixPoint;
      cbxMove3D.Checked = AppSettings.Instance.Move3D;
      cbxDrawCircle.Checked = AppSettings.Instance.DrawCircle;
      cbxLockOnExit.Checked = AppSettings.Instance.LockOnExit;
    }

    private bool IsTransparent(Color color) {
      return color.ToArgb() == transparentColor;
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      Close();
    }

    private void btnOk_Click(object sender, EventArgs e) {
      AppSettings.Instance.MixPoint = cbxMixPoint.Checked;
      AppSettings.Instance.Move3D = cbxMove3D.Checked;
      AppSettings.Instance.DrawCircle = cbxDrawCircle.Checked;
      AppSettings.Instance.LockOnExit = cbxLockOnExit.Checked;
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
      var dlg = new FontDialog {Font = new Font(AppSettings.Instance.FontName, AppSettings.Instance.FontSize)};
      if (dlg.ShowDialog() != DialogResult.OK) return;
      AppSettings.Instance.FontName = dlg.Font.Name;
      AppSettings.Instance.FontSize = dlg.Font.Size;
      btnFont.Text = $"{AppSettings.Instance.FontName} / {AppSettings.Instance.FontSize}";
    }
  }
}