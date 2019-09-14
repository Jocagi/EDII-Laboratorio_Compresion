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
        public static string archivoMisCompresiones = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/Mis_Compresiones/Lista.txt");
        public static string mensaje = "";
        public static string currentFile = "";
        
        public ActionResult Index()
        {
            return View();
        }
      
        
        public ActionResult ComprimirHuffman()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ComprimirHuffman(HttpPostedFileBase file) {
            try
            {
                string path = Path.Combine(directorioUploads, Path.GetFileName(file.FileName));

                UploadFile(path, file);
                Huffman.comprimir(path);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                throw;
            }

            return RedirectToAction("Index");
        }

        public ActionResult DescomprimirHuffman()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DescomprimirHuffman(HttpPostedFileBase file)
        {
            
            try
            {
                string path = Path.Combine(directorioUploads, Path.GetFileName(file.FileName));

                UploadFile(path, file);
                Huffman.descomprimir(path);

            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                throw;
            }

            return RedirectToAction("Index");
        }

        public ActionResult MisCompresiones()
        {
            misCompresiones = Models.MisCompresiones.leerLista();
            return View(misCompresiones);
        }

        public ActionResult DownloadFile() 
        {
            string path = currentFile;
            string fileName = Path.GetFileName(path);

            byte[] filedata = System.IO.File.ReadAllBytes(path);
            string contentType = MimeMapping.GetMimeMapping(path);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Path.GetFileName(path),
                Inline = true,
            };

            currentFile = "";

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(filedata, contentType);
        }

        public void UploadFile(string path, HttpPostedFileBase file)
        {
            //Subir archivos al servidor

            if (file != null && file.ContentLength > 0)
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    file.SaveAs(path);
                    ViewBag.Message = "Carga Exitosa";

                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "No ha especificado un archivo.";
            }
        }
    }
}