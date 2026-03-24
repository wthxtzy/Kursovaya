namespace Store
{
    partial class FormUSERMODE
    {
        private System.ComponentModel.IContainer components = null;

        // DataSet и адаптеры
        private Kursovaya1DataSet kursovaya1DataSet;
        private System.Windows.Forms.BindingSource gamesBindingSource;
        private System.Windows.Forms.BindingSource usersBindingSource;
        private System.Windows.Forms.BindingSource libraryBindingSource;
        private System.Windows.Forms.BindingSource cartBindingSource;
        private System.Windows.Forms.BindingSource ratingsBindingSource;
        private System.Windows.Forms.BindingSource reviewsBindingSource;

        private Store.Kursovaya1DataSetTableAdapters.GamesTableAdapter gamesTableAdapter;
        private Store.Kursovaya1DataSetTableAdapters.UsersTableAdapter usersTableAdapter;
        private Store.Kursovaya1DataSetTableAdapters.LibraryTableAdapter libraryTableAdapter;
        private Store.Kursovaya1DataSetTableAdapters.CartTableAdapter cartTableAdapter;
        private Store.Kursovaya1DataSetTableAdapters.RatingsTableAdapter ratingsTableAdapter;
        private Store.Kursovaya1DataSetTableAdapters.ReviewsTableAdapter reviewsTableAdapter;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.kursovaya1DataSet = new Store.Kursovaya1DataSet();
            this.gamesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.usersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.libraryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cartBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ratingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reviewsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gamesTableAdapter = new Store.Kursovaya1DataSetTableAdapters.GamesTableAdapter();
            this.usersTableAdapter = new Store.Kursovaya1DataSetTableAdapters.UsersTableAdapter();
            this.libraryTableAdapter = new Store.Kursovaya1DataSetTableAdapters.LibraryTableAdapter();
            this.cartTableAdapter = new Store.Kursovaya1DataSetTableAdapters.CartTableAdapter();
            this.ratingsTableAdapter = new Store.Kursovaya1DataSetTableAdapters.RatingsTableAdapter();
            this.reviewsTableAdapter = new Store.Kursovaya1DataSetTableAdapters.ReviewsTableAdapter();
            this.genresBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.genresTableAdapter = new Store.Kursovaya1DataSetTableAdapters.GenresTableAdapter();
            this.tableAdapterManager = new Store.Kursovaya1DataSetTableAdapters.TableAdapterManager();
            this.genresDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.kursovaya1DataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gamesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.libraryBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cartBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratingsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reviewsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.genresBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.genresDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // kursovaya1DataSet
            // 
            this.kursovaya1DataSet.DataSetName = "Kursovaya1DataSet";
            this.kursovaya1DataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // gamesBindingSource
            // 
            this.gamesBindingSource.DataMember = "Games";
            this.gamesBindingSource.DataSource = this.kursovaya1DataSet;
            // 
            // usersBindingSource
            // 
            this.usersBindingSource.DataMember = "Users";
            this.usersBindingSource.DataSource = this.kursovaya1DataSet;
            // 
            // libraryBindingSource
            // 
            this.libraryBindingSource.DataMember = "Library";
            this.libraryBindingSource.DataSource = this.kursovaya1DataSet;
            // 
            // cartBindingSource
            // 
            this.cartBindingSource.DataMember = "Cart";
            this.cartBindingSource.DataSource = this.kursovaya1DataSet;
            // 
            // ratingsBindingSource
            // 
            this.ratingsBindingSource.DataMember = "Ratings";
            this.ratingsBindingSource.DataSource = this.kursovaya1DataSet;
            // 
            // reviewsBindingSource
            // 
            this.reviewsBindingSource.DataMember = "Reviews";
            this.reviewsBindingSource.DataSource = this.kursovaya1DataSet;
            // 
            // gamesTableAdapter
            // 
            this.gamesTableAdapter.ClearBeforeFill = true;
            // 
            // usersTableAdapter
            // 
            this.usersTableAdapter.ClearBeforeFill = true;
            // 
            // libraryTableAdapter
            // 
            this.libraryTableAdapter.ClearBeforeFill = true;
            // 
            // cartTableAdapter
            // 
            this.cartTableAdapter.ClearBeforeFill = true;
            // 
            // ratingsTableAdapter
            // 
            this.ratingsTableAdapter.ClearBeforeFill = true;
            // 
            // reviewsTableAdapter
            // 
            this.reviewsTableAdapter.ClearBeforeFill = true;
            // 
            // genresBindingSource
            // 
            this.genresBindingSource.DataMember = "Genres";
            this.genresBindingSource.DataSource = this.kursovaya1DataSet;
            // 
            // genresTableAdapter
            // 
            this.genresTableAdapter.ClearBeforeFill = true;
            // 
            // tableAdapterManager
            // 
            this.tableAdapterManager.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager.CartTableAdapter = this.cartTableAdapter;
            this.tableAdapterManager.GamesTableAdapter = this.gamesTableAdapter;
            this.tableAdapterManager.GenresTableAdapter = this.genresTableAdapter;
            this.tableAdapterManager.LibraryTableAdapter = this.libraryTableAdapter;
            this.tableAdapterManager.RatingsTableAdapter = this.ratingsTableAdapter;
            this.tableAdapterManager.ReviewsTableAdapter = this.reviewsTableAdapter;
            this.tableAdapterManager.UpdateOrder = Store.Kursovaya1DataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            this.tableAdapterManager.UsersTableAdapter = this.usersTableAdapter;
            // 
            // genresDataGridView
            // 
            this.genresDataGridView.AutoGenerateColumns = false;
            this.genresDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.genresDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            this.genresDataGridView.DataSource = this.genresBindingSource;
            this.genresDataGridView.Location = new System.Drawing.Point(324, 153);
            this.genresDataGridView.Name = "genresDataGridView";
            this.genresDataGridView.RowHeadersWidth = 51;
            this.genresDataGridView.RowTemplate.Height = 24;
            this.genresDataGridView.Size = new System.Drawing.Size(300, 220);
            this.genresDataGridView.TabIndex = 0;
            this.genresDataGridView.Visible = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Id_genre";
            this.dataGridViewTextBoxColumn1.HeaderText = "Id_genre";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 125;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Name_genre";
            this.dataGridViewTextBoxColumn2.HeaderText = "Name_genre";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 6;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 125;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Description_genre";
            this.dataGridViewTextBoxColumn3.HeaderText = "Description_genre";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 6;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 125;
            // 
            // FormUSERMODE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Controls.Add(this.genresDataGridView);
            this.Name = "FormUSERMODE";
            this.Text = "Магазин игр";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUSERMODE_FormClosing);
            this.Load += new System.EventHandler(this.FormUSERMODE_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormUSERMODE_Paint);
            this.Resize += new System.EventHandler(this.FormUSERMODE_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.kursovaya1DataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gamesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.libraryBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cartBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratingsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reviewsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.genresBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.genresDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.BindingSource genresBindingSource;
        private Kursovaya1DataSetTableAdapters.GenresTableAdapter genresTableAdapter;
        private Kursovaya1DataSetTableAdapters.TableAdapterManager tableAdapterManager;
        private System.Windows.Forms.DataGridView genresDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    }
}