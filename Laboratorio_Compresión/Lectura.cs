using System;
using System.Collections.Generic;
using System.IO;

namespace Laboratorio_Compresión
{
    public class Lectura
    {
        private const int bufferLength = 1024;

        #region Default

        public void Leer(int lenght, string path)
        {
            List<char> tata = new List<char>();

            int bufferLength = lenght;

            var buffer = new byte[bufferLength];

            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLength);
                        foreach (var item in buffer)
                        {
                            tata.Add((char) item);
                        }
                    }
                }
            }
        }

        public static void Escritura(string text, string path)
        {
            List<char> tata = new List<char>();
            int txt = Convert.ToInt32(text);
            int bufferLength = txt;
            var buffer = new byte[bufferLength];
            using (var file = new FileStream(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(file))
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = Convert.ToByte(100 + i);
                    }

                    writer.Write(buffer);
                }
            }

        }

        #endregion

        #region Huffman

        public static Dictionary<char, int> obtenerDiccionarioFrecuencias(string path)
        {
            Dictionary<char, int> dictionary = new Dictionary<char, int>();

            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var buffer = reader.ReadBytes(bufferLength);

                        foreach (var item in buffer)
                        {
                            if (!dictionary.ContainsKey((char) item))
                            {
                                dictionary.Add((char) item, 1);
                            }
                            else
                            {
                                dictionary[(char) item]++;
                            }
                        }
                    }
                }
            }

            return dictionary;
        }

        public static string textoBinario(string path, Dictionary<char, string> diccionario)
        {
            string texto = "";

            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var buffer = reader.ReadBytes(bufferLength);
                        foreach (var item in buffer)
                        {
                            if (diccionario.ContainsKey((char) item))
                            {
                                texto += diccionario[(char) item];
                            }
                            else
                            {
                                throw new Exception("El diccionario no contiene el valor especificado");
                            }
                        }
                    }
                }
            }

            return texto;
        }

        #endregion

        #region LZW

        public static Dictionary<char, int> obtenerDiccionarioLZW(string path)
        {
            var dictionary = new Dictionary<char, int>();
            int hashKey = 1;

            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var buffer = reader.ReadBytes(bufferLength);

                        foreach (var item in buffer)
                        {
                            if (!dictionary.ContainsKey((char) item))
                            {
                                dictionary.Add((char) item, hashKey);
                                hashKey++;
                            }
                        }
                    }
                }
            }

            return dictionary;
        }

        #endregion
    }
}