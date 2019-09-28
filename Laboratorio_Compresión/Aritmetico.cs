using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Laboratorio_Compresión
{
    public class Aritmetico
    {
        public static string Aritmetica(string path, float[] primero, float[] segundo)
        {
            string Data = System.IO.File.ReadAllText(path, Encoding.Default); //buffer
            char[] carcateres = Data.ToCharArray(); 

            //declarar variables
            Boolean bandera = false;
            string mensaje = "";
            string ant = "";
            string sig = "";

            float ant2 = (float)0;
            float sig2 = (float)0;
            float antant = (float)0;

            //La formula
            for(int i = 0; i<= Data.Length -1; i++)
            {
                if(i==0)
                {
                    ant2 = primero[(int)Data[i]];
                    sig2 = sig[(int)Data[i]];
                }
                else
                {
                    antant = ant2;
                    ant2 = (sig2 - antant) * primero[(int)Data[i]] + antant;
                    sig2 = (sig2 - antant) * segundo[(int)Data[i]] + antant;
                }
            }

            while (ant2 != 1)
            {
                ant2 = ant2 * 2;
                if (ant2 > 1)
                {
                    ant = ant + "1";
                    ant2 = ant2 - 1;
                }
                else if (ant2 == 1)
                {
                    ant = ant + "1";
                }
                else
                {
                    ant = ant + "0";
                }
                if ((!bandera) && (ant2 != 1))
                {
                    sig2 = sig2 * 2;
                    if (sig2 > 1)
                    {
                        sig = sig + "1";
                        sig2 = sig2 - 1;
                    }
                    else if (sig2 == 1)
                    {
                        bandera = true;
                        sig = sig + "1";
                    }
                    else
                    {
                        sig = sig + "0";
                    }
                }
            }
            if(!bandera)
            {
                mensaje = sig;
            }
            else
            {
                mensaje = ant;
            }
            return (mensaje);
        }
    }
}