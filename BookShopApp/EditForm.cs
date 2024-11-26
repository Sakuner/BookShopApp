using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookShopApp
{
    public partial class EditForm : Form
    {
        private readonly int _rentalId;
        private readonly DatabaseHelper _dbHelper;

        public EditForm(int rentalId, DatabaseHelper dbHelper)
        {
            InitializeComponent();
            _rentalId = rentalId;
            _dbHelper = dbHelper;
            LoadRentalDetails();
        }

        private void LoadRentalDetails()
        {
            var parameters = new[] { new SqlParameter("@RentalId", _rentalId) };
            var rentalTable = _dbHelper.ExecuteStoredProcedure("GetRentalById", parameters);

            if (rentalTable.Rows.Count > 0)
            {
                var row = rentalTable.Rows[0];
                txtDueDate.Text = Convert.ToDateTime(row["DueDate"]).ToString("dd.MM.yyyy");
                txtReturnDate.Text = row["ReturnDate"] == DBNull.Value ? "" : Convert.ToDateTime(row["ReturnDate"]).ToString("dd.MM.yyyy");
                txtPenaltyFee.Text = row["PenaltyFee"].ToString();
            }
        }

        private void EditForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var parameters = new[]
            {
                new SqlParameter("@RentalId", _rentalId),
                new SqlParameter("@DueDate", DateTime.Parse(txtDueDate.Text)),
                new SqlParameter("@ReturnDate", string.IsNullOrEmpty(txtReturnDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtReturnDate.Text)),
                new SqlParameter("@PenaltyFee", decimal.Parse(txtPenaltyFee.Text))
            };

            _dbHelper.ExecuteStoredProcedure("UpdateRental", parameters);
            MessageBox.Show("Dane zostały zapisane.");
            DialogResult = DialogResult.OK;
        }

        private void txtReturnDate_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
