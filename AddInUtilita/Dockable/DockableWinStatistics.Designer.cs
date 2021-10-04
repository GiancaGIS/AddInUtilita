namespace AddInUtilita
{
    partial class DockableWinStatistics
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
            this.comboBoxAttrNumerici = new System.Windows.Forms.ComboBox();
            this.lblReport = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnPopulateLayerList
            // 
            this.btnPopulateLayerList.Location = new System.Drawing.Point(9, 13);
            this.btnPopulateLayerList.Name = "btnPopulateLayerList";
            this.btnPopulateLayerList.Size = new System.Drawing.Size(277, 23);
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
            this.lstLayer.Size = new System.Drawing.Size(277, 304);
            this.lstLayer.TabIndex = 1;
            this.lstLayer.SelectedIndexChanged += new System.EventHandler(this.lstLayer_SelectedIndexChanged);
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(9, 431);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(277, 23);
            this.btnCalculate.TabIndex = 2;
            this.btnCalculate.Text = "Calcola le statistiche per il layer";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // comboBoxAttrNumerici
            // 
            this.comboBoxAttrNumerici.FormattingEnabled = true;
            this.comboBoxAttrNumerici.Location = new System.Drawing.Point(9, 389);
            this.comboBoxAttrNumerici.Name = "comboBoxAttrNumerici";
            this.comboBoxAttrNumerici.Size = new System.Drawing.Size(277, 28);
            this.comboBoxAttrNumerici.TabIndex = 3;
            // 
            // lblReport
            // 
            this.lblReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReport.Location = new System.Drawing.Point(6, 473);
            this.lblReport.Name = "lblReport";
            this.lblReport.Size = new System.Drawing.Size(280, 114);
            this.lblReport.TabIndex = 4;
            this.lblReport.Text = "Statistiche";
            // 
            // DockableWinStatistics
            // 
            this.AutoScroll = true;
            this.Controls.Add(this.lblReport);
            this.Controls.Add(this.comboBoxAttrNumerici);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.lstLayer);
            this.Controls.Add(this.btnPopulateLayerList);
            this.Name = "DockableWinStatistics";
            this.Size = new System.Drawing.Size(300, 596);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPopulateLayerList;
        private System.Windows.Forms.ListBox lstLayer;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.ComboBox comboBoxAttrNumerici;
        private System.Windows.Forms.Label lblReport;

    }
}
