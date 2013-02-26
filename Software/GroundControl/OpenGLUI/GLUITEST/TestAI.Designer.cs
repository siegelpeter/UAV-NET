namespace GLUITEST
{
    partial class TestAI
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
            this.glai1 = new OpenGLUI.GLAI();
            this.SuspendLayout();
            // 
            // glai1
            // 
            this.glai1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glai1.Location = new System.Drawing.Point(0, 0);
            this.glai1.Name = "glai1";
            this.glai1.Size = new System.Drawing.Size(394, 266);
            this.glai1.TabIndex = 0;
            // 
            // TestAI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 266);
            this.Controls.Add(this.glai1);
            this.Name = "TestAI";
            this.Text = "TestAI";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenGLUI.GLAI glai1;

    }
}