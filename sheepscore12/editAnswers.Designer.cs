namespace sheepscore12
{
    partial class editAnswers
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonNewPlayer = new System.Windows.Forms.Button();
            this.buttonDeletePlayer = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonChangeName = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.numStartScore = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numStartScore)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(11, 78);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(323, 397);
            this.textBox1.TabIndex = 3;
            this.textBox1.WordWrap = false;
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(91, 15);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(243, 24);
            this.comboBox1.Sorted = true;
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoad.Location = new System.Drawing.Point(342, 15);
            this.buttonLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(118, 40);
            this.buttonLoad.TabIndex = 4;
            this.buttonLoad.Text = "Load...";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonNewPlayer
            // 
            this.buttonNewPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNewPlayer.Location = new System.Drawing.Point(342, 63);
            this.buttonNewPlayer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonNewPlayer.Name = "buttonNewPlayer";
            this.buttonNewPlayer.Size = new System.Drawing.Size(118, 40);
            this.buttonNewPlayer.TabIndex = 5;
            this.buttonNewPlayer.Text = "New Player";
            this.buttonNewPlayer.UseVisualStyleBackColor = true;
            this.buttonNewPlayer.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonDeletePlayer
            // 
            this.buttonDeletePlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeletePlayer.Location = new System.Drawing.Point(342, 159);
            this.buttonDeletePlayer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonDeletePlayer.Name = "buttonDeletePlayer";
            this.buttonDeletePlayer.Size = new System.Drawing.Size(118, 40);
            this.buttonDeletePlayer.TabIndex = 7;
            this.buttonDeletePlayer.Text = "Delete Player";
            this.buttonDeletePlayer.UseVisualStyleBackColor = true;
            this.buttonDeletePlayer.Click += new System.EventHandler(this.button3_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(342, 435);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(118, 40);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.button4_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(342, 387);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(118, 40);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "Save Changes";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.button5_Click);
            // 
            // buttonChangeName
            // 
            this.buttonChangeName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChangeName.Location = new System.Drawing.Point(342, 111);
            this.buttonChangeName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonChangeName.Name = "buttonChangeName";
            this.buttonChangeName.Size = new System.Drawing.Size(118, 40);
            this.buttonChangeName.TabIndex = 6;
            this.buttonChangeName.Text = "Change Name";
            this.buttonChangeName.UseVisualStyleBackColor = true;
            this.buttonChangeName.Click += new System.EventHandler(this.button7_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "Player:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // numStartScore
            // 
            this.numStartScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numStartScore.DecimalPlaces = 3;
            this.numStartScore.Location = new System.Drawing.Point(204, 47);
            this.numStartScore.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numStartScore.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numStartScore.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.numStartScore.Name = "numStartScore";
            this.numStartScore.Size = new System.Drawing.Size(130, 23);
            this.numStartScore.TabIndex = 2;
            this.numStartScore.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 17);
            this.label2.TabIndex = 12;
            this.label2.Text = "Starting Score:";
            // 
            // editAnswers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 488);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numStartScore);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonChangeName);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonDeletePlayer);
            this.Controls.Add(this.buttonNewPlayer);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(450, 400);
            this.Name = "editAnswers";
            this.Text = "Edit Entries";
            this.Shown += new System.EventHandler(this.editAnswers_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.numStartScore)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonNewPlayer;
        private System.Windows.Forms.Button buttonDeletePlayer;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonChangeName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numStartScore;
        private System.Windows.Forms.Label label2;
    }
}