
using NAudio.Wave;
using System;
using System.Collections.Generic;

using System.Numerics;


namespace VKRCs
{
    internal class ParamTest
    {
        private int[] range = new int[] { 55, 170, 1700, 2048 };
        private long distance = 1000000;
        private int matches = -100000;
        private int[] bestD = new int[5];
        private int[] bestF = new int[5];
        private string NAME = "krahmal-bol_vsego_mira.wav";

        public string[] countParam()
        {
            DistanceSearch searchD = new DistanceSearch();
            FastSearch searchF = new FastSearch();
            for (int i = 0; range[0] + 1 < 60; i += 1)
            {
                range = new int[] { 50 + i, 170, 1700, 2048 };
                //Open file EX/////////////////////////////////////////////////////////////////////
                string path = ".\\Music\\" + NAME;
                List<string>[] determinatedData = openFile(path);
                List<string> hashesEX = determinatedData[0];
                List<string> freqsEX = determinatedData[1];
                //Open file Test/////////////////////////////////////////////////////////////////////
                //string pathT = ".\\Test\\" + NAME;
                string pathT = ".\\Test\\bol2.wav";
                List<string>[] determinatedDataT = openFile(pathT);
                List<string> hashesTest = determinatedDataT[0];
                List<string> freqsTest = determinatedDataT[1];

                //string pathW = ".\\Test\\" + "Bad Apple.wav";
                string pathW = ".\\Test\\" + "apple2.wav";
                List<string>[] determinatedDataW = openFile(pathW);
                List<string> hashesWrong = determinatedDataW[0];
                List<string> freqsWrong = determinatedDataW[1];

                long distWr = searchD.Find(freqsEX.ToArray(), freqsWrong.ToArray(), 0);
                int matchesWr = 0;
                foreach (int match in searchF.Find(hashesWrong.ToArray(), hashesEX.ToArray()).Values)
                {
                    if (match >= matchesWr)
                    {
                        matchesWr = match;
                    }
                }
                for (int line = 0; line < freqsEX.Count - freqsTest.Count; ++line)
                {
                    long dist = searchD.Find(freqsEX.ToArray(), freqsTest.ToArray(), line);
                    if (dist - distWr < distance)
                    {
                        bestD = range;
                        distance = dist - distWr;
                    }
                }
                foreach (int match in searchF.Find(hashesTest.ToArray(), hashesEX.ToArray()).Values)
                {
                    if (match - matchesWr > matches)
                    {
                        matches = match - matchesWr;
                        bestF = range;
                    }
                    if (match - matchesWr > 0)
                    {
                        string s = range[0].ToString();
                    }
                }
            }
            string[] st = new string[2];
            st[0] = bestD.ToString() + " " + distance.ToString();
            st[1] = bestF.ToString() + " " + matches.ToString();
            return st;
        }
        private List<string>[] openFile(string path)
        {
            using (AudioFileReader _reader = new AudioFileReader(path))
            {
                byte[] _buffer = new byte[_reader.Length];
                _reader.Read(_buffer, 0, _buffer.Length);
                double[] _sampleBuffer = read(_reader, _buffer);
                Complex[][] _results = Transform(_sampleBuffer, _reader.WaveFormat.SampleRate * 40 / 1000);
                return Determinate(_results, _reader.WaveFormat.SampleRate * 40 / 1000);
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
        public Complex[][] Transform(double[] _buffer, int _chunkSize)
        {
            int _totalSize = _buffer.Length;
            int _amountPossible = _totalSize / _chunkSize;
            Complex[][] results = new Complex[_amountPossible][];

            //Для всех кусков:
            for (int i = 0; i < _amountPossible; i++)
            {
                Complex[] _complex = new Complex[_chunkSize];
                for (int j = 0; j < _chunkSize; j++)
                {
                    _complex[j] = new Complex(_buffer[(i * _chunkSize) + j], 0);
                }
                results[i] = Fourier.fft(_complex);
            }
            return results;
        }
        private int getIndex(int _freq)
        {
            int i = 0;
            while (range[i] < _freq)
            {
                i++;
            }
            return i;
        }
        public List<string>[] Determinate(Complex[][] _results, int chunkSize)
        {
            List<string> hashes = new List<string>();
            List<string> freqs = new List<string>();
            double[] _highscores = new double[chunkSize];
            int[] _recordPoints = new int[chunkSize];

            for (int i = 0; i < _results.Length; ++i)
            {
                for (int _freq = 30; _freq < chunkSize - 1; ++_freq)
                {
                    //Получим силу сигнала
                    double _mag = Math.Log(Complex.Abs(_results[i][_freq]) + 1);

                    //Выясним, в каком мы диапазоне
                    int _index = getIndex(_freq);

                    //Сохраним самое высокое значение силы сигнала и соответствующую частоту
                    if (_mag > _highscores[_index])
                    {
                        _highscores[_index] = _mag;
                        _recordPoints[_index] = _freq;
                    }
                }
                //Составление хеша
                long _hash = hash(_recordPoints[0], _recordPoints[1], _recordPoints[2], _recordPoints[3]);
                freqs.Add(_recordPoints[0] + " " + _recordPoints[1] + " " + _recordPoints[2] + " " + _recordPoints[3]);
                if (_hash == 0)
                {
                    hashes.Insert(i, "00000000");
                }
                else
                {
                    hashes.Insert(i, Convert.ToString(_hash));
                }
                _highscores = new double[chunkSize];
                _recordPoints = new int[chunkSize];
            }
            return new List<string>[] { hashes, freqs };
        }
        private static readonly int FUZ_FACTOR = 2;
        private long hash(long _point1, long _point2, long _point3, long _point4)
        {
            return ((_point4 - (_point4 % FUZ_FACTOR)) * 100000000
                    + (_point3 - (_point3 % FUZ_FACTOR)) * 100000
                    + (_point2 - (_point2 % FUZ_FACTOR)) * 100
                    + (_point1 - (_point1 % FUZ_FACTOR)));
        }
    }
}
