using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RakeTask
{
    public interface IRakeTask
    {
        bool Execute(IDictionary<string,string> parameters);
    }
}
