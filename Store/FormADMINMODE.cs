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
using System.IO;

namespace Store
{
    public partial class FormADMINMODE : Form
    {
        private string currentAdminLogin;
        private Panel contentPanel;

        // Цветовая схема
        private Color accentBlue = Color.FromArgb(33, 150, 243);
        private Color darkBg = Color.FromArgb(21, 25, 28);
        private Color darkPanel = Color.FromArgb(30, 30, 30);
        private Color accentGreen = Color.FromArgb(76, 175, 80);
        private Color accentRed = Color.FromArgb(244, 67, 54);
        private Color textLight = Color.FromArgb(251, 251, 252);
        private Color textGray = Color.FromArgb(191, 197, 210);

        public FormADMINMODE(string adminLogin,string userid)
        {
            InitializeComponent();
            currentAdminLogin = adminLogin;

            // Настройка формы
            this.Text = "Панель администратора";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // Создаем интерфейс
            CreateAdminInterface();

            // Загружаем данные
            LoadAllData();
        }

        private void CreateAdminInterface()
        {
            // Левая панель меню
            Panel menuPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(250, this.Height),
                BackColor = darkPanel
            };

            // Заголовок с приветствием
            Label welcomeLabel = new Label
            {
                Location = new Point(20, 30),
                Size = new Size(210, 60),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Gold,
                Text = $"Здравствуйте,\n{currentAdminLogin}!\nРежим администратора",
                TextAlign = ContentAlignment.MiddleLeft
            };
            menuPanel.Controls.Add(welcomeLabel);

            // Кнопки меню
            string[] menuItems = { "📋 ИГРЫ", "👥 ПОЛЬЗОВАТЕЛИ", "📚 БИБЛИОТЕКА", "⭐ РЕЙТИНГИ", "🚪 ВЫХОД" };
            int yPos = 120;

            for (int i = 0; i < menuItems.Length; i++)
            {
                Button menuButton = new Button
                {
                    Location = new Point(20, yPos),
                    Size = new Size(210, 50),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = textGray,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Text = menuItems[i],
                    TextAlign = ContentAlignment.MiddleLeft,
                    Tag = i // Запоминаем индекс для обработки
                };
                menuButton.FlatAppearance.BorderSize = 0;
                menuButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 50);
                menuButton.Click += MenuButton_Click;
                menuButton.MouseEnter += (s, e) => menuButton.ForeColor = textLight;
                menuButton.MouseLeave += (s, e) => menuButton.ForeColor = textGray;

                menuPanel.Controls.Add(menuButton);
                yPos += 60;
            }

            this.Controls.Add(menuPanel);

            // Основная панель для контента
            contentPanel = new Panel
            {
                Location = new Point(270, 20),
                Size = new Size(this.Width - 290, this.Height - 40),
                BackColor = Color.Transparent
            };
            this.Controls.Add(contentPanel);

            // Создаем таблицы для каждой секции
            CreateGamesSection();
            CreateUsersSection();
            CreateLibrarySection();
            CreateRatingsSection();

