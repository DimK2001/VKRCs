using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NAudio.Wave;
using LiteDB;

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

        public void Analyze(List<double[]> _data, int _type, SearchTypeForm _form)
        {
            //Обработать данные и сравнить их с БД
            List<string> _hashes;
            List<string> _freqs;
            List<double> _buffer = new List<double>();
            foreach (var _element in _data)
            {
                _buffer.AddRange(_element);
            }
            Complex[][] _results = Transform(_buffer.ToArray(), _data[0].Length);
            Determinator determinator = new Determinator(_data[0].Length);
            List<string>[] determinatedData = determinator.Determinate(_results);
            _hashes = determinatedData[0];
            _freqs = determinatedData[1];
            string _result = "Не найдено";
            switch (_type)
            {
                case 0:
                    search = new FastSearch();
                    _result = search.Search(_hashes);
                    break;
                case 1:
                    search = new DistanceSearch();
                    _result = search.Search(_freqs);
                    break;
            }
            _form.PrintResult(_result);
        }

        public void CreateBase()
        {
            List<string> _hashes;
            List<string> _freqs;
            File.Delete(".\\MyData.db");
            foreach (string _file in Directory.EnumerateFiles(Path, "*.wav"))
            {
                using (WaveFileReader _reader = new WaveFileReader(_file))
                {
                    byte[] _buffer = new byte[_reader.Length];
                    _reader.Read(_buffer, 0, _buffer.Length);
                    double[] _sampleBuffer = read(_reader, _buffer);
                    Complex[][] _results = Transform(_sampleBuffer, _reader.WaveFormat.SampleRate * 10 / 1000);
                    Determinator determinator = new Determinator(_reader.WaveFormat.SampleRate * 10 / 1000);
                    List<string>[] determinatedData = determinator.Determinate(_results);
                    _hashes = determinatedData[0];
                    _freqs = determinatedData[1];
                    //Записать результаты в БД
                    string _name = System.IO.Path.GetFileName(_file);
                    //TODO: Сделать получение автора, названия и жанра из тегов файла
                    //File.WriteAllLines(".\\Test\\" + _name + ".txt", _freqs);//для теста
                    using (var db = new LiteDatabase(".\\MyData.db"))
                    {
                        // Получить коллекцию (или создать)
                        var _col = db.GetCollection<SongData>("songs");
                        
                        var _song = new SongData
                        {
                            Name = _name,
                            HashData = _hashes,
                            FreqsData = _freqs,
                            IsActive = true
                        };
                        //Проверяем уникальность имени
                        _col.EnsureIndex(x => x.Name);
                        //Добавляем в коллекцию (Id auto-increment)
                        _col.Insert(_song);
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
            //_buffer = standatrize(_buffer);
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
        //Удаление нулей из начала и конца
        private double[] standatrize(double[] _data)
        {
            List<double> _newData = _data.ToList();
            while (_newData.Count() - 1 > 0 && _newData[_newData.Count() - 1] == 0)
            {
                _newData.RemoveAt(_newData.Count() - 1);
            }
            while (_newData.Count() - 1 > 0 && _newData[0] == 0)
            {
                _newData.RemoveAt(0);
            }
            return _newData.ToArray();
        }
        public void StopListening()
        {
            waveSource.StopRecording();
        }
    }
}
