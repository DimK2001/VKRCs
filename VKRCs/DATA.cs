﻿using System.Collections.Generic;

namespace VKRCs
{
	public class DATA
	{
        //public static readonly int CHUNK_SIZE = 2048;
        public static readonly int[] RANGE = new int[] { 64, 400, 1300, 8192};
        public static readonly int LOWER_LIMIT = 30;
	}

    class SongData
    {
        public string Name { get; set; }
        public List<string> HashData { get; set; }
        public List<string> FreqsData { get; set; }
        public bool IsActive { get; set; }
        public string[] Tags { get; set; }
    }
}
