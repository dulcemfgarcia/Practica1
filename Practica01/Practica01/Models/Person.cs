using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Practica01.Models
{
    public class Person
    {
        public string name { get; set; }
        public string dpi { get; set; }
        public string datebirth { get; set; }
        public string address { get; set; }
         

        public Comparison<Person> nameComparer = delegate (Person person1, Person person2)
        {
            return person1.name.CompareTo(person2.name);
        };

        public Comparison<Person> dpiComparer = delegate (Person person1, Person person2)
        {
            return person1.dpi.CompareTo(person2.dpi);
        };

    }
    
}
