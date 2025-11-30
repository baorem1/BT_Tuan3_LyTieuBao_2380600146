using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; // Bắt buộc cho các thao tác file (Save/Open)

namespace bttuan3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeListView(); // Initialize the ListView columns
        }

        // --- 1. INITIALIZATION: Setup ListView Columns ---
        private void InitializeListView()
        {
            // Essential setup for column display
            lvStudents.View = View.Details;
            lvStudents.FullRowSelect = true;
            lvStudents.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            lvStudents.Columns.Clear();

            // Column 1: Last Name
            ColumnHeader colLastName = new ColumnHeader();
            colLastName.Text = "Last Name";
            colLastName.Width = 100;
            colLastName.TextAlign = HorizontalAlignment.Left;
            lvStudents.Columns.Add(colLastName);

            // Column 2: First Name
            ColumnHeader colFirstName = new ColumnHeader();
            colFirstName.Text = "First Name";
            colFirstName.Width = 150;
            colLastName.TextAlign = HorizontalAlignment.Left;
            lvStudents.Columns.Add(colFirstName);

            // Column 3: Phone
            ColumnHeader colPhone = new ColumnHeader();
            colPhone.Text = "Phone";
            colPhone.Width = 80;
            colPhone.TextAlign = HorizontalAlignment.Center;
            lvStudents.Columns.Add(colPhone);
        }

        // --- 2. ADD NAME: Button Click Event ---
        private void btnAddName_Click(object sender, EventArgs e)
        {
            // Basic input validation
            if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Họ và Tên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create new ListViewItem (Last Name is the main item)
            ListViewItem item = new ListViewItem(txtLastName.Text);

            // Add SubItems (First Name and Phone)
            item.SubItems.Add(txtFirstName.Text);
            item.SubItems.Add(txtPhone.Text);

            // Add to ListView
            lvStudents.Items.Add(item);

            // Clear TextBoxes and set focus
            txtLastName.Clear();
            txtFirstName.Clear();
            txtPhone.Clear();
            txtLastName.Focus();
        }

        // --- 3. DISPLAY BACK: ListView Item Selection Change ---
        private void lvStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if any item is selected
            if (lvStudents.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvStudents.SelectedItems[0];

                // Display data back to TextBoxes
                txtLastName.Text = selectedItem.SubItems[0].Text; // Last Name
                txtFirstName.Text = selectedItem.SubItems[1].Text; // First Name
                txtPhone.Text = selectedItem.SubItems[2].Text; // Phone
            }
        }

        // ----------------------------------------------------------------------
        // --- 4. FILE OPERATIONS: Save/Open (Menu File) ---
        // ----------------------------------------------------------------------

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveDlg.Title = "Lưu danh sách sinh viên";

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveDlg.FileName))
                    {
                        writer.WriteLine("Last Name,First Name,Phone"); // Write Header

                        foreach (ListViewItem item in lvStudents.Items)
                        {
                            string line = $"{item.SubItems[0].Text},{item.SubItems[1].Text},{item.SubItems[2].Text}";
                            writer.WriteLine(line);
                        }
                    }
                    MessageBox.Show("Đã lưu dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi ghi file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openDlg.Title = "Mở danh sách sinh viên";

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lvStudents.Items.Clear(); // Clear existing data

                    using (StreamReader reader = new StreamReader(openDlg.FileName))
                    {
                        // Skip header line
                        if (!reader.EndOfStream)
                        {
                            reader.ReadLine();
                        }

                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] fields = line.Split(',');

                            if (fields.Length >= 3)
                            {
                                ListViewItem item = new ListViewItem(fields[0]); // Last Name
                                item.SubItems.Add(fields[1]); // First Name
                                item.SubItems.Add(fields[2]); // Phone
                                lvStudents.Items.Add(item);
                            }
                        }
                    }
                    MessageBox.Show("Đã tải dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi đọc file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ----------------------------------------------------------------------
        // --- 5. MENU STRIP: View and Format (Hoàn thiện chức năng) ---
        // ----------------------------------------------------------------------

        // Mục VIEW: Chuyển đổi chế độ hiển thị
        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lvStudents.View = View.Details;
        }

        private void largeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lvStudents.View = View.LargeIcon;
        }

        private void smallIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lvStudents.View = View.SmallIcon;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lvStudents.View = View.List;
        }

        // Mục FORMAT LISTVIEW: Xóa dữ liệu
        private void clearAllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
               "Bạn có chắc chắn muốn xóa tất cả dữ liệu?",
               "Xác nhận xóa",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning
           );

            if (result == DialogResult.Yes)
            {
                lvStudents.Items.Clear(); // Xóa tất cả các mục
            }
        }

        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvStudents.SelectedItems.Count > 0)
            {
                // Xóa item đầu tiên trong danh sách các item được chọn
                lvStudents.SelectedItems[0].Remove();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một mục để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- Các hàm Menu cấp cao (thường để trống) ---
        private void fILEToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void vIEWToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void formatListViewToolStripMenuItem_Click(object sender, EventArgs e) { }


        // --- Unused handlers (Giữ nguyên) ---
        private void label1_Click(object sender, EventArgs e) { }
        private void txtLastName_TextChanged(object sender, EventArgs e) { }
        private void txtFirstName_TextChanged(object sender, EventArgs e) { }
        private void txtPhone_TextChanged(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
    }
}