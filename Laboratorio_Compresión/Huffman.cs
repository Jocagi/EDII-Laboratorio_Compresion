using System;
using System.Collections.Generic;
using System.Linq;
using Laboratorio_Compresión.Models;
using System.Text;
using System.IO;

using Laboratorio_Compresión.Controllers;

namespace Laboratorio_Compresión
{
    public static class Huffman
    {
        private const char EOF = '\u0003';

        public static void comprimir(string path)
        {

            #region Caracteres
            //Leer caracteres y contarlos

            Dictionary<char, int> dictionary = Lectura.obtenerDiccionarioFrecuencias(path);

            dictionary.Add(EOF, 1); //End of file
            #endregion

            #region Codigos_Prefijo
            //Crear arbol Huffman
            ArbolHuffman arbol = new ArbolHuffman(dictionary);
            Dictionary<char, string> diccionario = arbol.obtenerDiccionarioBinario();

            //Leer archivo original y sustituir por codigos prefijo

            string textoBinario = Lectura.textoBinario(path, diccionario);
            textoBinario += diccionario[EOF]; //End of file

            #endregion

            #region Escritura
            //Escribir nuevo archivo

            string nombreNuevoArchivo = Path.GetFileNameWithoutExtension(path) + ".huff";
            string rutaComprimido = Path.Combine(HomeController.directorioHuffman, nombreNuevoArchivo);

            Archivo.crearArchivo(rutaComprimido);
            
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
                        ByteArrayToFile(rutaComprimido, new[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) }); //Escribir en archivo
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

                        ByteArrayToFile(rutaComprimido, new[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) });
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

            Archivo.crearArchivo(rutaDescomprimido);

            //Leer arreglos de bits y guardarlos en memoria principal

            string bits = "";
            
            int bufferLength = 1024;
            byte[] buffer;

            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLength);
                        foreach (var item in buffer)
                        {
                            string tmp = "";

                            //Formatear bits
                            string ByteString = Convert.ToString(item, 2).PadLeft(8, '0'); // produce cadena "00111111"
                            bits += ByteString;

                            //Comparar con codigos prefijo
                            bool continuar = true;

                            foreach (var numero in bits)
                            {
                                tmp += numero;

                                if (codigosPrefijo.ContainsKey(tmp)) //Al encontrar una coincidencia escbribir el resultado en un archivo
                                {
                                    if (codigosPrefijo[tmp] != EOF)
                                    {
                                        bits = bits.Remove(0, tmp.Length); //Remover cadena de bits encontrada
                                        ByteArrayToFile(rutaDescomprimido, new[] { Convert.ToByte(Convert.ToInt32(codigosPrefijo[tmp])) });
                                        tmp = "";
                                    }
                                    else
                                    {
                                        continuar = false;
                                        break;
                                    }
                                }
                            }

                            if (!continuar) //Si se ha encontrado el EOF no seguir
                            {
                                break;
                            }
                        }
                    }
                }
            }
            //Repetir hasta leertodo el archivo

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
        
        private static void configuracionParaDescomprimir(string path, Dictionary<char, string> diccionario)
        {

            string nombreArchivo = Path.GetFileNameWithoutExtension(path);

            string rutaCharacters = Path.Combine(HomeController.directorioHuffmanConfig, nombreArchivo + ".char");
            string rutaPrefijos = Path.Combine(HomeController.directorioHuffmanConfig, nombreArchivo + ".config");
            
            Archivo.crearArchivo(rutaCharacters);
            Archivo.crearArchivo(rutaPrefijos);

            var csv = new StringBuilder();
            string configuracion = $"{Path.GetFileName(path)}"; //Linea con informacion del archivo
            csv.AppendLine(configuracion);

            foreach (var item in diccionario)
            {
                ByteArrayToFile(rutaCharacters, new[] { Convert.ToByte(item.Key) }); //Escribir en archivo
                csv.AppendLine(item.Value);
            }
            
            //Escribir
            File.WriteAllText(rutaPrefijos, csv.ToString());

        }

        private static Dictionary<string, char> leerArchivoConfiguracion(string pathConfig, string pathCharacters,  ref string nombreArchivo)
        {

            Dictionary<string, char> configuracion = new Dictionary<string, char>();

            if (!File.Exists(pathConfig))
            {
                throw new Exception("No existe el archivo");
            }
            else if (!File.Exists(pathCharacters))
            {
                throw new Exception("No existe el archivo");
            }
            else
            {
                byte[] bytes = File.ReadAllBytes(pathCharacters);
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
                            configuracion.Add(line, Convert.ToChar(bytes[i]));
                            i++;
                        }
                    }
                    reader.Close();
                }
            }

            return configuracion;

        }

    }
}