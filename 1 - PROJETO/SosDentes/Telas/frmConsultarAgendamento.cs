using SosDentes.ClnNegocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SosDentes.Telas
{
    public partial class frmConsultarAgendamento : Form
    {
        clnAgenda ObjAgenda = new clnAgenda();


        public frmConsultarAgendamento()
        {
            InitializeComponent();
        }


        public void CarregaDataGrid()
        {
            dgv.DataSource = ObjAgenda.RetornaAgendamento(txtPesquisar.Text);

            dgv.AutoResizeColumns();
            dgv.Columns[0].HeaderText = "CÓDIGO";
            dgv.Columns[1].Visible = false; // codigo paciente
            dgv.Columns[2].HeaderText = "PACIENTE";
            dgv.Columns[3].HeaderText = "CELULAR";
            dgv.Columns[4].Visible = false; // codigo SERVICO
            dgv.Columns[5].HeaderText = "SERVIÇO";
            dgv.Columns[6].HeaderText = "TEMPO ATENDIMENTO";
            dgv.Columns[7].Visible = false; // CODIGO DENTISTA
            dgv.Columns[8].HeaderText = "DENTISTA";
            dgv.Columns[9].HeaderText = "DATA";
            dgv.Columns[10].Visible = false; // DATA FIM
            dgv.Columns[11].HeaderText = "STATUS";

            if (dgv.RowCount == 0)
            {
                btnFinalizar.Enabled = false;
                btnCancelar.Enabled = false;
                MessageBox.Show("NÃO FORAM ENCONTRADOS DADOS COM A INFORMAÇÃO " + txtPesquisar.Text, "Verificar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgv.DataSource = null;
                txtPesquisar.Text = "";
                txtPesquisar.Focus();
            }
            else
            {
                btnFinalizar.Enabled = true;
                btnCancelar.Enabled = true;

            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("Deseja excluir o Agendamento: " + Convert.ToString(dgv.CurrentRow.Cells[0].Value + "?"),
                        "E X C L U S Ã O", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (DialogResult.Yes == resultado)
            {
                if (dgv.SelectedCells.Count == 1)
                {
                    MudarStatusItem("CANCELADO");
                }
            }
            else
            {
                MessageBox.Show("Operação cancelada ", "cancelamento E X C L U S Ã O", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            CarregaDataGrid();
        }

        private void MudarStatusItem(string status)
        {
            int[] indexSelecionados = dgv.SelectedCells.Cast<DataGridViewCell>().Select(p => p.RowIndex).Distinct().ToArray();
            if (indexSelecionados.Count() == 1)
            {
                int index = dgv.SelectedCells[0].RowIndex;
                DataTable dataTable = (DataTable)dgv.DataSource;
                string id_tratamento = dataTable.Rows[index]["id_tratamento"].ToString();

                ObjAgenda.AtualizarStatus(id_tratamento, status);
                CarregaDataGrid();

                MessageBox.Show(status + " COM SUCESSO", "AGENDAMENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("SELECIONE UM AGENDAMENTO POR VEZ ", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("DESEJA CONCLUIR O AGENDAMENTO: " + Convert.ToString(dgv.CurrentRow.Cells[0].Value + "?"),
                  "CONCLUSÃO", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (DialogResult.Yes == resultado)
            {
                if (dgv.SelectedCells.Count == 1)
                {
                    MudarStatusItem("CONCLUÍDO");
                }
            }
            else
            {
                MessageBox.Show("OPERAÇÃO CANCELADA ", "CONCLUÍDO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            CarregaDataGrid();
        }

        private void btnPesquisar_Click_1(object sender, EventArgs e)
        {
            CarregaDataGrid();
        }

        private void frmConsultarAgendamento_Load(object sender, EventArgs e)
        {

            btnCancelar.Enabled = false;
            btnFinalizar.Enabled = false;
            dgv.RowHeadersVisible = false;
        }

    }
}

