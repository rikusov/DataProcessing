namespace Default
{
    static class OutputWorker// работа с выводом данных
    {
        public delegate void Output(string output);//делегат для вывода сообшений

        public static void WriteLine(Output writeln, string[] output)//вывод массива строк
        {
            foreach (var outline in output) writeln(outline);
        }
    }
}
