namespace EVEASS_V1
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.txbAPIKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAccessToken = new System.Windows.Forms.Label();
            this.btnExchangeKey = new System.Windows.Forms.Button();
            this.lvBluePrints = new System.Windows.Forms.ListView();
            this.AppStatusStrip = new System.Windows.Forms.StatusStrip();
            this.ddbCharacters = new System.Windows.Forms.ToolStripDropDownButton();
            this.sslShoppingList = new System.Windows.Forms.ToolStripStatusLabel();
            this.sslApplicationStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.AppStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // txbAPIKey
            // 
            this.txbAPIKey.Location = new System.Drawing.Point(252, 50);
            this.txbAPIKey.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txbAPIKey.Name = "txbAPIKey";
            this.txbAPIKey.Size = new System.Drawing.Size(755, 38);
            this.txbAPIKey.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(124, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 27);
            this.label1.TabIndex = 3;
            this.label1.Text = "API Key:";
            // 
            // lblAccessToken
            // 
            this.lblAccessToken.AutoSize = true;
            this.lblAccessToken.Location = new System.Drawing.Point(124, 115);
            this.lblAccessToken.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAccessToken.Name = "lblAccessToken";
            this.lblAccessToken.Size = new System.Drawing.Size(96, 27);
            this.lblAccessToken.TabIndex = 4;
            this.lblAccessToken.Text = "label2";
            // 
            // btnExchangeKey
            // 
            this.btnExchangeKey.Location = new System.Drawing.Point(1024, 50);
            this.btnExchangeKey.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnExchangeKey.Name = "btnExchangeKey";
            this.btnExchangeKey.Size = new System.Drawing.Size(138, 38);
            this.btnExchangeKey.TabIndex = 7;
            this.btnExchangeKey.Text = "交换密钥";
            this.btnExchangeKey.UseVisualStyleBackColor = true;
            this.btnExchangeKey.Click += new System.EventHandler(this.btnExchangeKey_Click);
            // 
            // lvBluePrints
            // 
            this.lvBluePrints.Dock = System.Windows.Forms.DockStyle.Right;
            this.lvBluePrints.Location = new System.Drawing.Point(857, 0);
            this.lvBluePrints.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lvBluePrints.Name = "lvBluePrints";
            this.lvBluePrints.Size = new System.Drawing.Size(853, 851);
            this.lvBluePrints.TabIndex = 8;
            this.lvBluePrints.UseCompatibleStateImageBehavior = false;
            // 
            // AppStatusStrip
            // 
            this.AppStatusStrip.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.AppStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddbCharacters,
            this.sslShoppingList,
            this.sslApplicationStatus,
            this.toolStripProgressBar});
            this.AppStatusStrip.Location = new System.Drawing.Point(0, 851);
            this.AppStatusStrip.Name = "AppStatusStrip";
            this.AppStatusStrip.Padding = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.AppStatusStrip.Size = new System.Drawing.Size(1710, 89);
            this.AppStatusStrip.TabIndex = 9;
            this.AppStatusStrip.Text = "AppStatusStrip";
            // 
            // ddbCharacters
            // 
            this.ddbCharacters.Image = ((System.Drawing.Image)(resources.GetObject("ddbCharacters.Image")));
            this.ddbCharacters.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddbCharacters.Name = "ddbCharacters";
            this.ddbCharacters.Size = new System.Drawing.Size(211, 87);
            this.ddbCharacters.Text = "Characters";
            // 
            // sslShoppingList
            // 
            this.sslShoppingList.Name = "sslShoppingList";
            this.sslShoppingList.Size = new System.Drawing.Size(184, 84);
            this.sslShoppingList.Text = "ShoppingList";
            // 
            // sslApplicationStatus
            // 
            this.sslApplicationStatus.AutoSize = false;
            this.sslApplicationStatus.Name = "sslApplicationStatus";
            this.sslApplicationStatus.Size = new System.Drawing.Size(800, 84);
            this.sslApplicationStatus.Text = "ApplicationStatus";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(800, 83);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1710, 940);
            this.Controls.Add(this.lvBluePrints);
            this.Controls.Add(this.btnExchangeKey);
            this.Controls.Add(this.lblAccessToken);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbAPIKey);
            this.Controls.Add(this.AppStatusStrip);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Main";
            this.Text = "商人助手辅助";
            this.AppStatusStrip.ResumeLayout(false);
            this.AppStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvCustomerBluePrints;
        private System.Windows.Forms.TextBox txbAPIKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblAccessToken;
        private System.Windows.Forms.Button btnExchangeKey;
        private System.Windows.Forms.ListView lvBluePrints;
        private System.Windows.Forms.StatusStrip AppStatusStrip;
        private System.Windows.Forms.ToolStripDropDownButton ddbCharacters;
        private System.Windows.Forms.ToolStripStatusLabel sslApplicationStatus;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel sslShoppingList;
    }
}

