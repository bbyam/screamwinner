namespace ScreamControl
{
    partial class ScreamControl
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
            btnStartStop = new Button();
            SuspendLayout();
            // 
            // btnStartStop
            // 
            btnStartStop.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStartStop.Location = new Point(292, 167);
            btnStartStop.Name = "btnStartStop";
            btnStartStop.Size = new Size(205, 96);
            btnStartStop.TabIndex = 0;
            btnStartStop.Text = "Start";
            btnStartStop.UseVisualStyleBackColor = true;
            btnStartStop.Click += btnStartStop_Click;
            // 
            // ScreamControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnStartStop);
            Name = "ScreamControl";
            Text = "Scream Winner";
            FormClosing += ScreamControl_FormClosing;
            Load += ScreamControl_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnStartStop;
    }
}
