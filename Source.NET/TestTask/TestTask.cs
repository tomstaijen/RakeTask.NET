using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RakeTask;

namespace TestTask
{
    public class TestTask : IRakeTask
    {
        public bool Execute(IDictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return false; 

            foreach( var v in parameters)
            {
                Console.WriteLine(string.Format("{0} => {1}", v.Key, v.Value));
            }
            return true;
        }
    }
}
