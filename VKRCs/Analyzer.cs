using System;
using System.Linq;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace vkrC
{
    class Analyzer
    {
        public bool Running = false;
        public int SearchType;
        public string Result = "Не найдено.";

        private ISearch search;

        private WaveIn waveSource;
        private WaveFormat waveFormat = new WaveFormat(44100, 1);

        public void Thread()
        {
            //Настройка записи
            waveSource = new WaveIn();
            //TODO: Читать формат с микрофона
            waveSource.WaveFormat = waveFormat;
            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(dataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(recordingStopped);
            waveSource.StartRecording();

            using (AudioFileReader _reader = new AudioFileReader(path))
            {
                int _count = 0;
                if (_reader.WaveFormat.Channels == 2)
                {
                    var _mono = new StereoToMonoProvider16(_reader);
                    byte[] _buffer = new byte[_reader.Length / 2];
                    _count = _mono.Read(_buffer, 0, _buffer.Length);
                }
                else if (_reader.WaveFormat.Channels == 1)
                {
                    byte[] _buffer = new byte[_reader.Length];
                    _count = _reader.Read(_buffer, 0, _buffer.Length);
                }
                for (int i = 0; i < _count; i++)
                {
                    
                }
            }
        }
        private void dataAvailable(object sender, WaveInEventArgs e)
        {
            
        }
        private void recordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }
        }
    }
}
