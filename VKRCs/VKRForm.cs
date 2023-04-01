using NAudio.Wave;
using System;
using System.Windows.Forms;

namespace VKRCs
{
    public partial class VKRForm : Form
    {
        public VKRForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            using (WaveFileReader _reader = new WaveFileReader(".\\Music\\Bad Apple.wav"))
            {
                byte[] _buffer = new byte[_reader.Length];
                int _count = _reader.Read(_buffer, 0, _buffer.Length);
                short[] _sampleBuffer = new short[_count/* / 2*/];
                Buffer.BlockCopy(_buffer, 0, _sampleBuffer, 0, _count);
                this.label1.Text = string.Join("\n", _sampleBuffer);
            }
        }
    }
}
