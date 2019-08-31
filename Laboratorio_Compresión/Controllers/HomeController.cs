using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Laboratorio_Compresión.Controllers
{
    public class HomeController : Controller
    {
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