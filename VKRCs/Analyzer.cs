using System;
using System.Linq;
using NAudio.Wave;

namespace vkrC
{
    class Analyzer
    {
        public bool running = false;
        public int SearchType;
        public string Result = "Не найдено.";
        private ISearch search;

    }
}
