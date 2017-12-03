using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Threading.Tasks;

namespace ParseStatic
{
    enum TypeStage {Predseson=1,Season,AllStar,PlayOFF}//типы игр в индексе
    
    struct IndexoftheMatch//индекс матча на сайте
    {
        private TypeStage type;
        private int season;
        private int index;
        public string IndexGame { get; private set;}
        public IndexoftheMatch(TypeStage ts, int _season, int _index)//создание индексов
        {
            type = ts;
            if (_season < 1983 || _season>DateTime.Now.Year) throw DataExeption.DataProcessingExeptions.ErrorFormatIndex("Invalid year of index(year should be more 1983)");
            season = _season;
            if (type==TypeStage.Season)
                if (!Default.Usual.SwitchinRangeCountGame(_index,_season,type)) throw DataExeption.DataProcessingExeptions.ErrorFormatIndex($"Invalid number game of index(game {_index.ToString()}) in year {_season} dont dound");
            index = _index;
            string template = index.ToString();
            while (template.Length != 5) template = '0' + template;
            IndexGame = "00" + ((int)type).ToString() + (_season % 100).ToString()+template;
        }
        public IndexoftheMatch(string _index)//создание индексов 
        {
            if (_index.Length != 10) throw DataExeption.DataProcessingExeptions.ErrorFormatIndex("Invalid year of index(index is ten symbols)");
            int _typestage = Convert.ToInt32(_index[2].ToString());
            type = (TypeStage)_typestage;
            if (_typestage<1 || _typestage>4) throw DataExeption.DataProcessingExeptions.ErrorFormatIndex("Invalid year of index(false type stage)");
            int _season = Convert.ToInt32(_index[3].ToString() + _index[4].ToString());
            if (_season <= DateTime.Now.Year - 2000) _season += 2000;
            else _season += 1900;
            if (_season<1983 || _season>DateTime.Now.Year) throw DataExeption.DataProcessingExeptions.ErrorFormatIndex("Invalid year of index(year should be more 1983)");
            season = _season;
            int ind = Convert.ToInt32(_index[5].ToString()+_index[6].ToString()+ _index[7].ToString() + _index[8].ToString() + _index[9].ToString());
            if (type == TypeStage.Season)
                if (!Default.Usual.SwitchinRangeCountGame(ind,_season,type)) throw DataExeption.DataProcessingExeptions.ErrorFormatIndex($"Invalid number game of index(game {ind.ToString()}) in year {_season} dont found");
            index = ind;
            IndexGame = _index;
        }
        public void Increment()//инкримент
        {
            index++;
            if (type == TypeStage.Season)
                if (!Default.Usual.SwitchinRangeCountGame(index, season,type)) throw DataExeption.DataProcessingExeptions.ErrorFormatIndex($"Invalid number game of index(game {index.ToString()}) in year {season} dont dound");
            string template = index.ToString();
            while (template.Length != 5) template = '0' + template;
            IndexGame = "00" + ((int)type).ToString() + (season % 100).ToString() + template;
        }
        public int CompareTo(IndexoftheMatch _index)//сравнение идексов
        {
            if (_index.type != type) throw DataExeption.DataProcessingExeptions.ErrorCompare("It is impossible to compare indices of different types");
            if (_index.season != season) throw DataExeption.DataProcessingExeptions.ErrorCompare("It is impossible to compare indices of different seasons");
            return index.CompareTo(_index.index);
        }
        public int Subtraction(IndexoftheMatch _index)//вычетание
        {
            if (_index.type != type) throw DataExeption.DataProcessingExeptions.ErrorCompare("It is impossible to subtract indices of different types");
            if (_index.season != season) throw DataExeption.DataProcessingExeptions.ErrorCompare("It is impossible to subtract indices of different seasons");
            return (index - _index.index);
        }
    }

