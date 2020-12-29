using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Byte.Library.Utilities.Proxies
{
    public class Proxies
    {
        public string host { get; set; }
        public string port { get; set; }

        public string username { get; set; }
        public string password { get; set; }

        public Status status { get; set; }
        public bool haveCredentials { get; set; } = false;

        public Proxies(string completeLine)
        {
            if (!completeLine.Contains('.') || !completeLine.Contains(':'))
            {
                status = Status.InvalidProxy;
                return;
            }

            try
            {
                host = completeLine.Split(':')[0];
                port = completeLine.Split(':')[1];

                if (completeLine.Split(':').Length == 4)
                {
                    haveCredentials = true;

                    username = completeLine.Split(':')[2];
                    password = completeLine.Split(':')[3];
                }

                status = Status.Valid;
            }
            catch
            {
                status = Status.InvalidProxy;
                return;
            }
        }
    }
}
