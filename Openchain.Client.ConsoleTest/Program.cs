using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChain.Client.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new Class1();
            var t=c.Run();
            Console.ReadKey();
            t.Wait();
        }
    }
}
