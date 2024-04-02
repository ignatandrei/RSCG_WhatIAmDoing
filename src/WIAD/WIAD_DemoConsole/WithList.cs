using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIAD_DemoConsole;
internal class WithList
{
    public List<Person> people = new ();
    public bool FindData(string name)
    {
        return people.Any(p => p.FullName().Contains(name));
    }
}
