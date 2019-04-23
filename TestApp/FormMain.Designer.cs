namespace TestApp
{
    partial class FormMain
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
            this.TextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonTest1 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TextBoxOutput
            // 
            this.TextBoxOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TextBoxOutput.Location = new System.Drawing.Point(0, 163);
            this.TextBoxOutput.Name = "TextBoxOutput";
            this.TextBoxOutput.Size = new System.Drawing.Size(689, 247);
            this.TextBoxOutput.TabIndex = 0;
            this.TextBoxOutput.Text = "";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.ButtonTest1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(0, 32);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(689, 163);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // ButtonTest1
            // 
            this.ButtonTest1.Location = new System.Drawing.Point(3, 3);
            this.ButtonTest1.Name = "ButtonTest1";
            this.ButtonTest1.Size = new System.Drawing.Size(75, 23);
            this.ButtonTest1.TabIndex = 0;
            this.ButtonTest1.Text = "Test";
            this.ButtonTest1.UseVisualStyleBackColor = true;
            this.ButtonTest1.Click += new System.EventHandler(this.ButtonTest1_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 410);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.TextBoxOutput);
            this.MinimumSize = new System.Drawing.Size(705, 448);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestApp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox TextBoxOutput;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button ButtonTest1;
    }
}

