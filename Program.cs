using ParseStatic;
using System;
using System.IO;

namespace DataProcessing
{
    class Program
    {

        static void Main(string[] args)
        {
            //Console.WriteLine(GetStatic.GetListMatchinData(DateTime.Now));
            DateTime dt = new DateTime(2017,11,28);
            //Default.OutputWorker.WriteLine(Console.Out, GetStatic.GetListIndexMatchinData(dt));
            //GetMachesAsync(dt, Console.Out);
            IndexoftheMatch id = new IndexoftheMatch("0021700318");
            //id.Increment();
            //Console.WriteLine(id.Subtraction(new IndexoftheMatch("0021700329")));
            //Default.OutputWorker.WriteLine(Console.WriteLine,GetStatic.GetStaticinIndex(new IndexoftheMatch("0021700318")));
            //GetStatic.GetStaticinIndexAsync(new IndexoftheMatch("0021700318"), PrintConsole,0, PrintProgress);
            // PrintConsole(GetStatic.GetStaticinIndex(id,new IndexoftheMatch("0021700319"),PrintProgress));
           // GetStatic.GetStaticinIndexAsync(id, new IndexoftheMatch("0021700319"), PrintConsole, -1, PrintProgress);
            Console.ReadLine();
        }

       /* static async void GetMachesAsync(DateTime dt, TextWriter stream)
        {
            Console.WriteLine("Run");
            //var output = await GetStatic.GetListIndexMatchinDataAcync(dt);
            Console.WriteLine("End");
            Default.OutputWorker.WriteLine(Console.WriteLine,output);
        }*/

        static void PrintConsole(string[] str,int id=-1)
        {
            Default.OutputWorker.WriteLine(Console.WriteLine, str);
        }

        static void PrintProgress(int a)
        {
            Console.WriteLine("Complete: "+a.ToString()+"%");
        }
    }
}
