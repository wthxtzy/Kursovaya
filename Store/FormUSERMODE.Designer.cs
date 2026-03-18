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

            // Инициализация BindingSource
            this.gamesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.usersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.libraryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cartBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ratingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reviewsBindingSource = new System.Windows.Forms.BindingSource(this.components);

            // Инициализация TableAdapters
            this.gamesTableAdapter = new Store.Kursovaya1DataSetTableAdapters.GamesTableAdapter();
            this.usersTableAdapter = new Store.Kursovaya1DataSetTableAdapters.UsersTableAdapter();
            this.libraryTableAdapter = new Store.Kursovaya1DataSetTableAdapters.LibraryTableAdapter();
            this.cartTableAdapter = new Store.Kursovaya1DataSetTableAdapters.CartTableAdapter();
            this.ratingsTableAdapter = new Store.Kursovaya1DataSetTableAdapters.RatingsTableAdapter();
            this.reviewsTableAdapter = new Store.Kursovaya1DataSetTableAdapters.ReviewsTableAdapter();

            // Настройка BindingSource
            this.gamesBindingSource.DataMember = "Games";
            this.gamesBindingSource.DataSource = this.kursovaya1DataSet;

            this.usersBindingSource.DataMember = "Users";
            this.usersBindingSource.DataSource = this.kursovaya1DataSet;

            this.libraryBindingSource.DataMember = "Library";
            this.libraryBindingSource.DataSource = this.kursovaya1DataSet;

            this.cartBindingSource.DataMember = "Cart";
            this.cartBindingSource.DataSource = this.kursovaya1DataSet;

            this.ratingsBindingSource.DataMember = "Ratings";
            this.ratingsBindingSource.DataSource = this.kursovaya1DataSet;

            this.reviewsBindingSource.DataMember = "Reviews";
            this.reviewsBindingSource.DataSource = this.kursovaya1DataSet;

            // Настройка формы
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Name = "FormUSERMODE";
            this.Text = "Магазин игр";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormUSERMODE_Paint);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUSERMODE_FormClosing);
            this.Resize += new System.EventHandler(this.FormUSERMODE_Resize);

            ((System.ComponentModel.ISupportInitialize)(this.kursovaya1DataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gamesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.libraryBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cartBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratingsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reviewsBindingSource)).EndInit();
        }
    }
}