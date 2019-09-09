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

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

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

            string Data = System.IO.File.ReadAllText(path); //To Do.. Sustituir por Bufffer
            List<char> Caracteres = Data.ToList<char>();

            Dictionary<char, int> dictionary = new Dictionary<char, int>();

            //Leer caracteres y contarlos

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
            
            //Leer archivo original y sustituir por codigos prefijo

            string Data1 = System.IO.File.ReadAllText(path); //To Do.. Sustituir por Bufffer 
            string textoBinario = "";


            foreach (char caracter in Data1)
            {
                if (diccionario.ContainsKey(caracter))
                {
                    textoBinario += diccionario[caracter];
                }
                else
                {
                    throw new Exception("El diccionario no contiene el valor especificado");
                }
            }


            //Escribir nuevo archivo

            string nombreNuevoArchivo = Path.GetFileNameWithoutExtension(path) + ".huff";
            string rutaComprimido = Path.Combine(Server.MapPath("~/Huffman"), nombreNuevoArchivo);

            if (System.IO.File.Exists(rutaComprimido))
            {
                System.IO.File.Delete(rutaComprimido);
            }
            FileStream file = System.IO.File.Create(rutaComprimido);
            file.Close();

            //Construir texto huffman 
            string Byte = ""; //Valor de 8 bits 
            List<char> binario = textoBinario.ToArray().ToList(); //Arreglo de texto en binario
            string textoComprimido = ""; //Resultado
            bool completed = false;

            while (!completed)
            {
                if (binario.Count > 0)
                {
                    //Construir byte
                    Byte += binario[0];
                    binario.RemoveAt(0); //Remover valor del arreglo

                    if (Byte.Length == 8)
                    {
                        //To Do... Escribir en archivo directamente
                        //Agregar Ascii al comprimido
                        textoComprimido += Encoding.ASCII.GetString(new byte[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) });
                        Byte = "";
                    }
                }
                else
                {
                    //Llenar el resto de espacio con 0's
                    if (Byte != "")
                    {
                        while (Byte.Length != 8)
                        {
                            Byte += "0";
                        }

                        textoComprimido += Encoding.UTF8.GetString(new byte[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) });
                        Byte = "";
                    }

                    completed = true;
                }
            }

            System.IO.File.WriteAllText(rutaComprimido, textoComprimido); //Escribir en archivo
            
            //Guardar configuracion para descomprimir

            //To Do...
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