using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskRunner.Task;

namespace TestTask
{
    public class TestTask : ITask
    {
        public string Param1
        {
            set
            {
                Console.WriteLine("Param1 set to " + value);
            }
        }
        
        public bool Execute()
        {
            return true;
        }
    }
}
