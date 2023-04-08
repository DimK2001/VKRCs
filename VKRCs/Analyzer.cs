using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using NAudio.Wave;

namespace VKRCs
{
    class Analyzer
    {
        public bool Running = false;
        public int SearchType;
        public string Result = "Не найдено.";
        public string Path = ".\\Music";

        private ISearch search;

        private WaveIn waveSource;
        private WaveFormat waveFormat = new WaveFormat(44100, 1);
        private Stream stream;

        public void Analyze()
        {
            //Thread thread = new Thread(listenMic);
            //thread.Start();
            listenMic();
        }

        private void listenMic()
        {
            //Настройка записи
            waveSource = new WaveIn();
            //TODO: Читать формат с микрофона?
            waveSource.WaveFormat = waveFormat;
            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(dataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(recordingStopped);
            waveSource.StartRecording();
        }
        private void dataAvailable(object _sender, WaveInEventArgs e)
        {
            if (waveSource == null)
                return;
            short[] _audioData = new short[e.Buffer.Length / 2];
            Buffer.BlockCopy(e.Buffer, 0, _audioData, 0, e.Buffer.Length);
            byte[] _bytes = new byte[_audioData.Length];
            Buffer.BlockCopy(_audioData, 0, _bytes, 0, _audioData.Length);
            stream = new MemoryStream(_bytes.ToArray());
        }
        public void CreateBase()
        {
            List<string> _hashes;
            List<string> _freqs;
            foreach (string _file in Directory.EnumerateFiles(Path, "*.wav"))
            {
                using (WaveFileReader _reader = new WaveFileReader(_file))
                {
                    int _count = 0;
                    byte[] _sampleBuffer = { };
                    if (_reader.WaveFormat.Channels == 2)
                    {
                        var _mono = new StereoToMonoProvider16(_reader);
                        byte[] _buffer = new byte[_reader.Length / 2];
                        _count = _mono.Read(_buffer, 0, _buffer.Length);
                        _sampleBuffer = new byte[_count];
                        Buffer.BlockCopy(_buffer, 0, _sampleBuffer, 0, _count);
                    }
                    else if (_reader.WaveFormat.Channels == 1)
                    {
                        byte[] _buffer = new byte[_reader.Length];
                        _count = _reader.Read(_buffer, 0, _buffer.Length);
                        _sampleBuffer = new byte[_count];
                        Buffer.BlockCopy(_buffer, 0, _sampleBuffer, 0, _count);
                    }
                    Complex[][] _results = Transform(_sampleBuffer);
                    Determinator determinator = new Determinator();
                    List<string>[] determinatedData = standatrize(determinator.Determinate(_results));
                    _hashes = determinatedData[0];
                    _freqs = determinatedData[1];
                    //TODO: записать результаты в БД
                }
            }
        }
        private void recordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }
        }
        private Complex[][] Transform(byte[] _buffer)
        {
            int _totalSize = _buffer.Length;
            int _amountPossible = _totalSize / DATA.CHUNK_SIZE;
            Complex[][] results = new Complex[_amountPossible][];

            //Для всех кусков:
            for (int i = 0; i < _amountPossible; i++)
            {
                Complex[] complex = new Complex[DATA.CHUNK_SIZE];
                for (int j = 0; j < DATA.CHUNK_SIZE; j++)
                {
                    complex[j] = new Complex(_buffer[(i * DATA.CHUNK_SIZE) + j], 0);
                }
                //Быстрое преобразование фурье
                results[i] = FFT.fft(complex);
            }
            return results;
        }
        private List<string>[] standatrize(List<string>[] _data)
        {
            while (_data[0][_data[0].Count() - 1] == "00000000000")
            {
                _data[0].RemoveAt(_data[0].Count() - 1);
            }
            while (_data[0][0] == "00000000000")
            {
                _data[0].RemoveAt(0);
            }
            while (_data[1][_data[1].Count() - 1] == "0 0 0 0 0")
            {
                _data[1].RemoveAt(_data[1].Count() - 1);
            }
            while (_data[1][0] == "0 0 0 0 0")
            {
                _data[1].RemoveAt(0);
            }
            return _data;
        }
        public void StopListening()
        {
            waveSource.StopRecording();
        }
    }
}
