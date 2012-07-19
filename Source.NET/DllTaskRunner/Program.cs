using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Rake Task Runner v0.9");
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: TaskRunner.exe <dllPath> <task>");
                Environment.Exit(0);
            }


            string path = args[0];
            string task = args[1];

            var ps = new Dictionary<string, string>();

            for( int i = 2; i < args.Length; i++)
            {
                string[] keyvalue = args[i].Split('=');
                if( keyvalue.Length == 2 )
                {
                    ps.Add(keyvalue[0],keyvalue[1]);
                }
            }
            bool result = new TaskRunner().Run(path, task, ps);
            Environment.Exit(result?0:1);
        }
    }
}
