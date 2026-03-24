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
using Store.Kursovaya1DataSetTableAdapters;

namespace Store
{
    public partial class FormUSERMODE : Form
    {
        private int userIDint;
        private string currentLogin;
        private Panel contentPanel;

        private Color darkBg = Color.FromArgb(21, 25, 28);
        private Color darkPanel = Color.FromArgb(30, 30, 30);
        private Color accentGreen = Color.FromArgb(76, 175, 80);
        private Color accentBlue = Color.FromArgb(33, 150, 243);
        private Color accentOrange = Color.FromArgb(255, 152, 0);
        private Color accentRed = Color.FromArgb(244, 67, 54);
        private Color textLight = Color.FromArgb(251, 251, 252);
        private Color textGray = Color.FromArgb(191, 197, 210);

        public FormUSERMODE(string currentLogin, string userID)
        {
            InitializeComponent();

            this.currentLogin = currentLogin;
            this.userIDint = int.Parse(userID);

            this.Text = "Магазин игр";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Opacity = 1;

            CreateUserInterface();
            LoadData();
        }

        #region Инициализация интерфейса

        private void CreateUserInterface()
        {
            Panel menuPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(280, this.Height),
                BackColor = darkPanel,
                Name = "menuPanel"
            };

            Label logoLabel = new Label
            {
                Location = new Point(30, 30),
                Size = new Size(220, 60),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = accentGreen,
                Text = "GAME STORE",
                TextAlign = ContentAlignment.MiddleLeft
            };
            menuPanel.Controls.Add(logoLabel);

            Panel userInfoPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(240, 80),
                BackColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label welcomeLabel = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(220, 60),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = textLight,
                Text = $"👤 {currentLogin}\n🆔 ID: {userIDint}",
                TextAlign = ContentAlignment.MiddleLeft
            };
            userInfoPanel.Controls.Add(welcomeLabel);
            menuPanel.Controls.Add(userInfoPanel);

            Panel separator = new Panel
            {
                Location = new Point(20, 200),
                Size = new Size(240, 2),
                BackColor = Color.FromArgb(60, 60, 60)
            };
            menuPanel.Controls.Add(separator);

            string[] menuItems = {
                "🏠 ГЛАВНАЯ",
                "📚 МОИ ИГРЫ",
                "🛒 КОРЗИНА",
                "⚙️ НАСТРОЙКИ",
                "🚪 ВЫЙТИ"
            };

            int yPos = 220;
            for (int i = 0; i < menuItems.Length; i++)
            {
                Button menuButton = CreateMenuButton(menuItems[i], i);
                menuButton.Location = new Point(20, yPos);
                menuPanel.Controls.Add(menuButton);
                yPos += 60;
            }

            this.Controls.Add(menuPanel);

            contentPanel = new Panel
            {
                Location = new Point(300, 20),
                Size = new Size(this.Width - 320, this.Height - 40),
                BackColor = Color.Transparent
            };
            this.Controls.Add(contentPanel);

            CreateHomeSection();
            CreateLibrarySection();
            CreateCartSection();
            CreateSettingsSection();