            // Показываем секцию игр по умолчанию
            ShowSection(0);
        }

        private void CreateGamesSection()
        {
            Panel gamesPanel = new Panel
            {
                Name = "panelGames",
                Location = new Point(0, 0),
                Size = contentPanel.Size,
                BackColor = Color.Transparent,
                Visible = false
            };

            // Заголовок
            Label titleLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(contentPanel.Width, 50),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = textLight,
                Text = "УПРАВЛЕНИЕ ИГРАМИ",
                TextAlign = ContentAlignment.MiddleCenter
            };
            gamesPanel.Controls.Add(titleLabel);

            // DataGridView для игр
            DataGridView dgvGames = new DataGridView
            {
                Name = "dgvGames",
                Location = new Point(0, 60),
                Size = new Size(contentPanel.Width, contentPanel.Height - 150),
                BackgroundColor = darkPanel,
                ForeColor = textLight,
                GridColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                ReadOnly = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Стиль для DataGridView
            dgvGames.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvGames.DefaultCellStyle.ForeColor = textLight;
            dgvGames.DefaultCellStyle.SelectionBackColor = accentGreen;
            dgvGames.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvGames.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvGames.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
            dgvGames.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            dgvGames.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvGames.EnableHeadersVisualStyles = false;

            // Привязка данных
            dgvGames.DataSource = gamesBindingSource;

            gamesPanel.Controls.Add(dgvGames);

            // Панель с кнопками действий
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, contentPanel.Height - 80),
                Size = new Size(contentPanel.Width, 70),
                BackColor = Color.Transparent
            };

            Button saveButton = CreateActionButton("💾 СОХРАНИТЬ", accentGreen, 0);
            saveButton.Location = new Point(contentPanel.Width - 400, 10);
            saveButton.Click += (s, e) => SaveGames();

            Button refreshButton = CreateActionButton("🔄 ОБНОВИТЬ", Color.FromArgb(100, 100, 100), 1);
            refreshButton.Location = new Point(contentPanel.Width - 270, 10);
            refreshButton.Click += (s, e) => RefreshGames();

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(refreshButton);

            gamesPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(gamesPanel);
        }

        private void CreateUsersSection()
        {
            Panel usersPanel = new Panel
            {
                Name = "panelUsers",
                Location = new Point(0, 0),
                Size = contentPanel.Size,
                BackColor = Color.Transparent,
                Visible = false
            };

            Label titleLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(contentPanel.Width, 50),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = textLight,
                Text = "УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ",
                TextAlign = ContentAlignment.MiddleCenter
            };
            usersPanel.Controls.Add(titleLabel);

            DataGridView dgvUsers = new DataGridView
            {
                Name = "dgvUsers",
                Location = new Point(0, 60),
                Size = new Size(contentPanel.Width, contentPanel.Height - 150),
                BackgroundColor = darkPanel,
                ForeColor = textLight,
                GridColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                ReadOnly = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgvUsers.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvUsers.DefaultCellStyle.ForeColor = textLight;
            dgvUsers.DefaultCellStyle.SelectionBackColor = accentGreen;
            dgvUsers.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvUsers.EnableHeadersVisualStyles = false;

            dgvUsers.DataSource = usersBindingSource;

            usersPanel.Controls.Add(dgvUsers);

            Panel buttonPanel = new Panel
            {
                Location = new Point(0, contentPanel.Height - 80),
                Size = new Size(contentPanel.Width, 70),
                BackColor = Color.Transparent
            };

            Button saveButton = CreateActionButton("💾 СОХРАНИТЬ", accentGreen, 0);
            saveButton.Location = new Point(contentPanel.Width - 400, 10);
            saveButton.Click += (s, e) => SaveUsers();

            Button refreshButton = CreateActionButton("🔄 ОБНОВИТЬ", Color.FromArgb(100, 100, 100), 1);
            refreshButton.Location = new Point(contentPanel.Width - 270, 10);
            refreshButton.Click += (s, e) => RefreshUsers();

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(refreshButton);

            usersPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(usersPanel);
        }

        private void CreateLibrarySection()
        {
            Panel libraryPanel = new Panel
            {
                Name = "panelLibrary",
                Location = new Point(0, 0),
                Size = contentPanel.Size,
                BackColor = Color.Transparent,
                Visible = false
            };

            Label titleLabel = new Label
            {
                Location = new Point(0, 20),
                Size = new Size(contentPanel.Width, 50),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = textLight,
                Text = "📚 УПРАВЛЕНИЕ БИБЛИОТЕКОЙ",
                TextAlign = ContentAlignment.MiddleCenter
            };
            libraryPanel.Controls.Add(titleLabel);

            // DataGridView для библиотеки
            DataGridView dgvLibrary = new DataGridView
            {
                Name = "dgvLibrary",
                Location = new Point(50, 80),
                Width = contentPanel.Width - 100,
                Height = contentPanel.Height - 200,
                BackgroundColor = darkPanel,
                ForeColor = textLight,
                GridColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                ReadOnly = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Стиль для DataGridView
            dgvLibrary.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvLibrary.DefaultCellStyle.ForeColor = textLight;
            dgvLibrary.DefaultCellStyle.SelectionBackColor = accentBlue;
            dgvLibrary.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvLibrary.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvLibrary.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
            dgvLibrary.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            dgvLibrary.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvLibrary.EnableHeadersVisualStyles = false;

            // Отключаем автоматическую генерацию колонок
            dgvLibrary.AutoGenerateColumns = false;
            dgvLibrary.Columns.Clear();

            // Колонка ID записи в библиотеке
            DataGridViewTextBoxColumn colLibId = new DataGridViewTextBoxColumn
            {
                Name = "id_lib",
                HeaderText = "ID записи",
                DataPropertyName = "id_lib",
                Width = 80,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { ForeColor = textGray }
            };
            dgvLibrary.Columns.Add(colLibId);

            // Колонка ID пользователя
            DataGridViewTextBoxColumn colUserId = new DataGridViewTextBoxColumn
            {
                Name = "id_user",
                HeaderText = "ID пользователя",
                DataPropertyName = "id_user",
                Width = 100,
                ReadOnly = false
            };
            dgvLibrary.Columns.Add(colUserId);

            // Колонка ID игры
            DataGridViewTextBoxColumn colGameId = new DataGridViewTextBoxColumn
            {
                Name = "id_game",
                HeaderText = "ID игры",
                DataPropertyName = "id_game",
                Width = 80,
                ReadOnly = false
            };
            dgvLibrary.Columns.Add(colGameId);

            // Колонка для названия игры (вычисляемое поле)
            DataGridViewTextBoxColumn colGameName = new DataGridViewTextBoxColumn
            {
                Name = "game_name",
                HeaderText = "Название игры",
                Width = 250,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = textLight,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                }
            };
            dgvLibrary.Columns.Add(colGameName);

            // Колонка для имени пользователя (для удобства)
            DataGridViewTextBoxColumn colUserName = new DataGridViewTextBoxColumn
            {
                Name = "nickname",
                HeaderText = "Имя пользователя",
                Width = 150,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { ForeColor = accentGreen }
            };
            dgvLibrary.Columns.Add(colUserName);

            // Кнопки действий
            DataGridViewButtonColumn saveButton = new DataGridViewButtonColumn
            {
                Name = "SaveButton",
                HeaderText = "Действия",
                Text = "💾 Сохранить",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            saveButton.DefaultCellStyle.BackColor = accentGreen;
            saveButton.DefaultCellStyle.ForeColor = Color.White;
            saveButton.DefaultCellStyle.SelectionBackColor = accentGreen;
            saveButton.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvLibrary.Columns.Add(saveButton);

            DataGridViewButtonColumn refreshButton = new DataGridViewButtonColumn
            {
                Name = "RefreshButton",
                HeaderText = "",
                Text = "🔄 Обновить",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            refreshButton.DefaultCellStyle.BackColor = Color.FromArgb(100, 100, 100);
            refreshButton.DefaultCellStyle.ForeColor = Color.White;
            refreshButton.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 100, 100);
            refreshButton.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvLibrary.Columns.Add(refreshButton);

            // Привязываем данные
            dgvLibrary.DataSource = libraryBindingSource;

            // Подписываемся на событие форматирования ячеек для отображения названий игр и имен пользователей
            dgvLibrary.CellFormatting += DgvLibrary_CellFormatting;

            // Обработчик клика по кнопкам
            dgvLibrary.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == dgvLibrary.Columns["SaveButton"].Index)
                    {
                        SaveLibraryChanges();
                    }
                    else if (e.ColumnIndex == dgvLibrary.Columns["RefreshButton"].Index)
                    {
                        RefreshLibraryData();
                    }
                }
            };

            libraryPanel.Controls.Add(dgvLibrary);

            // Панель с информацией и дополнительными кнопками
            Panel bottomPanel = new Panel
            {
                Location = new Point(50, contentPanel.Height - 110),
                Width = contentPanel.Width - 100,
                Height = 80,
                BackColor = Color.Transparent
            };

            // Информационная метка
            Label infoLabel = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 12),
                ForeColor = textGray,
                Text = "📊 Всего записей в библиотеке: 0",
                Name = "infoLabel"
            };
            bottomPanel.Controls.Add(infoLabel);

            // Кнопка глобального сохранения
            Button globalSaveBtn = new Button
            {
                Location = new Point(bottomPanel.Width - 240, 15),
                Size = new Size(220, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = accentGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Text = "💾 СОХРАНИТЬ ВСЁ"
            };
            globalSaveBtn.FlatAppearance.BorderSize = 0;
            globalSaveBtn.Click += (s, e) => SaveLibraryChanges();
            bottomPanel.Controls.Add(globalSaveBtn);

            libraryPanel.Controls.Add(bottomPanel);

            contentPanel.Controls.Add(libraryPanel);
        }
        private void DgvLibrary_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv == null) return;

                // Форматирование колонки с названием игры
                if (dgv.Columns[e.ColumnIndex].Name == "game_name" && e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgv.Rows[e.RowIndex];
                    if (row.Cells["id_game"].Value != null && row.Cells["id_game"].Value != DBNull.Value)
                    {
                        int gameId = Convert.ToInt32(row.Cells["id_game"].Value);

                        // Ищем название игры в таблице Games
                        DataRow[] gameRows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                        if (gameRows.Length > 0)
                        {
                            e.Value = gameRows[0]["name_game"].ToString();
                            e.FormattingApplied = true;
                        }
                        else
                        {
                            e.Value = "Неизвестная игра";
                            e.FormattingApplied = true;
                        }
                    }
                }

                // Форматирование колонки с именем пользователя
                if (dgv.Columns[e.ColumnIndex].Name == "nickname" && e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgv.Rows[e.RowIndex];
                    if (row.Cells["id_user"].Value != null && row.Cells["id_user"].Value != DBNull.Value)
                    {
                        int userId = Convert.ToInt32(row.Cells["id_user"].Value);

                        // Ищем имя пользователя в таблице Users
                        DataRow[] userRows = kursovaya1DataSet.Users.Select($"id_user = {userId}");
                        if (userRows.Length > 0)
                        {
                            string login = userRows[0]["login"].ToString();
                            e.Value = login;
                            e.FormattingApplied = true;
                        }
                        else
                        {
                            e.Value = "Неизвестный пользователь";
                            e.FormattingApplied = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в CellFormatting: {ex.Message}");
            }
        }
        private void SaveLibraryChanges()
        {
            try
            {
                libraryTableAdapter.Update(kursovaya1DataSet.Library);
                MessageBox.Show("Изменения в библиотеке сохранены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Обновляем отображение
                RefreshLibraryData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RefreshLibraryData()
        {
            try
            {
                kursovaya1DataSet.Library.Clear();
                libraryTableAdapter.Fill(kursovaya1DataSet.Library);

                // Обновляем информационную метку
                Panel libraryPanel = contentPanel.Controls["panelLibrary"] as Panel;
                if (libraryPanel != null)
                {
                    Panel bottomPanel = libraryPanel.Controls.OfType<Panel>().LastOrDefault();
                    if (bottomPanel != null)
                    {
                        Label infoLabel = bottomPanel.Controls["infoLabel"] as Label;
                        if (infoLabel != null)
                        {
                            infoLabel.Text = $"📊 Всего записей в библиотеке: {kursovaya1DataSet.Library.Rows.Count}";
                        }
                    }
                }

                MessageBox.Show("Данные библиотеки обновлены!", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateRatingsSection()
        {
            Panel ratingsPanel = new Panel
            {
                Name = "panelRatings",
                Location = new Point(0, 0),
                Size = contentPanel.Size,
                BackColor = Color.Transparent,
                Visible = false
            };

            Label titleLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(contentPanel.Width, 50),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = textLight,
                Text = "УПРАВЛЕНИЕ РЕЙТИНГАМИ",
                TextAlign = ContentAlignment.MiddleCenter
            };
            ratingsPanel.Controls.Add(titleLabel);

            DataGridView dgvRatings = new DataGridView
            {
                Name = "dgvRatings",
                Location = new Point(0, 60),
                Size = new Size(contentPanel.Width, contentPanel.Height - 150),
                BackgroundColor = darkPanel,
                ForeColor = textLight,
                GridColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                ReadOnly = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgvRatings.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvRatings.DefaultCellStyle.ForeColor = textLight;
            dgvRatings.DefaultCellStyle.SelectionBackColor = accentGreen;
            dgvRatings.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvRatings.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvRatings.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
            dgvRatings.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            dgvRatings.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvRatings.EnableHeadersVisualStyles = false;

            dgvRatings.DataSource = ratingsBindingSource;

            ratingsPanel.Controls.Add(dgvRatings);

            Panel buttonPanel = new Panel
            {
                Location = new Point(0, contentPanel.Height - 80),
                Size = new Size(contentPanel.Width, 70),
                BackColor = Color.Transparent
            };

            Button saveButton = CreateActionButton("💾 СОХРАНИТЬ", accentGreen, 0);
            saveButton.Location = new Point(contentPanel.Width - 400, 10);
            saveButton.Click += (s, e) => SaveRatings();

            Button refreshButton = CreateActionButton("🔄 ОБНОВИТЬ", Color.FromArgb(100, 100, 100), 1);
            refreshButton.Location = new Point(contentPanel.Width - 270, 10);
            refreshButton.Click += (s, e) => RefreshRatings();

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(refreshButton);

            ratingsPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(ratingsPanel);
        }

        private Button CreateActionButton(string text, Color color, int type)
        {
            return new Button
            {
                Text = text,
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int index = (int)btn.Tag;
                ShowSection(index);
            }
        }

        private void ShowSection(int index)
        {
            // Скрываем все панели
            foreach (Control control in contentPanel.Controls)
            {
                if (control is Panel)
                    control.Visible = false;
            }

            // Показываем нужную панель
            switch (index)
            {
                case 0: // Игры
                    contentPanel.Controls["panelGames"].Visible = true;
                    break;
                case 1: // Пользователи
                    contentPanel.Controls["panelUsers"].Visible = true;
                    break;
                case 2: // Библиотека
                    contentPanel.Controls["panelLibrary"].Visible = true;
                    break;
                case 3: // Рейтинги
                    contentPanel.Controls["panelRatings"].Visible = true;
                    break;
                case 4: // Выход
                    Logout();
                    break;
            }
        }

        private void LoadAllData()
        {
            try
            {
                // Очищаем DataSet перед загрузкой
                kursovaya1DataSet.Clear();

                // Загружаем данные в правильном порядке
                gamesTableAdapter.Fill(kursovaya1DataSet.Games);
                usersTableAdapter.Fill(kursovaya1DataSet.Users);
                libraryTableAdapter.Fill(kursovaya1DataSet.Library);
                ratingsTableAdapter.Fill(kursovaya1DataSet.Ratings);
                reviewsTableAdapter.Fill(kursovaya1DataSet.Reviews);

                // Обновляем информационные метки
                UpdateLibraryInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateLibraryInfo()
        {
            Panel libraryPanel = contentPanel.Controls["panelLibrary"] as Panel;
            if (libraryPanel != null)
            {
                Panel bottomPanel = libraryPanel.Controls.OfType<Panel>().LastOrDefault();
                if (bottomPanel != null)
                {
                    Label infoLabel = bottomPanel.Controls["infoLabel"] as Label;
                    if (infoLabel != null)
                    {
                        infoLabel.Text = $"📊 Всего записей в библиотеке: {kursovaya1DataSet.Library.Rows.Count}";
                    }
                }
            }
        }
 

        private void SaveGames()
        {
            try
            {
                gamesTableAdapter.Update(kursovaya1DataSet.Games);
                MessageBox.Show("Игры успешно сохранены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveUsers()
        {
            try
            {
                usersTableAdapter.Update(kursovaya1DataSet.Users);
                MessageBox.Show("Пользователи успешно сохранены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveLibrary()
        {
            try
            {
                libraryTableAdapter.Update(kursovaya1DataSet.Library);
                MessageBox.Show("Библиотека успешно сохранена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveRatings()
        {
            try
            {
                ratingsTableAdapter.Update(kursovaya1DataSet.Ratings);
                MessageBox.Show("Рейтинги успешно сохранены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGames()
        {
            kursovaya1DataSet.Games.Clear();
            gamesTableAdapter.Fill(kursovaya1DataSet.Games);
            MessageBox.Show("Данные обновлены!", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshUsers()
        {
            kursovaya1DataSet.Users.Clear();
            usersTableAdapter.Fill(kursovaya1DataSet.Users);
            MessageBox.Show("Данные обновлены!", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshLibrary()
        {
            kursovaya1DataSet.Library.Clear();
            libraryTableAdapter.Fill(kursovaya1DataSet.Library);
            MessageBox.Show("Данные обновлены!", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshRatings()
        {
            kursovaya1DataSet.Ratings.Clear();
            ratingsTableAdapter.Fill(kursovaya1DataSet.Ratings);
            MessageBox.Show("Данные обновлены!", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Logout()
        {
            DialogResult result = MessageBox.Show("Выйти из режима администратора?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                Form1 loginForm = new Form1();
                loginForm.FormClosed += (s, args) => Application.Exit();
                loginForm.Show();
            }
        }

        private void FormADMINMODE_Paint(object sender, PaintEventArgs e)
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

        private void FormADMINMODE_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void FormADMINMODE_Resize(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                contentPanel.Size = new Size(this.Width - 290, this.Height - 40);

                // Обновляем размеры всех панелей
                foreach (Control control in contentPanel.Controls)
                {
                    if (control is Panel)
                    {
                        control.Size = contentPanel.Size;

                        // Обновляем размеры DataGridView
                        foreach (Control subControl in control.Controls)
                        {
                            if (subControl is DataGridView dgv)
                            {
                                dgv.Size = new Size(contentPanel.Width, contentPanel.Height - 150);
                            }
                            else if (subControl is Panel btnPanel && btnPanel.Location.Y > 0)
                            {
                                btnPanel.Location = new Point(0, contentPanel.Height - 80);
                                btnPanel.Size = new Size(contentPanel.Width, 70);

                                // Обновляем позиции кнопок
                                foreach (Control btn in btnPanel.Controls)
                                {
                                    if (btn is Button)
                                    {
                                        if (btn.Text.Contains("СОХРАНИТЬ"))
                                            btn.Location = new Point(contentPanel.Width - 400, 10);
                                        else if (btn.Text.Contains("ОБНОВИТЬ"))
                                            btn.Location = new Point(contentPanel.Width - 270, 10);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}