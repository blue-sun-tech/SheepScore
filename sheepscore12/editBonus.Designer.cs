namespace sheepscore12
{
    partial class editBonus
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.radioBonus = new System.Windows.Forms.RadioButton();
            this.radioOverride = new System.Windows.Forms.RadioButton();
            this.numScore = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numScore)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(100, 136);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(72, 31);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(179, 136);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 31);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // radioBonus
            // 
            this.radioBonus.AutoSize = true;
            this.radioBonus.Checked = true;
            this.radioBonus.Location = new System.Drawing.Point(12, 12);
            this.radioBonus.Name = "radioBonus";
            this.radioBonus.Size = new System.Drawing.Size(227, 21);
            this.radioBonus.TabIndex = 1;
            this.radioBonus.TabStop = true;
            this.radioBonus.Text = "Bonus/Penalty (add or subtract)";
            this.radioBonus.UseVisualStyleBackColor = true;
            // 
            // radioOverride
            // 
            this.radioOverride.AutoSize = true;
            this.radioOverride.Location = new System.Drawing.Point(12, 39);
            this.radioOverride.Name = "radioOverride";
            this.radioOverride.Size = new System.Drawing.Size(208, 21);
            this.radioOverride.TabIndex = 2;
            this.radioOverride.Text = "Override (set score equal to)";
            this.radioOverride.UseVisualStyleBackColor = true;
            // 
            // numScore
            // 
            this.numScore.DecimalPlaces = 3;
            this.numScore.Location = new System.Drawing.Point(110, 87);
            this.numScore.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numScore.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.numScore.Name = "numScore";
            this.numScore.Size = new System.Drawing.Size(141, 23);
            this.numScore.TabIndex = 3;
            this.numScore.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Score:";
            // 
            // editBonus
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 179);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numScore);
            this.Controls.Add(this.radioOverride);
            this.Controls.Add(this.radioBonus);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "editBonus";
            this.Text = "Edit Score";
            this.Shown += new System.EventHandler(this.editBonus_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.numScore)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RadioButton radioBonus;
        private System.Windows.Forms.RadioButton radioOverride;
        private System.Windows.Forms.NumericUpDown numScore;
        private System.Windows.Forms.Label label1;
    }
}