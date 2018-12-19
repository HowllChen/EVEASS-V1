namespace EVEASS_V1
{
    partial class ESIAndDBConfirm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lvwESIData = new System.Windows.Forms.ListView();
            this.lvwDBData = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(-5, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(267, 36);
            this.label1.TabIndex = 2;
            this.label1.Text = "ESI数据(增加):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(-5, 506);
            this.label2.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(357, 36);
            this.label2.TabIndex = 3;
            this.label2.Text = "原数据库数据(删除):";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(404, 1010);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(175, 52);
            this.btnConfirm.TabIndex = 4;
            this.btnConfirm.Text = "确认";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(656, 1010);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(175, 52);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lvwESIData
            // 
            this.lvwESIData.FullRowSelect = true;
            this.lvwESIData.Location = new System.Drawing.Point(28, 58);
            this.lvwESIData.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.lvwESIData.Name = "lvwESIData";
            this.lvwESIData.Size = new System.Drawing.Size(1173, 436);
            this.lvwESIData.TabIndex = 6;
            this.lvwESIData.UseCompatibleStateImageBehavior = false;
            this.lvwESIData.View = System.Windows.Forms.View.Details;
            // 
            // lvwDBData
            // 
            this.lvwDBData.FullRowSelect = true;
            this.lvwDBData.Location = new System.Drawing.Point(28, 549);
            this.lvwDBData.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.lvwDBData.Name = "lvwDBData";
            this.lvwDBData.Size = new System.Drawing.Size(1173, 443);
            this.lvwDBData.TabIndex = 7;
            this.lvwDBData.UseCompatibleStateImageBehavior = false;
            this.lvwDBData.View = System.Windows.Forms.View.Details;
            // 
            // ESIAndDBConfirm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 1089);
            this.Controls.Add(this.lvwDBData);
            this.Controls.Add(this.lvwESIData);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.Name = "ESIAndDBConfirm";
            this.Text = "变更确认";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListView lvwESIData;
        private System.Windows.Forms.ListView lvwDBData;
    }
}