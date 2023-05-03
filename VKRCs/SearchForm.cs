using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace VKRCs
{
    public partial class SearchForm : Form
    {
        readonly WasapiCapture AudioDevice;
        readonly double[] audioValues;
        private List<double[]> listenedData = new List<double[]>();
        private Stream stream;
        private bool recording = false;
        public SearchForm(WasapiCapture _audioDevice)
        {
            InitializeComponent();
            AudioDevice = _audioDevice;
            WaveFormat _format = _audioDevice.WaveFormat;

            audioValues = new double[_format.SampleRate * 10 / 1000];//Рассматриваются 10 миллисекунд

            formsPlot1.Plot.AddSignal(audioValues, _format.SampleRate / 1000);
            formsPlot1.Plot.YLabel("Уровень");
            formsPlot1.Plot.XLabel("Время (миллисекунд)");
            formsPlot1.Plot.Title($"Формат:{_format.Encoding} ({_format.BitsPerSample}-bit) {_format.SampleRate} KHz");
            formsPlot1.Plot.SetAxisLimitsY(-.5, .5);
            formsPlot1.Refresh();
            AudioDevice.DataAvailable += dataAvailable;
            AudioDevice.StartRecording();
            FormClosed += formClose;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            formsPlot1.RefreshRequest();
        }

        private void dataAvailable(object _sender, WaveInEventArgs e)
        {
            int _bytesPerSamplePerChannel = AudioDevice.WaveFormat.BitsPerSample / 8;
            int _bytesPerSample = _bytesPerSamplePerChannel * AudioDevice.WaveFormat.Channels;
            int _bufferSampleCount = e.Buffer.Length / _bytesPerSample;

            if (_bufferSampleCount >= audioValues.Length)
            {
                _bufferSampleCount = audioValues.Length;
            }

            if (_bytesPerSamplePerChannel == 2 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                for (int i = 0; i < _bufferSampleCount; i++)
                    audioValues[i] = BitConverter.ToInt16(e.Buffer, i * _bytesPerSample);
            }
            else if (_bytesPerSamplePerChannel == 4 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                for (int i = 0; i < _bufferSampleCount; i++)
                    audioValues[i] = BitConverter.ToInt32(e.Buffer, i * _bytesPerSample);
            }
            else if (_bytesPerSamplePerChannel == 4 && AudioDevice.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                for (int i = 0; i < _bufferSampleCount; i++)
                    audioValues[i] = BitConverter.ToSingle(e.Buffer, i * _bytesPerSample);
            }
            else
            {
                throw new NotSupportedException(AudioDevice.WaveFormat.ToString());
            }

            if (recording)
            {
                listenedData.Add(audioValues);
            }
        }

        private void formClose(object _sender, FormClosedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Закрытие устройства: {AudioDevice}");
            AudioDevice.StopRecording();
            AudioDevice.Dispose();
        }

        private void formLoad(object sender, EventArgs e)
        {

        }

        private void formsPlot1_Load(object sender, EventArgs e)
        {

        }

        private void formsPlot1_Load_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            recording = !recording;
            if (!recording)
            {
                //Отправка данных на обработку
                new SearchTypeForm(listenedData).ShowDialog();
                //TODO: открыть форму с результатом
                //this.Close();
            }
        }
    }
}
