using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VKRCs
{
    public partial class SearchForm : Form
    {
        readonly WasapiCapture AudioDevice;
        public SearchForm(WasapiCapture _audioDevice)
        {
            InitializeComponent();
            AudioDevice = _audioDevice;
            WaveFormat _format = _audioDevice.WaveFormat;
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {

        }
    }
}
