using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laboratorio_Compresión
{
    public class ArbolBinario
    {
        
        public Nodo Raiz { get; set; }

        public bool esVacio { get { return Raiz == null; } }

        //Constructor 
        public ArbolBinario() { }

    }
}