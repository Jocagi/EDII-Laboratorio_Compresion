using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
        public MisCompresiones(string nombre, double razon, double factor, double porcentaje)
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
            return Math.Round(Convert.ToDouble(pesoComprimido) / Convert.ToDouble(pesoOriginal), 2);
        }

        private double calcularFactor(long pesoOriginal, long pesoComprimido)
        {
            return Math.Round( Convert.ToDouble(pesoOriginal) / Convert.ToDouble(pesoComprimido) , 2);
        }

        private double calcularPorcentaje (long pesoOriginal, long pesoComprimido)
        {
            return Math.Round(100 - ( (Convert.ToDouble(pesoComprimido) / Convert.ToDouble(pesoOriginal)) * 100 ), 2);
        }
        
        public static void agregarNuevaCompresion(MisCompresiones nuevo)
        {
            string path = HomeController.archivoMisCompresiones;

            using (StreamWriter sw = File.AppendText(path))
            {
                string text =
                    $"{nuevo.nombreOriginal}, {nuevo.razonDeCompresion}, {nuevo.factorDeCompresion}, {nuevo.porcentajeDeCompresion}";
                sw.WriteLine(text);
            }

        }

        public static List<MisCompresiones> leerLista()
        {

            string path = HomeController.archivoMisCompresiones;

            Stack<MisCompresiones> lista = new Stack<MisCompresiones>();

            if (!File.Exists(path)) //No existe el archivo
            {
                File.Create(path);
            }
            else
            {
                using (var reader = new StreamReader(path))
                {
                    
                    while (!reader.EndOfStream) //Recorrer archivo hasta el final
                    {

                        var line = reader.ReadLine();

                        string[] datos = line?.Split(','); //dividir datos

                        lista.Push(new MisCompresiones(datos?[0], Convert.ToDouble(datos?[1]),
                            Convert.ToDouble(datos?[2]), Convert.ToDouble(datos?[3])));
                    }
                    reader.Close();
                }
            }

            return lista.ToList();
        }
    }
}