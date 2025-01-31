using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class receipt : Form
    {
        int _saleID;
        public receipt(int saleID)
        {
            InitializeComponent();
            _saleID = saleID;
        }

        private void style(DataGridView datagridview)
        {
            datagridview.ColumnHeadersDefaultCellStyle.Font = new Font("NRT Bold", 12, FontStyle.Regular);
            datagridview.ColumnHeadersDefaultCellStyle.BackColor = Color.Teal;
            datagridview.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            datagridview.AllowUserToAddRows = false;
            datagridview.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            datagridview.RowTemplate.Height = 40;
            datagridview.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            datagridview.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            datagridview.EnableHeadersVisualStyles = false;
            datagridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            datagridview.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            datagridview.RowsDefaultCellStyle.BackColor = Color.White;
            datagridview.BorderStyle = BorderStyle.Fixed3D;
            datagridview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            datagridview.GridColor = Color.Gray;
            datagridview.DefaultCellStyle.SelectionBackColor = Color.DarkBlue;
            datagridview.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void receipt_Load(object sender, EventArgs e)
        {
            style(dataGridView1);
            DB db = new DB();

            // Query SaleDetails for ProductID, Quantity, UnitPrice, Subtotal, and Discount
            string detailsQuery = @"
            SELECT 
                sd.ProductID, 
                p.ProductName, 
                sd.Quantity, 
                sd.UnitPrice, 
                sd.Subtotal, 
               sd.DiscountAmount  -- ✅ Fetch total discount per item
            FROM SalesDetails sd
            INNER JOIN Product p ON sd.ProductID = p.ProductID
            WHERE sd.SaleID = @SaleID";

            var detailsParams = new Dictionary<string, object> { { "@SaleID", _saleID } };
            DataTable detailsTable = db.GetDataTable(detailsQuery, detailsParams);

            dataGridView1.DataSource = detailsTable;  // Show data in DataGridView

            // Query TotalAmount from Sales table
            string totalQuery = "SELECT TotalAmount FROM Sales WHERE SaleID = @SaleID";
            object totalObj = db.ExecuteScalar(totalQuery, detailsParams);

            if (totalObj != null)
            {
                decimal totalAmount = Convert.ToDecimal(totalObj);
                label13.Text = totalAmount.ToString("N0") + " IQD"; // Format with thousands separator
            }

            // Query Total Discount from SalesDetails table
            string discountQuery = "SELECT SUM(DiscountAmount) FROM SalesDetails WHERE SaleID = @SaleID";
            object discountObj = db.ExecuteScalar(discountQuery, detailsParams);

            if (discountObj != DBNull.Value && discountObj != null)
            {
                decimal totalDiscount = Convert.ToDecimal(discountObj);
                labelTotalDiscount.Text = "Total Discount: " + totalDiscount.ToString("N0") + " IQD"; // ✅ Show discount
            }
            else
            {
                labelTotalDiscount.Text = "Total Discount: 0 IQD";
            }
        }
    }
}