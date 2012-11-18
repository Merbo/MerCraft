namespace MerCraft
{
    partial class LaunchForm
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblRate = new System.Windows.Forms.Label();
            this.lblCurrentAction = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 51);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(318, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(102, 13);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Updating MerCraft...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Location = new System.Drawing.Point(349, 9);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(258, 13);
            this.lblRate.TabIndex = 2;
            this.lblRate.Text = "ERROR DISPLAYING DOWNLOAD INFORMATION";
            this.lblRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurrentAction
            // 
            this.lblCurrentAction.AutoSize = true;
            this.lblCurrentAction.Location = new System.Drawing.Point(12, 35);
            this.lblCurrentAction.Name = "lblCurrentAction";
            this.lblCurrentAction.Size = new System.Drawing.Size(77, 13);
            this.lblCurrentAction.TabIndex = 3;
            this.lblCurrentAction.Text = "Current Action:";
            this.lblCurrentAction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LaunchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 84);
            this.Controls.Add(this.lblCurrentAction);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar1);
            this.Name = "LaunchForm";
            this.Text = "MerCraft Updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Label lblStatus;
        public System.Windows.Forms.Label lblRate;
        public System.Windows.Forms.Label lblCurrentAction;
    }
}