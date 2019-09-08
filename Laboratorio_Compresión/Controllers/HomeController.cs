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
        //Lista de archivos comprimidos
        public static List<MisCompresiones> misCompresiones = new List<MisCompresiones>();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            //Subir archivos al servidor

            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Archivos"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "Carga Exitosa";

                    comprimir(path);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "No ha especificado un archivo.";
            }

            return View();
        }

        public void comprimir(string path)
        {
            string Data = System.IO.File.ReadAllText(path);
            List<char> Caracteres = Data.ToList<char>();

            Dictionary<char, int> dictionary = new Dictionary<char, int>();

            foreach (var item in Caracteres)
            {
                if (!dictionary.Keys.Contains(item))
                {
                    dictionary.Add(item, 1);
                }
                else
                {
                    dictionary[item]++;
                }
            }

            //Crear arbol Huffman
            ArbolBinario arbol = new ArbolBinario(dictionary);
            Dictionary<char, string> diccionario = arbol.obtenerDiccionarioBinario();

            diccionario.Add('+', ""); //Test
        }//No se como sirve pero no tocar

        public ActionResult DownloadFile()
        {
            string filename = "Poster.pdf";
            string filepath = Server.MapPath("~/Archivos/") +  filename;
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }


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