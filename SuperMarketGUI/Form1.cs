using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace SuperMarketGUI
{
    internal class Product
    {
        public int Id { get; }
        public string Name { get; }
        public double Price { get; }

        public Product(int id, string name, double price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public override string ToString() => $"{Name} - {Price.ToString("F2", CultureInfo.CurrentCulture)}€";
    }

    public partial class Form1 : Form
    {
        List<string> products = new List<string>();
        List<double> prices = new List<double>();
        List<Product> dbProducts = new List<Product>();
        double total = 0;

        // context menu fields
        private ContextMenuStrip cmsList;
        private ContextMenuStrip cmsDb;

        string connStr = "server=localhost;user=root;password=;database=supermarket;";

        public Form1()
        {
            InitializeComponent();

            listBoxDB.DoubleClick += listBoxDB_DoubleClick;

            // Create a runtime Delete DB button only if one doesn't exist in the designer
            if (this.Controls.Find("btnDeleteDB", true).Length == 0)
            {
                var btnDeleteDB = new Button()
                {
                    Name = "btnDeleteDB",
                    Text = "Delete DB",
                    AutoSize = true
                };

                // Position under listBoxDB if it exists, otherwise default
                try
                {
                    btnDeleteDB.Left = listBoxDB.Left;
                    btnDeleteDB.Top = listBoxDB.Bottom + 6;
                }
                catch
                {
                    btnDeleteDB.Location = new Point(12, 260);
                }

                btnDeleteDB.Click += btnDeleteDB_Click;
                Controls.Add(btnDeleteDB);
            }

            // create and wire context menu for listBox1 (cart)
            cmsList = new ContextMenuStrip();
            var deleteCart = new ToolStripMenuItem("Delete");
            deleteCart.Click += DeleteCart_Click;
            cmsList.Items.Add(deleteCart);
            listBox1.ContextMenuStrip = cmsList;
            listBox1.MouseUp += ListBox_MouseUp;

            // create and wire context menu for listBoxDB (database)
            cmsDb = new ContextMenuStrip();
            var deleteDb = new ToolStripMenuItem("Delete");
            deleteDb.Click += DeleteDb_Click;
            cmsDb.Items.Add(deleteDb);
            listBoxDB.ContextMenuStrip = cmsDb;
            listBoxDB.MouseUp += ListBox_MouseUp;

            LoadFromDatabase();
            // Initialize label
            lblTotal.Text = "Σύνολο: " + total.ToString("F2", CultureInfo.CurrentCulture) + "€";
        }

        private void LoadFromDatabase()
        {
            dbProducts.Clear();
            listBoxDB.Items.Clear();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT id, name, price FROM products";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id"]);
                        string name = reader["name"].ToString();
                        double price = Convert.ToDouble(reader["price"]);
                        var p = new Product(id, name, price);
                        dbProducts.Add(p);
                        listBoxDB.Items.Add(p.ToString());
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = (txtName.Text ?? string.Empty).Trim();
            string rawPrice = txtPrice.Text?.Replace("€", "").Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(rawPrice))
            {
                MessageBox.Show("Add an item");
                return;
            }

            if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(rawPrice))
            {
                MessageBox.Show("Please enter a name");
                return;
            }

            // Prevent duplicate names (case-insensitive, trimmed)
            if (dbProducts.Exists(p => string.Equals(p.Name?.Trim(), name, StringComparison.CurrentCultureIgnoreCase)))
            {
                MessageBox.Show("An item with this name already exists.");
                return;
            }

            if (!double.TryParse(rawPrice, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out double price))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }

            string item = name + " - " + price.ToString("F2", CultureInfo.CurrentCulture) + "€";

            products.Add(item);
            prices.Add(price);
            listBox1.Items.Add(item);

            total += price;
            lblTotal.Text = "Σύνολο: " + total.ToString("F2", CultureInfo.CurrentCulture) + "€";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "INSERT INTO products (name, price) VALUES (@name, @price)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadFromDatabase();

            // Clear the form and set focus for faster entry
            txtName.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtName.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;

            if (index != -1)
            {
                // capture values before removal
                string item = products[index];
                double price = prices[index];

                // try to extract name portion to match DB product
                string[] parts = item.Split('-');
                string name = parts.Length > 0 ? parts[0].Trim() : string.Empty;

                // find matching DB product by name and price (tolerance for floating)
                Product matchingDb = dbProducts.Find(p =>
                    string.Equals(p.Name?.Trim(), name, StringComparison.CurrentCultureIgnoreCase)
                    && Math.Abs(p.Price - price) < 0.001);

                if (matchingDb != null)
                {
                    var confirmDb = MessageBox.Show($"Delete '{matchingDb.Name}' (ID {matchingDb.Id}) from database as well?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (confirmDb == DialogResult.Yes)
                    {
                        using (MySqlConnection conn = new MySqlConnection(connStr))
                        {
                            conn.Open();
                            string query = "DELETE FROM products WHERE id = @id";
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@id", matchingDb.Id);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // refresh DB list after deletion
                        LoadFromDatabase();
                    }
                }

                total -= price;

                products.RemoveAt(index);
                prices.RemoveAt(index);
                listBox1.Items.RemoveAt(index);

                lblTotal.Text = "Σύνολο: " + total.ToString("F2", CultureInfo.CurrentCulture) + "€";
            }
        }

        private void btnDeleteDB_Click(object sender, EventArgs e)
        {
            int idx = listBoxDB.SelectedIndex;
            if (idx == -1)
            {
                MessageBox.Show("Please select an item to delete.");
                return;
            }

            var prod = dbProducts[idx];
            var confirm = MessageBox.Show($"Delete '{prod.Name}' (ID {prod.Id})?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "DELETE FROM products WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", prod.Id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadFromDatabase();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            string formattedTotal = total.ToString("F2", CultureInfo.CurrentCulture);
            MessageBox.Show("Συνολικό ποσό: " + formattedTotal + "€");

            products.Clear();
            prices.Clear();
            listBox1.Items.Clear();
            total = 0;

            lblTotal.Text = "Σύνολο: " + 0.ToString("F2", CultureInfo.CurrentCulture) + "€";
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadFromDatabase();
        }

        private void listBoxDB_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxDB.SelectedIndex == -1)
                return;

            int idx = listBoxDB.SelectedIndex;
            // use dbProducts to get reliable data
            var dbItem = dbProducts[idx];
            string item = dbItem.Name + " - " + dbItem.Price.ToString("F2", CultureInfo.CurrentCulture) + "€";

            products.Add(item);
            prices.Add(dbItem.Price);
            listBox1.Items.Add(item);

            total += dbItem.Price;
            lblTotal.Text = "Σύνολο: " + total.ToString("F2", CultureInfo.CurrentCulture) + "€";
        }

        private void ListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (sender is not ListBox lb) return;

            int idx = lb.IndexFromPoint(e.Location);
            if (idx == ListBox.NoMatches) return;

            lb.SelectedIndex = idx;

            if (lb == listBox1 && cmsList != null)
                cmsList.Show(lb, e.Location);
            else if (lb == listBoxDB && cmsDb != null)
                cmsDb.Show(lb, e.Location);
        }
                        
        private void DeleteCart_Click(object sender, EventArgs e)
        {
            // reuse main delete behavior for cart delete
            btnDelete_Click(sender, e);
        }

        private void DeleteDb_Click(object sender, EventArgs e)
        {
            int idx = listBoxDB.SelectedIndex;
            if (idx == -1) return;

            var prod = dbProducts[idx];
            var confirm = MessageBox.Show($"Delete '{prod.Name}' (ID {prod.Id})?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "DELETE FROM products WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", prod.Id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadFromDatabase();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Refresh database list (matches btnLoad behavior)
            LoadFromDatabase();
        }
    }
}
