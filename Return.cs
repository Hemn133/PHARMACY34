using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp1
{
    public partial class Return : UserControl
    {
        private string _id;
        public Return(string id = "")
        {
            InitializeComponent();
            _id = id;
        }
        private void style(DataGridView datagridview)
        {
            datagridview.ColumnHeadersDefaultCellStyle.Font = new Font("NRT Bold", 12, FontStyle.Regular); // Adjust size if needed
            datagridview.ColumnHeadersDefaultCellStyle.BackColor = Color.Teal; // Set background color to teal
            datagridview.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // Set text color to white for better contrast
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
            ReverseColumnsOrder(datagridview);
        }

        private void ReverseColumnsOrder(DataGridView dataGridView)
        {
            int columnCount = dataGridView.Columns.Count;

            for (int i = 0; i < columnCount; i++)
            {
                dataGridView.Columns[i].DisplayIndex = columnCount - 1 - i;
            }
        }

        private void ReturnAdmin_Load(object sender, EventArgs e)
        {
            style(dataGridView1);
            style(dataGridView2);
            ExpenseAmount.Text = _id;

            if (!string.IsNullOrWhiteSpace(ExpenseAmount.Text))
            {
                // Trigger button1 click event
                button2.PerformClick();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(ExpenseAmount.Text, out int saleId))
            {
                LoadSaleDetails(saleId);
            }
            else
            {
                MessageBox.Show("ئەم کۆدە بوونی نییە.");

            }
        }
        private void LoadSaleDetails(int saleId)
        {
            string query = @"
    SELECT sd.ProductID, p.ProductName, sd.Quantity,sd.Subtotal, sd.ReturnedQuantity 
    FROM SalesDetails sd
    JOIN Product p ON sd.ProductID = p.ProductID
    WHERE sd.SaleID = @SaleID AND (sd.Quantity - sd.ReturnedQuantity) > 0";

            Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "@SaleID", saleId }
    };

            DB db = new DB();
            DataTable saleDetails = db.GetDataTableParam(query, parameters);

            if (saleDetails.Rows.Count > 0)
            {
                // Bind to DataGridView
                dataGridView1.DataSource = saleDetails;

                // Rename DataGridView column headers
                dataGridView1.Columns["ProductID"].Visible = false;
                dataGridView1.Columns["Subtotal"].HeaderText = "کۆی گشتی";
                dataGridView1.Columns["ProductName"].HeaderText = "ناوی کاڵا";
                dataGridView1.Columns["Quantity"].HeaderText = "دانە";
                //dataGridView1.Columns["ReturnedQuantity"].HeaderText = "ژمارەی کاڵای گەڕاوە";
                dataGridView1.Columns["ReturnedQuantity"].Visible = false;
                // Populate ComboBox with product names
                //ProductSelection.Items.Clear();
                //foreach (DataRow row in saleDetails.Rows)
                //{
                //    ProductSelection.Items.Add(row["ProductName"].ToString());
                //}
            }
            else
            {
                MessageBox.Show("هێچ کاڵایەک بۆ گەڕاندن بەردەست نییە.");
            }
        }

        private void addtolist_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Columns.Count == 0) // Only set up columns once
            {
                dataGridView2.Columns.Clear();
                dataGridView2.Columns.Add("ProductID", "کۆدی کاڵا");
                dataGridView2.Columns.Add("ProductName", "ناوی کاڵا");
                dataGridView2.Columns.Add("Quantity", "گەڕاوە");
            }

            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    var selectedProductID = row.Cells["ProductID"].Value;
                    var selectedProductName = row.Cells["ProductName"].Value;
                    int soldQuantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    int alreadyReturned = Convert.ToInt32(row.Cells["ReturnedQuantity"].Value);
                    int remainingQuantity = soldQuantity - alreadyReturned;
                    int returnQuantity = (int)numericUpDown1.Value; // Use the NumericUpDown value

                    if (returnQuantity <= 0)
                    {
                        MessageBox.Show("تکایە ژمارەی دروست داخڵ بکە.");
                        return;
                    }

                    if (returnQuantity > remainingQuantity)
                    {
                        MessageBox.Show($"ژمارەی گەڕاندن زیاترە لە ژمارەی بەردەست ({remainingQuantity}).");
                        return;
                    }

                    // Check if the product already exists in the return list
                    bool exists = false;
                    foreach (DataGridViewRow returnRow in dataGridView2.Rows)
                    {
                        if (Convert.ToInt32(returnRow.Cells["ProductID"].Value) == Convert.ToInt32(selectedProductID))
                        {
                            int currentReturnQuantity = Convert.ToInt32(returnRow.Cells["Quantity"].Value);
                            int newTotalReturn = currentReturnQuantity + returnQuantity;

                            if (newTotalReturn > remainingQuantity)
                            {
                                MessageBox.Show($"تکایە ژمارەی گەڕاوە بۆ ئەم کاڵایە لە {remainingQuantity} زیاتر نەبێت.");
                                return;
                            }

                            returnRow.Cells["Quantity"].Value = newTotalReturn; // Update existing row
                            exists = true;
                            break;
                        }
                    }

                    // If product is not already in return list, add it
                    if (!exists)
                    {
                        dataGridView2.Rows.Add(selectedProductID, selectedProductName, returnQuantity);
                    }
                }
            }
            else
            {
                MessageBox.Show("تکایە کاڵایەک هەڵبژێرە بۆ گەڕاندنەوە.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("هیچ کاڵایەک لە لیستی گەڕانەوە نییە.");
                return;
            }

            DB db = new DB();

            try
            {
                // Validate SaleID
                if (string.IsNullOrWhiteSpace(ExpenseAmount.Text))
                {
                    MessageBox.Show("کۆدی پسوڵە داخڵ بکە.");
                    return;
                }

                if (!int.TryParse(ExpenseAmount.Text, out int saleID))
                {
                    MessageBox.Show("کۆدی پسوڵە هەڵەیە.");
                    return;
                }

                decimal totalRefundAmount = 0;

                // Check if the sale is credit
                string checkCreditQuery = "SELECT IsCredit, CustomerID FROM Sales WHERE SaleID = @SaleID";
                var creditParams = new Dictionary<string, object> { { "@SaleID", saleID } };
                var creditData = db.ExecuteReader(checkCreditQuery, creditParams);

                if (!creditData.Read())
                {
                    MessageBox.Show("Sale ID not found.");
                    return;
                }

                bool isCredit = Convert.ToBoolean(creditData["IsCredit"]);
                int customerID = isCredit ? Convert.ToInt32(creditData["CustomerID"]) : 0;

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["ProductID"]?.Value == null || row.Cells["Quantity"]?.Value == null)
                    {
                        MessageBox.Show("One or more rows in the return list are missing required information.");
                        return;
                    }

                    int productID = Convert.ToInt32(row.Cells["ProductID"].Value);
                    int returnQuantity = Convert.ToInt32(row.Cells["Quantity"].Value);

                    // Fetch UnitPrice from SalesDetails
                    string querySalesDetails = @"
            SELECT UnitPrice, Quantity 
            FROM SalesDetails 
            WHERE SaleID = @SaleID AND ProductID = @ProductID";

                    var salesDetailsParams = new Dictionary<string, object>
            {
                { "@SaleID", saleID },
                { "@ProductID", productID }
            };

                    var salesDetailsData = db.ExecuteReader(querySalesDetails, salesDetailsParams);

                    if (!salesDetailsData.Read())
                    {
                        MessageBox.Show("Sales details not found for the product.");
                        return;
                    }

                    decimal unitPrice = Convert.ToDecimal(salesDetailsData["UnitPrice"]);
                    decimal refundAmount = returnQuantity * unitPrice;
                    totalRefundAmount += refundAmount;

                    // Update SalesDetails for the returned product
                    string updateSaleDetailsQuery = @"
            UPDATE SalesDetails
            SET 
                Quantity = Quantity - @ReturnedQuantity,
                Subtotal = (Quantity - @ReturnedQuantity) * UnitPrice,
                ReturnedQuantity = ReturnedQuantity + @ReturnedQuantity
            WHERE 
                SaleID = @SaleID AND ProductID = @ProductID";

                    db.ExecuteWithParameters(updateSaleDetailsQuery, new Dictionary<string, object>
            {
                { "@ReturnedQuantity", returnQuantity },
                { "@SaleID", saleID },
                { "@ProductID", productID }
            });

                    // Update Product stock
                    string updateStockQuery = @"
            UPDATE Product 
            SET QuantityAvailable = QuantityAvailable + @ReturnQuantity 
            WHERE ProductID = @ProductID";

                    db.ExecuteWithParameters(updateStockQuery, new Dictionary<string, object>
            {
                { "@ReturnQuantity", returnQuantity },
                { "@ProductID", productID }
            });
                }

                // Update Sales table
                string updateSalesQuery = @"
        UPDATE Sales 
        SET TotalAmount = TotalAmount - @ReducedAmount
        WHERE SaleID = @SaleID";

                db.ExecuteWithParameters(updateSalesQuery, new Dictionary<string, object>
        {
            { "@ReducedAmount", totalRefundAmount },
            { "@SaleID", saleID }
        });

                // Update Customer debt if sale was on credit
                if (isCredit)
                {
                    string updateCustomerDebtQuery = @"
            UPDATE Customer
            SET TotalDebt = TotalDebt - @ReducedAmount
            WHERE CustomerID = @CustomerID";

                    db.ExecuteWithParameters(updateCustomerDebtQuery, new Dictionary<string, object>
            {
                { "@ReducedAmount", totalRefundAmount },
                { "@CustomerID", customerID }
            });
                }

                MessageBox.Show("کاڵا دیاریکراوەکان بە سەرکەوتویی گەڕانەوە.");
                button2.PerformClick();
                dataGridView2.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing the return: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Display a confirmation dialog
            DialogResult result = MessageBox.Show(
                "دڵنیای لە گەڕانەوەی پسوڵە؟.",
                "Confirm Return",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                int saleID = Convert.ToInt32(ExpenseAmount.Text);

                string getSalesDetailsQuery = @"
        SELECT ProductID, Quantity, UnitPrice 
        FROM SalesDetails
        WHERE SaleID = @SaleID";

                string updateSalesDetailsQuery = @"
        UPDATE SalesDetails
        SET 
            ReturnedQuantity = Quantity,
            Quantity = 0,
            Subtotal = 0
        WHERE 
            SaleID = @SaleID";

                string updateProductStockQuery = @"
        UPDATE Product
        SET 
            QuantityAvailable = QuantityAvailable + @ReturnedQuantity
        WHERE 
            ProductID = @ProductID";

                string updateSalesQuery = @"
        UPDATE Sales
        SET 
            IsReturned = 1,
            TotalAmount = 0
        WHERE 
            SaleID = @SaleID";

                string checkCreditQuery = "SELECT IsCredit, CustomerID, TotalAmount FROM Sales WHERE SaleID = @SaleID";

                DB db = new DB();

                try
                {
                    // Fetch the Sale details
                    DataTable salesDetails;
                    using (SqlDataReader reader = db.ExecuteReader(getSalesDetailsQuery, new Dictionary<string, object>
            {
                { "@SaleID", saleID }
            }))
                    {
                        salesDetails = new DataTable();
                        salesDetails.Load(reader);
                    }

                    decimal totalRefundAmount = 0;

                    // Update Product stock for each item in the Sale and calculate refund amount
                    foreach (DataRow row in salesDetails.Rows)
                    {
                        int productID = Convert.ToInt32(row["ProductID"]);
                        int quantity = Convert.ToInt32(row["Quantity"]);
                        decimal unitPrice = Convert.ToDecimal(row["UnitPrice"]);

                        decimal refundAmount = quantity * unitPrice;
                        totalRefundAmount += refundAmount;

                        // Update Product stock for the returned items
                        db.ExecuteNonQuery(updateProductStockQuery, new Dictionary<string, object>
                {
                    { "@ProductID", productID },
                    { "@ReturnedQuantity", quantity }
                });
                    }

                    // Check if sale was on credit
                    var creditData = db.ExecuteReader(checkCreditQuery, new Dictionary<string, object>
            {
                { "@SaleID", saleID }
            });

                    if (creditData.Read())
                    {
                        bool isCredit = Convert.ToBoolean(creditData["IsCredit"]);
                        int customerID = isCredit ? Convert.ToInt32(creditData["CustomerID"]) : 0;
                        decimal saleTotalAmount = Convert.ToDecimal(creditData["TotalAmount"]);

                        if (isCredit)
                        {
                            // Deduct total refund amount from customer debt
                            string updateCustomerDebtQuery = @"
                    UPDATE Customer
                    SET TotalDebt = TotalDebt - @ReducedAmount
                    WHERE CustomerID = @CustomerID";

                            db.ExecuteWithParameters(updateCustomerDebtQuery, new Dictionary<string, object>
                    {
                        { "@ReducedAmount", saleTotalAmount },
                        { "@CustomerID", customerID }
                    });
                        }
                    }

                    // Update SalesDetails
                    db.ExecuteNonQuery(updateSalesDetailsQuery, new Dictionary<string, object>
            {
                { "@SaleID", saleID }
            });

                    // Mark the Sale as Returned
                    db.ExecuteNonQuery(updateSalesQuery, new Dictionary<string, object>
            {
                { "@SaleID", saleID }
            });

                    MessageBox.Show("پسوڵە بە سەرکەوتویی گەڕایەوە.");
                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void ExpenseAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys (like Backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                // Suppress the key if it's not a digit or control key
                e.Handled = true;
            }
        }

        //private void ProductSelection_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string selectedProduct = ProductSelection.SelectedItem.ToString();

        //    foreach (DataGridViewRow row in dataGridView1.Rows)
        //    {
        //        if (row.Cells["ProductName"].Value != null && row.Cells["ProductName"].Value.ToString() == selectedProduct)
        //        {
        //            int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
        //            numericUpDown1.Maximum = quantity > 0 ? quantity : 1; // Prevents setting 0 as the max
        //            numericUpDown1.Value = 1; // Reset value to 1 when selection changes
        //            break;
        //        }
        //    }
        //}

        private void button3_Click(object sender, EventArgs e)
        {

            // Clear all rows from dataGridView2
            dataGridView2.Rows.Clear();

            // Optionally, you can also clear the columns if needed
            // dataGridView2.Columns.Clear();

            MessageBox.Show("DataGridView has been cleared.");

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Assuming "Quantity" is the name of the column
                var selectedRow = dataGridView1.SelectedRows[0];
                if (int.TryParse(selectedRow.Cells["Quantity"].Value?.ToString(), out int quantity))
                {
                    numericUpDown1.Minimum = 1;
                    numericUpDown1.Maximum = quantity > 1 ? quantity : 1;
                    numericUpDown1.Value = 1;
                }
            }
        }
    }

}
