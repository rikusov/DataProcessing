namespace Default
{
    static class Usual
    {
        //перечесление хранешее количество матчей в временой отрезок
        enum CountGame { EightyThreeEightySeven = 943, EightyEight = 1025, EightyNineNinetyFoure = 1107, NinetyFiveZeroThree = 1189, Eleven = 990, Other = 1230 }

        public static bool SwitchinRangeCountGame(int value,int season, ParseStatic.TypeStage type)//проверяем на сушествование индекс матча
        {
            if (type == ParseStatic.TypeStage.Season)
            {
                if (value < 0) return false;
                if (season > 1982 && season < 1988) return (value <= (int)CountGame.EightyThreeEightySeven);
                if (season == 1988) return (value <= (int)CountGame.EightyEight);
                if (season > 1988 && season < 1995) return (value <= (int)CountGame.EightyNineNinetyFoure);
                if (season > 1994 && season < 2003) return (value <= (int)CountGame.NinetyFiveZeroThree);
                if (season == 2011) return (value <= (int)CountGame.Eleven);
                return (value <= (int)CountGame.Other);
            }
            //не проверяем нечиго уроме сезона
            return false;
        }

        public static string ReadIndecatorsTeam(System.Collections.Generic.IEnumerator<OpenQA.Selenium.IWebElement> _indecaotrs,char separator='\t')//преобразование считаной строки показателей мача
        {
            if (_indecaotrs.Current.Text != "") throw DataExeption.DataProcessingExeptions.ErrorWriteData("Reading indecators()");
            string output = "";
            for (int i = 0; i < 19; i++)
            {
                if (!_indecaotrs.MoveNext()) throw DataExeption.DataProcessingExeptions.ErrorWriteData("Reading indecators");
                output += _indecaotrs.Current.Text.Replace('.',',') + separator;
            }
            return output;
        }
    }
}
