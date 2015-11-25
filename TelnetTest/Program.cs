using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelnetTCP;

namespace TelnetTest
{
    class Program
    {
        private static int counter = 0;
        static void Main(string[] args)
        {
            TelnetApi telnetApi = new TelnetApi(28961);

            while (true)
            {
                telnetApi.BroadCastData(++counter + " Hello bello\n");
                Thread.Sleep(500);
            }
        }
    }
}
