namespace TheDotFactory
{
    partial class OutputConfigurationForm
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbxByteOrderMsbFirst = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.cbxBitLayout = new System.Windows.Forms.ComboBox();
            this.cbxByteLeadingChar = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.cbxByteFormat = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.gbxPadding = new System.Windows.Forms.GroupBox();
            this.tbWidth = new System.Windows.Forms.TextBox();
            this.tbHeight = new System.Windows.Forms.TextBox();
            this.cbxPaddingVert = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cbxPaddingHoriz = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxFlipVert = new System.Windows.Forms.CheckBox();
            this.cbxFlipHoriz = new System.Windows.Forms.CheckBox();
            this.cbxRotation = new System.Windows.Forms.ComboBox();
            this.cbxOutputConfigurations = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnSaveNewConfig = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtBmpVisualizerChar = new System.Windows.Forms.ComboBox();
            this.cbxCommentStyle = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxCommentCharDesc = new System.Windows.Forms.CheckBox();
            this.cbxCommentCharVisual = new System.Windows.Forms.CheckBox();
            this.cbxCommentVarName = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbnLineWrapAtBitmap = new System.Windows.Forms.RadioButton();
            this.rbnLineWrapAtColumn = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.btnDeleteConfig = new System.Windows.Forms.Button();
            this.btnUpdateConfig = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtVarNfImageInfo = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtVarNfImageBitmap = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtVarNfFontInfo = new System.Windows.Forms.TextBox();
            this.txtVarNfCharInfo = new System.Windows.Forms.TextBox();
            this.txtVarNfFontBitmaps = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cbxCharacterEncoding = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cbxGenerateSpaceBitmap = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSpacePixels = new System.Windows.Forms.TextBox();
            this.cbxGenerateLookupArray = new UIToolbox.CheckGroupBox();
            this.cbxImgHeightFormat = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.cbxImgWidthFormat = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.cbxGenerateLookupBlocks = new UIToolbox.CheckGroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.txtLookupBlocksNewAfterCharCount = new System.Windows.Forms.TextBox();
            this.cbxFontHeightFormat = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxCharHeightFormat = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxCharWidthFormat = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxAddCodePage = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.gbxPadding.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.cbxGenerateLookupArray.SuspendLayout();
            this.cbxGenerateLookupBlocks.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbxByteOrderMsbFirst);
            this.groupBox3.Controls.Add(this.label24);
            this.groupBox3.Controls.Add(this.cbxBitLayout);
            this.groupBox3.Controls.Add(this.cbxByteLeadingChar);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.cbxByteFormat);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(209, 156);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(271, 113);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Byte";
            // 
            // cbxByteOrderMsbFirst
            // 
            this.cbxByteOrderMsbFirst.AutoSize = true;
            this.cbxByteOrderMsbFirst.Location = new System.Drawing.Point(70, 39);
            this.cbxByteOrderMsbFirst.Name = "cbxByteOrderMsbFirst";
            this.cbxByteOrderMsbFirst.Size = new System.Drawing.Size(74, 17);
            this.cbxByteOrderMsbFirst.TabIndex = 45;
            this.cbxByteOrderMsbFirst.Text = "MsbFirst";
            this.cbxByteOrderMsbFirst.UseVisualStyleBackColor = true;
            this.cbxByteOrderMsbFirst.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label24.Location = new System.Drawing.Point(6, 20);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(57, 13);
            this.label24.TabIndex = 44;
            this.label24.Text = "Bit Layout:";
            // 
            // cbxBitLayout
            // 
            this.cbxBitLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBitLayout.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxBitLayout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxBitLayout.FormattingEnabled = true;
            this.cbxBitLayout.Location = new System.Drawing.Point(69, 14);
            this.cbxBitLayout.Name = "cbxBitLayout";
            this.cbxBitLayout.Size = new System.Drawing.Size(112, 21);
            this.cbxBitLayout.TabIndex = 43;
            // 
            // cbxByteLeadingChar
            // 
            this.cbxByteLeadingChar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxByteLeadingChar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxByteLeadingChar.FormattingEnabled = true;
            this.cbxByteLeadingChar.Location = new System.Drawing.Point(69, 86);
            this.cbxByteLeadingChar.Name = "cbxByteLeadingChar";
            this.cbxByteLeadingChar.Size = new System.Drawing.Size(75, 21);
            this.cbxByteLeadingChar.TabIndex = 42;
            this.cbxByteLeadingChar.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label20.Location = new System.Drawing.Point(15, 90);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(48, 13);
            this.label20.TabIndex = 26;
            this.label20.Text = "Leading:";
            // 
            // cbxByteFormat
            // 
            this.cbxByteFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxByteFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxByteFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxByteFormat.FormattingEnabled = true;
            this.cbxByteFormat.Location = new System.Drawing.Point(69, 62);
            this.cbxByteFormat.Name = "cbxByteFormat";
            this.cbxByteFormat.Size = new System.Drawing.Size(112, 21);
            this.cbxByteFormat.TabIndex = 24;
            this.cbxByteFormat.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            this.cbxByteFormat.TextChanged += new System.EventHandler(this.cbxByteFormat_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label18.Location = new System.Drawing.Point(21, 66);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(42, 13);
            this.label18.TabIndex = 23;
            this.label18.Text = "Format:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label19.Location = new System.Drawing.Point(27, 42);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(36, 13);
            this.label19.TabIndex = 21;
            this.label19.Text = "Order:";
            // 
            // gbxPadding
            // 
            this.gbxPadding.Controls.Add(this.tbWidth);
            this.gbxPadding.Controls.Add(this.tbHeight);
            this.gbxPadding.Controls.Add(this.cbxPaddingVert);
            this.gbxPadding.Controls.Add(this.label17);
            this.gbxPadding.Controls.Add(this.cbxPaddingHoriz);
            this.gbxPadding.Controls.Add(this.label16);
            this.gbxPadding.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbxPadding.Location = new System.Drawing.Point(125, 58);
            this.gbxPadding.Name = "gbxPadding";
            this.gbxPadding.Size = new System.Drawing.Size(237, 92);
            this.gbxPadding.TabIndex = 21;
            this.gbxPadding.TabStop = false;
            this.gbxPadding.Text = "Padding Removal";
            // 
            // tbWidth
            // 
            this.tbWidth.Location = new System.Drawing.Point(182, 54);
            this.tbWidth.Name = "tbWidth";
            this.tbWidth.Size = new System.Drawing.Size(46, 21);
            this.tbWidth.TabIndex = 26;
            this.tbWidth.Text = "16";
            this.tbWidth.TextChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(182, 28);
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.Size = new System.Drawing.Size(46, 21);
            this.tbHeight.TabIndex = 25;
            this.tbHeight.Text = "16";
            this.tbHeight.TextChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxPaddingVert
            // 
            this.cbxPaddingVert.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPaddingVert.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxPaddingVert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxPaddingVert.FormattingEnabled = true;
            this.cbxPaddingVert.Location = new System.Drawing.Point(73, 54);
            this.cbxPaddingVert.Name = "cbxPaddingVert";
            this.cbxPaddingVert.Size = new System.Drawing.Size(103, 21);
            this.cbxPaddingVert.TabIndex = 24;
            this.cbxPaddingVert.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label17.Location = new System.Drawing.Point(16, 57);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(54, 13);
            this.label17.TabIndex = 23;
            this.label17.Text = "Width (X):";
            // 
            // cbxPaddingHoriz
            // 
            this.cbxPaddingHoriz.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPaddingHoriz.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxPaddingHoriz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxPaddingHoriz.FormattingEnabled = true;
            this.cbxPaddingHoriz.Location = new System.Drawing.Point(73, 28);
            this.cbxPaddingHoriz.Name = "cbxPaddingHoriz";
            this.cbxPaddingHoriz.Size = new System.Drawing.Size(103, 21);
            this.cbxPaddingHoriz.TabIndex = 22;
            this.cbxPaddingHoriz.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label16.Location = new System.Drawing.Point(13, 31);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(57, 13);
            this.label16.TabIndex = 21;
            this.label16.Text = "Height (Y):";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxFlipVert);
            this.groupBox1.Controls.Add(this.cbxFlipHoriz);
            this.groupBox1.Controls.Add(this.cbxRotation);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(17, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(101, 92);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Flip/ Rotate";
            // 
            // cbxFlipVert
            // 
            this.cbxFlipVert.AutoSize = true;
            this.cbxFlipVert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxFlipVert.Location = new System.Drawing.Point(15, 41);
            this.cbxFlipVert.Name = "cbxFlipVert";
            this.cbxFlipVert.Size = new System.Drawing.Size(52, 17);
            this.cbxFlipVert.TabIndex = 28;
            this.cbxFlipVert.Text = "Flip Y";
            this.cbxFlipVert.UseVisualStyleBackColor = true;
            this.cbxFlipVert.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxFlipHoriz
            // 
            this.cbxFlipHoriz.AutoSize = true;
            this.cbxFlipHoriz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxFlipHoriz.Location = new System.Drawing.Point(15, 23);
            this.cbxFlipHoriz.Name = "cbxFlipHoriz";
            this.cbxFlipHoriz.Size = new System.Drawing.Size(52, 17);
            this.cbxFlipHoriz.TabIndex = 27;
            this.cbxFlipHoriz.Text = "Flip X";
            this.cbxFlipHoriz.UseVisualStyleBackColor = true;
            this.cbxFlipHoriz.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxRotation
            // 
            this.cbxRotation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxRotation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxRotation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxRotation.FormattingEnabled = true;
            this.cbxRotation.Location = new System.Drawing.Point(15, 62);
            this.cbxRotation.Name = "cbxRotation";
            this.cbxRotation.Size = new System.Drawing.Size(59, 21);
            this.cbxRotation.TabIndex = 23;
            this.cbxRotation.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxOutputConfigurations
            // 
            this.cbxOutputConfigurations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxOutputConfigurations.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxOutputConfigurations.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxOutputConfigurations.FormattingEnabled = true;
            this.cbxOutputConfigurations.Location = new System.Drawing.Point(82, 12);
            this.cbxOutputConfigurations.Name = "cbxOutputConfigurations";
            this.cbxOutputConfigurations.Size = new System.Drawing.Size(497, 21);
            this.cbxOutputConfigurations.TabIndex = 36;
            this.cbxOutputConfigurations.SelectedIndexChanged += new System.EventHandler(this.cbxOutputConfigurations_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(40, 15);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 13);
            this.label15.TabIndex = 35;
            this.label15.Text = "Preset:";
            // 
            // btnSaveNewConfig
            // 
            this.btnSaveNewConfig.FlatAppearance.BorderSize = 0;
            this.btnSaveNewConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveNewConfig.Image = global::TheDotFactory.Properties.Resources.database_add;
            this.btnSaveNewConfig.Location = new System.Drawing.Point(617, 10);
            this.btnSaveNewConfig.Name = "btnSaveNewConfig";
            this.btnSaveNewConfig.Size = new System.Drawing.Size(26, 23);
            this.btnSaveNewConfig.TabIndex = 37;
            this.btnSaveNewConfig.UseVisualStyleBackColor = true;
            this.btnSaveNewConfig.Click += new System.EventHandler(this.btnSaveNewConfig_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtBmpVisualizerChar);
            this.groupBox4.Controls.Add(this.cbxCommentStyle);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.cbxCommentCharDesc);
            this.groupBox4.Controls.Add(this.cbxCommentCharVisual);
            this.groupBox4.Controls.Add(this.cbxCommentVarName);
            this.groupBox4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(17, 156);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(186, 113);
            this.groupBox4.TabIndex = 39;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Comments";
            // 
            // txtBmpVisualizerChar
            // 
            this.txtBmpVisualizerChar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.txtBmpVisualizerChar.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBmpVisualizerChar.FormattingEnabled = true;
            this.txtBmpVisualizerChar.Location = new System.Drawing.Point(117, 36);
            this.txtBmpVisualizerChar.Name = "txtBmpVisualizerChar";
            this.txtBmpVisualizerChar.Size = new System.Drawing.Size(47, 21);
            this.txtBmpVisualizerChar.TabIndex = 44;
            this.txtBmpVisualizerChar.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxCommentStyle
            // 
            this.cbxCommentStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCommentStyle.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxCommentStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxCommentStyle.FormattingEnabled = true;
            this.cbxCommentStyle.Location = new System.Drawing.Point(49, 80);
            this.cbxCommentStyle.Name = "cbxCommentStyle";
            this.cbxCommentStyle.Size = new System.Drawing.Size(89, 21);
            this.cbxCommentStyle.TabIndex = 43;
            this.cbxCommentStyle.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(9, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Style:";
            // 
            // cbxCommentCharDesc
            // 
            this.cbxCommentCharDesc.AutoSize = true;
            this.cbxCommentCharDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxCommentCharDesc.Location = new System.Drawing.Point(14, 56);
            this.cbxCommentCharDesc.Name = "cbxCommentCharDesc";
            this.cbxCommentCharDesc.Size = new System.Drawing.Size(97, 17);
            this.cbxCommentCharDesc.TabIndex = 40;
            this.cbxCommentCharDesc.Text = "Char descriptor";
            this.cbxCommentCharDesc.UseVisualStyleBackColor = true;
            this.cbxCommentCharDesc.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxCommentCharVisual
            // 
            this.cbxCommentCharVisual.AutoSize = true;
            this.cbxCommentCharVisual.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxCommentCharVisual.Location = new System.Drawing.Point(14, 38);
            this.cbxCommentCharVisual.Name = "cbxCommentCharVisual";
            this.cbxCommentCharVisual.Size = new System.Drawing.Size(107, 17);
            this.cbxCommentCharVisual.TabIndex = 26;
            this.cbxCommentCharVisual.Text = "Bitmap visualizer:";
            this.cbxCommentCharVisual.UseVisualStyleBackColor = true;
            this.cbxCommentCharVisual.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxCommentVarName
            // 
            this.cbxCommentVarName.AutoSize = true;
            this.cbxCommentVarName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxCommentVarName.Location = new System.Drawing.Point(14, 20);
            this.cbxCommentVarName.Name = "cbxCommentVarName";
            this.cbxCommentVarName.Size = new System.Drawing.Size(93, 17);
            this.cbxCommentVarName.TabIndex = 25;
            this.cbxCommentVarName.Text = "Variable name";
            this.cbxCommentVarName.UseVisualStyleBackColor = true;
            this.cbxCommentVarName.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // btnApply
            // 
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnApply.Image = global::TheDotFactory.Properties.Resources.accept;
            this.btnApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnApply.Location = new System.Drawing.Point(616, 439);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(64, 23);
            this.btnApply.TabIndex = 40;
            this.btnApply.Text = "Apply ";
            this.btnApply.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rbnLineWrapAtBitmap);
            this.groupBox5.Controls.Add(this.rbnLineWrapAtColumn);
            this.groupBox5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(368, 58);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(112, 92);
            this.groupBox5.TabIndex = 41;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Line wrap";
            // 
            // rbnLineWrapAtBitmap
            // 
            this.rbnLineWrapAtBitmap.AutoSize = true;
            this.rbnLineWrapAtBitmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.rbnLineWrapAtBitmap.Location = new System.Drawing.Point(14, 53);
            this.rbnLineWrapAtBitmap.Name = "rbnLineWrapAtBitmap";
            this.rbnLineWrapAtBitmap.Size = new System.Drawing.Size(69, 17);
            this.rbnLineWrapAtBitmap.TabIndex = 1;
            this.rbnLineWrapAtBitmap.Text = "At bitmap";
            this.rbnLineWrapAtBitmap.UseVisualStyleBackColor = true;
            this.rbnLineWrapAtBitmap.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // rbnLineWrapAtColumn
            // 
            this.rbnLineWrapAtColumn.AutoSize = true;
            this.rbnLineWrapAtColumn.Checked = true;
            this.rbnLineWrapAtColumn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.rbnLineWrapAtColumn.Location = new System.Drawing.Point(14, 35);
            this.rbnLineWrapAtColumn.Name = "rbnLineWrapAtColumn";
            this.rbnLineWrapAtColumn.Size = new System.Drawing.Size(72, 17);
            this.rbnLineWrapAtColumn.TabIndex = 0;
            this.rbnLineWrapAtColumn.TabStop = true;
            this.rbnLineWrapAtColumn.Text = "At column";
            this.rbnLineWrapAtColumn.UseVisualStyleBackColor = true;
            this.rbnLineWrapAtColumn.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(18, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(661, 2);
            this.label4.TabIndex = 42;
            // 
            // btnDeleteConfig
            // 
            this.btnDeleteConfig.FlatAppearance.BorderSize = 0;
            this.btnDeleteConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteConfig.Image = global::TheDotFactory.Properties.Resources.database_delete;
            this.btnDeleteConfig.Location = new System.Drawing.Point(649, 10);
            this.btnDeleteConfig.Name = "btnDeleteConfig";
            this.btnDeleteConfig.Size = new System.Drawing.Size(26, 23);
            this.btnDeleteConfig.TabIndex = 45;
            this.btnDeleteConfig.UseVisualStyleBackColor = true;
            this.btnDeleteConfig.Click += new System.EventHandler(this.btnDeleteConfig_Click);
            // 
            // btnUpdateConfig
            // 
            this.btnUpdateConfig.Enabled = false;
            this.btnUpdateConfig.FlatAppearance.BorderSize = 0;
            this.btnUpdateConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateConfig.Image = global::TheDotFactory.Properties.Resources.database_edit;
            this.btnUpdateConfig.Location = new System.Drawing.Point(585, 10);
            this.btnUpdateConfig.Name = "btnUpdateConfig";
            this.btnUpdateConfig.Size = new System.Drawing.Size(26, 23);
            this.btnUpdateConfig.TabIndex = 46;
            this.btnUpdateConfig.UseVisualStyleBackColor = true;
            this.btnUpdateConfig.Click += new System.EventHandler(this.btnUpdateConfig_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtVarNfImageInfo);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtVarNfImageBitmap);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtVarNfFontInfo);
            this.groupBox2.Controls.Add(this.txtVarNfCharInfo);
            this.groupBox2.Controls.Add(this.txtVarNfFontBitmaps);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(18, 275);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(462, 162);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Variable name format where {0} is the font/image name";
            // 
            // txtVarNfImageInfo
            // 
            this.txtVarNfImageInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVarNfImageInfo.Location = new System.Drawing.Point(93, 129);
            this.txtVarNfImageInfo.Name = "txtVarNfImageInfo";
            this.txtVarNfImageInfo.Size = new System.Drawing.Size(310, 20);
            this.txtVarNfImageInfo.TabIndex = 55;
            this.txtVarNfImageInfo.TextChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label12.Location = new System.Drawing.Point(27, 132);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 13);
            this.label12.TabIndex = 54;
            this.label12.Text = "Image info:";
            // 
            // txtVarNfImageBitmap
            // 
            this.txtVarNfImageBitmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVarNfImageBitmap.Location = new System.Drawing.Point(93, 103);
            this.txtVarNfImageBitmap.Name = "txtVarNfImageBitmap";
            this.txtVarNfImageBitmap.Size = new System.Drawing.Size(310, 20);
            this.txtVarNfImageBitmap.TabIndex = 53;
            this.txtVarNfImageBitmap.TextChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label11.Location = new System.Drawing.Point(13, 106);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(73, 13);
            this.label11.TabIndex = 52;
            this.label11.Text = "Image bitmap:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label10.Location = new System.Drawing.Point(92, 158);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(0, 13);
            this.label10.TabIndex = 51;
            // 
            // txtVarNfFontInfo
            // 
            this.txtVarNfFontInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVarNfFontInfo.Location = new System.Drawing.Point(93, 77);
            this.txtVarNfFontInfo.Name = "txtVarNfFontInfo";
            this.txtVarNfFontInfo.Size = new System.Drawing.Size(310, 20);
            this.txtVarNfFontInfo.TabIndex = 50;
            this.txtVarNfFontInfo.TextChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // txtVarNfCharInfo
            // 
            this.txtVarNfCharInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtVarNfCharInfo.Location = new System.Drawing.Point(93, 52);
            this.txtVarNfCharInfo.Name = "txtVarNfCharInfo";
            this.txtVarNfCharInfo.Size = new System.Drawing.Size(310, 20);
            this.txtVarNfCharInfo.TabIndex = 49;
            this.txtVarNfCharInfo.TextChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // txtVarNfFontBitmaps
            // 
            this.txtVarNfFontBitmaps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVarNfFontBitmaps.Location = new System.Drawing.Point(93, 28);
            this.txtVarNfFontBitmaps.Name = "txtVarNfFontBitmaps";
            this.txtVarNfFontBitmaps.Size = new System.Drawing.Size(310, 20);
            this.txtVarNfFontBitmaps.TabIndex = 48;
            this.txtVarNfFontBitmaps.TextChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label7.Location = new System.Drawing.Point(35, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 47;
            this.label7.Text = "Font info:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label8.Location = new System.Drawing.Point(10, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 13);
            this.label8.TabIndex = 45;
            this.label8.Text = "Character info:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label9.Location = new System.Drawing.Point(16, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 43;
            this.label9.Text = "Font bitmaps:";
            // 
            // cbxCharacterEncoding
            // 
            this.cbxCharacterEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCharacterEncoding.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxCharacterEncoding.FormattingEnabled = true;
            this.cbxCharacterEncoding.Location = new System.Drawing.Point(600, 377);
            this.cbxCharacterEncoding.Name = "cbxCharacterEncoding";
            this.cbxCharacterEncoding.Size = new System.Drawing.Size(92, 21);
            this.cbxCharacterEncoding.TabIndex = 51;
            this.cbxCharacterEncoding.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(486, 385);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(100, 13);
            this.label14.TabIndex = 52;
            this.label14.Text = "Character encoding";
            // 
            // cbxGenerateSpaceBitmap
            // 
            this.cbxGenerateSpaceBitmap.AutoSize = true;
            this.cbxGenerateSpaceBitmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxGenerateSpaceBitmap.Location = new System.Drawing.Point(489, 313);
            this.cbxGenerateSpaceBitmap.Name = "cbxGenerateSpaceBitmap";
            this.cbxGenerateSpaceBitmap.Size = new System.Drawing.Size(136, 17);
            this.cbxGenerateSpaceBitmap.TabIndex = 50;
            this.cbxGenerateSpaceBitmap.TabStop = false;
            this.cbxGenerateSpaceBitmap.Text = "Generate space bitmap";
            this.cbxGenerateSpaceBitmap.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label6.Location = new System.Drawing.Point(539, 339);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 49;
            this.label6.Text = "pixels for space char";
            // 
            // txtSpacePixels
            // 
            this.txtSpacePixels.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSpacePixels.Location = new System.Drawing.Point(489, 336);
            this.txtSpacePixels.Name = "txtSpacePixels";
            this.txtSpacePixels.Size = new System.Drawing.Size(36, 21);
            this.txtSpacePixels.TabIndex = 48;
            this.txtSpacePixels.Text = "2";
            this.txtSpacePixels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSpacePixels.TextChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxGenerateLookupArray
            // 
            this.cbxGenerateLookupArray.AutoSize = true;
            this.cbxGenerateLookupArray.Controls.Add(this.cbxImgHeightFormat);
            this.cbxGenerateLookupArray.Controls.Add(this.label22);
            this.cbxGenerateLookupArray.Controls.Add(this.cbxImgWidthFormat);
            this.cbxGenerateLookupArray.Controls.Add(this.label23);
            this.cbxGenerateLookupArray.Controls.Add(this.cbxGenerateLookupBlocks);
            this.cbxGenerateLookupArray.Controls.Add(this.cbxFontHeightFormat);
            this.cbxGenerateLookupArray.Controls.Add(this.label2);
            this.cbxGenerateLookupArray.Controls.Add(this.cbxCharHeightFormat);
            this.cbxGenerateLookupArray.Controls.Add(this.label3);
            this.cbxGenerateLookupArray.Controls.Add(this.cbxCharWidthFormat);
            this.cbxGenerateLookupArray.Controls.Add(this.label5);
            this.cbxGenerateLookupArray.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxGenerateLookupArray.Location = new System.Drawing.Point(489, 58);
            this.cbxGenerateLookupArray.Name = "cbxGenerateLookupArray";
            this.cbxGenerateLookupArray.Size = new System.Drawing.Size(237, 249);
            this.cbxGenerateLookupArray.TabIndex = 43;
            this.cbxGenerateLookupArray.TabStop = false;
            this.cbxGenerateLookupArray.Text = "Generate descriptors lookup array";
            this.cbxGenerateLookupArray.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxImgHeightFormat
            // 
            this.cbxImgHeightFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxImgHeightFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxImgHeightFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxImgHeightFormat.FormattingEnabled = true;
            this.cbxImgHeightFormat.Location = new System.Drawing.Point(91, 208);
            this.cbxImgHeightFormat.Name = "cbxImgHeightFormat";
            this.cbxImgHeightFormat.Size = new System.Drawing.Size(112, 21);
            this.cbxImgHeightFormat.TabIndex = 64;
            this.cbxImgHeightFormat.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label22.Location = new System.Drawing.Point(12, 211);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(71, 13);
            this.label22.TabIndex = 63;
            this.label22.Text = "Image height:";
            // 
            // cbxImgWidthFormat
            // 
            this.cbxImgWidthFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxImgWidthFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxImgWidthFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxImgWidthFormat.FormattingEnabled = true;
            this.cbxImgWidthFormat.Location = new System.Drawing.Point(91, 181);
            this.cbxImgWidthFormat.Name = "cbxImgWidthFormat";
            this.cbxImgWidthFormat.Size = new System.Drawing.Size(112, 21);
            this.cbxImgWidthFormat.TabIndex = 62;
            this.cbxImgWidthFormat.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label23.Location = new System.Drawing.Point(16, 186);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(67, 13);
            this.label23.TabIndex = 61;
            this.label23.Text = "Image width:";
            // 
            // cbxGenerateLookupBlocks
            // 
            this.cbxGenerateLookupBlocks.AutoSize = true;
            this.cbxGenerateLookupBlocks.Controls.Add(this.label13);
            this.cbxGenerateLookupBlocks.Controls.Add(this.label21);
            this.cbxGenerateLookupBlocks.Controls.Add(this.txtLookupBlocksNewAfterCharCount);
            this.cbxGenerateLookupBlocks.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxGenerateLookupBlocks.Location = new System.Drawing.Point(12, 99);
            this.cbxGenerateLookupBlocks.Name = "cbxGenerateLookupBlocks";
            this.cbxGenerateLookupBlocks.Size = new System.Drawing.Size(209, 77);
            this.cbxGenerateLookupBlocks.TabIndex = 53;
            this.cbxGenerateLookupBlocks.TabStop = false;
            this.cbxGenerateLookupBlocks.Text = "Multiple descriptor arrays";
            this.cbxGenerateLookupBlocks.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label13.Location = new System.Drawing.Point(6, 17);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(197, 13);
            this.label13.TabIndex = 59;
            this.label13.Text = "Create new descriptor array when space";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label21.Location = new System.Drawing.Point(6, 34);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(144, 13);
            this.label21.TabIndex = 60;
            this.label21.Text = "between characters exceeds";
            // 
            // txtLookupBlocksNewAfterCharCount
            // 
            this.txtLookupBlocksNewAfterCharCount.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLookupBlocksNewAfterCharCount.Location = new System.Drawing.Point(156, 36);
            this.txtLookupBlocksNewAfterCharCount.Name = "txtLookupBlocksNewAfterCharCount";
            this.txtLookupBlocksNewAfterCharCount.Size = new System.Drawing.Size(36, 21);
            this.txtLookupBlocksNewAfterCharCount.TabIndex = 58;
            this.txtLookupBlocksNewAfterCharCount.Text = "80";
            this.txtLookupBlocksNewAfterCharCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtLookupBlocksNewAfterCharCount.TextChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // cbxFontHeightFormat
            // 
            this.cbxFontHeightFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFontHeightFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxFontHeightFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxFontHeightFormat.FormattingEnabled = true;
            this.cbxFontHeightFormat.Location = new System.Drawing.Point(91, 72);
            this.cbxFontHeightFormat.Name = "cbxFontHeightFormat";
            this.cbxFontHeightFormat.Size = new System.Drawing.Size(112, 21);
            this.cbxFontHeightFormat.TabIndex = 48;
            this.cbxFontHeightFormat.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label2.Location = new System.Drawing.Point(22, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 47;
            this.label2.Text = "Font height:";
            // 
            // cbxCharHeightFormat
            // 
            this.cbxCharHeightFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCharHeightFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxCharHeightFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxCharHeightFormat.FormattingEnabled = true;
            this.cbxCharHeightFormat.Location = new System.Drawing.Point(91, 46);
            this.cbxCharHeightFormat.Name = "cbxCharHeightFormat";
            this.cbxCharHeightFormat.Size = new System.Drawing.Size(112, 21);
            this.cbxCharHeightFormat.TabIndex = 46;
            this.cbxCharHeightFormat.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label3.Location = new System.Drawing.Point(19, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "Char height:";
            // 
            // cbxCharWidthFormat
            // 
            this.cbxCharWidthFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCharWidthFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbxCharWidthFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cbxCharWidthFormat.FormattingEnabled = true;
            this.cbxCharWidthFormat.Location = new System.Drawing.Point(91, 20);
            this.cbxCharWidthFormat.Name = "cbxCharWidthFormat";
            this.cbxCharWidthFormat.Size = new System.Drawing.Size(112, 21);
            this.cbxCharWidthFormat.TabIndex = 44;
            this.cbxCharWidthFormat.SelectedIndexChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label5.Location = new System.Drawing.Point(23, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 43;
            this.label5.Text = "Char width:";
            // 
            // cbxAddCodePage
            // 
            this.cbxAddCodePage.AutoSize = true;
            this.cbxAddCodePage.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxAddCodePage.Location = new System.Drawing.Point(489, 407);
            this.cbxAddCodePage.Name = "cbxAddCodePage";
            this.cbxAddCodePage.Size = new System.Drawing.Size(197, 17);
            this.cbxAddCodePage.TabIndex = 65;
            this.cbxAddCodePage.Text = "Add codepage identifier to font info";
            this.cbxAddCodePage.UseVisualStyleBackColor = true;
            this.cbxAddCodePage.CheckedChanged += new System.EventHandler(this.onOutputConfigurationFormChange);
            // 
            // OutputConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 474);
            this.Controls.Add(this.cbxAddCodePage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtSpacePixels);
            this.Controls.Add(this.cbxCharacterEncoding);
            this.Controls.Add(this.cbxGenerateSpaceBitmap);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnUpdateConfig);
            this.Controls.Add(this.btnDeleteConfig);
            this.Controls.Add(this.cbxGenerateLookupArray);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnSaveNewConfig);
            this.Controls.Add(this.cbxOutputConfigurations);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.gbxPadding);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "OutputConfigurationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modify Output Configuration";
            this.Load += new System.EventHandler(this.OutputConfigurationForm_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbxPadding.ResumeLayout(false);
            this.gbxPadding.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.cbxGenerateLookupArray.ResumeLayout(false);
            this.cbxGenerateLookupArray.PerformLayout();
            this.cbxGenerateLookupBlocks.ResumeLayout(false);
            this.cbxGenerateLookupBlocks.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cbxByteFormat;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox gbxPadding;
        private System.Windows.Forms.ComboBox cbxPaddingVert;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox cbxPaddingHoriz;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbxOutputConfigurations;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnSaveNewConfig;
        private System.Windows.Forms.ComboBox cbxByteLeadingChar;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cbxCommentStyle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbxCommentCharDesc;
        private System.Windows.Forms.CheckBox cbxCommentCharVisual;
        private System.Windows.Forms.CheckBox cbxCommentVarName;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rbnLineWrapAtBitmap;
        private System.Windows.Forms.RadioButton rbnLineWrapAtColumn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbxRotation;
        private System.Windows.Forms.CheckBox cbxFlipVert;
        private System.Windows.Forms.CheckBox cbxFlipHoriz;
        private UIToolbox.CheckGroupBox cbxGenerateLookupArray;
        private System.Windows.Forms.ComboBox cbxFontHeightFormat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbxCharHeightFormat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbxCharWidthFormat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSpacePixels;
        private System.Windows.Forms.CheckBox cbxGenerateSpaceBitmap;
        private System.Windows.Forms.Button btnDeleteConfig;
        private System.Windows.Forms.Button btnUpdateConfig;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtVarNfFontInfo;
        private System.Windows.Forms.TextBox txtVarNfCharInfo;
        private System.Windows.Forms.TextBox txtVarNfFontBitmaps;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtVarNfImageBitmap;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtVarNfImageInfo;
        private System.Windows.Forms.Label label12;
        private UIToolbox.CheckGroupBox cbxGenerateLookupBlocks;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtLookupBlocksNewAfterCharCount;
        private System.Windows.Forms.ComboBox cbxImgHeightFormat;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox cbxImgWidthFormat;
        private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.ComboBox cbxBitLayout;
        private System.Windows.Forms.ComboBox txtBmpVisualizerChar;
        private System.Windows.Forms.ComboBox cbxCharacterEncoding;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox cbxAddCodePage;
        private System.Windows.Forms.CheckBox cbxByteOrderMsbFirst;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tbWidth;
        private System.Windows.Forms.TextBox tbHeight;
    }
}