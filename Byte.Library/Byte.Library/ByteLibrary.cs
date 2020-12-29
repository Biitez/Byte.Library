using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Byte.Library.Utilities.Combos;
using Leaf.xNet;

namespace Byte.Library
{
    public class ByteLibrary
    {
        internal List<Combo> comboList = new List<Combo>();
        internal List<Utilities.Proxies.Proxies> proxyList = new List<Utilities.Proxies.Proxies>();
        private TitleManager titleManager { get; set; }

        internal HitControl GetHitControl { get; set; }
        internal static HitEnc GetHitEnc = new HitEnc();

        internal static CheckStatus CheckerStatus { get; set; }

        private HttpRequest RequestSettings = new HttpRequest();
        public LibrarySettings librarySettings { get; set; }
        public Thread threadCheck { get; set; }
        public ByteLibrary() { }

        public ByteLibrary(LibrarySettings Settings)
        {
            librarySettings = Settings;
            comboList = Settings.UtilsManager.comboList;
            proxyList = Settings.UtilsManager.proxyList;
        }

        public ByteLibrary Start()
        {
            CheckerStatus = CheckStatus.Checking;
            ConsoleManager();

            for (int i = 0; i < librarySettings.Threads; i++)
            {
                new Thread(new ThreadStart(ControlThread))
                {
                    IsBackground = true
                }.Start();
            }

            return this;
        }

        private void ControlThread()
        {
            var acc = GetRandomAccount();

            try
            {
                Interlocked.Increment(ref TitleManager.checkPerSec);

                UpdateCombo(acc, Status.Checked);
                var proxyInfo = GetRandomProxy();

                RequestSettings.ConnectTimeout = librarySettings.Timeout;


                /*
                 * Usually this configuration is used, 
                 * If you need to remove it or make it optional, 
                 * create the issue in the github post and I will do it.
                 */

                RequestSettings.IgnoreProtocolErrors = true;
                RequestSettings.AllowAutoRedirect = true;

                RequestSettings.UserAgentRandomize();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                RequestSettings.SslCertificateValidatorCallback = (RemoteCertificateValidationCallback)Delegate.Combine(RequestSettings.SslCertificateValidatorCallback,
                new RemoteCertificateValidationCallback((object obj, X509Certificate cert, X509Chain ssl, SslPolicyErrors error) => (cert as X509Certificate2).Verify()));

                RequestSettings.Proxy = ProxyClient.Parse(librarySettings.proxyType, $"{proxyInfo.host}:{proxyInfo.port}");


                if (proxyInfo.haveCredentials) // In case the proxy has username:password authentication
                {
                    RequestSettings.Proxy.Username = proxyInfo.username;
                    RequestSettings.Proxy.Password = proxyInfo.password;
                }

                HitControl hitControl = librarySettings.MethodCheck.Invoke(RequestSettings, acc);

                if (hitControl == HitControl.HIT)
                {
                    GetHitEnc.Hit++;
                }
                else if (hitControl == HitControl.FREE)
                {
                    GetHitEnc.Free++;
                }
                else if (hitControl == HitControl.FAIL)
                {
                    GetHitEnc.Fail++;
                }
                else if (hitControl == HitControl.RETRY)
                {
                    GetHitEnc.Retry++;
                    UpdateCombo(acc, Status.Unchecked);
                }

            }
            catch
            {
                UpdateCombo(acc, Status.Unchecked);
            }
        }

        private void ConsoleManager()
        {
            ListeningKeys();
            TitleManager.Start();
        }

        internal void ListeningKeys()
        {
            if (librarySettings.ListeningKeys.Enabled)
            {
                Task.Factory.StartNew(delegate
                {
                    while (true)
                    {
                        if (Console.KeyAvailable)
                        {
                            Task.Factory.StartNew(delegate
                            {
                                var consoleKeyInfo = Console.ReadKey(true);

                                if (consoleKeyInfo.Key == librarySettings.ListeningKeys.GetUncheckedLines)
                                {
                                    Console.WriteLine("Saving un-checked lines ...");

                                    File.AppendAllLines("UncheckedLines.txt", GetUncheckedLines());

                                    Console.WriteLine("Saved");
                                }

                                if (consoleKeyInfo.Key == librarySettings.ListeningKeys.GetCheckedLines)
                                {
                                    Console.WriteLine("Saving checked lines ...");

                                    File.AppendAllLines("CheckedLines.txt", GetCheckedLines());

                                    Console.WriteLine("Saved");
                                }

                            });
                        }

                        Thread.Sleep(365);
                    }
                });
            }
        }

        internal void UploadCombo(List<string> accounts)
        {
            lock (comboList)
            {
                comboList = accounts.Select(x => new Combo(x, Status.Unchecked)).Where(a => a.status != Status.InvalidFormat).ToList();
            }
        }

        internal void UploadProxy(List<string> proxies)
        {
            lock (proxyList)
            {
                foreach (var i in proxies.Select(x => new Utilities.Proxies.Proxies(x)).Where(a => a.status == Utilities.Proxies.Status.Valid))
                {
                    proxyList.Add(i);
                }
            }
        }

        public Combo GetRandomAccount()
        {
            lock (comboList)
            {

                var account = comboList.OrderBy(x => Guid.NewGuid()).Where(x => x.status == Status.Unchecked).FirstOrDefault();
                UpdateCombo(account, Status.Checked);

                return account;
            }
        }

        public void UpdateCombo(Combo account, Status status)
        {
            lock (comboList)
            {
                comboList.Remove(account);
                comboList.Add(new Combo($"{account.email}:{account.password}", status));
            }
        }
        public List<string> GetUncheckedLines()
        {
            lock (comboList)
            {
                return comboList.Where(x => x.status == Status.Unchecked).Select(x => $"{x.email}:{x.password}").Distinct().ToList();
            }            
        }

        public List<string> GetCheckedLines()
        {
            lock (comboList)
            {
                return comboList.Where(x => x.status == Status.Checked).Select(x => $"{x.email}:{x.password}").Distinct().ToList();
            }
        }

        public List<string> GetAllTheCombo()
        {
            lock (comboList)
            {
                return comboList.Select(x => $"{x.email}:{x.password}").Distinct().ToList();
            }
        }

        public Utilities.Proxies.Proxies GetRandomProxy()
        {
            return proxyList[new Random().Next(proxyList.Count)];
        }

        public int comboCount() => comboList.Count;
    }
}
