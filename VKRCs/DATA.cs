using System.Collections.Generic;

namespace VKRCs
{
	public class DATA
	{
        //public static readonly int CHUNK_SIZE = 2048;
        public static readonly int[] RANGE = new int[] { 50, 80, 125, 250, 2049 };
        public static readonly int LOWER_LIMIT = 30;
	}

    class SongData
    {
        public string Name;
        public List<string> HashData;
        public List<string> FreqsData;
        public bool IsActive;
    }
}
