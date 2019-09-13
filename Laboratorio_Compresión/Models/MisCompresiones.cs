using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

using Laboratorio_Compresión.Controllers;

namespace Laboratorio_Compresión.Models
{
    public class MisCompresiones
    {
        public string nombreOriginal { get; set; }
        public double razonDeCompresion { get; set; }
        public double factorDeCompresion { get; set; }
        public double porcentajeDeCompresion { get; set; }

        public MisCompresiones() { }
        public MisCompresiones(string nombre, float razon, float factor, float porcentaje)
        {
            nombreOriginal = nombre;
            razonDeCompresion = razon;
            factorDeCompresion = factor;
            porcentajeDeCompresion = porcentaje;
        }
        public MisCompresiones(string nombre, long pesoOriginal, long pesoComprimido)
        {
            nombreOriginal = nombre;
            razonDeCompresion = calcularRazon(pesoOriginal, pesoComprimido);
            factorDeCompresion = calcularFactor(pesoOriginal, pesoComprimido);
            porcentajeDeCompresion = calcularPorcentaje(pesoOriginal, pesoComprimido);
        }

        //Calcular valores solicitados...
        private double calcularRazon(long pesoOriginal, long pesoComprimido)
        {
            return Convert.ToDouble(pesoComprimido) / Convert.ToDouble(pesoOriginal);
        }

        private double calcularFactor(long pesoOriginal, long pesoComprimido)
        {
            return Convert.ToDouble(pesoOriginal) / Convert.ToDouble( pesoComprimido);
        }

        private double calcularPorcentaje (long pesoOriginal, long pesoComprimido)
        {
            return 100 - ( (Convert.ToDouble(pesoComprimido) / Convert.ToDouble(pesoOriginal)) * 100 );
        }

        public static void leerAchivos()
        {
            //Eliminar la lista en controlador
            HomeController.misCompresiones.Clear();

            //Resumen: Se leen todos los archivos comprimidos crea la lista en el controlador

            string carpetaArchivos = HomeController.directorioUploads;
            string carpetaHuff = HomeController.directorioHuffman;

            //Se enlistan todos los archivos en la carpeta de huffman, se leen y definen las propiedades

            DirectoryInfo infoArchivo = new DirectoryInfo(carpetaArchivos);
            FileInfo[] listaArchivos = infoArchivo.GetFiles();

            DirectoryInfo infoHuff = new DirectoryInfo(carpetaHuff);
            FileInfo[] listaHuffman = infoHuff.GetFiles();

            foreach (var archivoComprimido in listaHuffman)
            {
                foreach (var archivoOriginal in listaArchivos)
                {
                    if (Path.GetFileNameWithoutExtension(archivoComprimido.FullName) == Path.GetFileNameWithoutExtension(archivoOriginal.FullName))
                    {
                        Controllers.HomeController.misCompresiones.Add(new MisCompresiones(archivoOriginal.Name, archivoOriginal.Length, archivoComprimido.Length) );
                    }
                }
            }
        }
    }
}