using Store.Kursovaya1DataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Store
{
    public partial class FormADMINMODE : Form
    {
        private string currentAdminLogin;
        private string adminID;
        private Panel contentPanel;

        // Цветовая схема
        private Color accentBlue = Color.FromArgb(33, 150, 243);
        private Color darkBg = Color.FromArgb(21, 25, 28);
        private Color darkPanel = Color.FromArgb(30, 30, 30);
        private Color accentGreen = Color.FromArgb(76, 175, 80);
        private Color accentRed = Color.FromArgb(244, 67, 54);
        private Color textLight = Color.FromArgb(251, 251, 252);
        private Color textGray = Color.FromArgb(191, 197, 210);

        public FormADMINMODE(string adminLogin, string userid)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += FormADMINMODE_KeyDown;
            currentAdminLogin = adminLogin;
            adminID = userid;

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
                Size = new Size(210, 80),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Gold,
                Text = $"Здравствуйте,\n{currentAdminLogin}!\nID: {adminID}\nРежим администратора",
                TextAlign = ContentAlignment.MiddleLeft
            };
            menuPanel.Controls.Add(welcomeLabel);

            // Кнопки меню
            string[] menuItems = { "📋 ИГРЫ", "👥 ПОЛЬЗОВАТЕЛИ", "📚 БИБЛИОТЕКА", "⭐ РЕЙТИНГИ", "🚪 ВЫХОД" };
            int yPos = 130;

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
                    Tag = i
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

            // Создаем секции
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

            // Создаем разделительную панель: слева - таблица, справа - изображение
            Panel leftPanel = new Panel
            {
                Location = new Point(0, 60),
                Size = new Size(contentPanel.Width - 400, contentPanel.Height - 150),
                BackColor = Color.Transparent
            };

            Panel rightPanel = new Panel
            {
                Location = new Point(contentPanel.Width - 380, 60),
                Size = new Size(360, contentPanel.Height - 150),
                BackColor = Color.Transparent
            };

            // DataGridView для игр (БЕЗ колонки Images)
            DataGridView dgvGames = new DataGridView
            {
                Name = "dgvGames",
                Location = new Point(0, 0),
                Size = new Size(leftPanel.Width, leftPanel.Height - 40),
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

            // Настройка колонок - ИСКЛЮЧАЕМ колонку Images
            dgvGames.AutoGenerateColumns = false;
            dgvGames.Columns.Clear();

            // Колонка ID
            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn
            {
                Name = "id_game",
                HeaderText = "ID",
                DataPropertyName = "id_game",
                Width = 50,
                ReadOnly = true
            };
            dgvGames.Columns.Add(colId);

            // Колонка названия
            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn
            {
                Name = "name_game",
                HeaderText = "Название игры",
                DataPropertyName = "name_game",
                Width = 200
            };
            dgvGames.Columns.Add(colName);

            // Колонка жанра
            DataGridViewComboBoxColumn colGenre = new DataGridViewComboBoxColumn
            {
                Name = "Genre_id",
                HeaderText = "Жанр",
                DataPropertyName = "Genre_id",               // Связь с таблицей Games (куда сохраняем ID)
                DataSource = kursovaya1DataSet.Genres,       // Откуда берем список жанров
                DisplayMember = "Name_genre",                // Что ПОКАЗЫВАЕМ (текст, индекс 1)
                ValueMember = "id_genre",                    // Что СОХРАНЯЕМ в базу (ID, индекс 0)
                Width = 100,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing, // Чтобы выглядело как обычный текст
                FlatStyle = FlatStyle.Flat
            };
            dgvGames.Columns.Add(colGenre);

            // Колонка цены
            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn
            {
                Name = "price",
                HeaderText = "Цена",
                DataPropertyName = "price",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    ForeColor = Color.Gold
                }
            };
            dgvGames.Columns.Add(colPrice);

            // Колонка года выпуска
            DataGridViewTextBoxColumn colYear = new DataGridViewTextBoxColumn
            {
                Name = "Year_release",
                HeaderText = "Год",
                DataPropertyName = "Year_release",
                Width = 80
            };
            dgvGames.Columns.Add(colYear);

            // Привязываем данные
            dgvGames.DataSource = gamesBindingSource;
            dgvGames.CellToolTipTextNeeded += (s, e) =>
            {
                // Проверяем, что навели именно на данные (не на заголовок таблицы) 
                // и именно на колонку жанра
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvGames.Columns["Genre_id"].Index)
                {
                    // Получаем ID жанра из ячейки
                    var cellValue = dgvGames[e.ColumnIndex, e.RowIndex].Value;

                    if (cellValue != null && cellValue != DBNull.Value)
                    {
                        int genreId = Convert.ToInt32(cellValue);

                        // Ищем строку с этим жанром в таблице Genres
                        DataRow[] genreRows = kursovaya1DataSet.Genres.Select($"id_genre = {genreId}");

                        if (genreRows.Length > 0)
                        {
                            // Берем описание. Как ты и сказал, это столбец с индексом 2 (третий по счету).
                            // Если у столбца есть имя в БД (например, "Description"), лучше писать genreRows[0]["Description"].ToString()
                            string genreDescription = genreRows[0][2].ToString();

                            // Формируем красивый текст для подсказки
                            e.ToolTipText = $"Описание жанра:\n{genreDescription}";
                        }
                    }
                }
            };
            leftPanel.Controls.Add(dgvGames);

            // Панель для изображения (справа)
            Panel imagePanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(rightPanel.Width, 350),
                BackColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label imageLabel = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(340, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = textLight,
                Text = "🎮 ИЗОБРАЖЕНИЕ ИГРЫ",
                TextAlign = ContentAlignment.MiddleCenter
            };
            imagePanel.Controls.Add(imageLabel);

            // Большой PictureBox для отображения изображения
            PictureBox gamePictureBox = new PictureBox
            {
                Name = "gamePictureBox",
                Location = new Point(20, 50),
                Size = new Size(300, 250),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(50, 50, 50)
            };
            imagePanel.Controls.Add(gamePictureBox);

            // Кнопки для работы с изображением
            Button loadImageBtn = new Button
            {
                Location = new Point(20, 310),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = accentBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Text = "📁 ЗАГРУЗИТЬ"
            };
            loadImageBtn.FlatAppearance.BorderSize = 0;
            loadImageBtn.Click += (s, e) => LoadGameImage(gamePictureBox);

            Button clearImageBtn = new Button
            {
                Location = new Point(170, 310),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = accentRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Text = "🗑️ ОЧИСТИТЬ"
            };
            clearImageBtn.FlatAppearance.BorderSize = 0;
            clearImageBtn.Click += (s, e) => ClearGameImage(gamePictureBox);

            imagePanel.Controls.Add(loadImageBtn);
            imagePanel.Controls.Add(clearImageBtn);

            rightPanel.Controls.Add(imagePanel);

            // Информационная панель о выбранной игре
            Panel infoPanel = new Panel
            {
                Location = new Point(0, 360),
                Size = new Size(rightPanel.Width, 100),
                BackColor = Color.FromArgb(35, 35, 35),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label selectedGameLabel = new Label
            {
                Name = "selectedGameLabel",
                Location = new Point(10, 10),
                Size = new Size(340, 80),
                Font = new Font("Segoe UI", 11),
                ForeColor = textGray,
                Text = "Выберите игру из списка,\nчтобы просмотреть\nили изменить изображение",
                TextAlign = ContentAlignment.MiddleLeft
            };
            infoPanel.Controls.Add(selectedGameLabel);

            rightPanel.Controls.Add(infoPanel);

            gamesPanel.Controls.Add(leftPanel);
            gamesPanel.Controls.Add(rightPanel);

            // Панель с кнопками действий (внизу)
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, contentPanel.Height - 80),
                Size = new Size(contentPanel.Width, 70),
                BackColor = Color.Transparent
            };

            Button saveButton = CreateActionButton("💾 СОХРАНИТЬ", accentGreen);
            saveButton.Location = new Point(contentPanel.Width - 400, 10);
            saveButton.Click += (s, e) => SaveGames();

            Button refreshButton = CreateActionButton("🔄 ОБНОВИТЬ", Color.FromArgb(100, 100, 100));
            refreshButton.Location = new Point(contentPanel.Width - 270, 10);
            refreshButton.Click += (s, e) => RefreshGames();

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(refreshButton);

            gamesPanel.Controls.Add(buttonPanel);

            // Обработчик выбора строки в DataGridView
            dgvGames.SelectionChanged += (s, e) =>
            {
                if (dgvGames.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dgvGames.SelectedRows[0];
                    int gameId = Convert.ToInt32(row.Cells["id_game"].Value);
                    string gameName = row.Cells["name_game"].Value?.ToString() ?? "Неизвестная игра";

                    // Обновляем информацию о выбранной игре
                    selectedGameLabel.Text = $"Выбрана игра:\n{gameName}\nID: {gameId}\n\nНажмите 'Загрузить' чтобы добавить изображение";

                    // Загружаем изображение в PictureBox
                    LoadGameImageToPictureBox(gameId, gamePictureBox);
                    gamePictureBox.Tag = gameId; // Сохраняем ID игры в Tag
                }
            };

            contentPanel.Controls.Add(gamesPanel);
        }

        // Метод для загрузки изображения из файла
        private void LoadGameImage(PictureBox pictureBox)
        {
            if (pictureBox.Tag == null)
            {
                MessageBox.Show("Сначала выберите игру из списка!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Выберите изображение для игры";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Загружаем изображение
                        Image image = Image.FromFile(openFileDialog.FileName);
                        pictureBox.Image = image;
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                        // Сохраняем изображение в DataSet
                        int gameId = (int)pictureBox.Tag;

                        // Конвертируем изображение в byte[]
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] imageData = ms.ToArray();

                            // Ищем строку с игрой в DataSet
                            DataRow[] rows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                            if (rows.Length > 0)
                            {
                                rows[0]["Images"] = imageData;
                            }
                        }

                        MessageBox.Show("Изображение успешно загружено! Не забудьте сохранить изменения.",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Метод для очистки изображения
        private void ClearGameImage(PictureBox pictureBox)
        {
            if (pictureBox.Tag == null)
            {
                MessageBox.Show("Сначала выберите игру из списка!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Удалить изображение для этой игры?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                int gameId = (int)pictureBox.Tag;

                // Очищаем изображение в DataSet
                DataRow[] rows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                if (rows.Length > 0)
                {
                    rows[0]["Images"] = DBNull.Value;
                }

                // Очищаем PictureBox
                SetDefaultGameImage(pictureBox);

                MessageBox.Show("Изображение удалено! Не забудьте сохранить изменения.",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Метод для загрузки изображения в PictureBox из DataSet
        private void LoadGameImageToPictureBox(int gameId, PictureBox pictureBox)
        {
            try
            {
                DataRow[] gameRows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                if (gameRows.Length > 0 && gameRows[0]["Images"] != DBNull.Value)
                {
                    byte[] imageData = (byte[])gameRows[0]["Images"];
                    if (imageData != null && imageData.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            if (pictureBox.Image != null)
                            {
                                pictureBox.Image.Dispose();
                                pictureBox.Image = null;
                            }
                            pictureBox.Image = Image.FromStream(ms);
                        }
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        SetDefaultGameImage(pictureBox);
                    }
                }
                else
                {
                    SetDefaultGameImage(pictureBox);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
                SetDefaultGameImage(pictureBox);
            }
        }

        // Метод для установки изображения по умолчанию
        private void SetDefaultGameImage(PictureBox pictureBox)
        {
            Bitmap defaultImage = new Bitmap(300, 250);
            using (Graphics g = Graphics.FromImage(defaultImage))
            {
                g.Clear(Color.FromArgb(60, 60, 60));
                using (Font font = new Font("Segoe UI", 20, FontStyle.Bold))
                {
                    string text = "🎮\nНЕТ\nИЗОБРАЖЕНИЯ";
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
                    {
                        g.DrawString(text, font, brush, new RectangleF(0, 0, 300, 250), sf);
                    }
                }
            }

            if (pictureBox.Image != null)
                pictureBox.Image.Dispose();

            pictureBox.Image = defaultImage;
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
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
                Location = new Point(50, 60),
                Size = new Size(contentPanel.Width - 100, contentPanel.Height - 150),
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

            Button saveButton = CreateActionButton("💾 СОХРАНИТЬ", accentGreen);
            saveButton.Location = new Point(contentPanel.Width - 400, 10);
            saveButton.Click += (s, e) => SaveUsers();

            Button refreshButton = CreateActionButton("🔄 ОБНОВИТЬ", Color.FromArgb(100, 100, 100));
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

            dgvLibrary.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvLibrary.DefaultCellStyle.ForeColor = textLight;
            dgvLibrary.DefaultCellStyle.SelectionBackColor = accentBlue;
            dgvLibrary.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvLibrary.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvLibrary.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
            dgvLibrary.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            dgvLibrary.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvLibrary.EnableHeadersVisualStyles = false;

            dgvLibrary.DataSource = libraryBindingSource;

            libraryPanel.Controls.Add(dgvLibrary);

            Panel bottomPanel = new Panel
            {
                Location = new Point(50, contentPanel.Height - 110),
                Width = contentPanel.Width - 100,
                Height = 80,
                BackColor = Color.Transparent
            };

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
                Location = new Point(50, 60),
                Size = new Size(contentPanel.Width - 100, contentPanel.Height - 150),
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

            Button saveButton = CreateActionButton("💾 СОХРАНИТЬ", accentGreen);
            saveButton.Location = new Point(contentPanel.Width - 400, 10);
            saveButton.Click += (s, e) => SaveRatings();

            Button refreshButton = CreateActionButton("🔄 ОБНОВИТЬ", Color.FromArgb(100, 100, 100));
            refreshButton.Location = new Point(contentPanel.Width - 270, 10);
            refreshButton.Click += (s, e) => RefreshRatings();

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(refreshButton);

            ratingsPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(ratingsPanel);
        }

        private Button CreateActionButton(string text, Color color)
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
            foreach (Control control in contentPanel.Controls)
            {
                if (control is Panel)
                    control.Visible = false;
            }

            switch (index)
            {
                case 0:
                    contentPanel.Controls["panelGames"].Visible = true;
                    break;
                case 1:
                    contentPanel.Controls["panelUsers"].Visible = true;
                    break;
                case 2:
                    contentPanel.Controls["panelLibrary"].Visible = true;
                    UpdateLibraryInfo();
                    break;
                case 3:
                    contentPanel.Controls["panelRatings"].Visible = true;
                    break;
                case 4:
                    Logout();
                    break;
            }
        }

        private void LoadAllData()
        {
            try
            {
                kursovaya1DataSet.Clear();

                gamesTableAdapter.Fill(kursovaya1DataSet.Games);
                usersTableAdapter.Fill(kursovaya1DataSet.Users);
                libraryTableAdapter.Fill(kursovaya1DataSet.Library);
                ratingsTableAdapter.Fill(kursovaya1DataSet.Ratings);
                reviewsTableAdapter.Fill(kursovaya1DataSet.Reviews);
                genresTableAdapter.Fill(kursovaya1DataSet.Genres);

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

        private void SaveLibraryChanges()
        {
            try
            {
                libraryTableAdapter.Update(kursovaya1DataSet.Library);
                MessageBox.Show("Изменения в библиотеке сохранены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        // =========================================================================
        // СЕКРЕТНЫЙ РАЗДЕЛ: ИНТЕГРАЦИЯ qBittorrent WebUI API
        // =========================================================================

        public class FormTorrentClient : Form
        {
            private Color darkBg = Color.FromArgb(21, 25, 28);
            private Color darkPanel = Color.FromArgb(30, 30, 30);
            private Color accentBlue = Color.FromArgb(33, 150, 243);
            private Color accentGreen = Color.FromArgb(76, 175, 80);
            private Color accentRed = Color.FromArgb(244, 67, 54);
            private Color textLight = Color.FromArgb(251, 251, 252);

            private DataGridView dgvTorrents;
            private System.Windows.Forms.Timer updateTimer;
            private QBittorrentAPI api;

            // Настройки подключения (Укажи свои данные от WebUI qBittorrent)
            private string qbUrl = "http://127.0.0.1:8080";
            private string qbUser = "admin";
            private string qbPass = "adminadmin";

            public FormTorrentClient()
            {
                this.Text = "Секретный терминал загрузок (qBittorrent)";
                this.Size = new Size(1000, 600);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = darkBg;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;

                api = new QBittorrentAPI(qbUrl);
                InitializeUI();

                updateTimer = new System.Windows.Forms.Timer();
                updateTimer.Interval = 1500; // Обновление каждые 1.5 секунды
                updateTimer.Tick += UpdateTimer_Tick;

                ConnectToApi();
            }

            private async void ConnectToApi()
            {
                bool connected = await api.LoginAsync(qbUser, qbPass);
                if (connected)
                {
                    updateTimer.Start();
                }
                else
                {
                    MessageBox.Show("Не удалось подключиться к qBittorrent WebUI.\nПроверьте, запущен ли клиент и включен ли WebUI в настройках.", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void InitializeUI()
            {
                // Верхняя панель управления
                Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = darkPanel };
                this.Controls.Add(topPanel);

                Label titleLabel = new Label
                {
                    Text = "⚡ qBittorrent Remote Manager",
                    ForeColor = accentBlue,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    Location = new Point(15, 15),
                    AutoSize = true
                };
                topPanel.Controls.Add(titleLabel);

                Button btnAdd = CreateBtn("📁 Загрузить .torrent", accentBlue, new Point(800, 15));
                btnAdd.Click += BtnAdd_Click;
                topPanel.Controls.Add(btnAdd);

                // Таблица торрентов
                dgvTorrents = new DataGridView
                {
                    Location = new Point(15, 75),
                    Size = new Size(950, 420),
                    BackgroundColor = darkBg,
                    ForeColor = textLight,
                    BorderStyle = BorderStyle.None,
                    RowHeadersVisible = false,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    EnableHeadersVisualStyles = false,
                    GridColor = Color.FromArgb(50, 50, 50)
                };

                dgvTorrents.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
                dgvTorrents.DefaultCellStyle.SelectionBackColor = Color.FromArgb(60, 60, 60);
                dgvTorrents.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                dgvTorrents.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
                dgvTorrents.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
                dgvTorrents.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                // Добавляем колонки
                dgvTorrents.Columns.Add("hash", "Hash");
                dgvTorrents.Columns["hash"].Visible = false;
                dgvTorrents.Columns.Add("name", "Название");
                dgvTorrents.Columns["name"].FillWeight = 40;
                dgvTorrents.Columns.Add("size", "Размер");
                dgvTorrents.Columns["size"].FillWeight = 15;
                dgvTorrents.Columns.Add("state", "Статус");
                dgvTorrents.Columns["state"].FillWeight = 15;
                dgvTorrents.Columns.Add("progress", "Прогресс"); // Эта колонка будет отрисовываться кастомно
                dgvTorrents.Columns["progress"].FillWeight = 25;
                dgvTorrents.Columns.Add("dlspeed", "Скорость");
                dgvTorrents.Columns["dlspeed"].FillWeight = 15;

                // Кастомная отрисовка прогресс-бара
                dgvTorrents.CellPainting += DgvTorrents_CellPainting;

                this.Controls.Add(dgvTorrents);

                // Нижняя панель с кнопками управления
                Panel bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = darkPanel };
                this.Controls.Add(bottomPanel);

                Button btnResume = CreateBtn("▶ Продолжить", accentGreen, new Point(15, 10));
                btnResume.Click += async (s, e) => await PerformAction("resume");
                bottomPanel.Controls.Add(btnResume);

                Button btnPause = CreateBtn("⏸ Пауза", Color.Orange, new Point(180, 10));
                btnPause.Click += async (s, e) => await PerformAction("pause");
                bottomPanel.Controls.Add(btnPause);

                Button btnDelete = CreateBtn("🗑 Удалить", accentRed, new Point(345, 10));
                btnDelete.Click += async (s, e) => await PerformAction("delete");
                bottomPanel.Controls.Add(btnDelete);
            }

            private Button CreateBtn(string text, Color color, Point loc)
            {
                Button btn = new Button
                {
                    Text = text,
                    Location = loc,
                    Size = new Size(150, 35),
                    BackColor = color,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                btn.FlatAppearance.BorderSize = 0;
                return btn;
            }

            private async void BtnAdd_Click(object sender, EventArgs e)
            {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Torrent Files|*.torrent" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        bool success = await api.AddTorrentAsync(ofd.FileName);
                        if (success) MessageBox.Show("Торрент успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else MessageBox.Show("Ошибка при добавлении торрента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            private async Task PerformAction(string action)
            {
                if (dgvTorrents.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Сначала выберите торрент из списка!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string hash = dgvTorrents.SelectedRows[0].Cells["hash"].Value.ToString();
                bool success = false;
                switch (action)
                {
                    case "pause": success = await api.PauseTorrentAsync(hash); break;
                    case "resume": success = await api.ResumeTorrentAsync(hash); break;
                    case "delete":
                        if (MessageBox.Show("Удалить этот торрент?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            success = await api.DeleteTorrentAsync(hash);
                        break;
                }
                if (success) await RefreshTorrents();
            }

            private async void UpdateTimer_Tick(object sender, EventArgs e)
            {
                await RefreshTorrents();
            }

            private async Task RefreshTorrents()
            {
                try
                {
                    var torrents = await api.GetTorrentsAsync();

                    // Сохраняем HASH выделенного торрента (надежнее, чем просто индекс строки)
                    string selectedHash = dgvTorrents.SelectedRows.Count > 0
                        ? dgvTorrents.SelectedRows[0].Cells["hash"].Value?.ToString()
                        : null;

                    dgvTorrents.Rows.Clear();
                    foreach (var t in torrents)
                    {
                        dgvTorrents.Rows.Add(
                            t.hash,
                            t.name,
                            FormatBytes(t.size),
                            TranslateState(t.state),
                            t.progress * 100,
                            FormatSpeed(t.dlspeed)
                        );
                    }

                    // Восстанавливаем выделение по HASH, чтобы кнопки всегда срабатывали
                    if (!string.IsNullOrEmpty(selectedHash))
                    {
                        foreach (DataGridViewRow row in dgvTorrents.Rows)
                        {
                            if (row.Cells["hash"].Value?.ToString() == selectedHash)
                            {
                                row.Selected = true;
                                break;
                            }
                        }
                    }
                }
                catch { /* Игнорируем ошибки при потере сети */ }
            }

            // Кастомная отрисовка прогресс-бара прямо в ячейке DataGridView
            private void DgvTorrents_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvTorrents.Columns["progress"].Index)
                {
                    e.PaintBackground(e.CellBounds, true);

                    double progress = Convert.ToDouble(e.Value);
                    int width = (int)((progress / 100.0) * e.CellBounds.Width);

                    Rectangle progressRect = new Rectangle(e.CellBounds.X, e.CellBounds.Y + 5, width, e.CellBounds.Height - 10);
                    e.Graphics.FillRectangle(new SolidBrush(accentGreen), progressRect);

                    string percentText = progress.ToString("0.0") + " %";
                    TextRenderer.DrawText(e.Graphics, percentText, e.CellStyle.Font, e.CellBounds, textLight, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                    e.Handled = true;
                }
            }

            // Вспомогательные методы форматирования
            private string FormatBytes(long bytes)
            {
                string[] suf = { "B", "KB", "MB", "GB", "TB" };
                if (bytes == 0) return "0 B";
                int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                return $"{num} {suf[place]}";
            }

            private string FormatSpeed(long bytes) => bytes > 0 ? FormatBytes(bytes) + "/s" : "0 B/s";

            private string TranslateState(string state)
            {
                switch (state.ToLower())
                {
                    case "downloading": return "Скачивается";
                    case "stalleddl": return "Ожидание";
                    case "pauseddl":
                    case "stoppeddl": return "На паузе (Загрузка)"; // Поддержка новых версий
                    case "pausedup":
                    case "stoppedup": return "На паузе (Раздача)"; // Поддержка новых версий
                    case "uploading": return "Раздается";
                    case "stalledup": return "Раздача (Простой)";
                    case "checkingup":
                    case "checkingdl": return "Проверка...";
                    case "queueddl": return "В очереди (Загрузка)";
                    case "queuedup": return "В очереди (Раздача)";
                    case "error": return "Ошибка";
                    default: return state;
                }
            }

            protected override void OnFormClosing(FormClosingEventArgs e)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
                base.OnFormClosing(e);
            }
        }

        // Класс для взаимодействия с API qBittorrent WebUI
        // Класс для взаимодействия с API qBittorrent WebUI
        public class QBittorrentAPI
        {
            private HttpClient client;
            private string baseUrl;

            public QBittorrentAPI(string url)
            {
                baseUrl = url;
                var handler = new HttpClientHandler { UseCookies = true };
                client = new HttpClient(handler);
            }

            public async Task<bool> LoginAsync(string username, string password)
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

                var response = await client.PostAsync($"{baseUrl}/api/v2/auth/login", content);
                return response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync() == "Ok.";
            }

            public async Task<List<TorrentInfo>> GetTorrentsAsync()
            {
                var response = await client.GetAsync($"{baseUrl}/api/v2/torrents/info");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<TorrentInfo>>(json) ?? new List<TorrentInfo>();
                }
                return new List<TorrentInfo>();
            }

            public async Task<bool> AddTorrentAsync(string filePath)
            {
                using (var content = new MultipartFormDataContent())
                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                using (var streamContent = new StreamContent(fileStream))
                {
                    streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-bittorrent");
                    content.Add(streamContent, "torrents", System.IO.Path.GetFileName(filePath));

                    var response = await client.PostAsync($"{baseUrl}/api/v2/torrents/add", content);
                    return response.IsSuccessStatusCode;
                }
            }

            // Обновленные методы Паузы, Продолжения и Удаления
            public async Task<bool> PauseTorrentAsync(string hash)
            {
                // Для qBittorrent 4.5.0+ используется "stop"
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("hashes", hash) });
                var response = await client.PostAsync($"{baseUrl}/api/v2/torrents/stop", content);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Если версия старая, используем "pause"
                    content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("hashes", hash) });
                    response = await client.PostAsync($"{baseUrl}/api/v2/torrents/pause", content);
                }
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> ResumeTorrentAsync(string hash)
            {
                // Для qBittorrent 4.5.0+ используется "start"
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("hashes", hash) });
                var response = await client.PostAsync($"{baseUrl}/api/v2/torrents/start", content);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Если версия старая, используем "resume"
                    content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("hashes", hash) });
                    response = await client.PostAsync($"{baseUrl}/api/v2/torrents/resume", content);
                }
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> DeleteTorrentAsync(string hash)
            {
                var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("hashes", hash),
                new KeyValuePair<string, string>("deleteFiles", "false")
            });
                var response = await client.PostAsync($"{baseUrl}/api/v2/torrents/delete", content);
                return response.IsSuccessStatusCode;
            }

            public class TorrentInfo
            {
                public string hash { get; set; }
                public string name { get; set; }
                public long size { get; set; }
                public double progress { get; set; }
                public long dlspeed { get; set; }
                public string state { get; set; }
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

                foreach (Control control in contentPanel.Controls)
                {
                    if (control is Panel panel)
                    {
                        panel.Size = contentPanel.Size;

                        // Обновляем размеры для секции игр
                        if (panel.Name == "panelGames")
                        {
                            foreach (Control subControl in panel.Controls)
                            {
                                if (subControl is Panel leftPanel && leftPanel.Location.X == 0)
                                {
                                    leftPanel.Size = new Size(contentPanel.Width - 400, contentPanel.Height - 150);

                                    DataGridView dgv = leftPanel.Controls["dgvGames"] as DataGridView;
                                    if (dgv != null)
                                    {
                                        dgv.Size = new Size(leftPanel.Width, leftPanel.Height - 40);
                                    }
                                }
                                else if (subControl is Panel rightPanel && rightPanel.Location.X > 0)
                                {
                                    rightPanel.Location = new Point(contentPanel.Width - 380, 60);
                                    rightPanel.Size = new Size(360, contentPanel.Height - 150);
                                }
                                else if (subControl is Panel btnPanel && btnPanel.Location.Y > 0)
                                {
                                    btnPanel.Location = new Point(0, contentPanel.Height - 80);
                                    btnPanel.Size = new Size(contentPanel.Width, 70);

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

        private void FormADMINMODE_KeyDown(object sender, KeyEventArgs e)
        {
            // Проверяем нажатие Ctrl + Shift + 8
            if (e.Control && e.Shift && e.KeyCode == Keys.D8)
            {
                // Открываем секретную форму qBittorrent
                FormTorrentClient torrentForm = new FormTorrentClient();
                torrentForm.ShowDialog();
            }
        }

        private void FormADMINMODE_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "kursovaya1DataSet.Genres". При необходимости она может быть перемещена или удалена.
            this.genresTableAdapter.Fill(this.kursovaya1DataSet.Genres);

        }
    }
}