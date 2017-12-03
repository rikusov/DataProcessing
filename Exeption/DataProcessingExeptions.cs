namespace DataExeption
{
    public static class DataProcessingExeptions// класс ошибок
    {
        public static System.Exception ErrorWriteData(string msg = null,object obj = null)//ошибка при чтении данных
        {
            return Error("Errors wraiting data", msg, obj);
        }

        public static System.Exception ErrorFormatIndex(string msg = null, object obj = null)//ошибка индеска матча
        {
            return Error("Error forming the index match",msg,obj);
        }

        public static System.Exception ErrorCompare(string msg = null, object obj = null)//ошибка сравнения
        {
            return Error("Error comparing items", msg, obj);
        }

        private static System.Exception Error(string _stdmsg=null, string msg=null,object obj=null)//форма для ошибки
        {
            string Errors = _stdmsg;
            if (Errors == null) Errors = "!!!ERORRS!!!";
            if (msg != null) Errors += $":{msg}";
            if (obj != null) Errors += $"(Object:{obj.GetType().ToString()})";
            return new System.Exception(Errors);
        }

    }
}
