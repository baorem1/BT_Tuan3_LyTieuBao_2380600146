using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; // Cần thiết cho các thao tác File I/O

namespace BTUAN3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeListView(); // Khởi tạo cột
        }

        // --- 1. INITIALIZATION: Setup ListView Columns ---
        private void InitializeListView()
        {
            // Thiết lập chế độ hiển thị chi tiết (Details) là bắt buộc
            lvStudents.View = View.Details;
            lvStudents.FullRowSelect = true;
            lvStudents.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            lvStudents.Columns.Clear();

            // Định nghĩa và thêm các cột
            lvStudents.Columns.Add(new ColumnHeader() { Text = "Last Name", Width = 100, TextAlign = HorizontalAlignment.Left });
            lvStudents.Columns.Add(new ColumnHeader() { Text = "First Name", Width = 150, TextAlign = HorizontalAlignment.Left });
            lvStudents.Columns.Add(new ColumnHeader() { Text = "Phone", Width = 80, TextAlign = HorizontalAlignment.Center });
        }

        // ----------------------------------------------------------------------
        // --- 2. CÁC HÀM XỬ LÝ CHỨC NĂNG CHÍNH ---
        // ----------------------------------------------------------------------

        // Xử lý nút THÊM
        private void btnAddName_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Họ và Tên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ListViewItem item = new ListViewItem(txtLastName.Text);
            item.SubItems.Add(txtFirstName.Text);
            item.SubItems.Add(txtPhone.Text);
            lvStudents.Items.Add(item);

            txtLastName.Clear();
            txtFirstName.Clear();
            txtPhone.Clear();
            txtLastName.Focus();
        }

        // Xử lý nút XÓA (btndeletename)
        private void btndeletename_Click(object sender, EventArgs e)
        {
            if (lvStudents.SelectedItems.Count > 0)
            {
                DialogResult result = MessageBox.Show(
                   "Bạn có chắc chắn muốn xóa mục đã chọn?",
                   "Xác nhận",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Warning
               );

                if (result == DialogResult.Yes)
                {
                    lvStudents.SelectedItems[0].Remove();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một mục để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Xử lý nút CHỈNH SỬA/CẬP NHẬT (btnfixname)
        private void btnfixname_Click(object sender, EventArgs e)
        {
            if (lvStudents.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvStudents.SelectedItems[0];

                // Kiểm tra dữ liệu mới
                if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
                {
                    MessageBox.Show("Không được để trống Họ và Tên khi cập nhật.", "Lỗi Cập Nhật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật SubItems của mục đang được chọn
                selectedItem.SubItems[0].Text = txtLastName.Text; // Last Name
                selectedItem.SubItems[1].Text = txtFirstName.Text; // First Name
                selectedItem.SubItems[2].Text = txtPhone.Text; // Phone

                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một mục để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // Xử lý sự kiện CLICK VÀO ITEM (Hiển thị ngược dữ liệu)
        private void lvStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvStudents.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvStudents.SelectedItems[0];

                // Display data back to TextBoxes
                txtLastName.Text = selectedItem.SubItems[0].Text;
                txtFirstName.Text = selectedItem.SubItems[1].Text;
                txtPhone.Text = selectedItem.SubItems[2].Text;
            }
        }

        // ----------------------------------------------------------------------
        // --- 3. CÁC HÀM FILE I/O VÀ MENU STRIP ---
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
                        writer.WriteLine("Last Name,First Name,Phone");
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
                    lvStudents.Items.Clear();

                    using (StreamReader reader = new StreamReader(openDlg.FileName))
                    {
                        if (!reader.EndOfStream) { reader.ReadLine(); }

                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] fields = line.Split(',');

                            if (fields.Length >= 3)
                            {
                                ListViewItem item = new ListViewItem(fields[0]);
                                item.SubItems.Add(fields[1]);
                                item.SubItems.Add(fields[2]);
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

        // Mục VIEW: Chuyển đổi chế độ hiển thị
        private void detailsToolStripMenuItem_Click(object sender, EventArgs e) { lvStudents.View = View.Details; }
        private void largeIconToolStripMenuItem_Click(object sender, EventArgs e) { lvStudents.View = View.LargeIcon; }
        private void smallIconToolStripMenuItem_Click(object sender, EventArgs e) { lvStudents.View = View.SmallIcon; }

        // Mục FORMAT LISTVIEW: Xóa dữ liệu
        private void clearAllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa tất cả dữ liệu?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                lvStudents.Items.Clear();
            }
        }

        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvStudents.SelectedItems.Count > 0)
            {
                lvStudents.SelectedItems[0].Remove();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một mục để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- Các hàm Menu/Control cấp cao và không sử dụng (Để trống hoặc giữ nguyên) ---
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void txtLastName_TextChanged(object sender, EventArgs e) { }
        private void txtFirstName_TextChanged(object sender, EventArgs e) { }
        private void txtPhone_TextChanged(object sender, EventArgs e) { }
        private void fILEToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void vIEWToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void formatListViewToolStripMenuItem_Click(object sender, EventArgs e) { }
    }
}