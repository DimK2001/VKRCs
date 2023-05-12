using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VKRCs
{
    class DistanceSearch : ISearch
    {
        private long Distance = 10000000;
		private long countDistance(long _x, long _y)
		{
			double f = Math.Cbrt(Math.Abs(_x - _y));

            return (long)(f * f);
		}
		public string Search(List<string> _data, string[] _tags)
		{
            //Открытие базы данных с сигнатурами
            SongData _found = new SongData();
			using (var db = new LiteDatabase(".\\MyData.db"))
			{
				var _col = db.GetCollection<SongData>("songs");
				var _songs = _col.FindAll();
				foreach (SongData _song in _songs)
				{
					if (_song.IsActive)
					{
						if (_song.FreqsData.Count > _data.Count)
						{
							for (int j = 0; j < _song.FreqsData.Count - _data.Count; ++j)
							{
								long dist = Find(_song.FreqsData.ToArray(), _data.ToArray(), j);
								if (dist < Distance)
								{
									_found = _song;
									Distance = dist;
								}
							}
						}
						else
						{
							for (int j = 0; j < _data.Count - _song.FreqsData.Count; ++j)
							{
								long dist = Find(_data.ToArray(), _song.FreqsData.ToArray(), j);
								if (dist < Distance)
								{
									_found = _song;
									Distance = dist;
								}
							}
						}
					}
                    else
                    {
                        return "Ничего не найдено";
                    }
                }
			}
			if (_found.Name == null)
			{
                return "Ничего не найдено";
            }
			return _found.Name + " с разницей " + Distance;

            /*string[] db = Directory.GetFiles(".\\DB");

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
			return db[found];*/
		}

		public long Find(string[] _dataBig, string[] _dataSmall, int _startLine)
		{
			long _res = 0;
			for (int f = 0; f < _dataSmall.Length; ++f)
			{
				string[] _wordsS = _dataSmall[f].Split(' ');
				string[] _wordsB = _dataBig[f + _startLine].Split(' ');
				for (int k = 0; k < 5; ++k)
				{
					_res += countDistance(long.Parse(_wordsS[k]), long.Parse(_wordsB[k]));
				}
				if (_res > Distance)
				{
					break;
				}
			}
			return _res;
		}
	}
}
