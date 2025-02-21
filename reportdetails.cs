using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp1;

namespace Pharmacy
{
    public partial class reportdetails : Form
    {

        private DateTime _startDate;
        private DateTime _endDate;

        DB DB = new DB();
        public reportdetails(DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            _startDate = startDate;
            _endDate = endDate;



        }



        private void CalculateTotals(DataTable dt)
        {
            decimal totalMaya = 0;
            decimal totalProfit = 0;

            foreach (DataRow row in dt.Rows)
            {
                totalMaya += Convert.ToDecimal(row["Maya"]);
                totalProfit += Convert.ToDecimal(row["Profit"]);
            }

            maya.Text = totalMaya.ToString("N0"); // Display as a formatted number
            xer.Text = totalProfit.ToString("N0");
        }



        private void reportdetails_Load(object sender, EventArgs e)
        {
            maya.ReadOnly = true;
            xer.ReadOnly = true;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("NRT Bold", 12, FontStyle.Regular); // Adjust size if needed
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Teal; // Set background color to teal
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // Set text color to white for better contrast
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.RowTemplate.Height = 40;
                        dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.RowsDefaultCellStyle.BackColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.GridColor = Color.Gray;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;




            // Style the DataGridView
          

            // SQL Query for Maya and Profit Calculation
            string query = @"
WITH DateTable AS (
    SELECT @StartDate AS [Date]
    UNION ALL
    SELECT DATEADD(DAY, 1, [Date])
    FROM DateTable
    WHERE [Date] < @EndDate
)
SELECT 
    dt.[Date],
    ISNULL(SUM(sd.Quantity * p.PurchasePrice), 0) AS Maya,
    ISNULL(SUM(sd.Quantity * (sd.UnitPrice - p.PurchasePrice)), 0) AS Profit
FROM DateTable dt
LEFT JOIN Sales s ON CONVERT(DATE, s.SaleDate) = dt.[Date]
LEFT JOIN SalesDetails sd ON s.SaleID = sd.SaleID
LEFT JOIN Product p ON sd.ProductID = p.ProductID
GROUP BY dt.[Date]
ORDER BY dt.[Date];";

            // Prepare parameters
            var parameters = new Dictionary<string, object>
    {
        { "@StartDate", _startDate },
        { "@EndDate", _endDate }
    };

            // Execute query
            DataTable dtResults = DB.GetDataTable(query, parameters);

            // Bind results to DataGridView
            dataGridView1.DataSource = dtResults;

            // Update column headers
            if (dataGridView1.Columns.Contains("Date"))
                dataGridView1.Columns["Date"].HeaderText = "بەروار";

            if (dataGridView1.Columns.Contains("Maya"))
                dataGridView1.Columns["Maya"].HeaderText = "مایە";

            if (dataGridView1.Columns.Contains("Profit"))
                dataGridView1.Columns["Profit"].HeaderText = "خێر";

            // Sum up Maya and Profit and display in respective textboxes
            CalculateTotals(dtResults);
        }
    }
    }

