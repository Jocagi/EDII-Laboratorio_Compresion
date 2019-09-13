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

            crearArchivo(rutaComprimido);
            
            //Construir texto huffman 
            string Byte = ""; //Valor de 8 bits 
            List<char> binario = textoBinario.ToArray().ToList(); //Arreglo de texto en binario
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

            crearArchivo(rutaConfig);
            
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
            string carpetaHuffConfig = HomeController.directorioHuffmanConfig;
            
            DirectoryInfo infoConfig = new DirectoryInfo(carpetaHuffConfig);
            FileInfo[] listaHuffman = infoConfig.GetFiles();

            bool success = false;

            foreach (var archivo in listaHuffman)
            {
                if (Path.GetFileNameWithoutExtension(archivo.FullName) == Path.GetFileNameWithoutExtension(path))
                {
                    success = !success;
                    descomprimir(path, archivo.FullName);
                    break;
                }
            }

            if (!success)
            {
                HomeController.mensaje = "No es posible descomprimir el archivo debido a que no se han encontrado los codigos prefijo";
            }
        }

        private static void descomprimir(string path, string pathConfig)
        {
            string rutaDescomprimido = HomeController.directorioHuffmanDecompress;
            string nombreArchivo = "";

            //Acceder a configuracion y recuperar diccionario

            Dictionary<string, char> codigosPrefijo = leerArchivoConfiguracion(pathConfig, ref nombreArchivo);
            rutaDescomprimido += "/" + nombreArchivo;
            
            //Crear nuevo archivo en blanco

            crearArchivo(rutaDescomprimido);

            //Leer arreglos de bits y guardarlos en memoria principal

            byte[] bytes = System.IO.File.ReadAllBytes(path); //To Do.. Sustituir por Bufffer
            string bits = "";
            
            foreach (var item in bytes)
            {
                //Formatear bits
                string ByteString = Convert.ToString(item, 2).PadLeft(8, '0'); // produce cadena "00111111"
                bits += ByteString;

                //Comparar con codigos prefijo
                string tmp = "";
                List<string> found = new List<string>();

                foreach (var numero in bits)
                {
                    tmp += numero;

                    if (codigosPrefijo.ContainsKey(tmp))
                    {
                        found.Add(tmp);
                        tmp = "";
                    }
                }

                //Al encontrar una coincidencia escbribir el resultado en un archivo

                foreach (var value in found)
                {
                    bits = bits.Remove(0, value.Length); //Remover cadena de bits encontrada
                    escribirEnArchivo(rutaDescomprimido, codigosPrefijo[value].ToString()); //Escribir
                }
            }

            //Repetir hasta leer todo el archivo
        }

        private static Dictionary<string, char> leerArchivoConfiguracion(string path, ref string nombreArchivo)
        {

            Dictionary<string, char> configuracion = new Dictionary<string, char>();

            if (!System.IO.File.Exists(path)) //No existe el archivo
            {
                throw new Exception("No existe el archivo");
            }
            else
            {
                using (var reader = new StreamReader(path))
                {
                    /*
                    Formato:

                    char, codigo
                    a, 101

                     */

                    bool firstline = true;

                    while (!reader.EndOfStream) //Recorrer archivo hasta el final
                    {

                        var line = reader.ReadLine();

                        if (firstline)
                        {
                            nombreArchivo = line;
                            firstline = false;
                        }
                        else
                        {
                            string[] datos = line.Split(','); //dividir datos

                            try
                            {
                                configuracion.Add(datos[1], Convert.ToChar(datos[0])); 
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    reader.Close();
                }
            }

            return configuracion;

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

        private static void crearArchivo(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            FileStream fs = System.IO.File.Create(path);
            fs.Close();
        }

        private static void escribirEnArchivo(string path, string text)
        {
            if (File.Exists(path))
            {
                //using (StreamWriter sw = File.AppendText(path))
                //{
                //    sw.WriteLine(text);
                //}

                File.AppendAllText(path, text);
            }
            else
            {
                throw new Exception("El Archivo no existe");
            }
            
        }
    }
}