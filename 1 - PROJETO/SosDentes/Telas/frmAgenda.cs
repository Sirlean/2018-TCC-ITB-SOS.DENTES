using SosDentes.BD;
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
    public partial class frmAgenda : Form
    {
        clnAgenda ObjAgenda = new clnAgenda();
        public static string texto;
        clnUtil ObjUtil = new clnUtil();
        public static string Temp2;

        public frmAgenda()
        {
            InitializeComponent();
        }

        private void btnAgendar_Click(object sender, EventArgs e)
        {
            clnAgenda ObjAgenda = new clnAgenda();

            if (txtPesClien.Text == "" || comboBox2.Text == "" || comboBox1.Text == "" || comboBox4.Text == "" || dateTimePicker1.Text == "")
            {
                MessageBox.Show("TODOS OS CAMPOS SÃO OBRIGATÓRIOS ", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string formato = "dd/MM/yyyy HH:mm";
                DataRowView servicoSelecionado = (DataRowView)comboBox2.SelectedItem;
                DataRowView especialistaSelecionado = (DataRowView)comboBox1.SelectedItem;
                string id_especialista = especialistaSelecionado["id_funcionario"].ToString();
                DateTime dataInicio = DateTime.ParseExact(dateTimePicker1.Text + " " + comboBox4.Text, formato, System.Globalization.CultureInfo.CurrentCulture);
                DateTime dataFim = dataInicio.Add(TimeSpan.Parse(servicoSelecionado["Tempo_Atendimento"].ToString()));

                if (ObjAgenda.ValidarDataAgendamento(dataInicio.ToString(formato), dataFim.ToString(formato), id_especialista))
                {
                    ObjAgenda.Procedimento = servicoSelecionado["id_servico"].ToString();
                    ObjAgenda.Nome = Temp2;
                    ObjAgenda.Especialista = id_especialista;
                    ObjAgenda.Hora = comboBox4.Text;
                    string data = dateTimePicker1.Text;
                    ObjAgenda.Status = "AGENDADO";
                    ObjAgenda.Data = data;
                    ObjAgenda.GravaNoBanco();
                    PreencherAgendamentos();
                    MessageBox.Show("AGENDADO COM SUCESSO", "AGENDAMENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("PERIODO INDISPONÍVEL PARA AGENDAMENTO ", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            //CarregaDataGrid();
        }


        //public void InsereAgendaGrid()
        //{

        //    dgv.Columns[0].HeaderText = ("Data");
        //    dgv.Columns[1].HeaderText = ("Hora");
        //    dgv.Columns[2].HeaderText = ("Procedimento");
        //    dgv.Columns[3].HeaderText = ("Especialista");
        //    dgv.Columns[4].HeaderText = ("Status");

        //    dgv.AutoResizeColumns();

        //    dgv.Rows.Add();
        //    dgv.CurrentRow.Cells[0].Value = dateTimePicker1.Text;
        //    dgv.CurrentRow.Cells[1].Value = comboBox4.Text;
        //    dgv.CurrentRow.Cells[2].Value = comboBox2.Text;
        //    dgv.CurrentRow.Cells[3].Value = comboBox1.Text;
        //    dgv.CurrentRow.Cells[4].Value = cboStatus.Text;
        //}
        private void btnPesquisa_Click(object sender, EventArgs e)
        {
            frmPesquisarPacientes abrir = new frmPesquisarPacientes();
            clnUtil.Temp = txtPesClien.Text;

            abrir.ShowDialog();

            txtPesClien.Text = clnUtil.Temp;

            if (!string.IsNullOrEmpty(txtPesClien.Text))
            {
                comboBox2.Enabled = true;
                PreencherTipoServico();
                PreencherAgendamentos();
            }

        }

        private void PreencherAgendamentos()
        {
            DataTable dataTable = ObjAgenda.LocalizarAgendamentos(txtPesClien.Text);
            dgv.Columns.Clear();
            dgv.DataSource = dataTable;
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
        }

        private void frmAgenda_Load(object sender, EventArgs e)
        {
            comboBox2.Enabled = false;
            comboBox1.Enabled = false;
        }

        public void PreencherTipoServico()
        {
            comboBox2.DataSource = ObjUtil.PreencherTipoServico();
            comboBox2.ValueMember = "Des_Servico";
            comboBox2.DisplayMember = "Des_servico";
            // comboBox2.SelectedIndex = 1;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {


            comboBox1.Enabled = true;
            comboBox1.Text = "";
            PreencherDentista();


        }

        private void comboBox2_Click(object sender, EventArgs e)
        {
            PreencherTipoServico();
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {

            PreencherDentista();
        }
        public void PreencherDentista()
        {
            string nome_servico = (comboBox2.SelectedValue).ToString();
            comboBox1.DataSource = ObjUtil.PreencherDentista(nome_servico);
            comboBox1.ValueMember = "DENTISTA";
            comboBox1.DisplayMember = "DENTISTA";
            //comboBox1.ValueMember = "id_dentista";
            //comboBox1.DisplayMember = "id_dentista";

            //    comboBox1.SelectedIndex = 1;
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedCells.Count == 1)
            {
                MudarStatusItem("CANCELADO");
            }
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedCells.Count == 1)
            {
                MudarStatusItem("CONCLUÍDO");
            }
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
                PreencherAgendamentos();

                MessageBox.Show(status + " COM SUCESSO", "AGENDAMENTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("SELECIONE UM AGENDAMENTO POR VEZ ", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

