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

        private IHostingEnvironment Environment;

        public PersonController(IHostingEnvironment _enviroment)
        {
            Environment = _enviroment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LeerArchivo(IFormFile postedFile)
        {
            string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = Path.GetFileName(postedFile.FileName);
            string filePath = Path.Combine(path, fileName);
            System.IO.StreamReader doc = new System.IO.StreamReader(filePath);
            string ch = ";";
            string line;
            while ((line = doc.ReadLine()) != null)
            {
                string[] rows = line.Split(';');
                Person newP = JsonSerializer.Deserialize<Person>(rows[1]);

                if (rows[0] == "INSERT")
                {
                    Singleton.Instance.AVLnames.Insert(newP, newP.nameComparer);
                }
                else if(rows[0] == "PATCH")
                {

                    Edition patch = Person.PatchData;

                    Person newPerson = new Person();
                    newPerson.name = newP.name;
                    newPerson.dpi = newP.dpi;
                    newPerson.datebirth = newP.datebirth;
                    newPerson.address = newP.address;
                    Node<Person> newNode = new Node<Person>();
                    newNode.value = newPerson;
                    Singleton.Instance.AVLnames.Search(newNode, newPerson.dpiComparer);

                    Singleton.Instance.AVLnames.Patch(newPerson,newNode, newPerson.dpiComparer, patch);

                }
                else
                {
                    Person newPerson = new Person();
                    newPerson.dpi = newP.dpi;
                    Node<Person> newNode = new Node<Person>();
                    newNode.value = newPerson;
                    Singleton.Instance.AVLnames.Delete(newPerson, newPerson.dpiComparer);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Search()
        {
            return View(Singleton.Instance.AVLnames);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string dpi)
        {
            try
            {
                if (dpi == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Person NewPerson = new Person();
                    NewPerson.name = dpi;
                    Node<Person> newNode = new Node<Person>();
                    newNode.value = NewPerson;
                    Singleton.Instance.AVLnames.Search(newNode, NewPerson.dpiComparer);
                    return RedirectToAction(nameof(Search));
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}