using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laboratorio_Compresión.Models;
using System.Text;

namespace Laboratorio_Compresión.Controllers
{
    public class HomeController : Controller
    {
        //Lista de archivos comprimidos
        public static List<MisCompresiones> misCompresiones = new List<MisCompresiones>();
        public static string directorioUploads = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/Uploads");
        public static string directorioHuffman = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/Huffman");
        public static string directorioHuffmanConfig = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/HuffmanConfig");
        public static string directorioHuffmanDecompress = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/Decompression");
        public static string mensaje = "";

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
                    string path = Path.Combine(directorioUploads, Path.GetFileName(file.FileName));

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    file.SaveAs(path);
                    ViewBag.Message = "Carga Exitosa";

                    Huffman.descomprimir(path);
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

        public ActionResult ComprimirHuffman()
        {
            return View();
        }

        public ActionResult DescomprimirHuffman()
        {
            return View();
        }

        public ActionResult MisCompresiones()
        {
            Laboratorio_Compresión.Models.MisCompresiones.leerAchivos();
            return View(misCompresiones);
        }

        public ActionResult DownloadFile() //No se como sirve pero no tocar
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
        
    }
}