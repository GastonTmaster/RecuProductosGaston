using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecupProductos1Gaston.Modelos
{
    internal class Producto
    {
        public int Id_Producto { get; set; }
        public string NombreProd { get; set; }
        public double Precio { get; set; }
        public string Activo { get; set; }

        public Producto(int id_producto, string nombreprod, double precio)
        {
            Id_Producto = id_producto;
            NombreProd = nombreprod;
            Precio = precio;
        }
        public Producto()
        {
            Id_Producto = 0;
            NombreProd = "";
            Precio = 0;
        }
    }
}
