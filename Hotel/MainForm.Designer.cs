namespace Hotel
{
    partial class MainForm
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
            this.hotelMap = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.hotelMap)).BeginInit();
            this.SuspendLayout();
            // 
            // hotelMap
            // 
            this.hotelMap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hotelMap.Location = new System.Drawing.Point(0, 0);
            this.hotelMap.Name = "hotelMap";
            this.hotelMap.Size = new System.Drawing.Size(1117, 532);
            this.hotelMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.hotelMap.TabIndex = 0;
            this.hotelMap.TabStop = false;
            
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1376, 656);
            this.Controls.Add(this.hotelMap);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.hotelMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox hotelMap;
    }
}

