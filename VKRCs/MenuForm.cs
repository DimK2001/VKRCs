using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Linq;
using System.Windows.Forms;

namespace VKRCs
{
    public partial class MenuForm : Form
    {
        public readonly MMDevice[] AudioDevices = new MMDeviceEnumerator()
           .EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
           .ToArray();

        public MenuForm()
        {
            InitializeComponent();
            foreach (MMDevice _device in AudioDevices)
            {
                string _deviceType = _device.DataFlow == DataFlow.Capture ? "Ввод" : "Вывод";
                string _deviceLabel = $"{_deviceType}: {_device.FriendlyName}";
                lbDevice.Items.Add(_deviceLabel);
            }
            lbDevice.SelectedIndex = 0;
        }

        private WasapiCapture GetSelectedDevice()
        {
            MMDevice _selectedDevice = AudioDevices[lbDevice.SelectedIndex];
            return _selectedDevice.DataFlow == DataFlow.Render
                ? new WasapiLoopbackCapture(_selectedDevice)
                : new WasapiCapture(_selectedDevice, true, 10);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WasapiCapture _captureDevice = GetSelectedDevice();
            new SearchForm(_captureDevice).ShowDialog();
        }

        private void button2_Click(object _sender, EventArgs e)
        {
            new CreateBaseForm().ShowDialog();
        }

        private void lbDevice_SelectedIndexChanged(object _sender, EventArgs e)
        {

        }

        private void lbDevice_SelectedIndexChanged_1(object _sender, EventArgs e)
        {

        }
    }
}
