namespace AddInUtilita
{
    partial class DockableFeatureDuplicate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPopulateLayerList = new System.Windows.Forms.Button();
            this.lstLayer = new System.Windows.Forms.ListBox();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.lblReport = new System.Windows.Forms.Label();
            this.checkBoxSelezione = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnPopulateLayerList
            // 
            this.btnPopulateLayerList.Location = new System.Drawing.Point(9, 13);
            this.btnPopulateLayerList.Name = "btnPopulateLayerList";
            this.btnPopulateLayerList.Size = new System.Drawing.Size(277, 37);
            this.btnPopulateLayerList.TabIndex = 0;
            this.btnPopulateLayerList.Text = "Lista dei Layer";
            this.btnPopulateLayerList.UseVisualStyleBackColor = true;
            this.btnPopulateLayerList.Click += new System.EventHandler(this.btnPopulateLayerList_Click);
            // 
            // lstLayer
            // 
            this.lstLayer.FormattingEnabled = true;
            this.lstLayer.ItemHeight = 20;
            this.lstLayer.Location = new System.Drawing.Point(9, 56);
            this.lstLayer.Name = "lstLayer";
            this.lstLayer.Size = new System.Drawing.Size(277, 244);
            this.lstLayer.TabIndex = 1;
            this.lstLayer.SelectedIndexChanged += new System.EventHandler(this.lstLayer_SelectedIndexChanged);
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(9, 348);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(277, 30);
            this.btnCalculate.TabIndex = 2;
            this.btnCalculate.Text = "Trova oggetti doppioni";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // lblReport
            // 
            this.lblReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReport.Location = new System.Drawing.Point(6, 396);
            this.lblReport.Name = "lblReport";
            this.lblReport.Size = new System.Drawing.Size(280, 166);
            this.lblReport.TabIndex = 4;
            this.lblReport.Text = "Nessun oggetto doppione trovato";
            // 
            // checkBoxSelezione
            // 
            this.checkBoxSelezione.AutoSize = true;
            this.checkBoxSelezione.Location = new System.Drawing.Point(9, 318);
            this.checkBoxSelezione.Name = "checkBoxSelezione";
            this.checkBoxSelezione.Size = new System.Drawing.Size(274, 24);
            this.checkBoxSelezione.TabIndex = 5;
            this.checkBoxSelezione.Text = "Esegui l\'analisi sui solo selezionati";
            this.checkBoxSelezione.UseVisualStyleBackColor = true;
            // 
            // DockableFeatureDuplicate
            // 
            this.AutoScroll = true;
            this.Controls.Add(this.checkBoxSelezione);
            this.Controls.Add(this.lblReport);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.lstLayer);
            this.Controls.Add(this.btnPopulateLayerList);
            this.Name = "DockableFeatureDuplicate";
            this.Size = new System.Drawing.Size(317, 596);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPopulateLayerList;
        private System.Windows.Forms.ListBox lstLayer;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Label lblReport;
        private System.Windows.Forms.CheckBox checkBoxSelezione;
    }
}
