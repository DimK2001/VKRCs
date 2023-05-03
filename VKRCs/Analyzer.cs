using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NAudio.Wave;
using LiteDB;
using ScottPlot.SnapLogic;
using System.Drawing.Drawing2D;

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

        public void Analyze(List<double[]> _data)
        {
            //TODO: Обработать данные и сравнить их с БД
            List<string> _hashes;
            List<string> _freqs;
            List<double> _buffer = new List<double>();
            foreach (var _element in _data)
            {
                _buffer.AddRange(_element);
            }
            Complex[][] _results = Transform(_buffer.ToArray(), _data[0].Length);
            Determinator determinator = new Determinator(_data[0].Length);
            List<string>[] determinatedData = standatrize(determinator.Determinate(_results));
            _hashes = determinatedData[0];
            _freqs = determinatedData[1];
        }

        public void CreateBase()
        {
            List<string> _hashes;
            List<string> _freqs;
            foreach (string _file in Directory.EnumerateFiles(Path, "*.wav"))
            {
                using (WaveFileReader _reader = new WaveFileReader(_file))
                {
                    byte[] _buffer = new byte[_reader.Length];
                    _reader.Read(_buffer, 0, _buffer.Length);
                    double[] _sampleBuffer = read(_reader, _buffer);
                    Complex[][] _results = Transform(_sampleBuffer, _reader.WaveFormat.SampleRate * 10 / 1000);
                    Determinator determinator = new Determinator(_reader.WaveFormat.SampleRate * 10 / 1000);
                    List<string>[] determinatedData = standatrize(determinator.Determinate(_results));
                    _hashes = determinatedData[0];
                    _freqs = determinatedData[1];
                    //TODO: записать результаты в БД
                    string _name = System.IO.Path.GetFileName(_file);
                    //TODO: Сделать получение автора, названия и жанра из тегов файла
                    //File.WriteAllLines(".\\Test\\" + _name + ".txt", _freqs);//для теста
                    using (var db = new LiteDatabase(@"MyData.db"))
                    {
                        // Получить коллекцию (или создать)
                        var col = db.GetCollection<SongData>("songs");
                        
                        var song = new SongData
                        {
                            Name = _name,
                            HashData = _hashes,
                            FreqsData = _freqs,
                            IsActive = true
                        };

                        //Добавляем в коллекцию (Id auto-increment)
                        col.Insert(song);

                        //Обновляем документ в коллекции
                        song.Name = _name;

                        col.EnsureIndex(x => x.Name);
                    }
                }
            }
        }
        private double[] read(WaveFileReader _reader, byte[] _buffer)
        {
            int _bytesPerSamplePerChannel = _reader.WaveFormat.BitsPerSample / 8;
            int _bytesPerSample = _bytesPerSamplePerChannel * _reader.WaveFormat.Channels;
            int _bufferSampleCount = _buffer.Length / _bytesPerSample;

            double[] _audioValues = new double[_bufferSampleCount];//Длинна массива исходя из длинны буфера и времени записи, наверное

            if (_bytesPerSamplePerChannel == 2 && _reader.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                for (int i = 0; i < _bufferSampleCount; i++)
                    _audioValues[i] = BitConverter.ToInt16(_buffer, i * _bytesPerSample);
            }
            else if (_bytesPerSamplePerChannel == 4 && _reader.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                for (int i = 0; i < _bufferSampleCount; i++)
                    _audioValues[i] = BitConverter.ToInt32(_buffer, i * _bytesPerSample);
            }
            else if (_bytesPerSamplePerChannel == 4 && _reader.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                for (int i = 0; i < _bufferSampleCount; i++)
                    _audioValues[i] = BitConverter.ToSingle(_buffer, i * _bytesPerSample);
            }
            else
            {
                throw new NotSupportedException(_reader.WaveFormat.ToString());
            }
            return _audioValues;
        }
        private void recordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }
        }
        public Complex[][] Transform(double[] _buffer, int _chunkSize)
        {
            int _totalSize = _buffer.Length;
            int _amountPossible = _totalSize / _chunkSize;
            Complex[][] results = new Complex[_amountPossible][];

            //Для всех кусков:
            for (int i = 0; i < _amountPossible; i++)
            {
                Complex[] complex = new Complex[_chunkSize];
                for (int j = 0; j < _chunkSize; j++)
                {
                    complex[j] = new Complex(_buffer[(i * _chunkSize) + j], 0);
                }
                Complex[] _twoPwr = FFT.ZeroPad(complex);
                //Быстрое преобразование фурье
                results[i] = FFT.fft(_twoPwr);
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

    class SongData
    {
        public string Name;
        public List<string> HashData;
        public List<string> FreqsData;
        public bool IsActive;
    }
}
