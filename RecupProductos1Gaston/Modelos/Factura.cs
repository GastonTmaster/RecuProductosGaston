using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RecupProductos1Gaston.Modelos
{
    internal class Factura
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public int Forma_Pago { get; set; }
        public DateTime Fecha_Baja { get; set; }
        public double Total { get; set; }

        public List<DetalleFactura> lDetalle { get; set; }

        public Factura()
        {
            lDetalle = new List<DetalleFactura>();
        }

        public void AgregarDetalle(DetalleFactura detalle)
        {
            lDetalle.Add(detalle);
        }

        public void QuitarDetalle(int indice)
        {
            lDetalle.RemoveAt(indice);
        }

        public double CalcularTotal()
        {
            double total = 0;
            foreach (DetalleFactura detalle in lDetalle)
            {
                total += detalle.CalcularSubTotal();
            }
            return total;
        }

        public bool Confirmar()
        {
            SqlTransaction transaccion = null;
            SqlConnection conexion = new SqlConnection(@"Data Source=DESKTOP-93BGHGN\SQLEXPRESS;Initial Catalog=Recup2022ProductoDetalle;Integrated Security=True");

            bool flag = true;
            try
            {
                conexion.Open();
                transaccion = conexion.BeginTransaction();

                SqlCommand cmdFactura = new SqlCommand("SP_INSERTAR_FACTURA", conexion, transaccion);
                cmdFactura.CommandType = CommandType.StoredProcedure;

                //cmdFactura.Parameters.AddWithValue("@fecha", Fecha);
                cmdFactura.Parameters.AddWithValue("@cliente", Cliente);
                cmdFactura.Parameters.AddWithValue("@forma_pago", Forma_Pago);
                cmdFactura.Parameters.AddWithValue("@total", Total);
               

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@nro_fact";
                param.SqlDbType = SqlDbType.Int;

                param.Direction = ParameterDirection.Output;
                cmdFactura.Parameters.Add(param);
                cmdFactura.ExecuteNonQuery();

                int nroFact = (int)param.Value;

                int nroDetalle = 1;


                foreach (DetalleFactura det in lDetalle)
                {
                    SqlCommand cmdDetalle = new SqlCommand("SP_INSERTAR_DETALLES", conexion, transaccion);
                    cmdDetalle.CommandType = CommandType.StoredProcedure;

                    cmdDetalle.Parameters.AddWithValue("@detalle_nro", ++nroDetalle);
                    cmdDetalle.Parameters.AddWithValue("@nro_fact", nroFact);
                    cmdDetalle.Parameters.AddWithValue("@id_producto", det.Producto.Id_Producto);
                    cmdDetalle.Parameters.AddWithValue("@cantidad", det.Cantidad);
                    cmdDetalle.ExecuteNonQuery();
                }

                transaccion.Commit();
            }
            catch
            {
                transaccion.Rollback();
                flag = false;
            }
            finally
            {
                if (conexion != null && conexion.State == ConnectionState.Open)
                    conexion.Close();
            }
            return flag;
        }
    }
}
