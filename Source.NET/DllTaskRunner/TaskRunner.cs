using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TaskRunner.Task;

namespace TaskRunner
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
                    return false;
                }
                var type = GetType(dllPath, taskName);
                if (type == null)
                {
                    return false;
                }
                var constr = type.GetConstructor(new Type[] {});
                ITask task = (ITask) constr.Invoke(new object[] {});

                var properties = task.GetType().GetProperties();

                foreach( var p in parameters )
                {
                    var property = properties.Where(prop => prop.Name.Equals(p.Key)).SingleOrDefault();
                    if (property == null || !property.CanWrite || !(property.PropertyType == typeof(string)))
                    {
                        Console.WriteLine(string.Format("Can't write string to property \"{0}\" on task \"{1}\"", p.Key, task.GetType().Name));
                        return false;
                    }
                    property.SetValue(task, p.Value, null);
                }
                bool result = task.Execute();
                return result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_assemblies.ContainsKey(args.Name))
                return _assemblies[args.Name];

//            var dlls = Directory.GetFiles(_directory, args.Name + ".dll");
//            if (dlls.FirstOrDefault() != null)
//            {
//                Console.WriteLine("Redirecting assemblyLoad" + dlls.First());
//                return Assembly.LoadFrom(dlls.First());
//            }
//            var exes = Directory.GetFiles(_directory, args.Name + ".exe");
//            if (exes.FirstOrDefault() != null)
//            {
//                Console.WriteLine("Redirecting assemblyLoad" + exes.First());
//                return Assembly.LoadFrom(exes.First());
//            }
            return null;
        }

        private Dictionary<string,Assembly> _assemblies = new Dictionary<string, Assembly>();

        public Type GetType(string dllPath, string taskName)
        {
            var absolute = Path.Combine(Directory.GetCurrentDirectory(), dllPath);
            //Directory.SetCurrentDirectory(Path.GetDirectoryName(absolute));

            Console.WriteLine("Assembly: " + absolute);
            var a = Assembly.LoadFrom(absolute);
            Console.WriteLine("Loaded: " + a.FullName);
            foreach( var reffedAsm in a.GetReferencedAssemblies() )
            {
                if( reffedAsm.Name != "TaskRunner.Task")
                {
                    var reffedFile = Path.Combine(Path.GetDirectoryName(absolute), reffedAsm.Name + ".dll");
                    if (File.Exists(reffedFile))
                    {
                        var assembly = Assembly.LoadFrom(reffedFile);
                        Console.WriteLine("Loaded: " + assembly.FullName);
                        _assemblies.Add(assembly.FullName, assembly);
                    }
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var t = a.GetType(taskName);
            if (t == null)
                Console.WriteLine(string.Format("Task not found : {0}", taskName));
            else if( t.GetInterface(typeof(ITask).FullName) == null)
                Console.WriteLine(string.Format("Task is not a ITask"));
            else
                return t;
            return null;
        }

        public bool IsTask(Type t)
        {
            return t.GetMethod("Execute", new[] {typeof (bool)}) != null;
        }
    }
}
