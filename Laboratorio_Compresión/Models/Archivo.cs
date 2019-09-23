using System;
using System.IO;

namespace Laboratorio_Compresión.Models
{
    public class Archivo
    {
        public static void crearArchivo(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            FileStream fs = System.IO.File.Create(path);
            fs.Close();
        }

        public static void escribirEnArchivo(string path, string text)
        {
            if (File.Exists(path))
            {
                File.AppendAllText(path, text);
            }
            else
            {
                throw new Exception("El Archivo no existe");
            }
        }
    }
}