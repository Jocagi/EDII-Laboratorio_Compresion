using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laboratorio_Compresión.Models;

namespace Laboratorio_Compresión.Controllers
{
    public class HomeController : Controller
    {
        List<Archivo> Frecuencias = new List<Archivo>();
        List<char> Caracteres = new List<char>();
        //GET: Archivo
        public ActionResult Index()
        {
            return View(new List<char>());
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase archivo)
        {
           // List<Frecuencias> frecuencia = new List<Frecuencias>();
            string filePath = string.Empty;
            if (archivo != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(archivo.FileName);
                string extension = Path.GetExtension(archivo.FileName);
                archivo.SaveAs(filePath);

                string csvData = System.IO.File.ReadAllText(filePath);
                List<char> Caracteres = csvData.ToList<char>();

                // char[] caracter = csvData.ToList<char>();
                // foreach(char item in caracter)
                // {
                //     Caracteres.Add(item);
                // }

                /* var result = from item in csvData.ToArray()
                              group item by item into c
                              select new
                              {
                                  carac = c.Key,
                                  repeticiones = c.Count()
                              };*/

                /*  foreach(string row in csvData.Split('\n'))
                  {
                      if(!string.IsNullOrEmpty(row))
                      {

                          Frecuencias.Add(new Archivo
                          {

                          });
                      }
                  }*/

                Dictionary<char, int> didid = new Dictionary<char, int>();
                foreach (var item in Caracteres)
                {
                    if (!didid.Keys.Contains(item))
                    {
                        didid.Add(item, 1);
                    }
                    else
                    {
                        didid[item]++;
                    }
                }
                int v = 0;

            }
            return View(Caracteres);
        }
    
        public ActionResult contar()
        {
            char carac;
            int frec;
            foreach (var item in Caracteres.GroupBy(x => x))
                carac = item.Key;


            return View();
        }
      /*  public ActionResult Diccionario()
        {
            var res = new Dictionary<Char, int>();
            foreach(Char item in Caracteres)
            {
                if(!res.ContainsKey(item))
                {
                    res.Add(item, new Archivo{caracter=item.)
                }
            }
        }*/

       /*[HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            //Subir archivos al servidor

            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Uploads"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }*/

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}