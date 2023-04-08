using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
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
            foreach (MMDevice device in AudioDevices)
            {
                string deviceType = device.DataFlow == DataFlow.Capture ? "INPUT" : "OUTPUT";
                string deviceLabel = $"{deviceType}: {device.FriendlyName}";
                lbDevice.Items.Add(deviceLabel);
            }
            lbDevice.SelectedIndex = 0;
        }

        private WasapiCapture GetSelectedDevice()
        {
            MMDevice selectedDevice = AudioDevices[lbDevice.SelectedIndex];
            return selectedDevice.DataFlow == DataFlow.Render
                ? new WasapiLoopbackCapture(selectedDevice)
                : new WasapiCapture(selectedDevice, true, 10);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WasapiCapture captureDevice = GetSelectedDevice();
            new SearchForm(captureDevice).ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WasapiCapture captureDevice = GetSelectedDevice();
            new CreateBaseForm(captureDevice).ShowDialog();
        }

        private void lbDevice_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbDevice_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
