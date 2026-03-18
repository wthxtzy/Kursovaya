namespace Store
{
    partial class FormADMINMODE
    {
        private System.ComponentModel.IContainer components = null;

        // DataSet и адаптеры
        private Kursovaya1DataSet kursovaya1DataSet;
        private System.Windows.Forms.BindingSource gamesBindingSource;
        private System.Windows.Forms.BindingSource usersBindingSource;
        private System.Windows.Forms.BindingSource libraryBindingSource;
        private System.Windows.Forms.BindingSource ratingsBindingSource;
        private System.Windows.Forms.BindingSource reviewsBindingSource;

        private Store.Kursovaya1DataSetTableAdapters.GamesTableAdapter gamesTableAdapter;
        private Store.Kursovaya1DataSetTableAdapters.UsersTableAdapter usersTableAdapter;
        private Store.Kursovaya1DataSetTableAdapters.LibraryTableAdapter libraryTableAdapter;
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
            this.ratingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reviewsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gamesTableAdapter = new Store.Kursovaya1DataSetTableAdapters.GamesTableAdapter();
            this.usersTableAdapter = new Store.Kursovaya1DataSetTableAdapters.UsersTableAdapter();
            this.libraryTableAdapter = new Store.Kursovaya1DataSetTableAdapters.LibraryTableAdapter();
            this.ratingsTableAdapter = new Store.Kursovaya1DataSetTableAdapters.RatingsTableAdapter();
            this.reviewsTableAdapter = new Store.Kursovaya1DataSetTableAdapters.ReviewsTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.kursovaya1DataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gamesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.libraryBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratingsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reviewsBindingSource)).BeginInit();
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
            // ratingsTableAdapter
            // 
            this.ratingsTableAdapter.ClearBeforeFill = true;
            // 
            // reviewsTableAdapter
            // 
            this.reviewsTableAdapter.ClearBeforeFill = true;
            // 
            // FormADMINMODE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 650);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormADMINMODE";
            this.Text = "Панель администратора";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormADMINMODE_FormClosing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormADMINMODE_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormADMINMODE_KeyDown);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.FormADMINMODE_PreviewKeyDown);
            this.Resize += new System.EventHandler(this.FormADMINMODE_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.kursovaya1DataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gamesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.libraryBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratingsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reviewsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }
    }
}