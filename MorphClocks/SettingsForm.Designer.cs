namespace MorphClocks
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panCommon = new System.Windows.Forms.Panel();
            this.cbxDrawCircle = new System.Windows.Forms.CheckBox();
            this.cbxBackTimer = new System.Windows.Forms.CheckBox();
            this.lblBackTimer = new System.Windows.Forms.Label();
            this.btnLinesColor = new System.Windows.Forms.Button();
            this.lblLinesColor = new System.Windows.Forms.Label();
            this.btnTextColor = new System.Windows.Forms.Button();
            this.lblTextColor = new System.Windows.Forms.Label();
            this.btnFont = new System.Windows.Forms.Button();
            this.lblFont = new System.Windows.Forms.Label();
            this.cbxMove3D = new System.Windows.Forms.CheckBox();
            this.numBackTimer = new System.Windows.Forms.NumericUpDown();
            this.cbxMixPoint = new System.Windows.Forms.CheckBox();
            this.panControls = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBackColor = new System.Windows.Forms.Button();
            this.lblBackColor = new System.Windows.Forms.Label();
            this.panCommon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBackTimer)).BeginInit();
            this.panControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // panCommon
            // 
            this.panCommon.Controls.Add(this.btnBackColor);
            this.panCommon.Controls.Add(this.lblBackColor);
            this.panCommon.Controls.Add(this.cbxDrawCircle);
            this.panCommon.Controls.Add(this.cbxBackTimer);
            this.panCommon.Controls.Add(this.lblBackTimer);
            this.panCommon.Controls.Add(this.btnLinesColor);
            this.panCommon.Controls.Add(this.lblLinesColor);
            this.panCommon.Controls.Add(this.btnTextColor);
            this.panCommon.Controls.Add(this.lblTextColor);
            this.panCommon.Controls.Add(this.btnFont);
            this.panCommon.Controls.Add(this.lblFont);
            this.panCommon.Controls.Add(this.cbxMove3D);
            this.panCommon.Controls.Add(this.numBackTimer);
            this.panCommon.Controls.Add(this.cbxMixPoint);
            this.panCommon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panCommon.Location = new System.Drawing.Point(0, 0);
            this.panCommon.Name = "panCommon";
            this.panCommon.Size = new System.Drawing.Size(264, 241);
            this.panCommon.TabIndex = 0;
            // 
            // cbxDrawCircle
            // 
            this.cbxDrawCircle.AutoSize = true;
            this.cbxDrawCircle.Location = new System.Drawing.Point(12, 207);
            this.cbxDrawCircle.Name = "cbxDrawCircle";
            this.cbxDrawCircle.Size = new System.Drawing.Size(80, 17);
            this.cbxDrawCircle.TabIndex = 13;
            this.cbxDrawCircle.Text = "Draw Circle";
            this.cbxDrawCircle.UseVisualStyleBackColor = true;
            // 
            // cbxBackTimer
            // 
            this.cbxBackTimer.AutoSize = true;
            this.cbxBackTimer.Location = new System.Drawing.Point(12, 184);
            this.cbxBackTimer.Name = "cbxBackTimer";
            this.cbxBackTimer.Size = new System.Drawing.Size(80, 17);
            this.cbxBackTimer.TabIndex = 12;
            this.cbxBackTimer.Text = "Back Timer";
            this.cbxBackTimer.UseVisualStyleBackColor = true;
            // 
            // lblBackTimer
            // 
            this.lblBackTimer.AutoSize = true;
            this.lblBackTimer.Location = new System.Drawing.Point(9, 160);
            this.lblBackTimer.Name = "lblBackTimer";
            this.lblBackTimer.Size = new System.Drawing.Size(61, 13);
            this.lblBackTimer.TabIndex = 10;
            this.lblBackTimer.Text = "Back Timer";
            // 
            // btnLinesColor
            // 
            this.btnLinesColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLinesColor.BackColor = System.Drawing.Color.White;
            this.btnLinesColor.Location = new System.Drawing.Point(74, 133);
            this.btnLinesColor.Name = "btnLinesColor";
            this.btnLinesColor.Size = new System.Drawing.Size(171, 19);
            this.btnLinesColor.TabIndex = 9;
            this.btnLinesColor.UseVisualStyleBackColor = false;
            this.btnLinesColor.Click += new System.EventHandler(this.btnLinesColor_Click);
            // 
            // lblLinesColor
            // 
            this.lblLinesColor.AutoSize = true;
            this.lblLinesColor.Location = new System.Drawing.Point(9, 139);
            this.lblLinesColor.Name = "lblLinesColor";
            this.lblLinesColor.Size = new System.Drawing.Size(59, 13);
            this.lblLinesColor.TabIndex = 8;
            this.lblLinesColor.Text = "Lines Color";
            // 
            // btnTextColor
            // 
            this.btnTextColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTextColor.BackColor = System.Drawing.Color.SteelBlue;
            this.btnTextColor.Location = new System.Drawing.Point(74, 83);
            this.btnTextColor.Name = "btnTextColor";
            this.btnTextColor.Size = new System.Drawing.Size(171, 19);
            this.btnTextColor.TabIndex = 5;
            this.btnTextColor.UseVisualStyleBackColor = false;
            this.btnTextColor.Click += new System.EventHandler(this.btnTextColor_Click);
            // 
            // lblTextColor
            // 
            this.lblTextColor.AutoSize = true;
            this.lblTextColor.Location = new System.Drawing.Point(9, 89);
            this.lblTextColor.Name = "lblTextColor";
            this.lblTextColor.Size = new System.Drawing.Size(55, 13);
            this.lblTextColor.TabIndex = 4;
            this.lblTextColor.Text = "Text Color";
            // 
            // btnFont
            // 
            this.btnFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFont.Location = new System.Drawing.Point(74, 58);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(171, 19);
            this.btnFont.TabIndex = 3;
            this.btnFont.Text = "Font";
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // lblFont
            // 
            this.lblFont.AutoSize = true;
            this.lblFont.Location = new System.Drawing.Point(9, 64);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(28, 13);
            this.lblFont.TabIndex = 2;
            this.lblFont.Text = "Font";
            // 
            // cbxMove3D
            // 
            this.cbxMove3D.AutoSize = true;
            this.cbxMove3D.Location = new System.Drawing.Point(12, 35);
            this.cbxMove3D.Name = "cbxMove3D";
            this.cbxMove3D.Size = new System.Drawing.Size(81, 17);
            this.cbxMove3D.TabIndex = 1;
            this.cbxMove3D.Text = "Move in 3D";
            this.cbxMove3D.UseVisualStyleBackColor = true;
            // 
            // numBackTimer
            // 
            this.numBackTimer.Location = new System.Drawing.Point(74, 158);
            this.numBackTimer.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numBackTimer.Name = "numBackTimer";
            this.numBackTimer.Size = new System.Drawing.Size(56, 20);
            this.numBackTimer.TabIndex = 11;
            // 
            // cbxMixPoint
            // 
            this.cbxMixPoint.AutoSize = true;
            this.cbxMixPoint.Location = new System.Drawing.Point(12, 12);
            this.cbxMixPoint.Name = "cbxMixPoint";
            this.cbxMixPoint.Size = new System.Drawing.Size(73, 17);
            this.cbxMixPoint.TabIndex = 0;
            this.cbxMixPoint.Text = "Mix points";
            this.cbxMixPoint.UseVisualStyleBackColor = true;
            // 
            // panControls
            // 
            this.panControls.Controls.Add(this.btnOk);
            this.panControls.Controls.Add(this.btnCancel);
            this.panControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panControls.Location = new System.Drawing.Point(0, 241);
            this.panControls.Name = "panControls";
            this.panControls.Size = new System.Drawing.Size(264, 34);
            this.panControls.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(105, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(186, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBackColor
            // 
            this.btnBackColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackColor.BackColor = System.Drawing.Color.Black;
            this.btnBackColor.Location = new System.Drawing.Point(74, 108);
            this.btnBackColor.Name = "btnBackColor";
            this.btnBackColor.Size = new System.Drawing.Size(171, 19);
            this.btnBackColor.TabIndex = 7;
            this.btnBackColor.UseVisualStyleBackColor = false;
            this.btnBackColor.Click += new System.EventHandler(this.btnBackColor_Click);
            // 
            // lblBackColor
            // 
            this.lblBackColor.AutoSize = true;
            this.lblBackColor.Location = new System.Drawing.Point(9, 114);
            this.lblBackColor.Name = "lblBackColor";
            this.lblBackColor.Size = new System.Drawing.Size(59, 13);
            this.lblBackColor.TabIndex = 6;
            this.lblBackColor.Text = "Back Color";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(264, 275);
            this.Controls.Add(this.panCommon);
            this.Controls.Add(this.panControls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.panCommon.ResumeLayout(false);
            this.panCommon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBackTimer)).EndInit();
            this.panControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panCommon;
        private System.Windows.Forms.Panel panControls;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbxMove3D;
        private System.Windows.Forms.NumericUpDown numBackTimer;
        private System.Windows.Forms.CheckBox cbxMixPoint;
        private System.Windows.Forms.Button btnFont;
        private System.Windows.Forms.Label lblFont;
        private System.Windows.Forms.Button btnLinesColor;
        private System.Windows.Forms.Label lblLinesColor;
        private System.Windows.Forms.Button btnTextColor;
        private System.Windows.Forms.Label lblTextColor;
        private System.Windows.Forms.Label lblBackTimer;
        private System.Windows.Forms.CheckBox cbxBackTimer;
        private System.Windows.Forms.CheckBox cbxDrawCircle;
        private System.Windows.Forms.Button btnBackColor;
        private System.Windows.Forms.Label lblBackColor;
    }
}