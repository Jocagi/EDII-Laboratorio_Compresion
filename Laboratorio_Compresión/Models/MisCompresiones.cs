using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace Laboratorio_Compresión.Models
{
    public class MisCompresiones
    {
        public string nombreOriginal { get; set; }
        public string razonDeCompresion { get; set; }
        public string factorDeCompresion { get; set; }
        public int porcentajeDeCompresion { get; set; }

        public MisCompresiones() { }
        public MisCompresiones(string nombre, string razon, string factor, int porcentaje)
        {
            nombreOriginal = nombre;
            razonDeCompresion = razon;
            factorDeCompresion = factor;
            porcentajeDeCompresion = porcentaje;
        }

        //To Do... Calcular valores solicitados...


        public static void leerAchivos()
        {
            //Eliminar la lista en controlador
            Controllers.HomeController.misCompresiones.Clear();

            //Resumen: Se leen todos los archivos comprimidos crea la lista en el controlador

            string carpetaArchivos = System.Web.HttpContext.Current.Server.MapPath("~/Laboratorio_Compresión/Archivos");
            string carpetaHuff = System.Web.HttpContext.Current.Server.MapPath("~/Laboratorio_Compresión/Huffman");

            //Se enlistan todos los archivos en la carpeta de huffman, se leen y definen las propiedades

            DirectoryInfo infoArchivo = new DirectoryInfo(carpetaArchivos);
            FileInfo[] listaArchivos = infoArchivo.GetFiles();

            DirectoryInfo infoHuff = new DirectoryInfo(carpetaHuff);
            FileInfo[] listaHuffman = infoHuff.GetFiles();

            foreach (var archivo in listaHuffman)
            {
                //listaHuffman[0].Name
            }
        }
    }
}