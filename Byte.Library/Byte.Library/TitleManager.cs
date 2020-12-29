using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Byte.Library
{
    class TitleManager
    {
        public static ConcurrentDictionary<long, long> CheckingCache = new ConcurrentDictionary<long, long>();
        public static int checkPerSec;
        internal static string Preffix { get; set; }
        internal static string Suffix { get; set; } = null;

        public static void Start()
        {
            UpdateTitle();
            CPMCounter();            
        }        

        public static void UpdateTitle()
        {
            Task.Factory.StartNew(delegate
            {
                while (ByteLibrary.CheckerStatus == CheckStatus.Checking)
                {
                    ChangeTitle();
                    Thread.Sleep(100);
                }

                ChangeTitle();
            });
        }

        public static void ChangeTitle()
        {
            if (string.IsNullOrEmpty(Preffix))
            {
                throw new Exception("You must include at least one prefix in the title.");
            }

            var Checkeds = ByteLibrary.GetHitEnc.Checkeds.ToString("N0");
            var Hits = ByteLibrary.GetHitEnc.Hit.ToString("N0");
            var Frees = ByteLibrary.GetHitEnc.Free.ToString("N0");
            var Fails = ByteLibrary.GetHitEnc.Fail.ToString("N0");
            var Retries = ByteLibrary.GetHitEnc.Retry.ToString("N0");

            var CPM = string.Format("CPM: {0}", GetChecksPerMinute());

            if (ByteLibrary.CheckerStatus == CheckStatus.Checking)
            {
                Console.Title = Preffix + " | Checkeds: " + Checkeds + " - Hits: " + Hits + " - Free: " + Frees + " - Fails: " + Fails + " - Retries: " + Retries + " | " + CPM + (!string.IsNullOrEmpty(Suffix) ? $" - {Suffix}" : null);
            }
            else
            {
                Console.Title = Preffix + " - Finished | Checkeds: " + Checkeds + " - Hits: " + Hits + " - Free: " + Frees + " - Fails: " + Fails + " - Retries: " + Retries;
            }
        }

        public static void CPMCounter()
        {
            Task.Factory.StartNew(delegate
            {
                while (ByteLibrary.CheckerStatus == CheckStatus.Checking)
                {
                    CheckingCache.TryAdd(GetCurrentUnixTime(), checkPerSec);
                    checkPerSec = 0;
                    Thread.Sleep(465);
                }
            });
        }

        public static long GetChecksPerMinute()
        {
            var num = 0L;
            foreach (var keyValuePair in CheckingCache)
            {
                var flag = keyValuePair.Key >= GetCurrentUnixTime() - 60L;
                if (flag) num += keyValuePair.Value;
            }

            return num;
        }

        public static long GetCurrentUnixTime()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

    }
}
