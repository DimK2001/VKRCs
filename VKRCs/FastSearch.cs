using LiteDB;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace VKRCs
{
    class FastSearch : ISearch
    {
		public string Search(List<string> _data)
		{
            /*
            string[] _db = Directory.GetFiles(".\\HashDB");

			int _found = 0;
			int _matches = 0;
			for (int i = 0; i < _db.Length; ++i)
			{
				////////////////////////////////////////////////////////// Открытие сигнатуры по хешам
				string _pathHash = ".\\HashDB\\" + _db[i];
				string[] _readHash = File.ReadAllLines(_pathHash);
				foreach (int _match in Find(_data.ToArray(), _readHash).Values)
				{
					if (_match > _matches)
					{
						_matches = _match;
						_found = i;
					}
				}
			}
			return _db[_found];*/

            //Открытие базы данных с сигнатурами
            SongData _found = new SongData();
            int _matches = 0;
            using (var db = new LiteDatabase(".\\MyData.db"))
            {
                // Получить коллекцию (или создать)
                var _col = db.GetCollection<SongData>("songs");
                var _songs = _col.FindAll();
				foreach (SongData _song in _songs)
				{
					if (_song.IsActive)
					{
						foreach (int _match in Find(_data.ToArray(), _song.HashData.ToArray()).Values)
						{
							if (_match > _matches)
							{
								_matches = _match;
								_found = _song;
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
			return _found.Name;
        }
		public Dictionary<int, int> Find(string[] _data, string[] _readData)
		{
			////////////////////////////////////////////////////////// Быстрый поиск по смещению
			Dictionary<int, int> _offset = new Dictionary<int, int>();
			for (int j = 0; j < _readData.Length; ++j)
			{
				for (int k = 0; k < _data.Length; ++k)
				{
					if (_readData[j] == _data[k])
					{
						if (!_offset.ContainsKey(j - k))
						{
							_offset.Add(j - k, 1);
						}
						else
						{
							_offset.Add(j - k, _offset[j - k] + 1);
						}
					}
				}
			}
			return _offset;
		}
	}
}
