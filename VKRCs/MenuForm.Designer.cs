using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace VKRCs
{
    partial class MenuForm
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
            btnLaunchWaveform = new Button();
            btnLaunchFFT = new Button();
            lbDevice = new ListBox();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnSearch
            // 
            btnLaunchWaveform.Location = new Point(15, 365);
            btnLaunchWaveform.Name = "btnLaunchWaveform";
            btnLaunchWaveform.Size = new Size(117, 54);
            btnLaunchWaveform.TabIndex = 0;
            btnLaunchWaveform.Text = "Audio Waveform";
            btnLaunchWaveform.UseVisualStyleBackColor = true;
            btnLaunchWaveform.Click += button1_Click;
            // 
            // btnCreateBase
            // 
            btnLaunchFFT.Location = new Point(158, 365);
            btnLaunchFFT.Name = "btnLaunchFFT";
            btnLaunchFFT.Size = new Size(117, 54);
            btnLaunchFFT.TabIndex = 1;
            btnLaunchFFT.Text = "Audio FFT";
            btnLaunchFFT.UseVisualStyleBackColor = true;
            btnLaunchFFT.Click += button2_Click;
            // 
            // lbDevice
            // 
            lbDevice.Dock = DockStyle.Fill;
            lbDevice.FormattingEnabled = true;
            lbDevice.ItemHeight = 15;
            lbDevice.Location = new Point(3, 19);
            lbDevice.Name = "lbDevice";
            lbDevice.Size = new Size(394, 315);
            lbDevice.TabIndex = 2;
            lbDevice.SelectedIndexChanged += lbDevice_SelectedIndexChanged_1;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(lbDevice);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(400, 337);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Device";
            // 
            // MenuForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(424, 439);
            Controls.Add(groupBox1);
            Controls.Add(btnLaunchFFT);
            Controls.Add(btnLaunchWaveform);
            Name = "MenuForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Audio Monitor";
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btnLaunchWaveform;
        private Button btnLaunchFFT;
        private ListBox lbDevice;
        private GroupBox groupBox1;
    }
}