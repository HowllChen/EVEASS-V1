namespace EVEASS_V1
{
    partial class SignCharacter
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
            this.btnSign = new System.Windows.Forms.Button();
            this.lblCharacterName = new System.Windows.Forms.Label();
            this.cbxCEO = new System.Windows.Forms.CheckBox();
            this.cbxMarket = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSign
            // 
            this.btnSign.AutoSize = true;
            this.btnSign.Image = global::EVEASS_V1.Properties.Resources.eve_sso_login_white_small;
            this.btnSign.Location = new System.Drawing.Point(10, 10);
            this.btnSign.Margin = new System.Windows.Forms.Padding(1);
            this.btnSign.Name = "btnSign";
            this.btnSign.Size = new System.Drawing.Size(201, 36);
            this.btnSign.TabIndex = 7;
            this.btnSign.UseVisualStyleBackColor = true;
            this.btnSign.Click += new System.EventHandler(this.btnSign_Click);
            // 
            // lblCharacterName
            // 
            this.lblCharacterName.AutoSize = true;
            this.lblCharacterName.Location = new System.Drawing.Point(12, 54);
            this.lblCharacterName.Name = "lblCharacterName";
            this.lblCharacterName.Size = new System.Drawing.Size(59, 12);
            this.lblCharacterName.TabIndex = 8;
            this.lblCharacterName.Text = "角色名称:";
            // 
            // cbxCEO
            // 
            this.cbxCEO.AutoSize = true;
            this.cbxCEO.Location = new System.Drawing.Point(14, 80);
            this.cbxCEO.Name = "cbxCEO";
            this.cbxCEO.Size = new System.Drawing.Size(48, 16);
            this.cbxCEO.TabIndex = 9;
            this.cbxCEO.Text = "军团";
            this.cbxCEO.UseVisualStyleBackColor = true;
            // 
            // cbxMarket
            // 
            this.cbxMarket.AutoSize = true;
            this.cbxMarket.Location = new System.Drawing.Point(14, 102);
            this.cbxMarket.Name = "cbxMarket";
            this.cbxMarket.Size = new System.Drawing.Size(48, 16);
            this.cbxMarket.TabIndex = 10;
            this.cbxMarket.Text = "市场";
            this.cbxMarket.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(72, 141);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // SignCharacter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 176);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cbxMarket);
            this.Controls.Add(this.cbxCEO);
            this.Controls.Add(this.lblCharacterName);
            this.Controls.Add(this.btnSign);
            this.Name = "SignCharacter";
            this.Text = "SignCharacter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSign;
        private System.Windows.Forms.Label lblCharacterName;
        private System.Windows.Forms.CheckBox cbxCEO;
        private System.Windows.Forms.CheckBox cbxMarket;
        private System.Windows.Forms.Button btnSave;
    }
}