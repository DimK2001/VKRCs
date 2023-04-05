using System;
using System.Collections.Generic;
using System.IO;

namespace VKRCs
{
    class DistanceSearch : ISearch
    {
        private long Distance = 10000000;
		private long countDistance(long _x, long _y)
		{
			return (long)(Math.Cbrt(Math.Abs(_x - _y)) * Math.Cbrt(Math.Abs(_x - _y)));
		}
		public string Search(List<string> _data)
		{
			// TODO: Открытие базы данных с сигнатурами
			string[] db = Directory.GetFiles(".\\DB");

			int found = 0;
			for (int i = 0; i < db.Length; ++i)
			{
				string pathFr = ".\\DB\\" + db[i];
				string[] readFr = File.ReadAllLines(pathFr);

				if (readFr.Length > _data.Count)
				{
					for (int j = 0; j < readFr.Length - _data.Count; ++j)
					{
						long dist = Find(readFr, _data.ToArray(), j);
						if (dist < Distance)
						{
							found = i;
							Distance = dist;
						}
					}
				}
				else
				{
					for (int j = 0; j < _data.Count - readFr.Length; ++j)
					{
						long dist = Find(_data.ToArray(), readFr, j);
						if (dist < Distance)
						{
							found = i;
							Distance = dist;
						}
					}
				}
			}
			return db[found];
		}

		public long Find(string[] _dataBig, string[] _dataSmall, int _startLine)
		{
			long res = 0;
			for (int f = 0; f < _dataSmall.Length; ++f)
			{
				string[] wordsS = _dataSmall[f].Split("\\s+");
				string[] wordsB = _dataBig[f + _startLine].Split("\\s+");
				for (int k = 0; k < 5; ++k)
				{
					res += countDistance(long.Parse(wordsS[k]), long.Parse(wordsB[k]));
				}
				if (res > Distance)
				{
					break;
				}
			}
			return res;
		}
	}
}
