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
            int byteLenght = 0;

            #endregion

            #region Crear_Archivo

            string nombreNuevoArchivo = Path.GetFileNameWithoutExtension(path) + ".lzw";
            rutaComprimido = Path.Combine(HomeController.directorioHuffman, nombreNuevoArchivo);
            Archivo.crearArchivo(rutaComprimido);
            
            #endregion

            //Leer posibles caracteres del archivo
            #region Caracteres

            dictionary = Lectura.obtenerDiccionarioFrecuencias(path);
            dictionaryLenght = dictionary.Count;

            #endregion

            //Añadir metadata del archivo
            #region Configuracion
            //First byte: byteLenght, Second byte: dictionary lenght, then complete dictionary

            List<byte> metadata = new List<byte> {Convert.ToByte(byteLenght), Convert.ToByte(dictionaryLenght)};
            
            //Diccionario
            foreach (var item in dictionary)
            {
                metadata.Add(Convert.ToByte(item.Key));
            }

            byte[] metadataArray = metadata.ToArray();

            Lectura.InsertData(rutaComprimido, 0, metadataArray);

            #endregion

            //Analizar texto, creando diccionario y escribiendo en archivo
            #region Algoritmo

            //Aqui va tu codigo, Genesis
            //ToDo..

            #endregion

            //Obtener codigo mas grande y añadirlo a la metadata
            //ToDo...

        }

        public static void descomprimir(string path)
        {
            int bufferLength = 1024;

            int byteLenght;
            int dictionaryLenght;
            Dictionary<int, char> dictionary = new Dictionary<int, char>();
            
            string current = "";
            string previous = "";
            string newItem = "";
            
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    //leer tamaño de los bytes
                    byteLenght = Convert.ToInt16(reader.ReadByte());
                    // leer tamaño del diccionario
                    dictionaryLenght = Convert.ToInt16(reader.ReadByte());

                    //Definir diccionario
                    int position = 1;

                    while (position <= dictionaryLenght)
                    {
                        dictionary.Add(position, (char)reader.ReadByte());
                        position++;
                    }

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