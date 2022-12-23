using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace EjPokemon
{
    public partial class FrmPokemons : Form
    {
        private List<Pokemon> listaPokemon;

        public FrmPokemons()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Número");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripción");
            //try
            //{
            //acceder a los datos
            //PokemonNegocio negocio = new PokemonNegocio();
            //listaPokemon = negocio.listar();
            //dgvPokemon.DataSource = listaPokemon;
            //dgvPokemon.Columns["UrlImagen"].Visible = false;  //ocultar esta columna en la grilla
            //dgvPokemon.Columns["Id"].Visible = false;
            //cargarImagen(listaPokemon[0].UrlImagen);
            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show(ex.ToString());
            //}

        }

        private void dgvPokemon_SelectionChanged(object sender, EventArgs e)
        {
            //CurrentRow--> fila actual    DataBoundItem-->objeto enlazado         
            if (dgvPokemon.CurrentRow != null)
            {
                Pokemon seleccionado = (Pokemon)dgvPokemon.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
            }
        }

        private void cargar()
        {
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                listaPokemon = negocio.listar();
                dgvPokemon.DataSource = listaPokemon;
                ocultarColumnas();
                cargarImagen(listaPokemon[0].UrlImagen);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            dgvPokemon.Columns["UrlImagen"].Visible = false;
            dgvPokemon.Columns["Id"].Visible = false;
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxPokemon.Load(imagen);
            }
            catch (Exception ex)
            {

                pbxPokemon.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaPokemon alta = new frmAltaPokemon();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Pokemon seleccionado;
            seleccionado = (Pokemon)dgvPokemon.CurrentRow.DataBoundItem;

            frmAltaPokemon modificar = new frmAltaPokemon(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();   
        }

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }

        private void eliminar(bool logico = false)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            Pokemon seleccionado;
            try
            {
                //esto se hace para que pregunte si se elimina o no y lo guarda en la variable respuesta
                DialogResult respuesta = MessageBox.Show("¿De verdad querés eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)dgvPokemon.CurrentRow.DataBoundItem;

                    if (logico)
                        negocio.eliminarLogico(seleccionado.Id);
                    else
                        negocio.eliminar(seleccionado.Id);

                    cargar();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar.");
                return true;
            }
            if (cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar.");
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "Número")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar el filtro para numéricos...");
                    return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Solo nros para filtrar por un campo numérico...");
                    return true;
                }

            }

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvPokemon.DataSource = negocio.filtrar(campo, criterio, filtro);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void txtFiltro_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Pokemon> listaFiltrada;
            string filtro = txtFiltro.Text;

            if (filtro.Length >= 3)
            {
                listaFiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()));//toUpper convierte en mayusculas
            }
            else
            {
                listaFiltrada = listaPokemon;
            }

            dgvPokemon.DataSource = null;
            dgvPokemon.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if (opcion == "Número")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(@"C:\Users\Usuario\Documents\PDFGenerado.pdf",FileMode.Create);
            Document doc = new Document(PageSize.A4,5,5,7,7);
            PdfWriter ptr = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            //titulo y autor
            doc.AddAuthor("Pato");
            doc.AddTitle("Pokemons");

            //fuente
            iTextSharp.text.Font standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            //encabezado
            doc.Add(new Paragraph("Pokemon"));
            doc.Add(Chunk.NEWLINE);

            //encabezado de columnas
            PdfPTable tbleEjemplo = new PdfPTable(3);
            tbleEjemplo.WidthPercentage = 100;

            //titulo de las columnas
            PdfPCell c1Nombre = new PdfPCell(new Phrase("Nombre",standardFont));
            c1Nombre.BorderWidth = 0;
            c1Nombre.BorderWidthBottom = 0.75f;

            PdfPCell c1Descripcion = new PdfPCell(new Phrase("Descripcion", standardFont));
            c1Descripcion.BorderWidth = 0;
            c1Descripcion.BorderWidthBottom = 0.75f;


            //añadir las columnas a la tabla
            tbleEjemplo.AddCell(c1Nombre);
            tbleEjemplo.AddCell(c1Descripcion);

            //agregando datos
            foreach(var estudiante in listaPokemon)
            {
                c1Nombre = new PdfPCell(new Phrase(estudiante.Nombre, standardFont));
                c1Nombre.BorderWidth = 0;

                c1Descripcion = new PdfPCell(new Phrase(estudiante.Descripcion, standardFont));
                c1Descripcion.BorderWidth = 0;
            }

            doc.Add(tbleEjemplo);

            doc.Close();
            ptr.Close();

            MessageBox.Show("Documento generado satisfactoriamente","Exito",MessageBoxButtons.OK,MessageBoxIcon.Information);

        }
    }
}
