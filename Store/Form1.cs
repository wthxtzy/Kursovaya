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
        }


        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Вы не ввели необходимые данные", "Авторизация", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool found = false;


            foreach (DataGridViewRow row in usersDataGridView.Rows)
            {

                if (row.Cells[1].Value == null || row.Cells[2].Value == null || row.Cells[3].Value == null)
                    continue;


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
                            throw new Exception($"Ошибка уровня пользователя: значение '{userLevel}' некорректно.");
                    }

                    found = true;
                    break;
                }
            }


            if (!found)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Clear();
                textBox2.Clear();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.usersTableAdapter.Fill(this.kursovaya1DataSet.Users);

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