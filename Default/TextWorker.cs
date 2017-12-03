namespace Default
{
    static class TextWorker//работа с тестовой информцией
    {
        public static string Repalase(string value,string[] oldValue, string newValue)//замена в строке всех елементов входящих в массив oldValuse на значение newValue
        {
            foreach (string old in oldValue) value=value.Replace(old, newValue);
            return value;
        }
    
    }
}
