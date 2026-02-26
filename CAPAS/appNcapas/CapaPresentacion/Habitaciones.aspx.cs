using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CapaPresentacion
{
    public partial class Habitaciones : System.Web.UI.Page
    {
        CNHabitaciones _habitaciones = new CNHabitaciones();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarGrid();
            }
        }
        protected void CargarGrid()
        {
            dgvHabitaciones.DataSource = _habitaciones.ObtenerhabitacionesN();
            dgvHabitaciones.DataBind();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int numero = Convert.ToInt32(txtNumero.Text);
            string descripcion = txtDescripcion.Text;
            int cant = Convert.ToInt32(txtCant.Text);

            bool correcto = _habitaciones.agregar_habitacion(numero, descripcion, cant);
            if (correcto)
            {
                Response.Write("<script>alert('Habitación agregada correctamente');</script>");
                CargarGrid();
            }
            else
            {
                Response.Write("<script>alert('Error al agregar la habitación');</script>");
            }
        }

        protected void dgvHabitaciones_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void dgvHabitaciones_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }

        protected void dvgHabitaciones_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(dgvHabitaciones.DataKeys[e.RowIndex].Value);
        }

        protected void dgvHabitaciones_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            dgvHabitaciones.EditIndex = -1;
            CargarGrid();
        }
    }
}