using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Store
{
    public partial class FormADMINMODE : Form
    {
        private string currentAdminLogin;
        private Panel contentPanel;


        private HttpClient httpClient;
        private string selectedDownloadPath = "";
        private string selectedTorrentFilePath = "";
        private TextBox txtQbitUrl;
        private TextBox txtQbitUser;
        private TextBox txtQbitPass;



        private DataGridView dgvTorrents;
        private Timer torrentTimer;
        private bool isQbitAuthenticated = false;


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
            var handler = new HttpClientHandler { UseCookies = true };
            httpClient = new HttpClient(handler);
            this.Text = "Панель администратора";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            // Настройка формы
            this.Text = "Панель администратора";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
            this.KeyDown += FormADMINMODE_KeyDown;
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
            CreateTorrentSection();
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
            if (torrentTimer != null) torrentTimer.Stop();
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
        private async Task AuthenticateQbit()
        {
            string baseUrl = txtQbitUrl.Text.TrimEnd('/');
            var loginData = new FormUrlEncodedContent(new[] {
        new KeyValuePair<string, string>("username", txtQbitUser.Text),
        new KeyValuePair<string, string>("password", txtQbitPass.Text)
    });

            try
            {
                var response = await httpClient.PostAsync($"{baseUrl}/api/v2/auth/login", loginData);
                if (response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync() != "Fails.")
                {
                    isQbitAuthenticated = true;
                    MessageBox.Show("Успешное подключение к qBittorrent!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Настраиваем таймер на 2 секунды (2000 мс) и ЗАПУСКАЕМ ЕГО
                    if (torrentTimer != null)
                    {
                        torrentTimer.Interval = 2000;
                        torrentTimer.Start();
                    }

                    // Запрашиваем первый список сразу
                    await UpdateTorrentsList();
                }
                else
                {
                    isQbitAuthenticated = false;
                    if (torrentTimer != null) torrentTimer.Stop();
                    MessageBox.Show("Ошибка авторизации. Проверьте логин/пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сети: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task UpdateTorrentsList()
        {
            if (!isQbitAuthenticated) return;
            try
            {
                string baseUrl = txtQbitUrl.Text.TrimEnd('/');
                var response = await httpClient.GetAsync($"{baseUrl}/api/v2/torrents/info");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var torrents = System.Text.Json.JsonSerializer.Deserialize<List<TorrentItem>>(json);

                    // Обновляем UI в главном потоке
                    dgvTorrents.Invoke((MethodInvoker)delegate
                    {
                        // Если количество торрентов поменялось, перерисовываем всю таблицу
                        if (dgvTorrents.Rows.Count != torrents.Count)
                        {
                            dgvTorrents.Rows.Clear();
                            foreach (var t in torrents)
                            {
                                string speedStr = t.dlspeed > 1048576 ? $"{(t.dlspeed / 1048576.0):F1} MB/s" : $"{(t.dlspeed / 1024.0):F1} KB/s";
                                string progStr = $"{(t.progress * 100):F1}%";
                                dgvTorrents.Rows.Add(t.hash, t.name, progStr, t.dlspeed == 0 ? "0 KB/s" : speedStr, t.state);
                            }
                        }
                        else
                        {
                            // Если количество такое же, плавно обновляем только данные в ячейках
                            for (int i = 0; i < torrents.Count; i++)
                            {
                                var t = torrents[i];
                                string speedStr = t.dlspeed > 1048576 ? $"{(t.dlspeed / 1048576.0):F1} MB/s" : $"{(t.dlspeed / 1024.0):F1} KB/s";
                                string progStr = $"{(t.progress * 100):F1}%";

                                dgvTorrents.Rows[i].Cells["hash"].Value = t.hash;
                                dgvTorrents.Rows[i].Cells["name"].Value = t.name;
                                dgvTorrents.Rows[i].Cells["progress"].Value = progStr;
                                dgvTorrents.Rows[i].Cells["speed"].Value = t.dlspeed == 0 ? "0 KB/s" : speedStr;
                                dgvTorrents.Rows[i].Cells["state"].Value = t.state;
                            }
                        }
                    });
                }
            }
            catch
            {
                // Если qBittorrent временно недоступен, игнорируем ошибку, чтобы приложение не вылетело
            }
        }
        private async void DgvTorrents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !isQbitAuthenticated) return;

            string hash = dgvTorrents.Rows[e.RowIndex].Cells["hash"].Value.ToString();
            string baseUrl = txtQbitUrl.Text.TrimEnd('/');
            string endpoint = "";

            if (e.ColumnIndex == dgvTorrents.Columns["btnPause"].Index) endpoint = "pause";
            else if (e.ColumnIndex == dgvTorrents.Columns["btnResume"].Index) endpoint = "resume";
            else if (e.ColumnIndex == dgvTorrents.Columns["btnDelete"].Index)
            {
                if (MessageBox.Show("Удалить торрент (без удаления файлов)?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("hashes", hash),
                        new KeyValuePair<string, string>("deleteFiles", "false")
                    });
                    await httpClient.PostAsync($"{baseUrl}/api/v2/torrents/delete", content);
                    await UpdateTorrentsList();
                }
                return;
            }

            if (!string.IsNullOrEmpty(endpoint))
            {
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("hashes", hash) });
                await httpClient.PostAsync($"{baseUrl}/api/v2/torrents/{endpoint}", content);
                await UpdateTorrentsList(); // Принудительно обновляем UI сразу после нажатия
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
                        // Растягиваем саму панель по размеру contentPanel
                        control.Size = contentPanel.Size;

                        // ВАЖНО: Если это наша секретная панель торрентов, 
                        // мы прерываем текущий шаг цикла и переходим к следующей панели.
                        // Таким образом мы защищаем её внутренние элементы от кривого ресайза.
                        if (control.Name == "panelTorrent")
                            continue;

                        // Обновляем размеры DataGridView и панелей с кнопками (для стандартных вкладок)
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
        private void FormADMINMODE_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }
        private void CreateTorrentSection()
        {
            Panel torrentPanel = new Panel
            {
                Name = "panelTorrent",
                Location = new Point(0, 0),
                Size = contentPanel.Size,
                BackColor = Color.Transparent,
                Visible = false
            };

            Label titleLabel = new Label
            {
                Location = new Point(0, 10),
                Size = new Size(contentPanel.Width, 40),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.MediumPurple,
                Text = "🏴‍☠️ СЕКРЕТНАЯ ПАНЕЛЬ ЗАГРУЗОК (qBittorrent)",
                TextAlign = ContentAlignment.MiddleCenter
            };
            torrentPanel.Controls.Add(titleLabel);

            // --- Настройки (сделаем их компактнее) ---
            int yOffset = 60;
            AddSettingsRow(torrentPanel, "URL qBittorrent:", "http://localhost:8080", yOffset, out txtQbitUrl);
            AddSettingsRow(torrentPanel, "Логин:", "admin", yOffset + 40, out txtQbitUser);
            AddSettingsRow(torrentPanel, "Пароль:", "adminadmin", yOffset + 80, out txtQbitPass);

            // Кнопка подключения
            Button btnConnect = new Button { Location = new Point(520, yOffset + 80), Size = new Size(150, 30), Text = "Подключиться", BackColor = accentBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Click += async (s, e) => await AuthenticateQbit();
            torrentPanel.Controls.Add(btnConnect);

            // --- Добавление торрента ---
            yOffset = 190;
            Label lblFile = new Label { Location = new Point(50, yOffset), Size = new Size(120, 30), ForeColor = textLight, Font = new Font("Segoe UI", 10), Text = "Файл:" };
            TextBox txtFile = new TextBox { Location = new Point(170, yOffset), Size = new Size(330, 25), ReadOnly = true, BackColor = darkPanel, ForeColor = textLight };
            Button btnSelectFile = new Button { Location = new Point(520, yOffset - 2), Size = new Size(100, 28), Text = "Обзор", BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSelectFile.Click += (s, e) => { using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Torrent|*.torrent" }) if (ofd.ShowDialog() == DialogResult.OK) { selectedTorrentFilePath = ofd.FileName; txtFile.Text = selectedTorrentFilePath; } };

            Label lblPath = new Label { Location = new Point(50, yOffset + 40), Size = new Size(120, 30), ForeColor = textLight, Font = new Font("Segoe UI", 10), Text = "Куда:" };
            TextBox txtPath = new TextBox { Location = new Point(170, yOffset + 40), Size = new Size(330, 25), ReadOnly = true, BackColor = darkPanel, ForeColor = textLight };
            Button btnSelectPath = new Button { Location = new Point(520, yOffset + 38), Size = new Size(100, 28), Text = "Обзор", BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSelectPath.Click += (s, e) => { using (FolderBrowserDialog fbd = new FolderBrowserDialog()) if (fbd.ShowDialog() == DialogResult.OK) { selectedDownloadPath = fbd.SelectedPath; txtPath.Text = selectedDownloadPath; } };

            Button btnDownload = new Button { Location = new Point(640, yOffset), Size = new Size(150, 66), Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = accentGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Text = "ДОБАВИТЬ" };
            btnDownload.Click += async (s, e) => await SendToQbittorrent();

            torrentPanel.Controls.AddRange(new Control[] { lblFile, txtFile, btnSelectFile, lblPath, txtPath, btnSelectPath, btnDownload });

            // --- Таблица активных загрузок ---
            dgvTorrents = new DataGridView
            {
                Name = "dgvTorrents",
                Location = new Point(50, yOffset + 90),
                Size = new Size(contentPanel.Width - 100, contentPanel.Height - 300),
                BackgroundColor = darkPanel,
                ForeColor = textLight,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false
            };
            dgvTorrents.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvTorrents.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
            dgvTorrents.ColumnHeadersDefaultCellStyle.ForeColor = textLight;

            // Колонки
            dgvTorrents.Columns.Add("hash", "Hash");
            dgvTorrents.Columns["hash"].Visible = false; // Прячем ID
            dgvTorrents.Columns.Add("name", "Название");
            dgvTorrents.Columns.Add("progress", "Прогресс");
            dgvTorrents.Columns.Add("speed", "Скорость");
            dgvTorrents.Columns.Add("state", "Статус");

            // Кнопки управления в таблице
            dgvTorrents.Columns.Add(new DataGridViewButtonColumn { Name = "btnPause", HeaderText = "", Text = "⏸", UseColumnTextForButtonValue = true, Width = 40, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            dgvTorrents.Columns.Add(new DataGridViewButtonColumn { Name = "btnResume", HeaderText = "", Text = "▶", UseColumnTextForButtonValue = true, Width = 40, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            dgvTorrents.Columns.Add(new DataGridViewButtonColumn { Name = "btnDelete", HeaderText = "", Text = "❌", UseColumnTextForButtonValue = true, Width = 40, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });

            dgvTorrents.CellClick += DgvTorrents_CellClick;
            torrentPanel.Controls.Add(dgvTorrents);
            contentPanel.Controls.Add(torrentPanel);

            // Настройка таймера обновления (раз в 1.5 секунды)
            torrentTimer = new Timer { Interval = 1500 };
            torrentTimer.Tick += async (s, e) => await UpdateTorrentsList();
        }
        // Вспомогательный метод для отрисовки полей ввода
        private void AddSettingsRow(Panel parent, string labelText, string defaultValue, int y, out TextBox textBox)
        {
            Label lbl = new Label { Location = new Point(50, y), Size = new Size(150, 30), ForeColor = textLight, Font = new Font("Segoe UI", 10), Text = labelText };
            textBox = new TextBox { Location = new Point(200, y), Size = new Size(300, 30), Font = new Font("Segoe UI", 12), Text = defaultValue, BackColor = darkPanel, ForeColor = textLight, BorderStyle = BorderStyle.FixedSingle };
            if (labelText == "Пароль:") textBox.PasswordChar = '*';
            parent.Controls.Add(lbl);
            parent.Controls.Add(textBox);
        }
        private void FormADMINMODE_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.D8)
            {
                ShowSecretTorrentSection();
            }
        }

        private async Task SendToQbittorrent()
        {
            if (string.IsNullOrEmpty(selectedTorrentFilePath) || string.IsNullOrEmpty(selectedDownloadPath))
            {
                MessageBox.Show("Пожалуйста, выберите торрент-файл и папку для сохранения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string baseUrl = txtQbitUrl.Text.TrimEnd('/');

            try
            {
                // 1. Авторизация
                var loginData = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("username", txtQbitUser.Text),
            new KeyValuePair<string, string>("password", txtQbitPass.Text)
        });

                var loginResponse = await httpClient.PostAsync($"{baseUrl}/api/v2/auth/login", loginData);

                if (!loginResponse.IsSuccessStatusCode || await loginResponse.Content.ReadAsStringAsync() == "Fails.")
                {
                    MessageBox.Show("Не удалось авторизоваться в qBittorrent. Проверьте логин/пароль и включен ли WebUI.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. Отправка файла
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    // Читаем файл торрента
                    byte[] fileBytes = File.ReadAllBytes(selectedTorrentFilePath);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-bittorrent");

                    // Добавляем файл в форму (поле "torrents")
                    multipartFormContent.Add(fileContent, "torrents", Path.GetFileName(selectedTorrentFilePath));

                    // Добавляем путь сохранения (поле "savepath")
                    multipartFormContent.Add(new StringContent(selectedDownloadPath), "savepath");

                    var uploadResponse = await httpClient.PostAsync($"{baseUrl}/api/v2/torrents/add", multipartFormContent);

                    if (uploadResponse.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Торрент успешно добавлен в очередь загрузок qBittorrent!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        selectedTorrentFilePath = ""; // Очищаем выбор
                        await UpdateTorrentsList();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при добавлении торрента: {uploadResponse.StatusCode}", "Ошибка API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка сети: {ex.Message}", "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ShowSecretTorrentSection()
        {
            foreach (Control control in contentPanel.Controls)
            {
                if (control is Panel) control.Visible = false;
            }

            if (contentPanel.Controls.ContainsKey("panelTorrent"))
            {
                contentPanel.Controls["panelTorrent"].Visible = true;
                // ЗАПУСКАЕМ ТАЙМЕР при открытии секретной панели
                if (isQbitAuthenticated) torrentTimer.Start();
            }
        }






    }
    // Добавьте этот класс перед последней закрывающей скобкой } файла
    public class TorrentItem
    {
        public string hash { get; set; }
        public string name { get; set; }
        public float progress { get; set; }
        public int dlspeed { get; set; }
        public string state { get; set; }
    }
}