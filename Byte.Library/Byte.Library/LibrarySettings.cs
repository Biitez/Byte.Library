using Byte.Library.Utilities;
using Byte.Library.Utilities.Combos;
using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Byte.Library
{
    public class LibrarySettings : ListeningKeys
    {
        public LibrarySettings(string TitlePreffix, string TitleSuffix = null)
        {
            TitleManager.Preffix = TitlePreffix;

            if (!string.IsNullOrEmpty(TitleSuffix))
                TitleManager.Suffix = TitleSuffix;
        }

        public int Timeout { get; set; }
        public int Threads { get; set; }
        public Func<HttpRequest, Combo, HitControl> MethodCheck { get; set; }
        public ProxyType proxyType { get; set; }

        public Manager UtilsManager { get; set; }
        public ListeningKeys ListeningKeys { get; set; }
    }
}
