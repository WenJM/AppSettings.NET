using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppSettings.Test
{
    static class RunStart
    {
        public static void Run(string title, int num, Action<int> action)
        {
            if (num<=0)
            {
                num = 1;
            }
            var start = DateTime.Now;
            for (int i = 0; i < num; i++)
            {
                action(i);
            }

            var ts = DateTime.Now - start;

            Console.WriteLine($"{title}：");
            Console.WriteLine($"耗时:{ts.Hours.ToString("00")}.{ts.Minutes.ToString("00")}.{ts.Seconds.ToString("00")}.{ts.Milliseconds.ToString("000")}");
        }
    }
}
