﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class AdminDashboard : Form
    {
        public string _userRole;
        public AdminDashboard(string userRole)
        {
            InitializeComponent();
            _userRole = userRole;
        }
        private void LoadUserControl(UserControl newControl)
        {
            // Clear any existing controls in the panel
            panel1.Controls.Clear();

            // Set the new UserControl to fill the panel
            newControl.Dock = DockStyle.Fill;

            // Add the UserControl to the panel
            panel1.Controls.Add(newControl);
        }
        private void Dashboard_Load(object sender, EventArgs e)
        {
            
            Sale categoryAControl = new Sale(_userRole);
            LoadUserControl(categoryAControl);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Product categoryAControl = new Product(_userRole);
            // Load it into the panel
            LoadUserControl(categoryAControl);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Sale adminSelling = new Sale(_userRole);
            LoadUserControl(adminSelling);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Customer customerAControl = new Customer(_userRole);

            LoadUserControl(customerAControl);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DebtSettlement debtSettlementAdmin = new DebtSettlement(_userRole);
            LoadUserControl(debtSettlementAdmin);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Expense expenseAControl = new Expense(_userRole);
            LoadUserControl(expenseAControl);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Return returnAdmin = new Return();
            LoadUserControl(returnAdmin);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Report adminReport = new Report(_userRole);
            LoadUserControl(adminReport);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
       "دڵنیایت لە چوونەدەرەوە؟?",
       "چوونەدەرەوە",
       MessageBoxButtons.YesNo,
       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Open the login form
                Login loginForm = new Login();
                loginForm.Show();

                // Close the current dashboard form
                this.Close();
            }
        }
    }
}

