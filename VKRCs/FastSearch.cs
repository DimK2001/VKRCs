using System;
using System.Collections.Generic;
using System.IO;

namespace vkrC
{
    class FastSearch : ISearch
    {
		public string Search(List<string> _data)
		{
			// TODO: Открытие базы данных с сигнатурами
			string[] db = Directory.GetFiles(".\\HashDB");

			int found = 0;
			int matches = 0;
			for (int i = 0; i < db.Length; ++i)
			{
				////////////////////////////////////////////////////////// Открытие сигнатуры по хешам
				string pathHash = ".\\HashDB\\" + db[i];
				string[] readHash = File.ReadAllLines(pathHash);
				foreach (int match in Find(_data.ToArray(), readHash).Values)
				{
					if (match > matches)
					{
						matches = match;
						found = i;
					}
				}
			}
			return db[found];
		}
		public Dictionary<int, int> Find(string[] _data, string[] _readData)
		{
			////////////////////////////////////////////////////////// Быстрый поиск по смещению
			Dictionary<int, int> offset = new Dictionary<int, int>();
			for (int j = 0; j < _readData.Length; ++j)
			{
				for (int k = 0; k < _data.Length; ++k)
				{
					if (_readData[j] == _data[k])
					{
						if (!offset.ContainsKey(j - k))
						{
							offset.Add(j - k, 1);
						}
						else
						{
							offset.Add(j - k, offset[j - k] + 1);
						}
					}
				}
			}
			return offset;
		}
	}
}
