using System;
using System.Data.SqlTypes;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using RecupProductos1Gaston.Modelos;

namespace RecupProductos1Gaston
{
    public partial class Frm_Alta : Form
    {

        private Factura oFactura;


        public Frm_Alta()
        {
            InitializeComponent();
            oFactura = new Factura();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            //    LimpiarCampos();
            
         
            //pasar datos al objeto
            oFactura.Fecha = dtpFecha.Value;
            oFactura.Cliente = txtCliente.Text;
            oFactura.Forma_Pago = cboForma.SelectedIndex;
            
           if (oFactura.Confirmar())
            {
                MessageBox.Show("Factura guardada con exito", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
            else
            {
                MessageBox.Show("No se pudo guardar la factura", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Está seguro que desea cancelar?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Dispose();
            }
            else
            {
                return;
            }
        }

        private void Frm_Alta_Presupuesto_Load(object sender, EventArgs e)
        {
            CargarCombo();
        }

        private void CargarCombo()
        {
            SqlConnection conexion = new SqlConnection();
            conexion.ConnectionString = @"Data Source=DESKTOP-93BGHGN\SQLEXPRESS;Initial Catalog=Recup2022ProductoDetalle;Integrated Security=True";
            conexion.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conexion;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_CONSULTAR_PRODUCTOS";

            DataTable tabla = new DataTable();
            tabla.Load(cmd.ExecuteReader());
            conexion.Close();

            cboProducto.DataSource = tabla;
            cboProducto.DisplayMember = "n_producto";
            cboProducto.ValueMember = "id_producto";

            //cboProducto.DataSource = Service.GetProductos();
            //cboProducto.DisplayMember = "n_producto";
            //cboProducto.ValueMember = "id_producto";
        }
        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            if (Valido())
            {
                DataRowView oDataRow = (DataRowView)cboProducto.SelectedItem;

                //creamos el producto
                Producto p = new Producto();
                p.Id_Producto = Int32.Parse(oDataRow[0].ToString());
                p.NombreProd = oDataRow[1].ToString();
                p.Precio = double.Parse(oDataRow[2].ToString());

                int cantProd = int.Parse(nudCantidad.Value.ToString());

                //creamos el detalle
                DetalleFactura detalle = new DetalleFactura(p, cantProd);
                if (!ExisteProductoEnGrilla(cboProducto.Text))
                {
                    oFactura.AgregarDetalle(detalle);

                    dgvDetalles.Rows.Add(new object[]
                    {
                        p.Id_Producto, p.NombreProd, p.Precio, detalle.Cantidad, detalle.CalcularSubTotal()
                    });
                    CargarTotal();
                }
                else
                {
                    MessageBox.Show("El producto ya se encuentra como detalle", "CONTROL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void CargarTotal()
        {
            if (cboForma.Text == "1-Contado")
            {
                lblTotal.Text = "Total: $" + oFactura.CalcularTotal();
            }
            else
            {
                lblTotal.Text = "Total: $" + (oFactura.CalcularTotal() * 1.10);
            }
            
        }

        private bool ExisteProductoEnGrilla(string text)
        {
            foreach (DataGridViewRow fila in dgvDetalles.Rows)
            {
                if (fila.Cells["producto"].Value.Equals(text))
                    return true;
            }
            return false;
        }

        private bool Valido()
        {
            bool ok = true;

            if (string.IsNullOrEmpty(txtCliente.Text))
            {
                ok = false;
                MessageBox.Show("Debe completar el campo Cliente", "CONTROL", MessageBoxButtons.OK);
            }
            if (cboForma.SelectedIndex == -1)
            {
                ok = false;
                MessageBox.Show("Debe elegir una forma de Pago", "CONTROL", MessageBoxButtons.OK);
            }
            if (cboProducto.SelectedIndex == -1)
            {
                ok = false;
                MessageBox.Show("Debe selecionar un Producto", "CONTROL", MessageBoxButtons.OK);
            }
            return ok;
        }


        private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDetalles.CurrentCell.ColumnIndex == 5)
            {
                oFactura.QuitarDetalle(dgvDetalles.CurrentRow.Index);
                dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);
                CargarTotal();
            }
        }

    }
}
