// Imported by default
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {

        static void Main(string[] args)
        {
            Program p = new Program();
        }

        public Program()
        {
            AsynchronousSocketListener.StartListening();
            System.Console.ReadLine();

        }

    }
}
