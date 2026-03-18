using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Store
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            this.textBox1.KeyDown += new KeyEventHandler(TextBox_KeyDown);
            this.textBox2.KeyDown += new KeyEventHandler(TextBox_KeyDown);
        }


      
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Предотвращаем дальнейшую обработку события
                e.SuppressKeyPress = true;
                e.Handled = true;

                // Вызываем метод авторизации
                PerformLogin();
            }
        }

        // Метод для выполнения авторизации
        private void PerformLogin()
        {
            // Проверяем, что оба поля заполнены
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Вы не ввели необходимые данные", "Авторизация",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Устанавливаем фокус на первое пустое поле
                if (string.IsNullOrEmpty(textBox1.Text))
                    textBox1.Focus();
                else
                    textBox2.Focus();

                return;
            }

            bool found = false;

            foreach (DataGridViewRow row in usersDataGridView.Rows)
            {
                // Пропускаем пустые строки
                if (row.IsNewRow) continue;

                // Проверяем, что все необходимые ячейки существуют и не null
                if (row.Cells.Count < 4 ||
                    row.Cells[1].Value == null ||
                    row.Cells[2].Value == null ||
                    row.Cells[3].Value == null)
                    continue;

                // Сравниваем логин и пароль
                if (textBox1.Text.Equals(row.Cells[1].Value.ToString(), StringComparison.OrdinalIgnoreCase) &&
                    textBox2.Text.Equals(row.Cells[2].Value.ToString()))
                {
                    int userLevel = Convert.ToInt32(row.Cells[3].Value);
                    string userid = row.Cells[0].Value.ToString();
                    string login = textBox1.Text;

                    switch (userLevel)
                    {
                        case 0:
                            FormUSERMODE formUser = new FormUSERMODE(login, userid);
                            formUser.Show();
                            Hide();
                            break;

                        case 9:
                            FormADMINMODE formAdmin = new FormADMINMODE(login, userid);
                            formAdmin.Show();
                            Hide();
                            break;

                        default:
                            MessageBox.Show($"Ошибка уровня пользователя: значение '{userLevel}' некорректно.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка авторизации",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Очищаем поля и устанавливаем фокус на поле логина
                textBox1.Clear();
                textBox2.Clear();
                textBox1.Focus();
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Просто вызываем общий метод авторизации
            PerformLogin();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.usersTableAdapter.Fill(this.kursovaya1DataSet.Users);

            // Устанавливаем фокус на поле логина при загрузке формы
            this.ActiveControl = textBox1;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Color colorTop = Color.FromArgb(41, 44, 49);
            Color colorBottom = Color.FromArgb(21, 25, 28);

            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                colorTop,
                colorBottom,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }
    }
}