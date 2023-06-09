﻿using LiteDB;
using System;
using System.Collections.Generic;

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
					bool _cont = true;
                    if (_tags.Length > 0 && _tags[0] != "")
                    {
                        _cont = false;
                        for (int i = 0; i < _tags.Length; ++i)
						{
							for (int j = 0; j < _song.Tags.Length; ++j)
							{
								if (_tags[i] == _song.Tags[j])
								{
									_cont = true;
								}
							}
						}
					}

					if (_song.IsActive && _cont)
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
                }
			}
			if (_found.Name == null)
			{
                return "Ничего не найдено";
            }
			return _found.Name + " с разницей " + Distance;
		}

		public long Find(string[] _dataBig, string[] _dataSmall, int _startLine)
		{
			long _res = 0;
			for (int f = 0; f < _dataSmall.Length; ++f)
			{
				string[] _wordsS = _dataSmall[f].Split(' ');
				string[] _wordsB = _dataBig[f + _startLine].Split(' ');
				for (int k = 0; k < 4; ++k)
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
