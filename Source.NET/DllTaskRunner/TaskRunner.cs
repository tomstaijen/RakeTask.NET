using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using RakeTask;

namespace DllTaskRunner
{
    public class TaskRunner
    {
        public bool Run(string dllPath, string taskName, IDictionary<string,string> parameters)
        {
            try
            {
                if (!File.Exists(dllPath))
                {
                    Console.WriteLine("File not found: " + dllPath);
                }
                var type = GetType(dllPath, taskName);
                if (type == null)
                {
                    Console.WriteLine(string.Format("Task not found", taskName));
                    return false;
                }
                var constr = type.GetConstructor(new Type[] {});
                IRakeTask task = (IRakeTask) constr.Invoke(new object[] {});
                bool result = task.Execute(parameters);
                return result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Type GetType(string dllPath, string taskName)
        {
            var a = Assembly.LoadFrom(dllPath);
            var t = a.GetType(taskName);
            if( t != null && typeof(IRakeTask).IsAssignableFrom(t))
            {
                return t;
            }
            return null;
        }
    }
}
