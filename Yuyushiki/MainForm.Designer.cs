namespace Yuyushiki
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.searchPmButton = new System.Windows.Forms.Button();
            this.comPortComboBox = new System.Windows.Forms.ComboBox();
            this.vfdTestButton = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.planTextBox = new System.Windows.Forms.TextBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripNewFileButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripOpenFileButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSaveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSaveAsButton = new System.Windows.Forms.ToolStripButton();
            this.vfdGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.vfdBrightnessGroupBox = new System.Windows.Forms.GroupBox();
            this.vfdBrightnessRadioButton4 = new System.Windows.Forms.RadioButton();
            this.vfdBrightnessRadioButton3 = new System.Windows.Forms.RadioButton();
            this.vfdBrightnessRadioButton2 = new System.Windows.Forms.RadioButton();
            this.vfdBrightnessRadioButton1 = new System.Windows.Forms.RadioButton();
            this.antGroupBox = new System.Windows.Forms.GroupBox();
            this.planGroupBox = new System.Windows.Forms.GroupBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.planStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.prevButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.vfdGroupBox.SuspendLayout();
            this.vfdBrightnessGroupBox.SuspendLayout();
            this.antGroupBox.SuspendLayout();
            this.planGroupBox.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchPmButton
            // 
            this.searchPmButton.Location = new System.Drawing.Point(29, 18);
            this.searchPmButton.Name = "searchPmButton";
            this.searchPmButton.Size = new System.Drawing.Size(75, 23);
            this.searchPmButton.TabIndex = 0;
            this.searchPmButton.Text = "Search PM";
            this.searchPmButton.UseVisualStyleBackColor = true;
            this.searchPmButton.Click += new System.EventHandler(this.searchPmButton_Click);
            // 
            // comPortComboBox
            // 
            this.comPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comPortComboBox.FormattingEnabled = true;
            this.comPortComboBox.Location = new System.Drawing.Point(6, 30);
            this.comPortComboBox.Name = "comPortComboBox";
            this.comPortComboBox.Size = new System.Drawing.Size(79, 20);
            this.comPortComboBox.TabIndex = 0;
            this.comPortComboBox.SelectedIndexChanged += new System.EventHandler(this.comPortComboBox_SelectedIndexChanged);
            this.comPortComboBox.Click += new System.EventHandler(this.comPortComboBox_Click);
            // 
            // vfdTestButton
            // 
            this.vfdTestButton.Location = new System.Drawing.Point(29, 175);
            this.vfdTestButton.Name = "vfdTestButton";
            this.vfdTestButton.Size = new System.Drawing.Size(75, 23);
            this.vfdTestButton.TabIndex = 2;
            this.vfdTestButton.Text = "Test";
            this.vfdTestButton.UseVisualStyleBackColor = true;
            this.vfdTestButton.Click += new System.EventHandler(this.vfdTestButton_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileFToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(596, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileFToolStripMenuItem
            // 
            this.fileFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileFToolStripMenuItem.Name = "fileFToolStripMenuItem";
            this.fileFToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileFToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newFile_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openFile_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.save_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAs_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(151, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.aboutToolStripMenuItem.Text = "&About Yuyushiki";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // planTextBox
            // 
            this.planTextBox.AcceptsReturn = true;
            this.planTextBox.AcceptsTab = true;
            this.planTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.planTextBox.Font = new System.Drawing.Font("MS Reference Sans Serif", 11F);
            this.planTextBox.Location = new System.Drawing.Point(6, 18);
            this.planTextBox.Multiline = true;
            this.planTextBox.Name = "planTextBox";
            this.planTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.planTextBox.Size = new System.Drawing.Size(444, 316);
            this.planTextBox.TabIndex = 0;
            this.planTextBox.TextChanged += new System.EventHandler(this.planTextBox_TextChanged);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripNewFileButton,
            this.toolStripOpenFileButton,
            this.toolStripSaveButton,
            this.toolStripSaveAsButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(596, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripNewFileButton
            // 
            this.toolStripNewFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripNewFileButton.Image = global::Yuyushiki.Properties.Resources.NewFile_16x;
            this.toolStripNewFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripNewFileButton.Name = "toolStripNewFileButton";
            this.toolStripNewFileButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripNewFileButton.Text = "New Plan";
            this.toolStripNewFileButton.Click += new System.EventHandler(this.newFile_Click);
            // 
            // toolStripOpenFileButton
            // 
            this.toolStripOpenFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripOpenFileButton.Image = global::Yuyushiki.Properties.Resources.OpenFolder_16x;
            this.toolStripOpenFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpenFileButton.Name = "toolStripOpenFileButton";
            this.toolStripOpenFileButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripOpenFileButton.Text = "Open Plan File";
            this.toolStripOpenFileButton.Click += new System.EventHandler(this.openFile_Click);
            // 
            // toolStripSaveButton
            // 
            this.toolStripSaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSaveButton.Image = global::Yuyushiki.Properties.Resources.Save_16x;
            this.toolStripSaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSaveButton.Name = "toolStripSaveButton";
            this.toolStripSaveButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripSaveButton.Text = "Save Plan File";
            this.toolStripSaveButton.Click += new System.EventHandler(this.save_Click);
            // 
            // toolStripSaveAsButton
            // 
            this.toolStripSaveAsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSaveAsButton.Image = global::Yuyushiki.Properties.Resources.SaveAs_16x;
            this.toolStripSaveAsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSaveAsButton.Name = "toolStripSaveAsButton";
            this.toolStripSaveAsButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripSaveAsButton.Text = "Save As";
            this.toolStripSaveAsButton.Click += new System.EventHandler(this.saveAs_Click);
            // 
            // vfdGroupBox
            // 
            this.vfdGroupBox.Controls.Add(this.label1);
            this.vfdGroupBox.Controls.Add(this.vfdBrightnessGroupBox);
            this.vfdGroupBox.Controls.Add(this.comPortComboBox);
            this.vfdGroupBox.Controls.Add(this.vfdTestButton);
            this.vfdGroupBox.Location = new System.Drawing.Point(12, 52);
            this.vfdGroupBox.Name = "vfdGroupBox";
            this.vfdGroupBox.Size = new System.Drawing.Size(110, 206);
            this.vfdGroupBox.TabIndex = 8;
            this.vfdGroupBox.TabStop = false;
            this.vfdGroupBox.Text = "Customer Display";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "COM Port:";
            // 
            // vfdBrightnessGroupBox
            // 
            this.vfdBrightnessGroupBox.Controls.Add(this.vfdBrightnessRadioButton4);
            this.vfdBrightnessGroupBox.Controls.Add(this.vfdBrightnessRadioButton3);
            this.vfdBrightnessGroupBox.Controls.Add(this.vfdBrightnessRadioButton2);
            this.vfdBrightnessGroupBox.Controls.Add(this.vfdBrightnessRadioButton1);
            this.vfdBrightnessGroupBox.Location = new System.Drawing.Point(6, 56);
            this.vfdBrightnessGroupBox.Name = "vfdBrightnessGroupBox";
            this.vfdBrightnessGroupBox.Size = new System.Drawing.Size(79, 113);
            this.vfdBrightnessGroupBox.TabIndex = 1;
            this.vfdBrightnessGroupBox.TabStop = false;
            this.vfdBrightnessGroupBox.Text = "Brightness";
            // 
            // vfdBrightnessRadioButton4
            // 
            this.vfdBrightnessRadioButton4.AutoSize = true;
            this.vfdBrightnessRadioButton4.Location = new System.Drawing.Point(7, 88);
            this.vfdBrightnessRadioButton4.Name = "vfdBrightnessRadioButton4";
            this.vfdBrightnessRadioButton4.Size = new System.Drawing.Size(47, 16);
            this.vfdBrightnessRadioButton4.TabIndex = 3;
            this.vfdBrightnessRadioButton4.Text = "100%";
            this.vfdBrightnessRadioButton4.UseVisualStyleBackColor = true;
            // 
            // vfdBrightnessRadioButton3
            // 
            this.vfdBrightnessRadioButton3.AutoSize = true;
            this.vfdBrightnessRadioButton3.Location = new System.Drawing.Point(7, 65);
            this.vfdBrightnessRadioButton3.Name = "vfdBrightnessRadioButton3";
            this.vfdBrightnessRadioButton3.Size = new System.Drawing.Size(41, 16);
            this.vfdBrightnessRadioButton3.TabIndex = 2;
            this.vfdBrightnessRadioButton3.Text = "60%";
            this.vfdBrightnessRadioButton3.UseVisualStyleBackColor = true;
            // 
            // vfdBrightnessRadioButton2
            // 
            this.vfdBrightnessRadioButton2.AutoSize = true;
            this.vfdBrightnessRadioButton2.Location = new System.Drawing.Point(7, 42);
            this.vfdBrightnessRadioButton2.Name = "vfdBrightnessRadioButton2";
            this.vfdBrightnessRadioButton2.Size = new System.Drawing.Size(41, 16);
            this.vfdBrightnessRadioButton2.TabIndex = 1;
            this.vfdBrightnessRadioButton2.Text = "40%";
            this.vfdBrightnessRadioButton2.UseVisualStyleBackColor = true;
            // 
            // vfdBrightnessRadioButton1
            // 
            this.vfdBrightnessRadioButton1.AutoSize = true;
            this.vfdBrightnessRadioButton1.Location = new System.Drawing.Point(7, 19);
            this.vfdBrightnessRadioButton1.Name = "vfdBrightnessRadioButton1";
            this.vfdBrightnessRadioButton1.Size = new System.Drawing.Size(41, 16);
            this.vfdBrightnessRadioButton1.TabIndex = 0;
            this.vfdBrightnessRadioButton1.Text = "20%";
            this.vfdBrightnessRadioButton1.UseVisualStyleBackColor = true;
            // 
            // antGroupBox
            // 
            this.antGroupBox.Controls.Add(this.searchPmButton);
            this.antGroupBox.Location = new System.Drawing.Point(12, 264);
            this.antGroupBox.Name = "antGroupBox";
            this.antGroupBox.Size = new System.Drawing.Size(110, 50);
            this.antGroupBox.TabIndex = 9;
            this.antGroupBox.TabStop = false;
            this.antGroupBox.Text = "ANT+";
            // 
            // planGroupBox
            // 
            this.planGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.planGroupBox.Controls.Add(this.statusStrip1);
            this.planGroupBox.Controls.Add(this.prevButton);
            this.planGroupBox.Controls.Add(this.nextButton);
            this.planGroupBox.Controls.Add(this.stopButton);
            this.planGroupBox.Controls.Add(this.planTextBox);
            this.planGroupBox.Controls.Add(this.playButton);
            this.planGroupBox.Location = new System.Drawing.Point(128, 52);
            this.planGroupBox.Name = "planGroupBox";
            this.planGroupBox.Size = new System.Drawing.Size(456, 391);
            this.planGroupBox.TabIndex = 10;
            this.planGroupBox.TabStop = false;
            this.planGroupBox.Text = "Plan";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.planStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(3, 366);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(450, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // planStatusLabel
            // 
            this.planStatusLabel.Name = "planStatusLabel";
            this.planStatusLabel.Size = new System.Drawing.Size(90, 17);
            this.planStatusLabel.Text = "planStatusLabel";
            // 
            // prevButton
            // 
            this.prevButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.prevButton.Image = global::Yuyushiki.Properties.Resources.PreviousFrame_16x;
            this.prevButton.Location = new System.Drawing.Point(294, 340);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(75, 23);
            this.prevButton.TabIndex = 3;
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.Image = global::Yuyushiki.Properties.Resources.NextFrameArrow_16x;
            this.nextButton.Location = new System.Drawing.Point(375, 340);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 4;
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stopButton.Image = global::Yuyushiki.Properties.Resources.Stop_16x;
            this.stopButton.Location = new System.Drawing.Point(213, 340);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 2;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // playButton
            // 
            this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.playButton.Image = global::Yuyushiki.Properties.Resources.Run_16x;
            this.playButton.Location = new System.Drawing.Point(132, 340);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 1;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 455);
            this.Controls.Add(this.planGroupBox);
            this.Controls.Add(this.antGroupBox);
            this.Controls.Add(this.vfdGroupBox);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Yuyushiki";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.vfdGroupBox.ResumeLayout(false);
            this.vfdGroupBox.PerformLayout();
            this.vfdBrightnessGroupBox.ResumeLayout(false);
            this.vfdBrightnessGroupBox.PerformLayout();
            this.antGroupBox.ResumeLayout(false);
            this.planGroupBox.ResumeLayout(false);
            this.planGroupBox.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button searchPmButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.ComboBox comPortComboBox;
        private System.Windows.Forms.Button vfdTestButton;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripOpenFileButton;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripSaveButton;
        private System.Windows.Forms.ToolStripButton toolStripSaveAsButton;
        private System.Windows.Forms.ToolStripButton toolStripNewFileButton;
        public System.Windows.Forms.TextBox planTextBox;
        private System.Windows.Forms.GroupBox vfdGroupBox;
        private System.Windows.Forms.GroupBox vfdBrightnessGroupBox;
        private System.Windows.Forms.RadioButton vfdBrightnessRadioButton4;
        private System.Windows.Forms.RadioButton vfdBrightnessRadioButton3;
        private System.Windows.Forms.RadioButton vfdBrightnessRadioButton2;
        private System.Windows.Forms.RadioButton vfdBrightnessRadioButton1;
        private System.Windows.Forms.GroupBox antGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox planGroupBox;
        private System.Windows.Forms.Button prevButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel planStatusLabel;
    }
}

