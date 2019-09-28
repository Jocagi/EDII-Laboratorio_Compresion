using System;
using System.Collections.Generic;
using System.IO;
using Laboratorio_Compresión.Controllers;
using Laboratorio_Compresión.Models;

namespace Laboratorio_Compresión
{
    public class LZW
    {
        public static void comprimir(string path)
        {

            #region Variables

            string rutaComprimido; 
            Dictionary<char, int> dictionary;
            int dictionaryLenght = 0;
            int byteLenght = 8;

            #endregion

            #region Crear_Archivo

            string nombreNuevoArchivo = Path.GetFileNameWithoutExtension(path) + ".lzw";
            rutaComprimido = Path.Combine(HomeController.directorioHuffman, nombreNuevoArchivo);
            Archivo.crearArchivo(rutaComprimido);
            
            #endregion

            //Crear dicionario
            #region Caracteres

            dictionary = Lectura.obtenerDiccionarioFrecuencias(path);
            dictionaryLenght = dictionary.Count;

            #endregion
            
            //Analizar texto, creando diccionario y escribiendo en archivo
            #region Algoritmo

            //Aqui va tu codigo, Genesis
            //ToDo..

            #endregion

            //Obtener codigo mas grande y añadirlo a la metadata
            

        }

        public static void descomprimir(string path)
        {
            int bufferLength = 1024;

            int byteLenght = 8;
            int byteLenghtMax;
            Dictionary<int, char> dictionary = new Dictionary<int, char>();
            
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    //Definir diccionario
                    

                    //Buffer para descomprimir
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var buffer = reader.ReadBytes(count: bufferLength);

                        foreach (var item in buffer)
                        {

                            //ToDo... Descomprimir

                            //Leer byte
                            //Tomar string y buscarlo en diccionario
                            //El resultado es current
                            //Se agrega al diccionario previous + primer char del current

                        }
                    }
                }
            }
        }
    }
}