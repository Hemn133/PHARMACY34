namespace Pharmacy
{
    partial class reportdetails
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            maya = new TextBox();
            xer = new TextBox();
            label1 = new Label();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 12);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Size = new Size(776, 368);
            dataGridView1.TabIndex = 42;
            // 
            // maya
            // 
            maya.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            maya.Location = new Point(299, 425);
            maya.Multiline = true;
            maya.Name = "maya";
            maya.Size = new Size(207, 46);
            maya.TabIndex = 43;
            // 
            // xer
            // 
            xer.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            xer.Location = new Point(581, 425);
            xer.Multiline = true;
            xer.Name = "xer";
            xer.Size = new Size(207, 46);
            xer.TabIndex = 44;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("NRT Bold", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Teal;
            label1.Location = new Point(372, 383);
            label1.Name = "label1";
            label1.Size = new Size(62, 39);
            label1.TabIndex = 69;
            label1.Text = "مایە";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("NRT Bold", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Teal;
            label3.Location = new Point(666, 383);
            label3.Name = "label3";
            label3.Size = new Size(52, 39);
            label3.TabIndex = 71;
            label3.Text = "خێر";
            // 
            // reportdetails
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 483);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(xer);
            Controls.Add(maya);
            Controls.Add(dataGridView1);
            Name = "reportdetails";
            Text = "reportdetails";
            Load += reportdetails_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private TextBox maya;
        private TextBox xer;
        private Label label1;
        private Label label3;
    }
}