    static class GetStatic//Класс для получения данных с сайта
    {
        public delegate void Progress(int _progress);//делегат для сообщения прогресса в ассинхронных процесах
        public delegate void Answer(string[] _answer,int id_queue);//делигат для вывода результата ассихронного процесса

        private static int MinYearoftheGame = 1983;//минимальная возможная дата(год)

        public static string[] GetListMatchinData(DateTime dataTime)//Список матчей в опеделенныею дату
        {
            if (dataTime.Year < MinYearoftheGame) throw DataExeption.DataProcessingExeptions.ErrorWriteData("Invalid date");
            var ChromeService = ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory());
            ChromeService.HideCommandPromptWindow = true;//скрываем консоль веб драйвера
            var WebBrowser = new ChromeDriver(ChromeService); //Иницализация веб драйвера Chrome
            string url = $"http://stats.nba.com/scores/{dataTime.Month.ToString()}/{dataTime.Day.ToString()}/{dataTime.Year.ToString()}";
            WebBrowser.Navigate().GoToUrl(url);
            IEnumerator<IWebElement> ListFindingElements;
            if (dataTime.Date.CompareTo(DateTime.Now.Date) < 0)
                ListFindingElements = WebBrowser.FindElements(By.CssSelector("div.linescores-container span.team-name")).GetEnumerator();
            else ListFindingElements = WebBrowser.FindElements(By.CssSelector("div.linescores-container span.team-abbrv")).GetEnumerator();
            List<string> ListMatch = new List<string>();
            ListFindingElements.Reset();
            while (ListFindingElements.MoveNext())
            {
                string temporary = ListFindingElements.Current.Text;
                if (!ListFindingElements.MoveNext())
                {
                    ListFindingElements.Dispose();
                    WebBrowser.Quit();
                    WebBrowser.Dispose();
                    ChromeService.Dispose();
                    throw DataExeption.DataProcessingExeptions.ErrorWriteData("Сould not read data from the site");
                }
                ListMatch.Add(temporary + '\t' + ListFindingElements.Current.Text);
            }
            ListFindingElements.Dispose();
            WebBrowser.Quit();
            WebBrowser.Dispose();
            ChromeService.Dispose();
            return ListMatch.ToArray();
        }

        private static Task<string[]> GetListMatchinDataHide(DateTime dataTime)//ассинхронный метод чтения данных(скрытый)
        {
            return Task.Run(() => GetListMatchinData(dataTime));
        }

        public static async void GetListMatchinDataAcync(DateTime dt ,Answer _answ=null,int id_queue=-1,Progress _prog = null)//ассинхронный метод чтения списка матчей по дате
        {
            _prog?.Invoke(0);
            var output = await GetListMatchinDataHide(dt);
            _prog?.Invoke(100);
            _answ?.Invoke(output,id_queue);
        }

        public static string[] GetListIndexMatchinData(DateTime dataTime)//получение индексов матчей в определенную дату
        {
            if (dataTime.Year < MinYearoftheGame) throw DataExeption.DataProcessingExeptions.ErrorWriteData("Invalid date");
            var ChromeService = ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory());
            ChromeService.HideCommandPromptWindow = true;//скрываем консоль веб драйвера
            var WebBrowser = new ChromeDriver(ChromeService); //Иницализация веб драйвера Chrome
            string url = $"http://stats.nba.com/scores/{dataTime.Month.ToString()}/{dataTime.Day.ToString()}/{dataTime.Year.ToString()}";
            WebBrowser.Navigate().GoToUrl(url);
            IEnumerator<IWebElement> ListFindingElementsIndex, ListFindingElementsMatch;

