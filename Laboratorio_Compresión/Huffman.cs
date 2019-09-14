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
        private static char EOF = '\u0003';

        public static void comprimir(string path)
        {

            string Data = System.IO.File.ReadAllText(path); //Sustituir por ciclo buffer.
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

            dictionary.Add(EOF , 1); //End of file
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

                        ByteArrayToFile(rutaComprimido, new byte[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) });
                        Byte = "";
                    }

                    completed = true;
                }
            }

            //Guardar configuracion para descomprimir

            configuracionParaDescomprimir(path, diccionario);

            #endregion

            HomeController.currentFile = rutaComprimido;

            FileInfo originalFile = new FileInfo(path);
            FileInfo compressedFile = new FileInfo(rutaComprimido);
            MisCompresiones.agregarNuevaCompresion(new MisCompresiones(Path.GetFileName(path), originalFile.Length, compressedFile.Length)); //Anadir a mis compresiones
        }

        public static void descomprimir(string path)
        {
            string carpetaHuffConfig = HomeController.directorioHuffmanConfig;
            
            DirectoryInfo infoConfig = new DirectoryInfo(carpetaHuffConfig);

            FileInfo[] listaHuffman = infoConfig.GetFiles();

            string rutaCaracteres = "";
            string rutaConfig = "";
            
            foreach (var archivo in listaHuffman)
            {
                if (Path.GetFileNameWithoutExtension(archivo.FullName) == Path.GetFileNameWithoutExtension(path))
                {
                    if (archivo.Extension == ".config")
                    {
                        rutaConfig = archivo.FullName;
                    }
                    if (archivo.Extension == ".char")
                    {
                        rutaCaracteres = archivo.FullName;
                    }
                }
            }
            
            descomprimir(path, rutaConfig, rutaCaracteres);
        }

        private static void descomprimir(string path, string pathConfig, string pathCharacters)
        {
            string rutaDescomprimido = HomeController.directorioHuffmanDecompress;
            string nombreArchivo = "";

            //Acceder a configuracion y recuperar diccionario

            Dictionary<string, char> codigosPrefijo = leerArchivoConfiguracion(pathConfig, pathCharacters, ref nombreArchivo);
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

                bool continuar = true;

                foreach (var value in found)
                {
                    if (value != EOF.ToString()) //End of file
                    {
                        bits = bits.Remove(0, value.Length); //Remover cadena de bits encontrada
                        escribirEnArchivo(rutaDescomprimido, codigosPrefijo[value].ToString()); //Escribir
                    }
                    else
                    {
                        continuar = false;
                        break;
                    }
                }

                if (!continuar) //Si se ha encontrado el EOF no seguir
                {
                    break;
                }
            }

            //Repetir hasta leer todo el archivo

            HomeController.currentFile = rutaDescomprimido;
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

        private static void configuracionParaDescomprimir(string path, Dictionary<char, string> diccionario)
        {

            string nombreArchivo = Path.GetFileNameWithoutExtension(path);

            string rutaCharacters = Path.Combine(HomeController.directorioHuffmanConfig, nombreArchivo + ".char");
            string rutaPrefijos = Path.Combine(HomeController.directorioHuffmanConfig, nombreArchivo + ".config");
            
            crearArchivo(rutaCharacters);
            crearArchivo(rutaPrefijos);

            var csv = new StringBuilder();
            string configuracion = string.Format("{0}", Path.GetFileName(path)); //Linea con informacion del archivo
            csv.AppendLine(configuracion);

            foreach (var item in diccionario)
            {
                ByteArrayToFile(rutaCharacters, new byte[] { Convert.ToByte(item.Key) }); //Escribir en archivo
                csv.AppendLine(item.Value);
            }
            
            //Escribir
            System.IO.File.WriteAllText(rutaPrefijos, csv.ToString());

        }

        private static Dictionary<string, char> leerArchivoConfiguracion(string pathConfig, string pathCharacters,  ref string nombreArchivo)
        {

            Dictionary<string, char> configuracion = new Dictionary<string, char>();

            if (!System.IO.File.Exists(pathConfig))
            {
                throw new Exception("No existe el archivo");
            }
            else if (!System.IO.File.Exists(pathCharacters))
            {
                throw new Exception("No existe el archivo");
            }
            else
            {
                byte[] bytes = System.IO.File.ReadAllBytes(pathCharacters);
                int i = 0;

                using (var reader = new StreamReader(pathConfig))
                {
                    bool firstline = true;
                    
                    while (!reader.EndOfStream) 
                    {
                        var line = reader.ReadLine();

                        if (firstline)
                        {
                            nombreArchivo = line;
                            firstline = false;
                        }
                        else
                        {
                            try
                            {
                                configuracion.Add(line, Convert.ToChar(bytes[i]));
                                i++;
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

    }
}