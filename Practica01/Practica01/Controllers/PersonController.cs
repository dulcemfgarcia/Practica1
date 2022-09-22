using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Practica01.Models;
using Practica01.Models.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Dynamic;
using Classlibrary;
using Newtonsoft.Json;

namespace Practica01.Controllers
{

    public class PersonController : Controller
    {
        public delegate Person Edition(Person person1, Person person2);
        public IActionResult Index()
        {
            return View(Singleton.Instance.AVLnames);
        }

        //Funciones para cargar datos en archivos .csv

        public ActionResult Reading()
        {
            return View("LeerArchivo");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LeerArchivo(IFormFile postedFile)
        {
            string path = @"C:\Users\nossu\Desktop\input.csv";
            string line = System.IO.File.ReadAllText(path);
            foreach (string row in line.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    string[] data = row.Split(';');
                    Person person = JsonConvert.DeserializeObject<Person>(data[1]);
                    if (data[0] == "INSERT")
                    {
                        Person newPerson = new Person();
                        newPerson.name = person.name;
                        newPerson.dpi = person.dpi;
                        newPerson.datebirth = person.datebirth;
                        newPerson.address = person.address;
                        Singleton.Instance.AVLnames.Insert(newPerson, newPerson.dpiComparer);
                        Singleton.Instance.AVLDpi.Insert(newPerson, newPerson.dpiComparer);
                    }
                    else if (data[0] == "PATCH")
                    {
                        Edition patch = Person.PatchData;
                        Person newPerson = new Person();
                        newPerson.name = person.name;
                        newPerson.dpi = person.dpi;
                        newPerson.datebirth = person.datebirth;
                        newPerson.address = person.address;
                        Node<Person> newNode = new Node<Person>();
                        newNode.value = newPerson;
                        Singleton.Instance.AVLnames.Patch(newPerson, newNode, newPerson.dpiComparer, patch);
                        Singleton.Instance.AVLDpi.Patch(newPerson, newNode, newPerson.dpiComparer, patch);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Search()
        {
            return View(Singleton.Instance.AVLDpi);
        }

        public ActionResult SearchName()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchName(string name)
        {
            try
            {
                if (name == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Person NewPerson = new Person();
                    NewPerson.name = name;
                    Node<Person> newNode = new Node<Person>();
                    newNode.value = NewPerson;
                    Singleton.Instance.AVLDpi.Search(newNode, NewPerson.nameComparer);
                    return RedirectToAction(nameof(Search));
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public ActionResult Delete(string Persona, string nombre)
        {
            Person nuevaPersona = new Person();
            nuevaPersona.dpi = Persona;
            nuevaPersona.name = nombre;
            Node<Person> nuevonodo = new Node<Person>();
            nuevonodo.value = nuevaPersona;
            Singleton.Instance.AVLnames.Delete(nuevaPersona, nuevaPersona.dpiComparer);
            Singleton.Instance.AVLDpi.Delete(nuevaPersona, nuevaPersona.dpiComparer);
            return RedirectToAction(nameof(Index));
        }
    }
}