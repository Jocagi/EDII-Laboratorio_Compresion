using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Laboratorio_Compresión.Controllers;

namespace Laboratorio_Compresión
{
    public class LZW
    {
        #region prueba1
        /* public Dictionary<int, string> Comprimir(string path, ref List<int> indices)
         {
             Dictionary<int, string> prefijos = new Dictionary<int, string>();

             for (int i = 0; i < 256; i++)
                 prefijos.Add(i, new string((char)i, 1));
             char c = '\0';
             int index = 1, n = path.Length, nextkey = 256;
             string s = new string(path[0], 1), sc = string.Empty;

             while (index<n)
             {
                 c = path[index++];
                 sc = s + c;

                 if (prefijos.ContainsValue(sc))
                     s = sc;

                 else
                 {
                     foreach(KeyValuePair<int, string> item in prefijos)
                     {
                         if (item.Value==s)
                         {
                             indices.Add(item.Key);
                                 break;
                         }
                     }

                     prefijos.Add(nextkey++, sc);
                     s = new string(c, 1);
                 }
             }

             foreach(KeyValuePair<int, string>item in prefijos)
             {
                 if(item.Value==s)
                 {
                     indices.Add(item.Key);
                     break;
                 }
             }
             return prefijos;
         }*/
        #endregion
            
        public static void comprimir(string path)
        {
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
            foreach(char t in Caracteres)
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

            if (!string.IsNullOrEmpty(c))
                comprimir.Add(diccionario[c]);
        }

        public static void descomprimir(string path)
        {
            string Data = System.IO.File.ReadAllText(path, Encoding.Default); //buffer
            List<char> Caracteres = Data.ToList<char>();

            //Volver a crear diccionario
            Dictionary<int, string> segundo = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                segundo.Add(i, ((char)i).ToString());

            string c = segundo[Caracteres[0]];
            Caracteres.RemoveAt(0);
            StringBuilder descomprimir = new StringBuilder(c);

            foreach(int t in Caracteres)
            {
                string entry = null;
                if(segundo.ContainsKey(t))
                {
                    entry = segundo[t];
                }
                else if (t==segundo.Count)
                {
                    entry = c + c[0];
                }

                descomprimir.Append(entry);

                //  Agregar nueva frase

                segundo.Add(segundo.Count, c + entry[0]);

                c = entry;
                    
            }

        }
    }
}