using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCrwaler
{
    class Logger
    {
        public static void Log(string content)
        {
            Console.WriteLine("[{0}]{1}", new DateTime(), content);
        }
    }
}
