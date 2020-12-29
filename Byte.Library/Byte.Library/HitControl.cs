using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Byte.Library
{
    public enum HitControl
    {
        HIT,
        FREE,
        RETRY,
        FAIL        
    }

    public class HitEnc
    {
        public int Hit { get; set; } = 0;
        public int Free { get; set; } = 0;
        public int Retry { get; set; } = 0;
        public int Fail { get; set; } = 0;

        public int Checkeds
        {
            get
            {
                return Hit + Free + Fail;
            }

        }
    }
}
