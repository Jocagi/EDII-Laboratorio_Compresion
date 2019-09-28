using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Laboratorio_Compresión.Controllers;
using Laboratorio_Compresión.Models;

namespace Laboratorio_Compresión
{
    public class LZW
    {
        public static void comprimir(string path)
        {
            
            #region Variables

            int maxDictionaryLenght = 0;
            int byteLenght = 8;

            #endregion

            #region Crear_Archivo

            string nombreNuevoArchivo = Path.GetFileNameWithoutExtension(path) + ".lzw";
            string rutaComprimido = Path.Combine(HomeController.directorioHuffman, nombreNuevoArchivo);
            Archivo.crearArchivo(rutaComprimido);
            
            #endregion

            //Crear dicionario
            #region Caracteres

            var diccionario = obtenerDiccionarioCompresion();
            maxDictionaryLenght = diccionario.Count;

            #endregion

            //Analizar texto, creando diccionario y escribiendo en archivo
            #region Algoritmo

            int bufferLength = 1024;

            //Empezar concatenar
            string c = string.Empty;
            List<int> comprimir = new List<int>();

            //Buffer para comprimir
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var buffer = reader.ReadBytes(count: bufferLength);

                        foreach (var t in buffer)
                        {
                            string ct = c + ((char) t);
                            if (diccionario.ContainsKey(ct))
                            {
                                c = ct;
                            }
                            else
                            {
                                //sacarlo
                                comprimir.Add(diccionario[c]);
                                //Aqui ya lo concatena y lo agrega
                                diccionario.Add(ct, diccionario.Count);
                                c = t.ToString();
                            }
                        }
                    }
                }
            }

            //Ultima cadena del archivo
            if (!string.IsNullOrEmpty(c))
            {
                comprimir.Add(diccionario[c]);
            }

            #endregion
        }

        public static void descomprimir(string path)
        {
            int bufferLength = 1024;

            int byteLenght = 8;
            int dictionaryLenghtMax;

            Dictionary<int, char> dictionary = new Dictionary<int, char>();
            
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    //Definir diccionario

                    Dictionary<int, string> diccionario = obtenerDiccionarioDescompresion();

                    //Operacion inicial

                    string c = diccionario[(int) reader.ReadByte()];
                    StringBuilder descomprimir = new StringBuilder(c);
                    
                    //Buffer para descomprimir
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var buffer = reader.ReadBytes(count: bufferLength);

                        foreach (var t in buffer)
                        {
                            string entry = null;
                            if (diccionario.ContainsKey((int) t))
                            {
                                entry = diccionario[(int) t];
                            }
                            else if ((int) t == diccionario.Count)
                            {
                                entry = c + c[0];
                            }

                            descomprimir.Append(entry);

                            //  Agregar nueva frase al diccionario

                            diccionario.Add(diccionario.Count, c + entry[0]);

                            c = entry;
                        }
                    }
                }
            }
        }

        private static Dictionary<int, string> obtenerDiccionarioDescompresion()
        {
            Dictionary<int, string> diccionario = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
            {
                diccionario.Add(i, ((char)i).ToString());
            }

            return diccionario;
        }

        private static Dictionary<string, int> obtenerDiccionarioCompresion()
        {
            Dictionary<string, int> diccionario = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
            {
                diccionario.Add(((char)i).ToString(), i);
            }

            return diccionario;
        }
    }
}