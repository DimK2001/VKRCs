using System;
using System.Collections.Generic;
using System.Numerics;

namespace vkrC
{
	class Determinator
	{
		public readonly int[] RANGE = new int[] { 65, 260, 1025, 1035, DATA.CHUNK_SIZE + 1 };
		private List<string> hashes = new List<string>();
		private List<string> freqs = new List<string>();

		// Функция для определения того, в каком диапазоне находится частота
		private int getIndex(int _freq)
		{
			int i = 0;
			while (RANGE[i] < _freq)
			{
				i++;
			}
			return i;
		}

		public List<string>[] Determinate(Complex[][] _results)
		{
			double[] highscores = new double[DATA.CHUNK_SIZE];
			int[] recordPoints = new int[DATA.CHUNK_SIZE];

			for (int i = 0; i<_results.Length; ++i)
			{
				for (int freq = DATA.LOWER_LIMIT; freq<DATA.CHUNK_SIZE - 1; ++freq)
				{
					//Получим силу сигнала
					double mag = Math.Log(Complex.Abs(_results[i][freq]) + 1);

					//Выясним, в каком мы диапазоне
					int index = getIndex(freq);

					//Сохраним самое высокое значение силы сигнала и соответствующую частоту
					if (mag > highscores[index])
					{
						highscores[index] = mag;
						recordPoints[index] = freq;
					}
				}
				//Составление хеша
				long h = hash(recordPoints[0], recordPoints[1], recordPoints[2], recordPoints[3], recordPoints[4]);
				freqs.Add(recordPoints[0] + " " + recordPoints[1] + " " + recordPoints[2] + " " + recordPoints[3] + " " + recordPoints[4]);
				if (h == 0)
				{
					hashes.Insert(i, "00000000000");
				}
				else
				{
					hashes.Insert(i, Convert.ToString(h));
				}
				highscores = new double[DATA.CHUNK_SIZE];
				recordPoints = new int[DATA.CHUNK_SIZE];
			}
			return new List<string>[] { hashes, freqs };
		}
		private static readonly int FUZ_FACTOR = 2;
		private long hash(long _point1, long _point2, long _point3, long _point4, long _point5)
		{
			return ((_point5 - (_point5 % FUZ_FACTOR)) * 100000000
					+ (_point4 - (_point4 % FUZ_FACTOR)) * 1000000
					+ (_point3 - (_point3 % FUZ_FACTOR)) * 10000
					+ (_point2 - (_point2 % FUZ_FACTOR)) * 100
					+ (_point1 - (_point1 % FUZ_FACTOR)));
		}
	}
}
