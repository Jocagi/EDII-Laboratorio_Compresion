using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Laboratorio_Compresión.Models;

namespace Laboratorio_Compresión.Controllers
{
    public class HomeController : Controller
    {
        #region Variables
        //Lista de archivos comprimidos
        public static List<MisCompresiones> misCompresiones = new List<MisCompresiones>();
        public static string directorioUploads = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/Uploads");
        public static string directorioHuffman = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/Huffman");
        public static string directorioLZW = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/LZW");
        public static string directorioHuffmanConfig = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/HuffmanConfig");
        public static string directorioHuffmanDecompress = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/Decompression");
        public static string archivoMisCompresiones = System.Web.HttpContext.Current.Server.MapPath("~/Archivos/Mis_Compresiones/Lista.txt");
        public static string currentFile = "";
        #endregion
        
        public ActionResult Index()
        {
            return View();
        }

        #region Huffman
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
                ViewBag.Message = "ERROR:" + ex.Message;
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
                ViewBag.Message = "ERROR:" + ex.Message;
                throw;
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region LZW
        public ActionResult ComprimirLZW()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ComprimirLZW(HttpPostedFileBase file)
        {
            try
            {
                string path = Path.Combine(directorioUploads, Path.GetFileName(file.FileName));

                UploadFile(path, file);
                LZW.comprimir(path);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message;
                throw;
            }

            return RedirectToAction("Index");
        }

        public ActionResult DescomprimirLZW()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DescomprimirLZW(HttpPostedFileBase file)
        {
            try
            {
                string path = Path.Combine(directorioUploads, Path.GetFileName(file.FileName));

                UploadFile(path, file);
                LZW.descomprimir(path);

            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message;
                throw;
            }

            return RedirectToAction("Index");
        }

        #endregion

#region Aritmetico
        public ActionResult Aritmetica()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Aritmetica(HttpPostedFileBase file)
        {
            try
            {
                string path = Path.Combine(directorioUploads, Path.GetFileName(file.FileName));
                UploadFile(path, file);
                string Data = System.IO.File.ReadAllText(path, Encoding.Default);
                int[] fuenteUno = new int[256];
                float[] L = new float[256];
                float[] H = new float[256];
                int longitud = file.ContentLength;
                int primero = 0;
                int segundo = 0;
                for (int i = 0; i < 255; i++)
                {
                    if(fuenteUno[i]>0)
                    {
                        primero = segundo;
                        segundo = segundo + fuenteUno[i];
                        L[i] = (float)primero / longitud;
                        H[i] = (float)segundo / longitud;
                    }
                }
                //Yo espero que jale los datos
                Aritmetico.Aritmetica(path, L, H);
                
 
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message;
                throw;
            }

            return RedirectToAction("Index");
        }
#endregion

        public ActionResult MisCompresiones()
        {
            misCompresiones = Models.MisCompresiones.leerLista();
            return View(misCompresiones);
        }

        public ActionResult DownloadFile() 
        {
            string path = currentFile;

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
                    ViewBag.Message = "ERROR:" + ex.Message;
                }
            else
            {
                ViewBag.Message = "No ha especificado un archivo.";
            }
        }
    }
}