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
using Newtonsoft.Json;
using System.Dynamic;
using Classlibrary;

namespace Practica01.Controllers
{

    public class PersonController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
            if (postedFile != null)
            {
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                string csvData = System.IO.File.ReadAllText(filePath);
                bool firstRow = true;
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            if (firstRow)
                            {
                                foreach (string cell in row.Split(';'))
                                {

                                }
                                firstRow = false;

                            }
                            else
                            {
                                int i = 0;
                                int cont = 0;
                                //Arreglo que guarda los datos individualmente del csv 
                                string[] NodoM = new string[6] { "", "", "", "", "", "" };
                                int encontrar = 0;
                                string cell2 = "";
                                foreach (string cell in row.Split(';'))
                                {
                                    if (cell.Substring(0, 1) != "\"" && encontrar == 0)
                                    {
                                        NodoM[cont] = cell.Trim();
                                        cell2 = "";
                                        cont++;
                                        i++;
                                    }
                                    else
                                    {
                                        cell2 = cell2 + cell + ";";
                                        encontrar++;
                                        if (cell.Substring((cell.Length - 1), 1) == "\"")
                                        {
                                            encontrar = 0;
                                            cell2 = cell2.Remove(0, 1);
                                            cell2 = cell2.Remove(cell2.Length - 3, 3);
                                            NodoM[cont] = cell2.Trim();
                                            cont++;
                                            i++;
                                            cell2 = "";
                                        }
                                    }
                                }

                                Person person = new Person();
                                var json = NodoM[1];
                                var listdata = JsonConvert.DeserializeObject<List<ExpandoObject>>(json);
                                if(NodoM[0] == "INSERT")
                                {
                                    foreach(dynamic prod in listdata)
                                    {
                                        person.name = prod.name;
                                        person.address = prod.adress;
                                        person.datebirth = prod.datebirth;
                                        person.dpi = prod.dpi;
                                    }
                                    Singleton.Instance.AVLnames.Insert(person, person.nameComparer);
                                } 
                                else if (NodoM[0] == "PATCH") 
                                {
                                    //Singleton.Instance.AVLnames.patch();
                                }
                                else
                                {
                                    foreach (dynamic prod in listdata)
                                    {
                                        person.name = prod.name;
                                        person.address = prod.adress;
                                        person.datebirth = prod.datebirth;
                                        person.dpi = prod.dpi;
                                    }
                                    Singleton.Instance.AVLnames.Delete(person, person.dpiComparer);
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

    }
}