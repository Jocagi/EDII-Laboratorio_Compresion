using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Laboratorio_Compresión.Models;
using System.Text;
using System.IO;

using System.Web.Mvc;
using Laboratorio_Compresión.Controllers;

namespace Laboratorio_Compresión
{
    public static class Huffman
    {

        public static void comprimir(string path)
        {

            string Data = System.IO.File.ReadAllText(path); //To Do.. Sustituir por Bufffer
            List<char> Caracteres = Data.ToList<char>();

            Dictionary<char, int> dictionary = new Dictionary<char, int>();

            #region Caracteres
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
            #endregion

            #region Codigos_Prefijo
            //Crear arbol Huffman
            ArbolHuffman arbol = new ArbolHuffman(dictionary);
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

            #endregion

            #region Escritura
            //Escribir nuevo archivo

            string nombreNuevoArchivo = Path.GetFileNameWithoutExtension(path) + ".huff";
            string rutaComprimido = Path.Combine(HomeController.directorioHuffman, nombreNuevoArchivo);

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
                            //Agregar Ascii al comprimido
                                //textoComprimido += Encoding.UTF8.GetString(new byte[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) });

                        ByteArrayToFile(rutaComprimido, new byte[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) }); //Escribir en archivo
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

                        //textoComprimido += Encoding.UTF8.GetString(new byte[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) });
                        ByteArrayToFile(rutaComprimido, new byte[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) });
                        Byte = "";
                    }

                    completed = true;
                }
            }

                             //System.IO.File.WriteAllText(rutaComprimido, textoComprimido); //Escribir en archivo

            //Guardar configuracion para descomprimir

            string nombreArchivoConfig = Path.GetFileNameWithoutExtension(path) + ".config";
            string rutaConfig = Path.Combine(HomeController.directorioHuffmanConfig, nombreNuevoArchivo);
            
            if (System.IO.File.Exists(rutaConfig))
            {
                System.IO.File.Delete(rutaConfig);
            }
            
            FileStream fs = System.IO.File.Create(rutaConfig);
            fs.Close();
            
            var csv = new StringBuilder();

            string configuracion = string.Format("{0}", Path.GetFileName(path)); //Linea con informacion del archivo
            csv.AppendLine(configuracion);

            foreach (var item in diccionario)
            {
                //Lineas del diccionario
                string newLine = string.Format("{0},{1}", item.Key, item.Value);
                csv.AppendLine(newLine);
            }
            
            //Escribir
            System.IO.File.WriteAllText(rutaConfig, csv.ToString());

            #endregion
        }

        public static void descomprimir(string path)
        {
            //Acceder a configuracion y recuperar diccionario

            //Crear nuevo archivo en blanco

            //Leer arreglos de bits y guardarlos en memoria principal

            //Formatear bits

            string yourByteString = Convert.ToString(byteArray[20], 2).PadLeft(8, '0'); // produces "00111111"


            //Comparar con codiigos prefijo

            //Al enccontrar una coincidencia escbribir el resultado en un archivo

            //Repetir hasta leer todo el archivo

        }

        private static void ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception caught in process: {0}", ex);
            }
        }
    }
}