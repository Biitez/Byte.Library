using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Byte.Library.Utilities
{
    public class Manager : ByteLibrary
    {
        public Manager UploadCombo(string PathCombo)
        {
            List<string> combos = File.ReadAllLines(PathCombo).Distinct().ToList();
            UploadCombo(combos);

            return this;
        }

        public Manager UploadProxies(string pathProxies)
        {
            List<string> proxies = File.ReadAllLines(pathProxies).Distinct().ToList();
            UploadProxy(proxies);

            return this;
        }
    }
}
