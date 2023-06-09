﻿using System;
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
        public string Path = ".\\Out";

        private ISearch search;

        private WaveIn waveSource;

        public void Analyze(List<double[]> _data, int _type, SearchTypeForm _form, string[] _tags)
        {
            //Обработать данные и сравнить их с БД
            List<string> _hashes;
            List<string> _freqs;
            double[] _d = _data.SelectMany(x => x).ToArray();
            Complex[][] _results = Transform(_d, _data[0].Length * 4);
            Determinator determinator = new Determinator(_data[0].Length * 4);
            List<string>[] determinatedData = determinator.Determinate(_results);
            _hashes = determinatedData[0];
            _freqs = determinatedData[1];
            string _result = "Не найдено";
            switch (_type)
            {
                case 0:
                    search = new FastSearch();
                    _result = search.Search(_hashes, _tags);
                    break;
                case 1:
                    search = new DistanceSearch();
                    _result = search.Search(_freqs, _tags);
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
                using (AudioFileReader _reader = new AudioFileReader(_file))
                {
                    byte[] _buffer = new byte[_reader.Length];
                    _reader.Read(_buffer, 0, _buffer.Length);
                    double[] _sampleBuffer = read(_reader, _buffer);
                    Complex[][] _results = Transform(_sampleBuffer, _reader.WaveFormat.SampleRate * 40 / 1000);
                    Determinator determinator = new Determinator(_reader.WaveFormat.SampleRate * 40 / 1000);
                    List<string>[] determinatedData = determinator.Determinate(_results);
                    _hashes = determinatedData[0];
                    _freqs = determinatedData[1];
                    //Записать результаты в БД
                    string _name = System.IO.Path.GetFileNameWithoutExtension(_file);
                    string[] _tags = File.ReadAllLines(".\\Tags\\" + _name + ".txt");
                    using (var db = new LiteDatabase(".\\MyData.db"))
                    {
                        // Получить коллекцию (или создать)
                        var _col = db.GetCollection<SongData>("songs");
                        
                        var _song = new SongData
                        {
                            Name = _name,
                            HashData = _hashes,
                            FreqsData = _freqs,
                            Tags = _tags,
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
        private double[] read(AudioFileReader _reader, byte[] _buffer)
        {
            int _bytesPerSamplePerChannel = _reader.WaveFormat.BitsPerSample / 8;
            int _bytesPerSample = _bytesPerSamplePerChannel * _reader.WaveFormat.Channels;
            int _bufferSampleCount = _buffer.Length / _bytesPerSample;

            double[] _audioValues = new double[_bufferSampleCount];

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
            _buffer = standatrize(_buffer);
            int _totalSize = _buffer.Length;
            int _amountPossible = _totalSize / _chunkSize;
            Complex[][] _results = new Complex[_amountPossible][];

            //Для всех кусков:
            for (int i = 0; i < _amountPossible; i++)
            {
                Complex[] _complex = new Complex[_chunkSize];
                for (int j = 0; j < _chunkSize; j++)
                {
                    _complex[j] = new Complex(_buffer[(i * _chunkSize) + j], 0);
                }
                //Быстрое преобразование фурье
                MathNet.Numerics.IntegralTransforms.Fourier.Forward(_complex);
                _results[i] = _complex;
            }
            return _results;
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
