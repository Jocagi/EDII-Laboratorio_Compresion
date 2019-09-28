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

            string Data = System.IO.File.ReadAllText(path, Encoding.Default); //buffer
            List<char> Caracteres = Data.ToList<char>();
            //Crea diccionario
            Dictionary<string, int> diccionario = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
                diccionario.Add(((char)i).ToString(), i);

            //Empezar concatenar
            string c = string.Empty;
            List<int> comprimir = new List<int>();

            //Sorry, ahora ya deberia leer el archivo
            foreach (char t in Caracteres)
            {
                string ct = c + t;
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

            #endregion
            
            if (!string.IsNullOrEmpty(c))
                comprimir.Add(diccionario[c]);

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

                    Dictionary<int, string> segundo = new Dictionary<int, string>();
                    for (int i = 0; i < 256; i++)
                    {
                        segundo.Add(i, ((char) i).ToString());
                    }

                    //Buffer para descomprimir
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var buffer = reader.ReadBytes(count: bufferLength);

                        foreach (var item in buffer)
                        {
                            string Data = System.IO.File.ReadAllText(path, Encoding.Default); //buffer
                            List<char> Caracteres = Data.ToList<char>();

                            string c = segundo[Caracteres[0]];
                            Caracteres.RemoveAt(0);
                            StringBuilder descomprimir = new StringBuilder(c);

                            foreach (int t in Caracteres)
                            {
                                string entry = null;
                                if (segundo.ContainsKey(t))
                                {
                                    entry = segundo[t];
                                }
                                else if (t == segundo.Count)
                                {
                                    entry = c + c[0];
                                }

                                descomprimir.Append(entry);

                                //  Agregar nueva frase

                                segundo.Add(segundo.Count, c + entry[0]);

                                c = entry;

                            }

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