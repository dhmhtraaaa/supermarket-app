using System;
using System.Windows.Forms;
using System.Drawing;

namespace SuperMarketGUI
{
    public partial class Form1 : Form
    {
        private System.ComponentModel.IContainer components = null;

        private Panel sidebar;
        private Panel topbar;
        private Panel contentPanel;

        private Label logoLabel;
        private Label titleLabel;

        private TextBox txtName;
        private TextBox txtPrice;

        private Button btnAdd;
        private Button btnDelete;
        private Button btnCheckout;
        private Button btnLoad;

        private ListBox listBox1;
        private ListBox listBoxDB;

        private Label lblTotal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // FORM
            this.Text = "SuperMarket Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1350, 780);
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // SIDEBAR
            sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 250;
            sidebar.BackColor = Color.FromArgb(30, 41, 59);

            logoLabel = new Label();
            logoLabel.Text = "🛒 MarketFlow";
            logoLabel.ForeColor = Color.White;
            logoLabel.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            logoLabel.Dock = DockStyle.Top;
            logoLabel.Height = 90;
            logoLabel.TextAlign = ContentAlignment.MiddleCenter;

            sidebar.Controls.Add(logoLabel);

            // TOPBAR
            topbar = new Panel();
            topbar.Dock = DockStyle.Top;
            topbar.Height = 80;
            topbar.BackColor = Color.FromArgb(20, 30, 48);

            titleLabel = new Label();
            titleLabel.Text = "Modern SuperMarket Dashboard";
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            titleLabel.Padding = new Padding(30, 0, 0, 0);

            topbar.Controls.Add(titleLabel);

            // CONTENT PANEL
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Padding = new Padding(30);
            contentPanel.BackColor = Color.FromArgb(15, 23, 42);

            // INPUTS
            txtName = new TextBox();
            txtName.PlaceholderText = "Product Name";
            txtName.Size = new Size(300, 40);
            txtName.Location = new Point(30, 40);
            txtName.BackColor = Color.FromArgb(51, 65, 85);
            txtName.ForeColor = Color.White;
            txtName.BorderStyle = BorderStyle.FixedSingle;

            txtPrice = new TextBox();
            txtPrice.PlaceholderText = "Price";
            txtPrice.Size = new Size(200, 40);
            txtPrice.Location = new Point(350, 40);
            txtPrice.BackColor = Color.FromArgb(51, 65, 85);
            txtPrice.ForeColor = Color.White;
            txtPrice.BorderStyle = BorderStyle.FixedSingle;

            // BUTTONS
            btnAdd = CreateButton("Add Product", 30, 110);
            btnAdd.Click += new EventHandler(this.btnAdd_Click);

            btnDelete = CreateButton("Delete Product", 210, 110);
            btnDelete.Click += new EventHandler(this.btnDelete_Click);

            btnCheckout = CreateButton("Checkout", 390, 110);
            btnCheckout.Click += new EventHandler(this.btnCheckout_Click);

            btnLoad = CreateButton("Load Database", 570, 110);
            btnLoad.Click += new EventHandler(this.btnLoad_Click);

            // CART LIST
            listBox1 = new ListBox();
            listBox1.Location = new Point(30, 200);
            listBox1.Size = new Size(520, 420);
            listBox1.BackColor = Color.FromArgb(30, 41, 59);
            listBox1.ForeColor = Color.White;
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.Font = new Font("Segoe UI", 11F);

            // DATABASE LIST
            listBoxDB = new ListBox();
            listBoxDB.Location = new Point(620, 200);
            listBoxDB.Size = new Size(520, 420);
            listBoxDB.BackColor = Color.FromArgb(30, 41, 59);
            listBoxDB.ForeColor = Color.White;
            listBoxDB.BorderStyle = BorderStyle.None;
            listBoxDB.Font = new Font("Segoe UI", 11F);

            // TOTAL LABEL
            lblTotal = new Label();
            lblTotal.Text = "Total: 0€";
            lblTotal.Location = new Point(30, 650);
            lblTotal.Size = new Size(400, 50);
            lblTotal.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(96, 165, 250);

            // ADD CONTROLS
            contentPanel.Controls.Add(txtName);
            contentPanel.Controls.Add(txtPrice);

            contentPanel.Controls.Add(btnAdd);
            contentPanel.Controls.Add(btnDelete);
            contentPanel.Controls.Add(btnCheckout);
            contentPanel.Controls.Add(btnLoad);

            contentPanel.Controls.Add(listBox1);
            contentPanel.Controls.Add(listBoxDB);

            contentPanel.Controls.Add(lblTotal);

            this.Controls.Add(contentPanel);
            this.Controls.Add(topbar);
            this.Controls.Add(sidebar);

            this.ResumeLayout(false);
        }

        private Button CreateButton(string text, int x, int y)
        {
            return new Button()
            {
                Text = text,
                Size = new Size(160, 50),
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };
        }
    }
}
