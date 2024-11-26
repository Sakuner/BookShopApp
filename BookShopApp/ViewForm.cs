using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookShopApp
{
    public partial class ViewForm : Form
    {
        private readonly DatabaseHelper _dbHelper;

        public ViewForm(string connectionString)
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper(connectionString);
            LoadRentals();
        }

        private void LoadRentals()
        {
            var rentalsTable = _dbHelper.ExecuteStoredProcedure("GetRentalList");
            rentalsGrid.DataSource = rentalsTable;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadRentals();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (rentalsGrid.CurrentRow == null) return;

            var rentalId = (int)rentalsGrid.CurrentRow.Cells["RentalId"].Value;
            var editForm = new EditForm(rentalId, _dbHelper);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadRentals();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
