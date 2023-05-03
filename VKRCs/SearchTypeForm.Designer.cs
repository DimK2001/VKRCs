namespace VKRCs
{
    partial class SearchTypeForm
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
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            result = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(12, 52);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(161, 84);
            button1.TabIndex = 0;
            button1.Text = "Быстрый поиск";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(301, 52);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(161, 84);
            button2.TabIndex = 1;
            button2.Text = "Тщательный поиск";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // result
            // 
            result.Location = new System.Drawing.Point(107, 9);
            result.Name = "result";
            result.Size = new System.Drawing.Size(260, 15);
            result.TabIndex = 2;
            result.Text = "Результат поиска: ";
            result.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            result.Click += result_Click;
            // 
            // SearchTypeForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(474, 148);
            Controls.Add(result);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "SearchTypeForm";
            Text = "Поиск песни";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label result;
    }
}