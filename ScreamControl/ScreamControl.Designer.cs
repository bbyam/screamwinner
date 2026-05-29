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
            btnStartStop1 = new Button();
            btnShow = new Button();
            btnStartStop2 = new Button();
            btnStartStop3 = new Button();
            SuspendLayout();
            // 
            // btnStartStop1
            // 
            btnStartStop1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStartStop1.Location = new Point(49, 169);
            btnStartStop1.Name = "btnStartStop1";
            btnStartStop1.Size = new Size(205, 96);
            btnStartStop1.TabIndex = 0;
            btnStartStop1.Text = "Start 1";
            btnStartStop1.UseVisualStyleBackColor = true;
            btnStartStop1.Click += btnStartStop_Click;
            // 
            // btnShow
            // 
            btnShow.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnShow.Location = new Point(49, 42);
            btnShow.Name = "btnShow";
            btnShow.Size = new Size(121, 58);
            btnShow.TabIndex = 1;
            btnShow.Text = "Show";
            btnShow.UseVisualStyleBackColor = true;
            btnShow.Click += btnShow_Click;
            // 
            // btnStartStop2
            // 
            btnStartStop2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStartStop2.Location = new Point(272, 169);
            btnStartStop2.Name = "btnStartStop2";
            btnStartStop2.Size = new Size(205, 96);
            btnStartStop2.TabIndex = 2;
            btnStartStop2.Text = "Start 2";
            btnStartStop2.UseVisualStyleBackColor = true;
            btnStartStop2.Click += btnStartStop_Click;
            // 
            // btnStartStop3
            // 
            btnStartStop3.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStartStop3.Location = new Point(495, 169);
            btnStartStop3.Name = "btnStartStop3";
            btnStartStop3.Size = new Size(205, 96);
            btnStartStop3.TabIndex = 3;
            btnStartStop3.Text = "Start 3";
            btnStartStop3.UseVisualStyleBackColor = true;
            btnStartStop3.Click += btnStartStop_Click;
            // 
            // ScreamControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnStartStop3);
            Controls.Add(btnStartStop2);
            Controls.Add(btnShow);
            Controls.Add(btnStartStop1);
            Name = "ScreamControl";
            Text = "Scream Winner";
            FormClosing += ScreamControl_FormClosing;
            Load += ScreamControl_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnStartStop1;
        private Button btnShow;
        private Button btnStartStop2;
        private Button btnStartStop3;
    }
}
