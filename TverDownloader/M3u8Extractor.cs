using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Collections;
using System.Text.RegularExpressions;

namespace TverDownloader
{
    class M3u8Extractor
    {
        private string extractUrl;
        private ChromeDriver driver;

        public M3u8Extractor(string url)
        {
            driver = null;
            extractUrl = url;
        }

        private void GetDriver(bool isIpad = false)
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            ChromeOptions option = new ChromeOptions();
            option.AddArgument("--headless --mute-audio");
            //option.AddArgument("--mute-audio");

            if (isIpad)
            {
                const string AGENT = "Mozilla/5.0 (iPad; CPU OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Version/10.0 Mobile/14G60 Safari/602.1";
                option.AddArgument("--user-agent=" + AGENT);
            }

            driver = new ChromeDriver(driverService, option);
        }

        public void DestroyDriver()
        {
            if (driver != null)
                driver.Quit();
            driver = null;
        }

        public Informations GetInformations()
        {
            Informations info = new Informations();
            
            if (IsUrlValid())
            {
                if (driver == null)
                    GetDriver();

                Console.WriteLine(driver.SessionId);

                driver.Url = extractUrl;
                driver.Navigate();

                try
                {
                    string _temp = driver.FindElementByCssSelector(".title span.tv").Text;
                    info.department = Regex.Match(_temp, @"^(.*)?　").Groups[1].ToString();
                    if (info.department == "")
                    {
                        info.department = _temp;
                        info.date = driver.FindElementByCssSelector(".title span.red").Text;
                    } else
                    {
                        info.date = Regex.Match(_temp, @"\d.*$").ToString();
                    }

                    info.title = driver.FindElementByCssSelector(".title h1").Text;
                    info.subtitle = driver.FindElementByCssSelector(".title p span.summary").Text;
                } catch (NoSuchElementException e)
                {
                    Console.WriteLine(e.Message);
                    info = null;
                }
            }

            return info;
        }

        private IList GetNetworkData()
        {
            const int SLEEP = 1500;
            object networkData = null;

            if (IsUrlValid())
            {
                if (driver == null)
                    GetDriver();

                driver.Url = extractUrl;
                driver.Navigate();

                System.Threading.Thread.Sleep(SLEEP);

                string script = "var performance = window.performance ||" +
                                "window.mozPerformance || window.msPerformance ||" +
                                "window.webkitPerformance || {}; " +
                                "var network = performance.getEntries() || {}; return network;";
                networkData = ((IJavaScriptExecutor)driver).ExecuteScript(script);
            }

            return (IList)networkData;
        }

        public string GetM3u8Url(bool isFOD = false)
        {
            string url = "";

            if (isFOD)
            {
                url = GetFODM3u8Url();
            } else
            {
                foreach (object data in GetNetworkData())
                {
                    foreach (DictionaryEntry entry in (IDictionary)data)
                    {
                        if (entry.Key.Equals("name"))
                        {
                            string _url = (string)entry.Value;
                            if (_url.Contains("master.m3u8") || _url.Contains(".m3u8?__nn__"))
                                url = _url;
                        }
                    }
                }
            }

            return url;
        }

        private string GetFODM3u8Url()
        {
            string url = "";
            bool isIpad = true;

            if (driver != null)
            {
                try
                {
                    DestroyDriver();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                GetDriver(isIpad);

                driver.Url = extractUrl;
                driver.Navigate();

                try
                {
                    driver.Url = driver.FindElementByCssSelector(".playbtn > a").GetAttribute("href");
                    driver.Navigate();

                    url = driver.FindElementByCssSelector("#ContentMain_lnkView1").GetAttribute("href");
                    url = Regex.Match(url, @"http.*?m3u8").ToString();
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            DestroyDriver();

            return url;
        }

        public bool IsUrlValid()
        {
            return extractUrl.Contains("https://tver.jp/") ? true : false;
        }
    }
}
