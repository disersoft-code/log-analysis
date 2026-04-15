namespace LogAnalysis
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSelectFiles = new Button();
            listBoxFiles = new ListBox();
            label1 = new Label();
            btnAnalyze = new Button();
            progBarGeneral = new ProgressBar();
            dataGridView1 = new DataGridView();
            btnExportToExcel = new Button();
            lblLineNumber = new Label();
            lblCurrentFile = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // btnSelectFiles
            // 
            btnSelectFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectFiles.Location = new Point(558, 201);
            btnSelectFiles.Name = "btnSelectFiles";
            btnSelectFiles.Size = new Size(112, 34);
            btnSelectFiles.TabIndex = 0;
            btnSelectFiles.Text = "Seleccionar";
            btnSelectFiles.UseVisualStyleBackColor = true;
            btnSelectFiles.Click += btnSelectFiles_Click;
            // 
            // listBoxFiles
            // 
            listBoxFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            listBoxFiles.FormattingEnabled = true;
            listBoxFiles.ItemHeight = 25;
            listBoxFiles.Location = new Point(12, 66);
            listBoxFiles.Name = "listBoxFiles";
            listBoxFiles.ScrollAlwaysVisible = true;
            listBoxFiles.Size = new Size(776, 129);
            listBoxFiles.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 20);
            label1.Name = "label1";
            label1.Size = new Size(350, 25);
            label1.TabIndex = 2;
            label1.Text = "Seleccione uno o varios archivos a analizar:";
            // 
            // btnAnalyze
            // 
            btnAnalyze.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAnalyze.Enabled = false;
            btnAnalyze.Location = new Point(676, 201);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(112, 34);
            btnAnalyze.TabIndex = 3;
            btnAnalyze.Text = "Analizar";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // progBarGeneral
            // 
            progBarGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progBarGeneral.Location = new Point(12, 201);
            progBarGeneral.Name = "progBarGeneral";
            progBarGeneral.Size = new Size(381, 34);
            progBarGeneral.TabIndex = 4;
            progBarGeneral.Visible = false;
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 254);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(776, 422);
            dataGridView1.TabIndex = 6;
            // 
            // btnExportToExcel
            // 
            btnExportToExcel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExportToExcel.Enabled = false;
            btnExportToExcel.Location = new Point(635, 682);
            btnExportToExcel.Name = "btnExportToExcel";
            btnExportToExcel.Size = new Size(153, 34);
            btnExportToExcel.TabIndex = 7;
            btnExportToExcel.Text = "Exportar CSV";
            btnExportToExcel.UseVisualStyleBackColor = true;
            btnExportToExcel.Click += btnExportToExcel_Click;
            // 
            // lblLineNumber
            // 
            lblLineNumber.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblLineNumber.AutoSize = true;
            lblLineNumber.Location = new Point(399, 206);
            lblLineNumber.Name = "lblLineNumber";
            lblLineNumber.Size = new Size(153, 25);
            lblLineNumber.TabIndex = 8;
            lblLineNumber.Text = "# línea: 00000000";
            lblLineNumber.Visible = false;
            // 
            // lblCurrentFile
            // 
            lblCurrentFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCurrentFile.AutoSize = true;
            lblCurrentFile.Location = new Point(572, 20);
            lblCurrentFile.Name = "lblCurrentFile";
            lblCurrentFile.Size = new Size(216, 25);
            lblCurrentFile.TabIndex = 9;
            lblCurrentFile.Text = "Procesando archivo: xxxxx";
            lblCurrentFile.Visible = false;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 728);
            Controls.Add(lblCurrentFile);
            Controls.Add(lblLineNumber);
            Controls.Add(btnExportToExcel);
            Controls.Add(dataGridView1);
            Controls.Add(progBarGeneral);
            Controls.Add(btnAnalyze);
            Controls.Add(label1);
            Controls.Add(listBoxFiles);
            Controls.Add(btnSelectFiles);
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Análisis de logs";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSelectFiles;
        private ListBox listBoxFiles;
        private Label label1;
        private Button btnAnalyze;
        private ProgressBar progBarGeneral;
        private DataGridView dataGridView1;
        private Button btnExportToExcel;
        private Label lblLineNumber;
        private Label lblCurrentFile;
    }
}
