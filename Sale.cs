using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public partial class Sale : UserControl
    {
        DB db = new DB();
        private string _userRole;
        int currentUserID = 0;
        public Sale(string userRole)
        {
            InitializeComponent();
            _userRole = userRole;
            if (userRole == "Admin")
            {
                currentUserID = 1;
            }
            else { currentUserID = 2; }
        }

        // Replace this with the actual logic for retrieving the logged-in user's ID

        private void UpdateTotalAmount()
        {

            decimal total = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Subtotal"].Value != null)
                {
                    total += Convert.ToDecimal(row.Cells["Subtotal"].Value);
                }
            }

            textBox1.Text = total.ToString("N0"); // Display total with formatting
        }

        private void LoadSalesData(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Ensure endDate is inclusive by setting it to the last second of the day
                endDate = endDate.AddDays(1).AddSeconds(-1);

                // Query to fetch sales data within the date range
                string query = "SELECT SaleID, SaleDate, UserAccountID, TotalAmount " +
                               "FROM Sales " +
                               "WHERE SaleDate >= @StartDate AND SaleDate <= @EndDate";

                using (SqlConnection conn = new SqlConnection(db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable salesData = new DataTable();
                        adapter.Fill(salesData);

                        // Add Button columns to the DataGridView if not already added
                        DataGridViewButtonColumn btnViewDetails = new DataGridViewButtonColumn
                        {
                            Name = "ViewDetails",
                            HeaderText = "زانیاریەکان",
                            Text = "زانیاریەکان",
                            UseColumnTextForButtonValue = true
                        };
                        dataGridView2.Columns.Add(btnViewDetails);
                        DataGridViewButtonColumn btnReturn = new DataGridViewButtonColumn
                        {
                            Name = "Return",
                            HeaderText = "گەڕانەوە",
                            Text = "گەڕانەوە",
                            UseColumnTextForButtonValue = true
                        };
                        dataGridView2.Columns.Add(btnReturn);
                        DataGridViewButtonColumn btnPrint = new DataGridViewButtonColumn
                        {
                            Name = "Print",
                            HeaderText = "چاپکردن",
                            Text = "چاپکردن",
                            UseColumnTextForButtonValue = true
                        };
                        dataGridView2.Columns.Add(btnPrint);

                        // Bind data to DataGridView
                        dataGridView2.DataSource = salesData;

                        // Hide SaleID column
                        if (dataGridView2.Columns.Contains("SaleID"))
                        {
                            dataGridView2.Columns["SaleID"].Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales data: " + ex.Message);
            }


            try
            {
                // Query to fetch sales data along with the username within the specified date range
                string query = @"
        SELECT 
            Sales.SaleID, 
            Sales.SaleDate, 
            UserAccount.Username AS SoldBy, -- Fetch the username from UserAccount
            Sales.TotalAmount
        FROM Sales
        INNER JOIN UserAccount ON Sales.UserAccountID = UserAccount.UserAccountID -- Join with UserAccount table
        WHERE SaleDate BETWEEN @startdate AND @enddate";

                // Create a dictionary to hold the parameters
                var parameters = new Dictionary<string, object>
    {
        { "@startdate", startDate }, // Ensure case matches query
        { "@enddate", endDate }     // Ensure case matches query
    };

                // Fetch data using the query and parameters
                DataTable salesData = db.GetDataTable(query, parameters);

                // Bind data to the DataGridView
                dataGridView2.DataSource = salesData;

                // Set column headers

                if (dataGridView2.Columns.Contains("SaleDate"))
                {
                    dataGridView2.Columns["SaleDate"].HeaderText = "بەرواری فرۆشتن";
                }

                if (dataGridView2.Columns.Contains("TotalAmount"))
                {

                    dataGridView2.Columns["TotalAmount"].HeaderText = "کۆی گشتی";
                }

                if (dataGridView2.Columns.Contains("SoldBy"))
                {
                    dataGridView2.Columns["SoldBy"].HeaderText = "فرۆشراوە لە لایەن";
                }

                // Hide the SaleID column if necessary
                if (dataGridView2.Columns.Contains("SaleID"))
                {
                    dataGridView2.Columns["SaleID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the event is triggered for valid rows and columns
            if (e.RowIndex >= 0 && (dataGridView1.Columns[e.ColumnIndex].Name == "UnitPrice" || dataGridView1.Columns[e.ColumnIndex].Name == "Quantity"))
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (decimal.TryParse(row.Cells["UnitPrice"].Value?.ToString(), out decimal unitPrice) &&
                    int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int quantity))
                {
                    // Recalculate Subtotal
                    row.Cells["Subtotal"].Value = (unitPrice * quantity).ToString("N0");

                    // Recalculate Total
                    UpdateTotalAmount();
                }
            }
        }
        private void AdminSelling_Load(object sender, EventArgs e)
        {

            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("ProductID", "کۆدی کاڵا");       // Product ID
                dataGridView1.Columns.Add("ProductName", "ناوی کاڵا");     // Product Name
                dataGridView1.Columns.Add("Quantity", "دانە");            // Quantity
                dataGridView1.Columns.Add("OriginalPrice", "Original Price"); // ✅ New Column for Tracking Original Price
                dataGridView1.Columns.Add("UnitPrice", "نرخی دانە");       // Unit Price (Editable)
                dataGridView1.Columns.Add("Subtotal", "کۆی گشتی");        // Subtotal
                dataGridView1.Columns.Add("TotalDiscount", "داشکاندن"); // Discount Per Product
            }

            // Set columns to ReadOnly except for UnitPrice & Quantity
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.ReadOnly = true;
            }

            dataGridView1.Columns["UnitPrice"].ReadOnly = false; // Allow editing of price
            dataGridView1.Columns["Quantity"].ReadOnly = false;  // Allow editing of quantity
            dataGridView1.Columns["OriginalPrice"].Visible = false;
            dataGridView1.Columns["ProductID"].Visible = false;

            // Apply styling to DataGridViews
            style(dataGridView1);
            style(dataGridView2);
            // Check if the Delete column already exists to avoid duplication
    if (!dataGridView1.Columns.Contains("Delete"))
    {
        DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn();
        deleteButton.Name = "Delete";
        deleteButton.HeaderText = "🗑 سڕینەوە";
        deleteButton.Text = "❌"; // You can replace it with "Delete"
        deleteButton.UseColumnTextForButtonValue = true;
        deleteButton.Width = 80;

        dataGridView1.Columns.Add(deleteButton);
    }



            // Set default values for DateTimePickers
            dateTimePicker1.Value = DateTime.Today; ; // Start date
            dateTimePicker2.Value = DateTime.Today; // End date

            try
            {
                // Populate Product ComboBox
                string productQuery = "SELECT ProductID, ProductName FROM Product";
                DataTable productData = db.GetDataTable(productQuery);

                if (productData.Rows.Count > 0)
                {
                    ProductSelection.DataSource = productData;
                    ProductSelection.DisplayMember = "ProductName";
                    ProductSelection.ValueMember = "ProductID";
                    ProductSelection.SelectedIndex = -1; // Clear selection initially
                }
                else
                {
                    MessageBox.Show("!هیچ کاڵایەک لە کۆگادا بوونی نییە", "ئاگادارکردنەوە", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Populate Customer ComboBox
                string customerQuery = "SELECT CustomerID, CustomerName FROM Customer";
                DataTable customerData = db.GetDataTable(customerQuery);

                if (customerData.Rows.Count > 0)
                {
                    comboBox1.DataSource = customerData;
                    comboBox1.DisplayMember = "CustomerName";
                    comboBox1.ValueMember = "CustomerID";
                    comboBox1.SelectedIndex = -1; // Clear selection initially
                }

                // Disable Customer ComboBox initially
                comboBox1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Load sales filtered by default date range
            LoadSalesData(dateTimePicker1.Value, dateTimePicker2.Value);


        }



        private void ReverseColumnsOrder(DataGridView dataGridView)
        {
            int columnCount = dataGridView.Columns.Count;

            for (int i = 0; i < columnCount; i++)
            {
                dataGridView.Columns[i].DisplayIndex = columnCount - 1 - i;
            }
        }

        private bool IsProductAlreadyAdded(string productId)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["ProductID"].Value != null && row.Cells["ProductID"].Value.ToString() == productId)
                {
                    return true; // Product already exists
                }
            }
            return false; // Product does not exist
        }

        private void addtolist_Click(object sender, EventArgs e)
        {
            if (ProductSelection.SelectedValue == null)
            {
                MessageBox.Show("Please select a product from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int productID = (int)ProductSelection.SelectedValue;

                // Check if the product already exists in DataGridView1
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["ProductID"].Value != null && (int)row.Cells["ProductID"].Value == productID)
                    {
                        int currentQuantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                        int additionalQuantity = (int)numericUpDown1.Value;
                        decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                        decimal discountPerUnitt = Convert.ToDecimal(row.Cells["TotalDiscount"].Value) / currentQuantity;

                        int newQuantity = currentQuantity + additionalQuantity;
                        row.Cells["Quantity"].Value = newQuantity;
                        row.Cells["Subtotal"].Value = newQuantity * unitPrice;
                        row.Cells["TotalDiscount"].Value = newQuantity * discountPerUnitt;

                        UpdateTotalAmount();
                        return;
                    }
                }

                // Fetch selling price and discount from the database
                string query = "SELECT SellingPrice, Discount FROM Product WHERE ProductID = @ProductID";
                Dictionary<string, object> parameters = new Dictionary<string, object> { { "@ProductID", productID } };
                DataTable productData = db.GetDataTable(query, parameters);

                if (productData.Rows.Count == 0)
                {
                    MessageBox.Show("Failed to fetch product details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal sellingPrice = Convert.ToDecimal(productData.Rows[0]["SellingPrice"]);
                decimal discountPerUnit = Convert.ToDecimal(productData.Rows[0]["Discount"]);
                decimal discountedPrice = sellingPrice - discountPerUnit;

                string productName = ProductSelection.Text;
                int quantity = (int)numericUpDown1.Value;
                decimal totalPrice = discountedPrice * quantity;
                decimal totalDiscount = discountPerUnit * quantity;

                // ✅ Now adding the OriginalPrice column to store the actual SellingPrice before discounts
                dataGridView1.Rows.Add(productID, productName, quantity, sellingPrice, discountedPrice, totalPrice, totalDiscount);

                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void save_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(new DB().ConnectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    if (dataGridView1.RowCount == 0)
                    {
                        MessageBox.Show("Data is Empty!");
                        return;
                    }

                    // Calculate total amount from DataGridView
                    decimal totalAmount = 0;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["Subtotal"].Value != null)
                        {
                            totalAmount += Convert.ToDecimal(row.Cells["Subtotal"].Value);
                        }
                    }

                    // Validate stock availability for each product
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["ProductID"].Value == null) continue;

                        int productID = Convert.ToInt32(row.Cells["ProductID"].Value);
                        int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);

                        // Check stock availability
                        string stockCheckQuery = "SELECT QuantityAvailable FROM Product WHERE ProductID = @ProductID";
                        SqlCommand cmdStockCheck = new SqlCommand(stockCheckQuery, conn, transaction);
                        cmdStockCheck.Parameters.AddWithValue("@ProductID", productID);

                        int availableQuantity = Convert.ToInt32(cmdStockCheck.ExecuteScalar());

                        if (availableQuantity < quantity)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"ژمارەی دیاریکراو بەردەست نییە بۆ کاڵای کۆدی {productID}. ژمارەی بەردەست: {availableQuantity}, داواکراو: {quantity}.",
                                "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Determine customer ID if sale is on credit
                    string customerID = isdebt.Checked ? comboBox1.SelectedValue.ToString() : null;
                    bool isCredit = isdebt.Checked;

                    // Insert into Sales table
                    string insertSaleQuery = @"
            INSERT INTO Sales (CustomerID, SaleDate, IsCredit, UserAccountID, TotalAmount) 
            OUTPUT INSERTED.SaleID 
            VALUES (@CustomerID, @SaleDate, @IsCredit, @UserAccountID, @TotalAmount)";
                    SqlCommand cmdSale = new SqlCommand(insertSaleQuery, conn, transaction);
                    cmdSale.Parameters.AddWithValue("@CustomerID", (object)customerID ?? DBNull.Value);
                    cmdSale.Parameters.AddWithValue("@SaleDate", DateTime.Now);
                    cmdSale.Parameters.AddWithValue("@IsCredit", isCredit);
                    cmdSale.Parameters.AddWithValue("@UserAccountID", currentUserID);
                    cmdSale.Parameters.AddWithValue("@TotalAmount", totalAmount);

                    int saleID = (int)cmdSale.ExecuteScalar();

                    // Insert into SalesDetails for each product and update Product stock
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["ProductID"].Value == null) continue;

                        int productID = Convert.ToInt32(row.Cells["ProductID"].Value);
                        int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                        decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                        decimal subtotal = Convert.ToDecimal(row.Cells["Subtotal"].Value);
                        decimal totalDiscount = Convert.ToDecimal(row.Cells["TotalDiscount"].Value); // ✅ Fetch total discount
                        int returnedQuantity = 0;

                        // Insert into SalesDetails
                        string insertDetailQuery = @"
    INSERT INTO SalesDetails (SaleID, ProductID, Quantity, UnitPrice, Subtotal, ReturnedQuantity, DiscountAmount) 
    VALUES (@SaleID, @ProductID, @Quantity, @UnitPrice, @Subtotal, @ReturnedQuantity, @Discount)";

                        SqlCommand cmdDetail = new SqlCommand(insertDetailQuery, conn, transaction);
                        cmdDetail.Parameters.AddWithValue("@SaleID", saleID);
                        cmdDetail.Parameters.AddWithValue("@ProductID", productID);
                        cmdDetail.Parameters.AddWithValue("@Quantity", quantity);
                        cmdDetail.Parameters.AddWithValue("@UnitPrice", unitPrice);
                        cmdDetail.Parameters.AddWithValue("@Subtotal", subtotal);
                        cmdDetail.Parameters.AddWithValue("@ReturnedQuantity", returnedQuantity);
                        cmdDetail.Parameters.AddWithValue("@Discount", totalDiscount);  // ✅ Insert discount into database
                        cmdDetail.ExecuteNonQuery();

                        // Update Product stock
                        string updateProductQuery = @"
                UPDATE Product 
                SET QuantityAvailable = QuantityAvailable - @Quantity
                WHERE ProductID = @ProductID";
                        SqlCommand cmdUpdateProduct = new SqlCommand(updateProductQuery, conn, transaction);
                        cmdUpdateProduct.Parameters.AddWithValue("@Quantity", quantity);
                        cmdUpdateProduct.Parameters.AddWithValue("@ProductID", productID);
                        cmdUpdateProduct.ExecuteNonQuery();
                    }

                    // Update customer's debt if sale is on credit
                    if (isCredit)
                    {
                        string updateCustomerDebtQuery = "UPDATE Customer SET TotalDebt = TotalDebt + @TotalAmount WHERE CustomerID = @CustomerID";
                        SqlCommand cmdDebt = new SqlCommand(updateCustomerDebtQuery, conn, transaction);
                        cmdDebt.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        cmdDebt.Parameters.AddWithValue("@CustomerID", customerID);
                        cmdDebt.ExecuteNonQuery();
                    }

                    // Commit the transaction
                    transaction.Commit();
                    MessageBox.Show("Sale saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear DataGridView1 for new entries
                    dataGridView1.Rows.Clear();
                    UpdateTotalAmount();

                    // Refresh DataGridView2 to reflect the latest Sales data
                    RefreshDataGridView2(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void RefreshDataGridView2(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // Base query to fetch sales with Username
                string query = @"
            SELECT 
                s.SaleID, 
                s.SaleDate, 
                u.Username AS [فرۆشراوە لە لایەن], 
                s.TotalAmount 
            FROM Sales s
            INNER JOIN UserAccount u ON s.UserAccountID = u.UserAccountID";

                // If startDate and endDate are provided, filter by date range
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                if (startDate.HasValue && endDate.HasValue)
                {
                    query += " WHERE s.SaleDate BETWEEN @StartDate AND @EndDate";
                    parameters.Add("@StartDate", startDate.Value);
                    parameters.Add("@EndDate", endDate.Value);
                }

                // Fetch data from the database
                DataTable salesData = db.GetDataTableParam(query, parameters);

                // Bind the fetched data to the DataGridView
                dataGridView2.DataSource = salesData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing sales data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }



        private void isdebt_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox1.Enabled = isdebt.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            isdebt.Checked = false;
            comboBox1.Enabled = false;
            textBox1.Text = "0";
            numericUpDown1.Value = 1;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked column is the "Delete" button
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete" && e.RowIndex >= 0)
            {
                // Confirm before deletion
                DialogResult result = MessageBox.Show("دڵنیای لە سڕینەوەی ئەم کاڵایە؟",
                                                      "Confirm Delete",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Remove row from DataGridView
                    dataGridView1.Rows.RemoveAt(e.RowIndex);

                    // Update the total after removing the row
                    UpdateTotalAmount();
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView2.Columns[e.ColumnIndex].Name == "ViewDetails")
            {
                try
                {
                    // Get the SaleID of the selected row
                    int saleID = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells["SaleID"].Value);

                    // Open the popup form
                    FormSaleDetail detailsForm = new FormSaleDetail(saleID);
                    detailsForm.ShowDialog(); // Use ShowDialog for modal behavior
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening sale details: " + ex.Message);
                }
            }
            if (e.RowIndex >= 0 && dataGridView2.Columns[e.ColumnIndex].Name == "Return")
            {
                try
                {
                    // Get the SaleID of the selected row
                    string saleID = dataGridView2.Rows[e.RowIndex].Cells["SaleID"].Value.ToString();

                    // Open the popup form
                    Return returnAdmin = new Return(saleID);
                    LoadUserControl(returnAdmin);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening sale details: " + ex.Message);
                }
            }
            if (e.RowIndex >= 0 && dataGridView2.Columns[e.ColumnIndex].Name == "Print")
            {
                try
                {
                    int saleID = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells["SaleID"].Value);
                    receipt receipt = new receipt(saleID);
                    receipt.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening sale details: " + ex.Message);
                }
            }
        }
        private void LoadUserControl(UserControl newControl)
        {
            AdminDashboard dash = this.ParentForm as AdminDashboard;
            // Clear any existing controls in the panel
            dash.panel1.Controls.Clear();

            // Set the new UserControl to fill the panel
            newControl.Dock = DockStyle.Fill;

            // Add the UserControl to the panel
            dash.panel1.Controls.Add(newControl);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the start and end dates, stripping the time portion
                DateTime startDate = dateTimePicker1.Value.Date;
                DateTime endDate = dateTimePicker2.Value.Date;

                // Query to fetch sales between the selected dates with Username
                string query = @"
            SELECT 
                s.SaleID, 
                s.SaleDate, 
                u.Username AS [فرۆشراوە لە لایەن], 
                s.TotalAmount 
            FROM Sales s
            INNER JOIN UserAccount u ON s.UserAccountID = u.UserAccountID
            WHERE s.SaleDate BETWEEN @StartDate AND @EndDate";

                // Prepare parameters for the query
                Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "@StartDate", startDate },
            { "@EndDate", endDate }
        };

                // Fetch data from the database
                DataTable salesData = db.GetDataTableParam(query, parameters);

                // Bind the fetched data to the DataGridView
                dataGridView2.DataSource = salesData;

                // Check if no rows were returned
                if (salesData.Rows.Count == 0)
                {
                    MessageBox.Show("هیچ فرۆشتنێک نییە لەم بەروارە.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and display error messages
                MessageBox.Show("Error fetching sales data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProductSelection_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name == "TotalAmount" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    // Format the value as a thousand separator
                    e.Value = value.ToString("N0");
                    e.FormattingApplied = true;
                }
            }

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Subtotal" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    // Format the value as a thousand separator
                    e.Value = value.ToString("N0");
                    e.FormattingApplied = true;
                }
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "UnitPrice" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    // Format the value as a thousand separator
                    e.Value = value.ToString("N0");
                    e.FormattingApplied = true;
                }
            }
        }





        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            if (dataGridView1.Columns[e.ColumnIndex].Name == "UnitPrice")
            {
                decimal originalPrice = 0;
                decimal newUnitPrice;
                int quantity;

                bool isOriginalPriceValid = decimal.TryParse(row.Cells["OriginalPrice"].Value?.ToString(), out originalPrice);
                bool isNewPriceValid = decimal.TryParse(row.Cells["UnitPrice"].Value?.ToString(), out newUnitPrice);
                bool isQuantityValid = int.TryParse(row.Cells["Quantity"].Value?.ToString(), out quantity);

                if (isOriginalPriceValid && isNewPriceValid && isQuantityValid)
                {
                    // Calculate discount per unit and total discount
                    decimal discountPerUnit = originalPrice - newUnitPrice;
                    decimal totalDiscount = discountPerUnit * quantity;

                    // Update Subtotal and TotalDiscount
                    row.Cells["Subtotal"].Value = (newUnitPrice * quantity).ToString("N0");
                    row.Cells["TotalDiscount"].Value = totalDiscount.ToString("N0");

                    UpdateTotalAmount();
                }
                else
                {
                    MessageBox.Show("Invalid Unit Price, Quantity, or Missing Original Price", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
