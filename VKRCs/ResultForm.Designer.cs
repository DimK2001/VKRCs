namespace VKRCs
{
    partial class ResultForm
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
            result = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // result
            // 
            result.AutoSize = false;
            result.Location = new System.Drawing.Point(120, 73);
            result.Name = "result";
            result.Size = new System.Drawing.Size(260, 15);
            result.TabIndex = 0;
            result.Text = "Результат поиска: ";
            result.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ResultForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(500, 160);
            Controls.Add(result);
            Name = "ResultForm";
            Text = "ResultForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label result;
    }
}