            HighlightActiveMenu(0);
            ShowSection(0);
        }

        private Button CreateMenuButton(string text, int index)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(240, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = textGray,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Tag = index
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 50);
            btn.Click += MenuButton_Click;

            btn.MouseEnter += (s, e) =>
            {
                if ((int)btn.Tag != GetCurrentSection())
                    btn.ForeColor = textLight;
            };
            btn.MouseLeave += (s, e) =>
            {
                if ((int)btn.Tag != GetCurrentSection())
                    btn.ForeColor = textGray;
            };

            return btn;
        }

        #endregion

        #region Секции интерфейса
        private void RateGame_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.Tag == null) return;

            Panel detailsPanel = btn.Parent as Panel;
            ComboBox ratingComboBox = detailsPanel.Controls["ratingComboBox"] as ComboBox;

            if (ratingComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите оценку от 1 до 5 перед тем, как голосовать.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int gameId = (int)btn.Tag;
            int score = int.Parse(ratingComboBox.SelectedItem.ToString());

            try
            {
                // Ищем, есть ли уже оценка от этого пользователя (учитываем название столбца id_users)
                DataRow[] existingRating = kursovaya1DataSet.Ratings.Select($"id_game = {gameId} AND id_users = {userIDint}");

                if (existingRating.Length > 0)
                {
                    // Обновляем существующую оценку
                    existingRating[0]["Overall_mark"] = score;
                }
                else
                {
                    // Создаем новую запись
                    DataRow newRating = kursovaya1DataSet.Ratings.NewRow();
                    newRating["id_game"] = gameId;
                    newRating["id_users"] = userIDint;
                    newRating["Overall_mark"] = score;
                    kursovaya1DataSet.Ratings.Rows.Add(newRating);
                }

                // Сохраняем изменения в базу данных SQL
                ratingsTableAdapter.Update(kursovaya1DataSet.Ratings);

                // Оповещаем пользователя
                MessageBox.Show("Ваша оценка успешно сохранена!", "Спасибо за отзыв",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Обновляем интерфейс, чтобы сразу пересчитался средний рейтинг
                Panel homePanel = contentPanel.Controls["panelHome"] as Panel;
                if (homePanel != null)
                {
                    Panel leftColumn = homePanel.Controls.OfType<Panel>().FirstOrDefault(p => p.Location.X == 50);
                    if (leftColumn != null)
                    {
                        DataGridView dgvGames = leftColumn.Controls["dgvGames"] as DataGridView;
                        if (dgvGames != null)
                        {
                            UpdateGameDetails(dgvGames);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении оценки: {ex.Message}",
                    "Ошибка базы данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateHomeSection()
        {
            Panel homePanel = new Panel
            {
                Name = "panelHome",
                Location = new Point(0, 0),
                Size = contentPanel.Size,
                BackColor = Color.Transparent,
                Visible = false
            };

            Label titleLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(contentPanel.Width, 60),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = textLight,
                Text = "🏠 ДОБРО ПОЖАЛОВАТЬ В МАГАЗИН",
                TextAlign = ContentAlignment.MiddleCenter
            };
            homePanel.Controls.Add(titleLabel);

            Panel searchPanel = new Panel
            {
                Location = new Point(50, 80),
                Size = new Size(contentPanel.Width - 100, 50),
                BackColor = Color.FromArgb(40, 40, 40)
            };

            TextBox searchBox = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(searchPanel.Width - 120, 30),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = textLight,
                BorderStyle = BorderStyle.None,
                Text = ""
            };
            searchBox.Enter += (s, e) => searchBox.BackColor = Color.FromArgb(60, 60, 60);
            searchBox.Leave += (s, e) => searchBox.BackColor = Color.FromArgb(50, 50, 50);
            searchBox.TextChanged += SearchBox_TextChanged;

            Button searchButton = new Button
            {
                Location = new Point(searchPanel.Width - 100, 5),
                Size = new Size(90, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = accentBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Text = "🔍 ПОИСК"
            };
            searchButton.FlatAppearance.BorderSize = 0;
            searchButton.Click += (s, e) => SearchGames(searchBox.Text);

            searchPanel.Controls.Add(searchBox);
            searchPanel.Controls.Add(searchButton);
            homePanel.Controls.Add(searchPanel);

            // Левая колонка для таблицы (её ширина вычисляется динамически!)
            Panel leftColumn = new Panel
            {
                Location = new Point(50, 150),
                Size = new Size((contentPanel.Width - 150) / 2, contentPanel.Height - 200),
                BackColor = Color.Transparent
            };

            Label gamesListLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(leftColumn.Width, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = textLight,
                Text = "📋 ДОСТУПНЫЕ ИГРЫ",
                TextAlign = ContentAlignment.MiddleLeft
            };
            leftColumn.Controls.Add(gamesListLabel);

            // Настройка DataGridView
            DataGridView dgvGames = new DataGridView
            {

                Name = "dgvGames",
                Location = new Point(0, 40),
                Size = new Size(leftColumn.Width, leftColumn.Height - 40), // Таблица заполняет всю левую панель
                BackgroundColor = darkPanel,
                ForeColor = textLight,
                GridColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, // Важно! Режим заполнения
                SelectionMode = DataGridViewSelectionMode.FullRowSelect

            };
            dgvGames.RowTemplate.Height = 35; // Делает строки таблицы выше и просторнее
            dgvGames.AllowUserToResizeColumns = false;
            dgvGames.AllowUserToResizeRows = false;
            dgvGames.AllowUserToOrderColumns = false;
            dgvGames.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvGames.MultiSelect = false;

            // Обработчик для всплывающего окна при наведении на жанр
            dgvGames.CellToolTipTextNeeded += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvGames.Columns["Genre_id"].Index)
                {
                    var cellValue = dgvGames[e.ColumnIndex, e.RowIndex].Value;
                    if (cellValue != null && cellValue != DBNull.Value)
                    {
                        int genreId = Convert.ToInt32(cellValue);
                        DataRow[] genreRows = kursovaya1DataSet.Genres.Select($"id_genre = {genreId}");

                        if (genreRows.Length > 0)
                        {
                            string genreDescription = genreRows[0][2].ToString();
                            e.ToolTipText = $"Описание жанра:\n{genreDescription}";
                        }
                    }
                }
            };

            dgvGames.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvGames.DefaultCellStyle.ForeColor = textLight;
            dgvGames.DefaultCellStyle.SelectionBackColor = accentGreen;
            dgvGames.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvGames.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvGames.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
            dgvGames.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            dgvGames.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvGames.EnableHeadersVisualStyles = false;

            dgvGames.AutoGenerateColumns = false;
            dgvGames.Columns.Clear();

            // Создаем колонки и СРАЗУ задаем им FillWeight (пропорции ширины)
            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn
            {
                Name = "id_game",
                HeaderText = "ID",
                DataPropertyName = "id_game",
                FillWeight = 10, // 10% от ширины
                ReadOnly = true
            };
            dgvGames.Columns.Add(colId);

            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn
            {
                Name = "name_game",
                HeaderText = "Название игры",
                DataPropertyName = "name_game",
                FillWeight = 35, // 35% от ширины
                ReadOnly = true
            };
            dgvGames.Columns.Add(colName);

            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn
            {
                Name = "price",
                HeaderText = "Цена",
                DataPropertyName = "price",
                FillWeight = 15, // 15% от ширины
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    ForeColor = Color.Gold
                }
            };
            dgvGames.Columns.Add(colPrice);

            DataGridViewComboBoxColumn colGenre = new DataGridViewComboBoxColumn
            {
                Name = "Genre_id",
                HeaderText = "Жанр",
                DataPropertyName = "Genre_id",
                DataSource = kursovaya1DataSet.Genres,
                DisplayMember = "Name_genre",
                ValueMember = "id_genre",
                FillWeight = 25, // 25% от ширины
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                FlatStyle = FlatStyle.Flat
            };
            dgvGames.Columns.Add(colGenre);

            DataGridViewTextBoxColumn colRelease = new DataGridViewTextBoxColumn
            {
                Name = "Year_release",
                HeaderText = "Дата выхода",
                DataPropertyName = "Year_release",
                FillWeight = 15, // 15% от ширины
                ReadOnly = true
            };
            dgvGames.Columns.Add(colRelease);

            dgvGames.DataSource = gamesBindingSource;

            // Подписка на изменение выбора строки
            dgvGames.SelectionChanged += (s, e) => UpdateGameDetails(dgvGames);

            leftColumn.Controls.Add(dgvGames);
            homePanel.Controls.Add(leftColumn);

            // Правая колонка для деталей
            Panel rightColumn = new Panel
            {
                Location = new Point(50 + leftColumn.Width + 50, 150),
                Size = new Size((contentPanel.Width - 150) / 2, contentPanel.Height - 200),
                BackColor = Color.Transparent
            };

            Label detailsLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(rightColumn.Width, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = textLight,
                Text = "🎮 ДЕТАЛИ ИГРЫ",
                TextAlign = ContentAlignment.MiddleLeft
            };
            rightColumn.Controls.Add(detailsLabel);

            Panel detailsPanel = new Panel
            {
                Location = new Point(0, 40),
                Size = new Size(rightColumn.Width, rightColumn.Height - 40),
                BackColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle,
                Name = "detailsPanel"
            };

            PictureBox gamePictureBox = new PictureBox
            {
                Location = new Point(20, 20),
                Size = new Size(detailsPanel.Width - 40, 250),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(50, 50, 50),
                Name = "gamePictureBox"
            };
            detailsPanel.Controls.Add(gamePictureBox);

            Label selectedGameLabel = new Label
            {
                Location = new Point(20, 280),
                Size = new Size(detailsPanel.Width - 40, 40),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = textLight,
                Text = "Выберите игру",
                Name = "selectedGameLabel",
                TextAlign = ContentAlignment.MiddleLeft
            };
            detailsPanel.Controls.Add(selectedGameLabel);

            Label priceLabel = new Label
            {
                Location = new Point(20, 330),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Gold,
                Text = "Цена: 0 ₽",
                Name = "priceLabel"
            };
            detailsPanel.Controls.Add(priceLabel);

            Label ratingLabel = new Label
            {
                Location = new Point(20, 360),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 14),
                ForeColor = accentOrange,
                Text = "Средний рейтинг: N/A",
                Name = "ratingLabel"
            };
            detailsPanel.Controls.Add(ratingLabel);

            Label releaseDateLabel = new Label
            {
                Location = new Point(20, 390),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 12),
                ForeColor = textGray,
                Text = "Дата выхода: N/A",
                Name = "releaseDateLabel"
            };
            detailsPanel.Controls.Add(releaseDateLabel);

            // --- ЭЛЕМЕНТЫ ДЛЯ ОЦЕНКИ ---
            ComboBox ratingComboBox = new ComboBox
            {
                Name = "ratingComboBox",
                Location = new Point(20, 430),
                Size = new Size(50, 30),
                DropDownStyle = ComboBoxStyle.DropDownList, // Только выбор из списка
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = textLight,
                FlatStyle = FlatStyle.Flat
            };
            ratingComboBox.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            detailsPanel.Controls.Add(ratingComboBox);

            Button rateBtn = new Button
            {
                Name = "rateBtn",
                Location = new Point(80, 428),
                Size = new Size(130, 32),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = accentOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Text = "ОЦЕНИТЬ ⭐",
                Enabled = false
            };
            rateBtn.FlatAppearance.BorderSize = 0;
            rateBtn.Click += RateGame_Click; // Привязываем логику
            detailsPanel.Controls.Add(rateBtn);
            // ----------------------------------

            Button addToCartBtn = new Button
            {
                Location = new Point(20, 475),
                Size = new Size(detailsPanel.Width - 40, 50),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = accentGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Text = "🛒 В КОРЗИНУ",
                Enabled = false,
                Name = "addToCartBtn"
            };
            addToCartBtn.FlatAppearance.BorderSize = 0;
            addToCartBtn.Click += AddToCart_Click;

            Button buyNowBtn = new Button
            {
                Location = new Point(20, 535),
                Size = new Size(detailsPanel.Width - 40, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = accentBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Text = "⚡ КУПИТЬ СЕЙЧАС",
                Enabled = false,
                Name = "buyNowBtn"
            };
            buyNowBtn.FlatAppearance.BorderSize = 0;
            buyNowBtn.Click += BuySelectedGame_Click;

            detailsPanel.Controls.Add(buyNowBtn);
            detailsPanel.Controls.Add(addToCartBtn);

            rightColumn.Controls.Add(detailsPanel);
            homePanel.Controls.Add(rightColumn);

            contentPanel.Controls.Add(homePanel);
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
                Text = "📚 МОЯ БИБЛИОТЕКА",
                TextAlign = ContentAlignment.MiddleCenter
            };
            libraryPanel.Controls.Add(titleLabel);

            DataGridView dgvLibrary = new DataGridView
            {
                Name = "dgvLibrary",
                Location = new Point(50, 80),
                Width = contentPanel.Width - 100,
                Height = contentPanel.Height - 180,
                BackgroundColor = darkPanel,
                ForeColor = textLight,
                GridColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvLibrary.AllowUserToResizeColumns = false;
            dgvLibrary.AllowUserToResizeRows = false;
            dgvLibrary.AllowUserToOrderColumns = false;
            dgvLibrary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvLibrary.MultiSelect = false;
            dgvLibrary.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvLibrary.DefaultCellStyle.ForeColor = textLight;
            dgvLibrary.DefaultCellStyle.SelectionBackColor = accentBlue;
            dgvLibrary.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvLibrary.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvLibrary.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
            dgvLibrary.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            dgvLibrary.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvLibrary.EnableHeadersVisualStyles = false;

            dgvLibrary.AutoGenerateColumns = false;
            dgvLibrary.Columns.Clear();

            DataGridViewTextBoxColumn colLibId = new DataGridViewTextBoxColumn
            {
                Name = "id_library_item",
                HeaderText = "ID",
                Width = 50,
                Visible = false
            };
            dgvLibrary.Columns.Add(colLibId);

            DataGridViewTextBoxColumn colGameId = new DataGridViewTextBoxColumn
            {
                Name = "id_game",
                HeaderText = "ID игры",
                Width = 50,
                Visible = false
            };
            dgvLibrary.Columns.Add(colGameId);

            DataGridViewTextBoxColumn colUserId = new DataGridViewTextBoxColumn
            {
                Name = "id_user",
                HeaderText = "ID пользователя",
                Width = 100,
                Visible = false
            };
            dgvLibrary.Columns.Add(colUserId);

            DataGridViewTextBoxColumn colGameName = new DataGridViewTextBoxColumn
            {
                Name = "game_name",
                HeaderText = "Название игры",
                Width = 300,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = textLight,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold)
                }
            };
            dgvLibrary.Columns.Add(colGameName);

            DataGridViewTextBoxColumn colReleaseDate = new DataGridViewTextBoxColumn
            {
                Name = "Year_release",
                HeaderText = "Дата выхода",
                Width = 120,
                ReadOnly = true
            };
            dgvLibrary.Columns.Add(colReleaseDate);

            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn
            {
                Name = "price",
                HeaderText = "Цена",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Gold,
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            };
            dgvLibrary.Columns.Add(colPrice);

            DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn
            {
                Name = "deleteButton",
                HeaderText = "Действие",
                Text = "🗑️ Удалить",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            deleteButton.DefaultCellStyle.BackColor = accentRed;
            deleteButton.DefaultCellStyle.ForeColor = Color.White;
            deleteButton.DefaultCellStyle.SelectionBackColor = accentRed;
            deleteButton.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvLibrary.Columns.Add(deleteButton);

            dgvLibrary.CellClick += (s, e) =>
            {
                if (e.ColumnIndex == dgvLibrary.Columns["deleteButton"].Index && e.RowIndex >= 0)
                {
                    int libId = Convert.ToInt32(dgvLibrary.Rows[e.RowIndex].Cells["id_library_item"].Value);
                    string gameName = dgvLibrary.Rows[e.RowIndex].Cells["game_name"].Value.ToString();
                    RemoveFromLibrary(libId, gameName);
                }
            };

            libraryPanel.Controls.Add(dgvLibrary);

            Panel infoPanel = new Panel
            {
                Location = new Point(50, contentPanel.Height - 90),
                Width = contentPanel.Width - 100,
                Height = 50,
                BackColor = Color.FromArgb(45, 45, 45),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label infoLabel = new Label
            {
                Location = new Point(20, 10),
                Size = new Size(infoPanel.Width - 40, 30),
                Font = new Font("Segoe UI", 10),
                ForeColor = textGray,
                Text = "📌 Всего игр в библиотеке: 0",
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "infoLabel"
            };
            infoPanel.Controls.Add(infoLabel);

            libraryPanel.Controls.Add(infoPanel);

            contentPanel.Controls.Add(libraryPanel);
        }

        private void CreateCartSection()
        {
            Panel cartPanel = new Panel
            {
                Name = "panelCart",
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
                Text = "🛒 МОЯ КОРЗИНА",
                TextAlign = ContentAlignment.MiddleCenter
            };
            cartPanel.Controls.Add(titleLabel);

            DataGridView dgvCart = new DataGridView
            {
                Name = "dgvCart",
                Location = new Point(50, 80),
                Width = contentPanel.Width - 100,
                Height = contentPanel.Height - 250,
                BackgroundColor = darkPanel,
                ForeColor = textLight,
                GridColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvCart.AllowUserToResizeColumns = false;
            dgvCart.AllowUserToResizeRows = false;
            dgvCart.AllowUserToOrderColumns = false;
            dgvCart.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvCart.MultiSelect = false;
            dgvCart.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgvCart.DefaultCellStyle.ForeColor = textLight;
            dgvCart.DefaultCellStyle.SelectionBackColor = accentOrange;
            dgvCart.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvCart.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvCart.ColumnHeadersDefaultCellStyle.BackColor = darkPanel;
            dgvCart.ColumnHeadersDefaultCellStyle.ForeColor = textLight;
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvCart.EnableHeadersVisualStyles = false;

            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();

            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn
            {
                Name = "id_item",
                HeaderText = "ID",
                DataPropertyName = "id_item",
                Width = 50,
                ReadOnly = true,
                Visible = false
            };
            dgvCart.Columns.Add(colId);

            DataGridViewTextBoxColumn colGameName = new DataGridViewTextBoxColumn
            {
                Name = "game_name",
                HeaderText = "Название игры",
                Width = 300,
                ReadOnly = true
            };
            dgvCart.Columns.Add(colGameName);

            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn
            {
                Name = "price",
                HeaderText = "Цена",
                Width = 120,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Gold,
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            };
            dgvCart.Columns.Add(colPrice);

            DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn
            {
                Name = "deleteButton",
                HeaderText = "Действие",
                Text = "🗑️ Удалить",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            deleteButton.DefaultCellStyle.BackColor = accentRed;
            deleteButton.DefaultCellStyle.ForeColor = Color.White;
            deleteButton.DefaultCellStyle.SelectionBackColor = accentRed;
            deleteButton.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvCart.Columns.Add(deleteButton);

            dgvCart.CellClick += (s, e) =>
            {
                if (e.ColumnIndex == dgvCart.Columns["deleteButton"].Index && e.RowIndex >= 0)
                {
                    int itemId = Convert.ToInt32(dgvCart.Rows[e.RowIndex].Cells["id_item"].Value);
                    RemoveFromCart(itemId);
                }
            };

            cartPanel.Controls.Add(dgvCart);

            Panel totalPanel = new Panel
            {
                Name = "totalPanel",
                Location = new Point(50, contentPanel.Height - 160),
                Width = contentPanel.Width - 100,
                Height = 60,
                BackColor = Color.FromArgb(45, 45, 45),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label totalLabel = new Label
            {
                Name = "totalLabel",
                Location = new Point(20, 15),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Gold,
                Text = "ИТОГО: 0 ₽",
                TextAlign = ContentAlignment.MiddleLeft
            };
            totalPanel.Controls.Add(totalLabel);

            Label countLabel = new Label
            {
                Name = "countLabel",
                Location = new Point(350, 15),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 14),
                ForeColor = textGray,
                Text = "Товаров: 0",
                TextAlign = ContentAlignment.MiddleLeft
            };
            totalPanel.Controls.Add(countLabel);

            cartPanel.Controls.Add(totalPanel);

            Panel buttonPanel = new Panel
            {
                Name = "buttonPanel",
                Location = new Point(50, contentPanel.Height - 90),
                Width = contentPanel.Width - 100,
                Height = 70,
                BackColor = Color.Transparent
            };

            Button checkoutBtn = new Button
            {
                Name = "checkoutBtn",
                Location = new Point(buttonPanel.Width - 230, 10),
                Size = new Size(220, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = accentGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Text = "✅ КУПИТЬ ВСЁ",
                Enabled = false
            };
            checkoutBtn.FlatAppearance.BorderSize = 0;
            checkoutBtn.Click += Checkout_Click;
            buttonPanel.Controls.Add(checkoutBtn);

            Button clearBtn = new Button
            {
                Name = "clearBtn",
                Location = new Point(buttonPanel.Width - 460, 10),
                Size = new Size(220, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = accentRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Text = "🗑️ ОЧИСТИТЬ ВСЁ"
            };
            clearBtn.FlatAppearance.BorderSize = 0;
            clearBtn.Click += ClearCart_Click;
            buttonPanel.Controls.Add(clearBtn);

            cartPanel.Controls.Add(buttonPanel);

            contentPanel.Controls.Add(cartPanel);
        }

        private void CreateSettingsSection()
        {
            Panel settingsPanel = new Panel
            {
                Name = "panelSettings",
                Location = new Point(0, 0),
                Size = contentPanel.Size,
                BackColor = Color.Transparent,
                Visible = false
            };

            Label titleLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(contentPanel.Width, 60),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = textLight,
                Text = "⚙️ НАСТРОЙКИ",
                TextAlign = ContentAlignment.MiddleCenter
            };
            settingsPanel.Controls.Add(titleLabel);

            // Профиль пользователя
            Panel profileCard = new Panel
            {
                Location = new Point(50, 80),
                Size = new Size(contentPanel.Width - 100, 150),
                BackColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label profileTitle = new Label
            {
                Location = new Point(20, 15),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = textLight,
                Text = "👤 ПРОФИЛЬ"
            };
            profileCard.Controls.Add(profileTitle);

            Label userInfo = new Label
            {
                Location = new Point(40, 60),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 12),
                ForeColor = textGray,
                Text = $"Логин: {currentLogin}"
            };
            profileCard.Controls.Add(userInfo);

            Label userIdInfo = new Label
            {
                Location = new Point(40, 90),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 12),
                ForeColor = textGray,
                Text = $"ID пользователя: {userIDint}"
            };
            profileCard.Controls.Add(userIdInfo);

            settingsPanel.Controls.Add(profileCard);

            // Статистика покупок
            Panel statsCard = new Panel
            {
                Location = new Point(50, 250),
                Size = new Size(contentPanel.Width - 100, 200),
                BackColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label statsTitle = new Label
            {
                Location = new Point(20, 15),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = textLight,
                Text = "📊 СТАТИСТИКА ПОКУПОК"
            };
            statsCard.Controls.Add(statsTitle);

            // Получаем статистику
            var stats = GetUserPurchaseStats();

            Label gamesStats = new Label
            {
                Name = "gamesStats",
                Location = new Point(40, 60),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = accentGreen,
                Text = $"📚 Игр в библиотеке: {stats.TotalGames}",
                TextAlign = ContentAlignment.MiddleLeft
            };
            statsCard.Controls.Add(gamesStats);

            Label uniqueGamesStats = new Label
            {
                Location = new Point(40, 95),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 12),
                ForeColor = textLight,
                Text = $"🎮 Уникальных игр: {stats.UniqueGames}",
                TextAlign = ContentAlignment.MiddleLeft
            };
            statsCard.Controls.Add(uniqueGamesStats);

            Label totalSpentStats = new Label
            {
                Name = "totalSpentStats",
                Location = new Point(40, 130),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Gold,
                Text = $"💰 Всего потрачено: {stats.TotalSpent:N2} ₽",
                TextAlign = ContentAlignment.MiddleLeft
            };
            statsCard.Controls.Add(totalSpentStats);

            Label avgPriceStats = new Label
            {
                Location = new Point(40, 165),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 12),
                ForeColor = textGray,
                Text = $"📊 Средняя цена игры: {stats.AvgPrice:N2} ₽",
                TextAlign = ContentAlignment.MiddleLeft
            };
            statsCard.Controls.Add(avgPriceStats);

            settingsPanel.Controls.Add(statsCard);

            // Дополнительная информация
            Panel infoCard = new Panel
            {
                Location = new Point(50, 470),
                Size = new Size(contentPanel.Width - 100, 100),
                BackColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label infoTitle = new Label
            {
                Location = new Point(20, 15),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = textLight,
                Text = "ℹ️ ИНФОРМАЦИЯ"
            };
            infoCard.Controls.Add(infoTitle);

            Label versionLabel = new Label
            {
                Location = new Point(40, 60),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 12),
                ForeColor = textGray,
                Text = "Версия приложения: 1.0.0"
            };
            infoCard.Controls.Add(versionLabel);

            settingsPanel.Controls.Add(infoCard);

            contentPanel.Controls.Add(settingsPanel);
        }

        // Класс для хранения статистики
        private class PurchaseStats
        {
            public int TotalGames { get; set; }
            public int UniqueGames { get; set; }
            public decimal TotalSpent { get; set; }
            public decimal AvgPrice { get; set; }
        }

        // Метод для получения статистики покупок пользователя
        private PurchaseStats GetUserPurchaseStats()
        {
            var stats = new PurchaseStats();

            try
            {
                // Получаем все записи из библиотеки пользователя
                DataRow[] libraryRows = kursovaya1DataSet.Library.Select($"id_user = {userIDint}");
                stats.TotalGames = libraryRows.Length;

                // Получаем уникальные игры (без учета копий)
                var uniqueGameIds = new HashSet<int>();
                decimal totalSpent = 0;

                foreach (DataRow libRow in libraryRows)
                {
                    int gameId = Convert.ToInt32(libRow["id_game"]);
                    uniqueGameIds.Add(gameId);

                    // Получаем цену игры
                    DataRow[] gameRows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                    if (gameRows.Length > 0)
                    {
                        totalSpent += Convert.ToDecimal(gameRows[0]["price"]);
                    }
                }

                stats.UniqueGames = uniqueGameIds.Count;
                stats.TotalSpent = totalSpent;

                // Рассчитываем среднюю цену
                if (stats.UniqueGames > 0)
                {
                    stats.AvgPrice = totalSpent / stats.UniqueGames;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении статистики: {ex.Message}");
            }

            return stats;
        }

        #endregion

        #region Логика

        private void LoadData()
        {
            try
            {
                kursovaya1DataSet.Clear();

                if (gamesTableAdapter != null)
                    gamesTableAdapter.Fill(kursovaya1DataSet.Games);

                if (usersTableAdapter != null)
                    usersTableAdapter.Fill(kursovaya1DataSet.Users);

                if (libraryTableAdapter != null)
                    libraryTableAdapter.Fill(kursovaya1DataSet.Library);

                if (ratingsTableAdapter != null)
                    ratingsTableAdapter.Fill(kursovaya1DataSet.Ratings);

                if (reviewsTableAdapter != null)
                    reviewsTableAdapter.Fill(kursovaya1DataSet.Reviews);

                if (cartTableAdapter != null)
                    cartTableAdapter.Fill(kursovaya1DataSet.Cart);
                if (genresTableAdapter != null)
                    genresTableAdapter.Fill(kursovaya1DataSet.Genres);

                // Проверка загрузки данных
                Console.WriteLine($"Загружено игр: {kursovaya1DataSet.Games.Rows.Count}");
                Console.WriteLine($"Загружено записей библиотеки: {kursovaya1DataSet.Library.Rows.Count}");
                Console.WriteLine($"Загружено записей корзины: {kursovaya1DataSet.Cart.Rows.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillLibraryData(DataGridView dgvLibrary)
        {
            try
            {
                dgvLibrary.Rows.Clear();

                if (dgvLibrary.DataSource != null)
                {
                    dgvLibrary.DataSource = null;
                }

                DataRow[] libraryRows = kursovaya1DataSet.Library.Select($"id_user = {userIDint}");

                foreach (DataRow libRow in libraryRows)
                {
                    int gameId = Convert.ToInt32(libRow["id_game"]);
                    int id_library_item = Convert.ToInt32(libRow["id_library_item"]);

                    DataRow[] gameRows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                    if (gameRows.Length > 0)
                    {
                        DataRow gameRow = gameRows[0];

                        string gameName = gameRow["name_game"].ToString();
                        string releaseDate = gameRow["Year_release"].ToString();
                        decimal price = Convert.ToDecimal(gameRow["price"]);

                        List<object> rowValues = new List<object>();

                        rowValues.Add(id_library_item);  // id_library_item
                        rowValues.Add(gameId);           // id_game
                        rowValues.Add(userIDint);        // id_user
                        rowValues.Add(gameName);         // game_name
                        rowValues.Add(releaseDate);      // Year_release
                        rowValues.Add(price);            // price

                        dgvLibrary.Rows.Add(rowValues.ToArray());
                    }
                }

                Panel libraryPanel = contentPanel.Controls["panelLibrary"] as Panel;
                if (libraryPanel != null)
                {
                    Panel infoPanel = libraryPanel.Controls.OfType<Panel>().LastOrDefault();
                    if (infoPanel != null)
                    {
                        Label infoLabel = infoPanel.Controls["infoLabel"] as Label;
                        if (infoLabel != null)
                        {
                            infoLabel.Text = $"📌 Всего игр в библиотеке: {dgvLibrary.Rows.Count}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке библиотеки: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveFromLibrary(int libId, string gameName)
        {
            DialogResult result = MessageBox.Show(
                $"Удалить игру \"{gameName}\" из библиотеки?\n\n" +
                "Это действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DataRow[] rows = kursovaya1DataSet.Library.Select($"id_library_item = {libId}");
                    if (rows.Length > 0)
                    {
                        rows[0].Delete();
                        libraryTableAdapter.Update(kursovaya1DataSet.Library);

                        MessageBox.Show($"Игра \"{gameName}\" удалена из библиотеки!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Panel libraryPanel = contentPanel.Controls["panelLibrary"] as Panel;
                        if (libraryPanel != null && libraryPanel.Visible)
                        {
                            DataGridView dgvLibrary = libraryPanel.Controls["dgvLibrary"] as DataGridView;
                            if (dgvLibrary != null)
                            {
                                FillLibraryData(dgvLibrary);
                            }
                        }

                        // Обновляем статистику в настройках, если они открыты
                        Panel settingsPanel = contentPanel.Controls["panelSettings"] as Panel;
                        if (settingsPanel != null && settingsPanel.Visible)
                        {
                            UpdateSettingsStats();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RemoveFromCart(int itemId)
        {
            DialogResult result = MessageBox.Show("Удалить игру из корзины?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DataRow[] rows = kursovaya1DataSet.Cart.Select($"id_item = {itemId}");
                    if (rows.Length > 0)
                    {
                        rows[0].Delete();
                        cartTableAdapter.Update(kursovaya1DataSet.Cart);

                        MessageBox.Show("Игра удалена из корзины!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        UpdateCartTotal();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int index = (int)btn.Tag;

                if (index == 4)
                {
                    Logout();
                }
                else
                {
                    HighlightActiveMenu(index);
                    ShowSection(index);
                }
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
                    contentPanel.Controls["panelHome"].Visible = true;
                    break;
                case 1:
                    {
                        Panel libraryPanel = contentPanel.Controls["panelLibrary"] as Panel;
                        if (libraryPanel != null)
                        {
                            libraryPanel.Visible = true;
                            DataGridView dgvLibrary = libraryPanel.Controls["dgvLibrary"] as DataGridView;
                            if (dgvLibrary != null)
                            {
                                FillLibraryData(dgvLibrary);
                            }
                        }
                    }
                    break;
                case 2:
                    contentPanel.Controls["panelCart"].Visible = true;
                    UpdateCartTotal();
                    break;
                case 3:
                    {
                        contentPanel.Controls["panelSettings"].Visible = true;
                        UpdateSettingsStats(); // Обновляем статистику при открытии настроек
                    }
                    break;
            }
        }

        // Новый метод для обновления статистики в настройках
        private void UpdateSettingsStats()
        {
            try
            {
                Panel settingsPanel = contentPanel.Controls["panelSettings"] as Panel;
                if (settingsPanel != null)
                {
                    var stats = GetUserPurchaseStats();

                    // Обновляем метки с новой статистикой
                    foreach (Control control in settingsPanel.Controls)
                    {
                        if (control is Panel statsCard && statsCard.Location.Y == 250)
                        {
                            foreach (Control subControl in statsCard.Controls)
                            {
                                if (subControl is Label label)
                                {
                                    if (label.Name == "gamesStats")
                                    {
                                        label.Text = $"📚 Игр в библиотеке: {stats.TotalGames}";
                                    }
                                    else if (label.Name == "totalSpentStats")
                                    {
                                        label.Text = $"💰 Всего потрачено: {stats.TotalSpent:N2} ₽";
                                    }
                                    else if (label.Text.StartsWith("🎮 Уникальных игр:"))
                                    {
                                        label.Text = $"🎮 Уникальных игр: {stats.UniqueGames}";
                                    }
                                    else if (label.Text.StartsWith("📊 Средняя цена:"))
                                    {
                                        label.Text = $"📊 Средняя цена игры: {stats.AvgPrice:N2} ₽";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении статистики: {ex.Message}");
            }
        }

        private void HighlightActiveMenu(int activeIndex)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel menuPanel && menuPanel.Name == "menuPanel")
                {
                    foreach (Control btn in menuPanel.Controls)
                    {
                        if (btn is Button menuButton && menuButton.Tag != null)
                        {
                            int btnIndex = (int)menuButton.Tag;
                            if (btnIndex == activeIndex)
                            {
                                menuButton.ForeColor = accentGreen;
                                menuButton.BackColor = Color.FromArgb(50, 50, 50);
                            }
                            else
                            {
                                menuButton.ForeColor = textGray;
                                menuButton.BackColor = Color.Transparent;
                            }
                        }
                    }
                }
            }
        }

        private int GetCurrentSection()
        {
            foreach (Control control in contentPanel.Controls)
            {
                if (control is Panel panel && panel.Visible)
                {
                    if (panel.Name == "panelHome") return 0;
                    if (panel.Name == "panelLibrary") return 1;
                    if (panel.Name == "panelCart") return 2;
                    if (panel.Name == "panelSettings") return 3;
                }
            }
            return 0;
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            TextBox searchBox = sender as TextBox;
            if (searchBox != null)
            {
                if (string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    gamesBindingSource.RemoveFilter();
                }
                else
                {
                    gamesBindingSource.Filter = $"name_game LIKE '%{searchBox.Text}%'";
                }
            }
        }

        private void SearchGames(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                gamesBindingSource.RemoveFilter();
            }
            else
            {
                gamesBindingSource.Filter = $"name_game LIKE '%{query}%'";
            }
        }

        private void UpdateGameDetails(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                Panel homePanel = contentPanel.Controls["panelHome"] as Panel;
                if (homePanel != null)
                {
                    Panel rightColumn = homePanel.Controls.OfType<Panel>().LastOrDefault(p => p.Location.X > 100);
                    if (rightColumn != null)
                    {
                        Panel detailsPanel = rightColumn.Controls["detailsPanel"] as Panel;
                        if (detailsPanel != null)
                        {
                            PictureBox gamePictureBox = detailsPanel.Controls["gamePictureBox"] as PictureBox;
                            Label selectedGameLabel = detailsPanel.Controls["selectedGameLabel"] as Label;
                            Label priceLabel = detailsPanel.Controls["priceLabel"] as Label;
                            Label ratingLabel = detailsPanel.Controls["ratingLabel"] as Label;
                            Label releaseDateLabel = detailsPanel.Controls["releaseDateLabel"] as Label;

                            // Кнопки и списки
                            Button addToCartBtn = detailsPanel.Controls["addToCartBtn"] as Button;
                            Button buyNowBtn = detailsPanel.Controls["buyNowBtn"] as Button;
                            ComboBox ratingComboBox = detailsPanel.Controls["ratingComboBox"] as ComboBox;
                            Button rateBtn = detailsPanel.Controls["rateBtn"] as Button;

                            DataGridViewRow row = dgv.SelectedRows[0];

                            int gameId = Convert.ToInt32(row.Cells["id_game"].Value);
                            string gameName = row.Cells["name_game"].Value?.ToString() ?? "Неизвестная игра";
                            decimal price = Convert.ToDecimal(row.Cells["price"].Value ?? 0);
                            string releaseDate = row.Cells["Year_release"].Value?.ToString() ?? "Не указана";

                            LoadGameImage(gameId, gamePictureBox);

                            selectedGameLabel.Text = gameName;
                            priceLabel.Text = $"Цена: {price:N2} ₽";
                            releaseDateLabel.Text = $"Дата выхода: {releaseDate}";

                            // Высчитываем средний рейтинг
                            double rating = GetGameRating(gameId);
                            ratingLabel.Text = rating > 0 ? $"Средний рейтинг: {rating:F1} ★" : "Рейтинг: Нет оценок";

                            // Проверяем, ставил ли ТЕКУЩИЙ пользователь оценку
                            DataRow[] userRatingRows = kursovaya1DataSet.Ratings.Select($"id_game = {gameId} AND id_users = {userIDint}");
                            if (userRatingRows.Length > 0)
                            {
                                int userScore = Convert.ToInt32(userRatingRows[0]["Overall_mark"]);
                                ratingComboBox.SelectedItem = userScore.ToString();
                                rateBtn.Text = "ИЗМЕНИТЬ";
                            }
                            else
                            {
                                ratingComboBox.SelectedIndex = -1; // Сбрасываем выбор
                                rateBtn.Text = "ОЦЕНИТЬ ⭐";
                            }

                            // ВКЛЮЧАЕМ КНОПКИ и передаем им ID игры
                            rateBtn.Tag = gameId;
                            rateBtn.Enabled = true;

                            addToCartBtn.Tag = gameId;
                            addToCartBtn.Enabled = true;

                            buyNowBtn.Tag = gameId;
                            buyNowBtn.Enabled = true;
                        }
                    }
                }
            }
        }
        private void LoadGameImage(int gameId, PictureBox pictureBox)
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
                        pictureBox.BackColor = Color.FromArgb(50, 50, 50);
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

        private void SetDefaultGameImage(PictureBox pictureBox)
        {
            Bitmap defaultImage = new Bitmap(400, 250);
            using (Graphics g = Graphics.FromImage(defaultImage))
            {
                g.Clear(Color.FromArgb(60, 60, 60));
                using (Font font = new Font("Segoe UI", 20, FontStyle.Bold))
                {
                    string text = "🎮";
                    SizeF textSize = g.MeasureString(text, font);
                    PointF textPoint = new PointF(
                        (defaultImage.Width - textSize.Width) / 2,
                        (defaultImage.Height - textSize.Height) / 2);

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
                    {
                        g.DrawString(text, font, brush, textPoint);
                    }
                }
            }

            if (pictureBox.Image != null)
                pictureBox.Image.Dispose();

            pictureBox.Image = defaultImage;
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private double GetGameRating(int gameId)
        {
            try
            {
                DataRow[] ratingRows = kursovaya1DataSet.Ratings.Select($"id_game = {gameId}");
                if (ratingRows.Length > 0)
                {
                    return ratingRows.Average(r => Convert.ToDouble(r["Overall_mark"]));
                }
            }
            catch { }
            return 0;
        }

        private void AddToCart_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int gameId = (int)btn.Tag;

                try
                {
                    DataRow[] gameRows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                    if (gameRows.Length == 0)
                    {
                        MessageBox.Show("Ошибка: игра не найдена!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string gameName = gameRows[0]["name_game"].ToString();

                    DataRow[] existingInCart = kursovaya1DataSet.Cart.Select($"id_user = {userIDint} AND id_game = {gameId}");

                    if (existingInCart.Length > 0)
                    {
                        MessageBox.Show($"Игра \"{gameName}\" уже есть в корзине!", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    DataRow newRow = kursovaya1DataSet.Cart.NewRow();
                    newRow["id_user"] = userIDint;
                    newRow["id_game"] = gameId;
                    newRow["name_game"] = gameName;

                    kursovaya1DataSet.Cart.Rows.Add(newRow);

                    cartTableAdapter.Update(kursovaya1DataSet.Cart);

                    MessageBox.Show($"Игра \"{gameName}\" добавлена в корзину!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Panel cartPanel = contentPanel.Controls["panelCart"] as Panel;
                    if (cartPanel != null && cartPanel.Visible)
                    {
                        UpdateCartTotal();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении в корзину: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateCartTotal()
        {
            try
            {
                Panel cartPanel = contentPanel.Controls["panelCart"] as Panel;
                if (cartPanel == null) return;

                DataGridView dgvCart = cartPanel.Controls["dgvCart"] as DataGridView;
                Panel totalPanel = cartPanel.Controls["totalPanel"] as Panel;
                Panel buttonPanel = cartPanel.Controls["buttonPanel"] as Panel;

                if (dgvCart == null || totalPanel == null || buttonPanel == null) return;

                Label totalLabel = totalPanel.Controls["totalLabel"] as Label;
                Label countLabel = totalPanel.Controls["countLabel"] as Label;
                Button checkoutBtn = buttonPanel.Controls["checkoutBtn"] as Button;

                if (totalLabel == null || countLabel == null || checkoutBtn == null) return;

                decimal total = 0;
                int count = 0;

                dgvCart.Rows.Clear();

                DataRow[] cartRows = kursovaya1DataSet.Cart.Select($"id_user = {userIDint}");

                foreach (DataRow row in cartRows)
                {
                    int gameId = Convert.ToInt32(row["id_game"]);
                    string gameName = row["name_game"].ToString();
                    int cartItemId = Convert.ToInt32(row["id_item"]);

                    DataRow[] gameRows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                    if (gameRows.Length > 0)
                    {
                        decimal price = Convert.ToDecimal(gameRows[0]["price"]);

                        dgvCart.Rows.Add(
                            cartItemId,
                            gameName,
                            price.ToString("N2") + " ₽"
                        );

                        total += price;
                        count++;
                    }
                }

                totalLabel.Text = $"ИТОГО: {total:N2} ₽";
                countLabel.Text = $"Товаров: {count}";

                if (count > 0)
                {
                    checkoutBtn.Text = $"✅ КУПИТЬ ВСЁ ({total:N2} ₽)";
                    checkoutBtn.Enabled = true;
                    checkoutBtn.BackColor = accentGreen;
                }
                else
                {
                    checkoutBtn.Text = "✅ КУПИТЬ ВСЁ";
                    checkoutBtn.Enabled = false;
                    checkoutBtn.BackColor = Color.FromArgb(100, 100, 100);
                }

                dgvCart.Refresh();
                totalPanel.Refresh();
                buttonPanel.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении корзины: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Checkout_Click(object sender, EventArgs e)
        {
            DataRow[] cartRows = kursovaya1DataSet.Cart.Select($"id_user = {userIDint}");

            if (cartRows.Length == 0)
            {
                MessageBox.Show("Корзина пуста!", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal total = 0;
            string gameList = "";
            string duplicateList = "";
            bool hasDuplicates = false;

            foreach (DataRow row in cartRows)
            {
                int gameId = Convert.ToInt32(row["id_game"]);
                string gameName = row["name_game"].ToString();

                DataRow[] existingInLibrary = kursovaya1DataSet.Library.Select($"id_user = {userIDint} AND id_game = {gameId}");

                if (existingInLibrary.Length > 0)
                {
                    duplicateList += $"• {gameName} (уже есть в библиотеке)\n";
                    hasDuplicates = true;
                }
                else
                {
                    gameList += $"• {gameName}\n";
                }

                DataRow[] gameRows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                if (gameRows.Length > 0)
                {
                    total += Convert.ToDecimal(gameRows[0]["price"]);
                }
            }

            string message = $"Оформить покупку на сумму {total:N2} ₽?\n\n";

            if (!string.IsNullOrEmpty(gameList))
            {
                message += $"Новые игры:\n{gameList}\n";
            }

            if (hasDuplicates)
            {
                message += $"⚠️ Внимание! Следующие игры уже есть в библиотеке:\n{duplicateList}\n";
                message += "При покупке они будут добавлены как копии.\n\n";
            }

            DialogResult result = MessageBox.Show(message, "Подтверждение покупки",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (DataRow row in cartRows)
                    {
                        int gameId = Convert.ToInt32(row["id_game"]);

                        DataRow libRow = kursovaya1DataSet.Library.NewRow();
                        libRow["id_user"] = userIDint;
                        libRow["id_game"] = gameId;

                        kursovaya1DataSet.Library.Rows.Add(libRow);
                    }

                    libraryTableAdapter.Update(kursovaya1DataSet.Library);

                    int purchasedCount = cartRows.Length;

                    foreach (DataRow row in cartRows)
                    {
                        row.Delete();
                    }
                    cartTableAdapter.Update(kursovaya1DataSet.Cart);

                    string duplicateMessage = hasDuplicates ?
                        "\n\nНекоторые игры уже были в библиотеке и добавлены как копии." : "";

                    MessageBox.Show(
                        $"Покупка успешно оформлена!\n\n" +
                        $"Сумма: {total:N2} ₽\n" +
                        $"Куплено игр: {purchasedCount}{duplicateMessage}\n\n" +
                        $"Игры добавлены в вашу библиотеку.",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    UpdateCartTotal();

                    Panel libraryPanel = contentPanel.Controls["panelLibrary"] as Panel;
                    if (libraryPanel != null && libraryPanel.Visible)
                    {
                        DataGridView dgvLibrary = libraryPanel.Controls["dgvLibrary"] as DataGridView;
                        if (dgvLibrary != null)
                        {
                            FillLibraryData(dgvLibrary);
                        }
                    }

                    // Обновляем статистику в настройках
                    Panel settingsPanel = contentPanel.Controls["panelSettings"] as Panel;
                    if (settingsPanel != null && settingsPanel.Visible)
                    {
                        UpdateSettingsStats();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при оформлении покупки: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BuySelectedGame_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int gameId = (int)btn.Tag;

                try
                {
                    DataRow[] gameRows = kursovaya1DataSet.Games.Select($"id_game = {gameId}");
                    if (gameRows.Length == 0) return;

                    string gameName = gameRows[0]["name_game"].ToString();
                    decimal price = Convert.ToDecimal(gameRows[0]["price"]);

                    DialogResult result = MessageBox.Show(
                        $"Купить игру \"{gameName}\" за {price:N2} ₽?",
                        "Подтверждение покупки",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        DataRow libRow = kursovaya1DataSet.Library.NewRow();
                        libRow["id_user"] = userIDint;
                        libRow["id_game"] = gameId;
                        kursovaya1DataSet.Library.Rows.Add(libRow);

                        libraryTableAdapter.Update(kursovaya1DataSet.Library);

                        MessageBox.Show(
                            $"Игра \"{gameName}\" успешно куплена!\n" +
                            $"Сумма: {price:N2} ₽\n\n" +
                            $"Игра добавлена в вашу библиотеку.",
                            "Успех",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Обновляем статистику в настройках
                        Panel settingsPanel = contentPanel.Controls["panelSettings"] as Panel;
                        if (settingsPanel != null && settingsPanel.Visible)
                        {
                            UpdateSettingsStats();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при покупке: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearCart_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Очистить всю корзину?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DataRow[] cartRows = kursovaya1DataSet.Cart.Select($"id_user = {userIDint}");

                    foreach (DataRow row in cartRows)
                    {
                        row.Delete();
                    }

                    cartTableAdapter.Update(kursovaya1DataSet.Cart);

                    MessageBox.Show("Корзина очищена!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateCartTotal();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при очистке корзины: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Logout()
        {
            DialogResult result = MessageBox.Show("Выйти из аккаунта?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                Form1 loginForm = new Form1();
                loginForm.FormClosed += (s, args) => Application.Exit();
                loginForm.Show();
            }
        }

        #endregion

        #region Обработчики событий формы

        private void FormUSERMODE_Paint(object sender, PaintEventArgs e)
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

        private void FormUSERMODE_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void FormUSERMODE_Resize(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                contentPanel.Size = new Size(this.Width - 320, this.Height - 40);

                foreach (Control control in contentPanel.Controls)
                {
                    if (control is Panel)
                    {
                        control.Size = contentPanel.Size;
                        UpdatePanelLayout(control as Panel);
                    }
                }
            }
        }

        private void UpdatePanelLayout(Panel panel)
        {
            if (panel.Name == "panelHome")
            {
                foreach (Control control in panel.Controls)
                {
                    if (control is Panel searchPanel && searchPanel.Location.Y == 80)
                    {
                        searchPanel.Width = panel.Width - 100;
                        TextBox searchBox = searchPanel.Controls.OfType<TextBox>().FirstOrDefault();
                        if (searchBox != null) searchBox.Width = searchPanel.Width - 120;
                        Button searchBtn = searchPanel.Controls.OfType<Button>().FirstOrDefault();
                        if (searchBtn != null) searchBtn.Location = new Point(searchPanel.Width - 100, 5);
                    }
                    else if (control is Panel leftCol && control.Location.X == 50 && control.Location.Y == 150)
                    {
                        leftCol.Size = new Size((panel.Width - 150) / 2, panel.Height - 200);
                        DataGridView dgv = leftCol.Controls["dgvGames"] as DataGridView;
                        if (dgv != null) dgv.Size = new Size(leftCol.Width, leftCol.Height - 40);
                    }
                    else if (control is Panel rightCol && control.Location.X > 50 && control.Location.Y == 150)
                    {
                        int leftWidth = (panel.Width - 150) / 2;
                        rightCol.Location = new Point(50 + leftWidth + 50, 150);
                        rightCol.Size = new Size(leftWidth, panel.Height - 200);

                        Panel detailsPanel = rightCol.Controls["detailsPanel"] as Panel;
                        if (detailsPanel != null)
                        {
                            detailsPanel.Size = new Size(rightCol.Width, rightCol.Height - 40);
                            foreach (Control c in detailsPanel.Controls)
                            {
                                if (c is PictureBox || c is Label || (c is Button && c.Name != "rateBtn"))
                                    c.Width = detailsPanel.Width - 40;
                            }
                        }
                    }
                }
            }
            else if (panel.Name == "panelCart")
            {
                // Ищем элементы КОРЗИНЫ по их именам (Name)
                DataGridView dgvCart = panel.Controls["dgvCart"] as DataGridView;
                Panel totalPanel = panel.Controls["totalPanel"] as Panel;
                Panel buttonPanel = panel.Controls["buttonPanel"] as Panel;

                if (dgvCart != null)
                {
                    dgvCart.Width = panel.Width - 100;
                    // Оставляем 260 пикселей снизу для панелей с итогами и кнопками
                    dgvCart.Height = panel.Height - 280;
                }

                if (totalPanel != null)
                {
                    totalPanel.Width = panel.Width - 100;
                    // Прижимаем к низу, учитывая высоту кнопок
                    totalPanel.Location = new Point(50, panel.Height - 160);
                }

                if (buttonPanel != null)
                {
                    buttonPanel.Width = panel.Width - 100;
                    // Самый низ
                    buttonPanel.Location = new Point(50, panel.Height - 90);

                    // Располагаем кнопки КУПИТЬ и ОЧИСТИТЬ внутри панели относительно правой стороны
                    Button checkoutBtn = buttonPanel.Controls["checkoutBtn"] as Button;
                    if (checkoutBtn != null)
                        checkoutBtn.Location = new Point(buttonPanel.Width - 230, 10);

                    Button clearBtn = buttonPanel.Controls["clearBtn"] as Button;
                    if (clearBtn != null)
                        clearBtn.Location = new Point(buttonPanel.Width - 460, 10);
                }
            }
            else if (panel.Name == "panelLibrary")
            {
                DataGridView dgv = panel.Controls["dgvLibrary"] as DataGridView;
                Panel infoPanel = panel.Controls.OfType<Panel>().FirstOrDefault(p => p.Height == 50);

                if (dgv != null)
                {
                    dgv.Width = panel.Width - 100;
                    dgv.Height = panel.Height - 180;
                }
                if (infoPanel != null)
                {
                    infoPanel.Width = panel.Width - 100;
                    infoPanel.Location = new Point(50, panel.Height - 90);
                }
            }
            else if (panel.Name == "panelSettings")
            {
                foreach (Control control in panel.Controls)
                {
                    if (control is Panel card) card.Width = panel.Width - 100;
                }
            }
        }
        #endregion

        private void FormUSERMODE_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "kursovaya1DataSet.Genres". При необходимости она может быть перемещена или удалена.
            this.genresTableAdapter.Fill(this.kursovaya1DataSet.Genres);

        }
    }
}