using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecupProductos1Gaston.Modelos
{
    internal class DetalleFactura
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }

       
        public DetalleFactura(Producto producto, int cant)
        {
            this.Producto = producto;
            this.Cantidad = cant;
        }

        public double CalcularSubTotal()
        {
            return Cantidad * Producto.Precio;
        }


    }
}