            if (dataTime.Date.CompareTo(DateTime.Now.Date) < 0)
            {
                ListFindingElementsIndex = WebBrowser.FindElementsByLinkText("Box Score").GetEnumerator();
                ListFindingElementsMatch = WebBrowser.FindElements(By.CssSelector("div.linescores-container span.team-name")).GetEnumerator();
            }
            else
            {
                WebBrowser.Quit();
                ChromeService.Dispose();
                WebBrowser.Dispose();
                throw DataExeption.DataProcessingExeptions.ErrorWriteData("Invalid date");
            }
            List<string> ListMatch = new List<string>();
            ListFindingElementsIndex.Reset();
            ListFindingElementsMatch.Reset();
            while (ListFindingElementsMatch.MoveNext())
            {
                string temporary = ListFindingElementsMatch.Current.Text;
                if (!ListFindingElementsMatch.MoveNext()) throw DataExeption.DataProcessingExeptions.ErrorWriteData("Could not read data from the site");
                temporary += '\t' + ListFindingElementsMatch.Current.Text;
                if (!ListFindingElementsIndex.MoveNext()) throw DataExeption.DataProcessingExeptions.ErrorWriteData("Could not read data from the site");
                temporary += '\t' + Default.TextWorker.Repalase(ListFindingElementsIndex.Current.GetAttribute("href"),
                new string[] { "http://stats.nba.com/game/", "/" }, "");
                ListMatch.Add(temporary);
            }
            WebBrowser.Quit();
            ListFindingElementsIndex.Dispose();
            ListFindingElementsMatch.Dispose();
            WebBrowser.Dispose();
            ChromeService.Dispose();
            return ListMatch.ToArray();
        }

        private static Task<string[]> GetListIndexMatchinDataHide(DateTime dataTime)//ассинхронный метод получене индексов(скрытый)
        {
            return Task.Run(() => GetListIndexMatchinData(dataTime));
        }

        public static async void GetListIndexMatchinDataAcync(DateTime dt, Answer _answ = null, int id_queue = -1, Progress _prog = null)////ассинхронный метод чтения списка индексов матчей по дате
        {
            _prog?.Invoke(0);
            var output = await GetListIndexMatchinDataHide(dt);
            _prog?.Invoke(100);
            _answ?.Invoke(output, id_queue);
        }

        private static string[] GetStaticinIndex(IndexoftheMatch _index, ChromeDriver chrome)//получение показателей матча
        {
            chrome.Navigate().GoToUrl($"http://stats.nba.com/game/{_index.IndexGame}/");
            IReadOnlyCollection<IWebElement> ListNamesTeam;
            IEnumerator<IWebElement> ListIndecatorsTeam;
            ListNamesTeam = chrome.FindElements(By.CssSelector("div.game-summary-team__name a"));
            ListIndecatorsTeam = chrome.FindElements(By.CssSelector("div.nba-stat-table__overflow tfoot td")).GetEnumerator();
            if (ListNamesTeam.Count != 2) throw DataExeption.DataProcessingExeptions.ErrorWriteData("Reading names teams");
            string names = "";
            foreach (var _name in ListNamesTeam) names += _name.Text + '\t';
            List<string> indecatorsteam = new List<string>();
            while (ListIndecatorsTeam.MoveNext())
                if (ListIndecatorsTeam.Current.Text == "Totals:")
                {
                    if (!ListIndecatorsTeam.MoveNext())
                    {
                        chrome.Quit();
                        chrome.Dispose();
                        ListIndecatorsTeam.Dispose();
                        throw DataExeption.DataProcessingExeptions.ErrorWriteData("Reading indecators");
                    }
                    indecatorsteam.Add(Default.Usual.ReadIndecatorsTeam(ListIndecatorsTeam));
                }
            if (indecatorsteam.Count != 2)
            {
                chrome.Quit();
                chrome.Dispose();
                ListIndecatorsTeam.Dispose();
                throw DataExeption.DataProcessingExeptions.ErrorWriteData("Reading indecators");
            }
            ListIndecatorsTeam.Dispose();
            return new string[] { names, indecatorsteam[0], indecatorsteam[1] };
        }

        private static Task<string[]> GetStaticinIndexHide(IndexoftheMatch _index, ChromeDriver chrome)//срытый ассинхорный получение показателей матча
        {
            return Task.Run(() => GetStaticinIndex(_index, chrome));
        }
        
        public static string[] GetStaticinIndex(IndexoftheMatch beginindex)//получение показателей по индексу
        {
            var ChromeService = ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory());
            ChromeService.HideCommandPromptWindow = true;//скрываем консоль веб драйвера
            var WebBrowser = new ChromeDriver(ChromeService); //Иницализация веб драйвера Chrome
            var output = GetStaticinIndex(beginindex, WebBrowser);
            WebBrowser.Quit();
            WebBrowser.Dispose();
            ChromeService.Dispose();
            return output;
        }

        public static async void GetStaticinIndexAsync(IndexoftheMatch beginindex, Answer _answ = null, int id_queue = -1, Progress _prog = null)//получение показателей по индексу(ассинхронный)
        {
            var ChromeService = ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory());
            ChromeService.HideCommandPromptWindow = true;//скрываем консоль веб драйвера
            var WebBrowser = new ChromeDriver(ChromeService); //Иницализация веб драйвера Chrome
            _prog?.Invoke(0);
            var output = await GetStaticinIndexHide(beginindex, WebBrowser);
            _prog?.Invoke(100);
            WebBrowser.Quit();
            WebBrowser.Dispose();
            ChromeService.Dispose();
            _answ?.Invoke(output,id_queue);
        }

        public static string[] GetStaticinIndex(IndexoftheMatch beginindex, IndexoftheMatch endindex, Progress _progress=null)// считывание матчей от индекса а до б
        {
            if (beginindex.CompareTo(endindex) == 1) throw DataExeption.DataProcessingExeptions.ErrorWriteData("Initial index is greater than the final");
            var ChromeService = ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory());
            ChromeService.HideCommandPromptWindow = true;//скрываем консоль веб драйвера
            var WebBrowser = new ChromeDriver(ChromeService); //Иницализация веб драйвера Chrome
            int partproc = endindex.Subtraction(beginindex)+1;
            var output = new List<string>();
            _progress(0);
            while (beginindex.CompareTo(endindex) != 1)
            {
                output.AddRange(GetStaticinIndex(beginindex, WebBrowser));
                _progress?.Invoke((100 - endindex.Subtraction(beginindex) * 100 / partproc));
                beginindex.Increment();
            }
            WebBrowser.Quit();
            WebBrowser.Dispose();
            ChromeService.Dispose();
            return output.ToArray();
        }

        public async static void GetStaticinIndexAsync(IndexoftheMatch beginindex, IndexoftheMatch endindex, Answer _answ = null, int id_queue = -1, Progress _prog = null)// считывание матчей от индекса а до б(accинхронный)
        {
            if (beginindex.CompareTo(endindex) == 1) throw DataExeption.DataProcessingExeptions.ErrorWriteData("Initial index is greater than the final");
            var ChromeService = ChromeDriverService.CreateDefaultService(System.IO.Directory.GetCurrentDirectory());
            ChromeService.HideCommandPromptWindow = true;//скрываем консоль веб драйвера
            var WebBrowser = new ChromeDriver(ChromeService); //Иницализация веб драйвера Chrome
            int partproc = endindex.Subtraction(beginindex) + 1;
            var output = new List<string>();
            _prog(0);
            while (beginindex.CompareTo(endindex) != 1)
            {
                output.AddRange(await GetStaticinIndexHide(beginindex, WebBrowser));
                _prog?.Invoke((100 - endindex.Subtraction(beginindex) * 100 / partproc));
                beginindex.Increment();
            }
            WebBrowser.Quit();
            WebBrowser.Dispose();
            ChromeService.Dispose();
            _answ?.Invoke(output.ToArray(),id_queue);
        }

       /* public static string[] GetStaticinData(DateTime dt, Progress progress=null)//получение всех показателей всех матчей в определеную дату
        {

        }*/
    }
}
