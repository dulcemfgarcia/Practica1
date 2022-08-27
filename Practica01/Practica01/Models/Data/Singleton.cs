using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classlibrary;

namespace Practica01.Models.Data
{
    public class Singleton
    {
        private readonly static Singleton _instance = new Singleton();

        public AVL<Person> AVLnames;
        public AVL<Person> AVLDpi;

        public Singleton()
        {
            AVLnames = new AVL<Person>();
            AVLDpi = new AVL<Person>();
        }
        public static Singleton Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
