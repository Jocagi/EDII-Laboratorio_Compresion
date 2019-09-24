using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Laboratorio_Compresión
{
    public class LZW
    {
        
        public Dictionary<int, string> Comprimir(string path, ref List<int> indices)
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
        }

        public static void descomprimir(string path)
        {

        }
    }
}