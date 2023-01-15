namespace VisPrWindowsDesktopRecorder
{
    partial class MainWindow
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
            this.btnStartRecording = new System.Windows.Forms.Button();
            this.btnStopRecording = new System.Windows.Forms.Button();
            this.btnPauseRecording = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartRecording
            // 
            this.btnStartRecording.Location = new System.Drawing.Point(12, 13);
            this.btnStartRecording.Name = "btnStartRecording";
            this.btnStartRecording.Size = new System.Drawing.Size(243, 23);
            this.btnStartRecording.TabIndex = 0;
            this.btnStartRecording.Text = "Start recording";
            this.btnStartRecording.UseVisualStyleBackColor = true;
            this.btnStartRecording.Click += new System.EventHandler(this.OnClickStart);
            // 
            // btnStopRecording
            // 
            this.btnStopRecording.Location = new System.Drawing.Point(12, 43);
            this.btnStopRecording.Name = "btnStopRecording";
            this.btnStopRecording.Size = new System.Drawing.Size(243, 23);
            this.btnStopRecording.TabIndex = 1;
            this.btnStopRecording.Text = "Stop recording";
            this.btnStopRecording.UseVisualStyleBackColor = true;
            this.btnStopRecording.Click += new System.EventHandler(this.OnClickStop);
            // 
            // btnPauseRecording
            // 
            this.btnPauseRecording.Location = new System.Drawing.Point(12, 73);
            this.btnPauseRecording.Name = "btnPauseRecording";
            this.btnPauseRecording.Size = new System.Drawing.Size(243, 23);
            this.btnPauseRecording.TabIndex = 2;
            this.btnPauseRecording.Text = "Pause recording";
            this.btnPauseRecording.UseVisualStyleBackColor = true;
            this.btnPauseRecording.Click += new System.EventHandler(this.OnClickPause);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 105);
            this.Controls.Add(this.btnPauseRecording);
            this.Controls.Add(this.btnStopRecording);
            this.Controls.Add(this.btnStartRecording);
            this.Name = "MainWindow";
            this.Text = "VisPr² Windows Recorder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartRecording;
        private System.Windows.Forms.Button btnStopRecording;
        private System.Windows.Forms.Button btnPauseRecording;
    }
}