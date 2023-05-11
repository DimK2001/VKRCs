using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKRCs
{
    interface ISearch
    {
        public string Search(List<String> _data, string[] _tags);
    }
}
