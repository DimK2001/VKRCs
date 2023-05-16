using System;
using System.Collections.Generic;
using System.Numerics;

namespace VKRCs
{
	class Determinator
	{
		private int chunkSize = 2048;
        private List<string> hashes = new List<string>();
		private List<string> freqs = new List<string>();

		public Determinator(int _size)
		{
			chunkSize = _size;
		}

		// Функция для определения того, в каком диапазоне находится частота
		private int getIndex(double _freq)
		{
			int i = 0;
			while (DATA.RANGE[i] < _freq)
			{
				i++;
			}
			return i;
		}

		public List<string>[] Determinate(Complex[][] _results)
		{
			double[] _highscores = new double[chunkSize];
			int[] _recordPoints = new int[chunkSize];

			for (int i = 0; i < _results.Length; ++i)
			{
				for (int _freq = DATA.LOWER_LIMIT; _freq < chunkSize - 1; ++_freq)
				{
					//Получим силу сигнала
					double _mag = Math.Log(Complex.Abs(_results[i][_freq]) + 1);
                    //double _mag = Complex.Abs(Math.Sqrt(Math.Pow(_results[i][_freq].Real, 2) + Math.Pow(_results[i][_freq].Imaginary, 2)));
                    
					//Приравняем разные частоты дискретизации к 44100
                    double _f = _freq * 441 * 4 / _results[i].Length;
                    //Выясним, в каком мы диапазоне
                    int _index = getIndex(_f);

					//Сохраним самое высокое значение силы сигнала и соответствующую частоту
					if (_mag > _highscores[_index])
					{
						_highscores[_index] = _mag;
						_recordPoints[_index] = Convert.ToInt32(_f);
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
			return (  (_point4 - (_point4 % FUZ_FACTOR)) * 100000000
					+ (_point3 - (_point3 % FUZ_FACTOR)) * 100000
					+ (_point2 - (_point2 % FUZ_FACTOR)) * 100
					+ (_point1 - (_point1 % FUZ_FACTOR)));
		}
	}
}
