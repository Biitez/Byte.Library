using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Byte.Library
{
    public class ListeningKeys
    {
        public bool Enabled { get; set; } = true;

        public ListeningKeys(bool Enable)
        {
            this.Enabled = Enable;
        }
        public ListeningKeys() { }

        public ConsoleKey GetUncheckedLines { get; set; } = ConsoleKey.S; // default key = S
        public ConsoleKey GetCheckedLines { get; set; } = ConsoleKey.C; // default key = C
    }
}
