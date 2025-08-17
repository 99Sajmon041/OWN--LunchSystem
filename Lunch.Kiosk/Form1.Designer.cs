namespace Lunch.Kiosk
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            CardIdTxt = new TextBox();
            InfoLbl = new Label();
            SuspendLayout();
            // 
            // CardIdTxt
            // 
            CardIdTxt.Anchor = AnchorStyles.None;
            CardIdTxt.Location = new Point(399, 234);
            CardIdTxt.Name = "CardIdTxt";
            CardIdTxt.PasswordChar = '*';
            CardIdTxt.ShortcutsEnabled = false;
            CardIdTxt.Size = new Size(302, 23);
            CardIdTxt.TabIndex = 0;
            CardIdTxt.TabStop = false;
            CardIdTxt.TextAlign = HorizontalAlignment.Center;
            // 
            // InfoLbl
            // 
            InfoLbl.Anchor = AnchorStyles.None;
            InfoLbl.AutoSize = true;
            InfoLbl.Font = new Font("Segoe UI", 20F);
            InfoLbl.ForeColor = Color.Black;
            InfoLbl.Location = new Point(332, 335);
            InfoLbl.Name = "InfoLbl";
            InfoLbl.Size = new Size(90, 37);
            InfoLbl.TabIndex = 1;
            InfoLbl.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1111, 607);
            ControlBox = false;
            Controls.Add(InfoLbl);
            Controls.Add(CardIdTxt);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Kiosk - stanice 1";
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox CardIdTxt;
        private Label InfoLbl;
    }
}
