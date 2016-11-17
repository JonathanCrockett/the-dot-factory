/*
 * Copyright 2009, Eran "Pavius" Duchan
 * This file is part of The Dot Factory.
 *
 * The Dot Factory is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License as published by the Free 
 * Software Foundation, either version 3 of the License, or (at your option) 
 * any later version. The Dot Factory is distributed in the hope that it will be 
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more 
 * details. You should have received a copy of the GNU General Public License along 
 * with The Dot Factory. If not, see http://www.gnu.org/licenses/.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TheDotFactory
{
    public partial class MainForm : Form
    {
        // current loaded bitmap
        private Bitmap m_currentLoadedBitmap = null;

        // output configuration
        public OutputConfigurationManager m_outputConfigurationManager = new OutputConfigurationManager();

        // output configuration
        private OutputConfiguration m_outputConfig;

        // contains all colors that are present in the image, colors which are value are True are handelt as background colors
        Dictionary<Color, bool> colorList = new Dictionary<Color, bool>();

        bool updateBitmapAllowed = false;

        public MainForm()
        {
            InitializeComponent();
        }

        #region event handler
        private void Form1_Load(object sender, EventArgs e)
        {
            // use double buffering
            DoubleBuffered = true;

            // set version
            Text = String.Format("The Dot Factory v.{0}", Application.ProductVersion);

            // set input box
            txtInputText.Text = Properties.Settings.Default.InputText;

            // load font
            fontDlgInputFont.Font = Properties.Settings.Default.InputFont;

            // load configurations from file
            m_outputConfigurationManager.loadFromFile("OutputConfigs.xml");

            // update the dropdown
            m_outputConfigurationManager.comboBoxPopulate(cbxOutputConfiguration);

            // get saved output config index
            int lastUsedOutputConfigurationIndex = Properties.Settings.Default.OutputConfigIndex;

            // load recently used preset
            if (lastUsedOutputConfigurationIndex >= 0 &&
                lastUsedOutputConfigurationIndex < cbxOutputConfiguration.Items.Count)
            {
                // last used
                cbxOutputConfiguration.SelectedIndex = lastUsedOutputConfigurationIndex;

                // load selected configuration
                m_outputConfig = m_outputConfigurationManager.configurationGetAtIndex(lastUsedOutputConfigurationIndex);
            }
            else
            {
                // there's no saved configuration. get the working copy (default)
                m_outputConfig = m_outputConfigurationManager.workingOutputConfiguration;
            }

            // set checkbox stuff
            populateTextInsertCheckbox();

            populateComboBoxInputImageCodepage();

            // apply font to all appropriate places
            updateSelectedFont(Properties.Settings.Default.InputFont);
        }

        // force a redraw on size changed
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Refresh();
        }

        private void btnFontSelect_Click(object sender, EventArgs e)
        {
            // set focus somewhere else
            label1.Focus();

            // open font chooser dialog
            if (fontDlgInputFont.ShowDialog(this) == DialogResult.OK)
            {
                updateSelectedFont(fontDlgInputFont.Font);
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // set focus somewhere else
            label1.Focus();

            FastColoredTextBoxNS.SyntaxDescriptor pytonSyntax = new FastColoredTextBoxNS.SyntaxDescriptor();

            pytonSyntax.leftBracket = '(';
            pytonSyntax.rightBracket = ')';

            // save default input text
            Properties.Settings.Default.InputText = txtInputText.Text;
            Properties.Settings.Default.Save();

            // will hold the resutl string
            StringBuilder textSource = new StringBuilder();
            StringBuilder textHeader = new StringBuilder();

            // check which tab is active
            if (tcInput.SelectedTab.Text == "Text")
            {
                // generate output text
                generateOutputForFont(m_outputConfig, fontDlgInputFont.Font, txtInputText.Text,  textSource, textHeader);
            }
            else if (tcInput.SelectedTab.Text == "Image")
            {
                // generate output bitmap
                if (checkGroupBoxFontImage.Checked)
                {
                    int codepage = 0;

                    TryParseCodePageString(comboBoxInputImageCodepage.Text, out codepage);

                    Size tileSize = new Size((int)numericUpDownInputImageTileSizeX.Value, (int)numericUpDownInputImageTileSizeY.Value);

                    generateOutputForFontImage(m_outputConfig, colorList, tileSize, codepage, m_currentLoadedBitmap, textSource, textHeader);
                }
                else generateOutputForImage(m_outputConfig, colorList, m_currentLoadedBitmap, textSource, textHeader);
            }
            else throw new Exception("Unknowen tabpage");

            switch(m_outputConfig.commentStyle)
            {
                case OutputConfiguration.CommentStyle.C:
                case OutputConfiguration.CommentStyle.Cpp:
                    txtOutputTextSource.Language = FastColoredTextBoxNS.Language.CSharp;
                    txtOutputTextHeader.Language = FastColoredTextBoxNS.Language.CSharp;
                    break;
                case OutputConfiguration.CommentStyle.Python:
                    txtOutputTextHeader.DescriptionFile = "pyhtonStyle.xml";
                    txtOutputTextSource.DescriptionFile = "pyhtonStyle.xml";
                    txtOutputTextSource.Language = FastColoredTextBoxNS.Language.Custom;
                    txtOutputTextHeader.Language = FastColoredTextBoxNS.Language.Custom;
                    break;
                default:
                    throw new NotImplementedException();
            }

            txtOutputTextSource.Text = textSource.ToString();
            txtOutputTextHeader.Text = textHeader.ToString();
        }

        private void btnBitmapLoad_Click(object sender, EventArgs e)
        {
            // set filter
            dlgOpenFile.Filter = string.Format("Image Files ({0})|{0}", "*.jpg; *.jpeg; *.gif; *.bmp; *.png");

            // open the dialog
            if (dlgOpenFile.ShowDialog() != DialogResult.Cancel)
            {
                if (m_currentLoadedBitmap != null) m_currentLoadedBitmap.Dispose();
                m_currentLoadedBitmap = MyExtensions.ChangePixelFormat(new Bitmap(dlgOpenFile.FileName), PixelFormat.Format32bppArgb);

                // set the path
                txtImagePath.Text = dlgOpenFile.FileName;

                // guess a name
                txtImageName.Text = Path.GetFileNameWithoutExtension(dlgOpenFile.FileName);

                //
                colorList = MyExtensions.GetColorList(m_currentLoadedBitmap).ToDictionary<Color, Color, bool>(x => x, x => false);
                colorList[colorList.ElementAt(0).Key] = true;

                if(colorList.Count > 16)
                {
                    MessageBox.Show("Convert the image into black/white in a proper image processing program, to get better results.");
                }

                dataGridViewBackgroundColor.RowCount = colorList.Count;
                dataGridViewBackgroundColor.Refresh();

                // Set picterbox background
                pbxBitmap.BackColor = GetBackColorForPicturbox();

                Size sz = DetectTileSize();

                updateBitmapAllowed = false;
                numericUpDownInputImageTilesPerLine.Value = 16;
                numericUpDownInputImageTileSizeX.Value = sz.Width;
                numericUpDownInputImageTileSizeY.Value = sz.Height;
                updateBitmapAllowed = true;

                UpdateInputImageFont(sender, e);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // close self
            Close();
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
            // no focus
            label1.Focus();
        }

        private void btnInsertText_Click(object sender, EventArgs e)
        {
            // no focus
            label1.Focus();

            // insert text
            txtInputText.Text += ((ComboBoxItem)cbxTextInsert.SelectedItem).value;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // about
            AboutForm about = new AboutForm();
            about.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            // show teh about form
            about.Show();
        }

        private void btnOutputConfig_Click(object sender, EventArgs e)
        {
            // no focus
            label1.Focus();

            // get it
            OutputConfigurationForm outputConfigForm = new OutputConfigurationForm(ref m_outputConfigurationManager);

            // get the oc
            int selectedConfigurationIndex = outputConfigForm.getOutputConfiguration(cbxOutputConfiguration.SelectedIndex);

            // update the dropdown
            m_outputConfigurationManager.comboBoxPopulate(cbxOutputConfiguration);

            // get working configuration
            m_outputConfig = m_outputConfigurationManager.workingOutputConfiguration;

            // set selected index
            cbxOutputConfiguration.SelectedIndex = selectedConfigurationIndex;
        }

        private void cbxOutputConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if any configuration selected
            if (cbxOutputConfiguration.SelectedIndex != -1)
            {
                // get the configuration
                m_outputConfig = m_outputConfigurationManager.configurationGetAtIndex(cbxOutputConfiguration.SelectedIndex);
            }

            // save selected index for next time
            Properties.Settings.Default.OutputConfigIndex = cbxOutputConfiguration.SelectedIndex;

            // save
            Properties.Settings.Default.Save();
        }

        private void tsmCopySource_Click(object sender, EventArgs e)
        {
            // copy if any text
            if (txtOutputTextSource.Text != "")
            {
                // copy
                Clipboard.SetText(txtOutputTextSource.Text);
            }
        }

        private void tsmCopyHeader_Click(object sender, EventArgs e)
        {
            // copy if any text
            if (txtOutputTextHeader.Text != "")
            {
                // copy
                Clipboard.SetText(txtOutputTextHeader.Text);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zero out file name
            dlgSaveAs.FileName = "";

            // try to prompt
            if (dlgSaveAs.ShowDialog() != DialogResult.Cancel)
            {
                // get the file name
                string moduleName = dlgSaveAs.FileName;

                // save the text
                txtOutputTextSource.SaveToFile(String.Format("{0}.c", moduleName), Encoding.UTF8);
                txtOutputTextHeader.SaveToFile(String.Format("{0}.h", moduleName), Encoding.UTF8);
            }
        }

        private void buttonImageColorInvert_Click(object sender, EventArgs e)
        {
            colorList = colorList.ToDictionary(p => p.Key, p => !p.Value);
            dataGridViewBackgroundColor.Refresh();
            updateBitmapPreview();
        }

        private void buttonImageColorAuto_Click(object sender, EventArgs e)
        {
            float avg = 0;

            if (colorList.Count <= 0) return;

            avg = colorList.Average(x => x.Key.GetBrightness());

            colorList = colorList.ToDictionary(p => p.Key, p => p.Key.GetBrightness() >= avg);

            dataGridViewBackgroundColor.Refresh();
            updateBitmapPreview();
        }

        #region dataGridViewBackgroundColor
        private void dataGridViewBackgroundColor_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == 0)
                e.Value = colorList.ElementAt(e.RowIndex).Value;
            else if (e.ColumnIndex == 1)
            {
                Color c = colorList.ElementAt(e.RowIndex).Key;
                e.Value = c;
                dataGridViewBackgroundColor[e.ColumnIndex, e.RowIndex].Style.BackColor = c;
                dataGridViewBackgroundColor[e.ColumnIndex, e.RowIndex].Style.ForeColor = (c.GetBrightness() < 0.5) ? Color.White : Color.Black;
            }
        }

        private void dataGridViewBackgroundColor_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                colorList[dataGridViewBackgroundColor[1, e.RowIndex].Style.BackColor]
                    = (bool)dataGridViewBackgroundColor[0, e.RowIndex].EditedFormattedValue;
                updateBitmapPreview();

            }
        }

        private void dataGridViewBackgroundColor_CurrentCellDirtyStateChanged_1(object sender, EventArgs e)
        {
            dataGridViewBackgroundColor.EndEdit();
        }
        #endregion
        #endregion

        #region form helper funktions
        // update input font
        private void updateSelectedFont(Font fnt)
        {
            // set text name in the text box
            txtInputFont.Text = FontDescriptor.getFontName(fnt);

            // set the font in the text box
            txtInputText.Font = (Font)fnt.Clone();

            // save into settings
            Properties.Settings.Default.InputFont = fnt;
            Properties.Settings.Default.Save();
        }

        // populate preformatted text
        private void populateTextInsertCheckbox()
        {
            string allEnglish = "", numbers = "", letters = "", uppercaseLetters = "", lowercaseLetters = "", symbols = "";

            // generate characters
            for (char character = ' '; character < 127; ++character)
            {
                // add to all
                allEnglish += character;

                // classify letter
                if (Char.IsNumber(character)) numbers += character;
                else if (Char.IsSymbol(character)) symbols += character;
                else if (Char.IsLetter(character) && Char.IsLower(character)) { letters += character; lowercaseLetters += character; }
                else if (Char.IsLetter(character) && !Char.IsLower(character)) { letters += character; uppercaseLetters += character; }
            }

            string allEuropean = allEnglish, extraPunctuations = "", extraSymbols = "", extraNumbers = "";

            for (char character = (char)129; character <= 255; ++character)
            {
                if (Char.IsLetter(character)) allEuropean += character;
                if (Char.IsPunctuation(character)) extraPunctuations += character;
                if (Char.IsSymbol(character)) extraSymbols += character;
                if (Char.IsNumber(character)) extraNumbers += character;
            }

            // add items
            cbxTextInsert.Items.Add(new ComboBoxItem("All European", allEuropean));
            cbxTextInsert.Items.Add(new ComboBoxItem("All English(ASCCI)", allEnglish));
            cbxTextInsert.Items.Add(new ComboBoxItem("Numbers (0-9)", numbers));
            cbxTextInsert.Items.Add(new ComboBoxItem("Letters (A-z)", letters));
            cbxTextInsert.Items.Add(new ComboBoxItem("Lowercase letters (a-z)", lowercaseLetters));
            cbxTextInsert.Items.Add(new ComboBoxItem("Uppercase letters (A-Z)", uppercaseLetters));
            cbxTextInsert.Items.Add(new ComboBoxItem("Extra Punctuations", extraPunctuations));
            cbxTextInsert.Items.Add(new ComboBoxItem("Extra Symbols", extraSymbols));
            cbxTextInsert.Items.Add(new ComboBoxItem("Extra Numbers", extraNumbers));

            foreach (string s in CodePageInfo.GetEncoderNameList())
            {
                if(s.ToLower() != "us-ascii"
                    //&& s.ToLower() != "utf-16"
                    )
                cbxTextInsert.Items.Add(new ComboBoxItem(s, new string(new CodePageInfo(s).GetAllValidCharacter())));
            }

            // select the first
            cbxTextInsert.SelectedIndex = 0;
        }
        
        // Gussing the best background color
        private Color GetBackColorForPicturbox()
        {
            return colorList.Keys.Aggregate<Color, List<Color>, Color>(new List<Color>(),
                    (list, c) =>
                    {
                        if (c.A == 0) list.Add(c);
                        return list;
                    }, list =>
                    {
                        if (list.Count > 0)
                        {
                            return Color.FromArgb(255, list[0]);
                        }

                        return Color.White;
                    });
        }

        private void populateComboBoxInputImageCodepage()
        {
            comboBoxInputImageCodepage.Items.Clear();
            comboBoxInputImageCodepage.Items.AddRange(Encoding.GetEncodings().Select( e => e.GetEncoding().WebName  ).ToArray());
        }

        private bool TryParseCodePageString(string s, out int codepage)
        {
            if (int.TryParse(s, out codepage)) return true;

            try
            {
                codepage = Encoding.GetEncoding(s.TrimEnd('\t')).CodePage;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private Size DetectTileSize()
        {
            if(m_currentLoadedBitmap != null)
            {
                return new Size(m_currentLoadedBitmap.Width / 16, m_currentLoadedBitmap.Height / 16);
            }
            else
            {
                throw new Exception();
            }
        }

        private void updateBitmap()
        {
            if (!updateBitmapAllowed) return;

            int w = (int)numericUpDownInputImageTileSizeX.Value;
            int h = (int)numericUpDownInputImageTileSizeY.Value;
            int tilesPerLine = (int)numericUpDownInputImageTilesPerLine.Value;
            Size sz = new Size(Math.Max(m_currentLoadedBitmap.Width, w * tilesPerLine), Math.Max(m_currentLoadedBitmap.Height, (h * ((256 / tilesPerLine) + ((256 % tilesPerLine != 0) ? 1 : 0)) )));
            Bitmap bmp = new Bitmap(sz.Width, sz.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.DrawImage(m_currentLoadedBitmap, 0,0, m_currentLoadedBitmap.Width, m_currentLoadedBitmap.Height);

            if (checkGroupBoxFontImage.Checked)
            {
                if (checkBoxInputImageOverlay.Checked)
                { 
                    Color c = Color.FromArgb(64, Color.Black);
                    Color d = Color.FromArgb(64, Color.Magenta);
                    Brush b = new SolidBrush(c);
                    Brush m = new SolidBrush(d);

                    int x, y;

                    for (y = 0, x = 0; y * h < sz.Height && y * tilesPerLine <= 256; y++)
                    {
                        for (x = 0; x < tilesPerLine && y * tilesPerLine + x <= 256; x++)
                        {
                            if ((y % 2 != 0) != (x % 2 != 0))
                            {
                                g.FillRectangle(b, x * w, y * h, w, h);
                            }
                            else
                            {
                                g.FillRectangle(m, x * w, y * h, w, h);
                            }
                        }
                    }
                }

                Point p = new Point(hScrollBarInputImageCharacterPos.Value % tilesPerLine * w, hScrollBarInputImageCharacterPos.Value / tilesPerLine * h);

                Rectangle r = new Rectangle(p, new Size(w, h));

                g.DrawRectangle(Pens.Black, r);
            }

            pbxBitmap.Image = bmp;
            pbxBitmap.Size = bmp.Size;
        }

        private void updateBitmapPreview()
        {
            if (checkGroupBoxFontImage.Checked && m_currentLoadedBitmap != null)
            {
                int w = (int)numericUpDownInputImageTileSizeX.Value;
                int h = (int)numericUpDownInputImageTileSizeY.Value;
                int tilesPerLine = (int)numericUpDownInputImageTilesPerLine.Value;

                Point p = new Point(hScrollBarInputImageCharacterPos.Value % tilesPerLine * w, hScrollBarInputImageCharacterPos.Value / tilesPerLine * h);

                Rectangle r = new Rectangle(p, new Size(w, h));
                Color c;
                if (colorList.ContainsValue(true)) c = MyExtensions.GetEnabledKeys(colorList)[0];
                else c = Color.Transparent;
                Bitmap character = MyExtensions.Clone(m_currentLoadedBitmap, r, PixelFormat.Format32bppArgb, c); ;

                Size newSize = r.Size;
                int faktor = 1;

                while (newSize.Height <= 32)
                {
                    newSize.Height *= 2;
                    newSize.Width *= 2;
                    faktor *= 2;
                }

                //convert to black white image
                character = MyExtensions.ToBitmap(MyExtensions.ToArgbArray(character).Select(argb =>
                {
                    return colorList[Color.FromArgb(argb)] ? Color.Black.ToArgb() : Color.White.ToArgb();
                }).ToArray(), r.Size);

                pictureBoxInputImageFontCharacterPreview.Image = MyExtensions.ResizeImage(character, faktor);
                pictureBoxInputImageFontCharacterPreview.Size = newSize;
            }
        }
        #endregion

        #region Image
        // generate the required output for image
        private void generateOutputForImage(OutputConfiguration outConfig, Dictionary<Color, bool> colorList, Bitmap bitmapOriginal, StringBuilder textSource, StringBuilder textHeader)
        {
            // the name of the bitmap
            string imageName;
            Color[] backgroundColors;
            BitmapInfo bi;

            textSource.Clear();
            textHeader.Clear();

            if (bitmapOriginal == null || txtImageName.Text == "")
            {
                textSource.Append("No image found ");
                return;
            }

            imageName = MyExtensions.ScrubVariableName(txtImageName.Text);
            backgroundColors = MyExtensions.GetEnabledKeys<Color>(colorList);            

            // check if bitmap is assigned
            if (m_currentLoadedBitmap != null)
            {
                //
                // Bitmap manipulation
                //
                bi = new BitmapInfo(m_outputConfig, m_currentLoadedBitmap, colorList);

                // try to manipulate the bitmap

                if (!bi.GenerateManipulatetBitmap(bi.OriginalBorder))
                {
                    // show error
                    MessageBox.Show("No blackground pixels found in bitmap",
                                    "Can't convert bitmap",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                    // stop here, failed to manipulate the bitmap for whatever reason
                    return;
                }

                // according to config
                if (m_outputConfig.addCommentVariableName)
                {
                    // add source file header
                    textSource.AppendFormat("{0}" + m_outputConfig.nl + "{1} Image data for {2}" + m_outputConfig.nl + "{3}" + m_outputConfig.nl + m_outputConfig.nl,
                                                        m_outputConfig.CommentStart, m_outputConfig.CommentBlockMiddle, imageName,
                                                        m_outputConfig.CommentBlockEnd);

                    // add header file header
                    textHeader.AppendFormat("{0}Bitmap info for {1}{2}" + m_outputConfig.nl,
                                                        m_outputConfig.CommentStart, imageName,
                                                        m_outputConfig.CommentEnd);
                }

                // bitmap varname
                string bitmapVarName = String.Format(m_outputConfig.varNfImageBitmap, imageName) + "[]";

                // add to source
                textSource.AppendFormat("{0} =" + m_outputConfig.nl + "{{" + m_outputConfig.nl, bitmapVarName);

                //
                // Bitmap to string
                //
                // page array
                bi.GeneratePageArray();

                // assign pages for fully populated 8 bits
                int pagesPerRow = MyExtensions.ConvertValueByDescriptorFormat(OutputConfiguration.DescriptorFormat.DisplayInBytes, bi.BitmapToGenerate.Width);

                // now convert to string
                bi.GenerateCharacterDataDescriptorAndVisulazer();
                textSource.Append(bi.Descriptor);

                // close
                textSource.AppendFormat("}};" + m_outputConfig.nl + m_outputConfig.nl);

                // according to config
                if (m_outputConfig.addCommentVariableName)
                {
                    // set sizes comment
                    textSource.AppendFormat("{0}Bitmap sizes for {1}{2}" + m_outputConfig.nl,
                                                        m_outputConfig.CommentStart, imageName, m_outputConfig.CommentEnd);
                }

                Func<string> getImageWidthString = () =>
                {
                    const string format = "\t{2}, {0} {3}{1}{4}";
                    // display width in bytes?
                    switch (m_outputConfig.descImgWidth)
                    {
                        case OutputConfiguration.DescriptorFormat.DisplayInBytes:
                            return string.Format(format ,
                                m_outputConfig.CommentStart,
                                m_outputConfig.CommentEnd,
                                pagesPerRow,
                                "Image width in bytes (pages)",
                                m_outputConfig.nl);
                        case OutputConfiguration.DescriptorFormat.DisplayInBits:
                            return string.Format(format,
                                m_outputConfig.CommentStart,
                                m_outputConfig.CommentEnd,
                                bi.BitmapToGenerate.Width,
                                "Image width in pixels",
                                m_outputConfig.nl);
                        case OutputConfiguration.DescriptorFormat.DontDisplay:
                            return "";
                        default:
                            throw new NotImplementedException();
                    }
                };

                Func<string> getImageHeigtString = () =>
                {
                    const string format = "\t{2}, {0} {3}{1}{4}";

                    switch (m_outputConfig.descImgHeight)
                    {
                        case OutputConfiguration.DescriptorFormat.DisplayInBytes:
                            return string.Format(format,
                                m_outputConfig.CommentStart,
                                m_outputConfig.CommentEnd,
                                MyExtensions.ConvertValueByDescriptorFormat(OutputConfiguration.DescriptorFormat.DisplayInBytes, bi.BitmapToGenerate.Height),
                                "Image height in bytes (pages)",
                                m_outputConfig.nl);
                        case OutputConfiguration.DescriptorFormat.DisplayInBits:
                            return string.Format(format,
                                m_outputConfig.CommentStart,
                                m_outputConfig.CommentEnd,
                                bi.BitmapToGenerate.Height,
                                "Image height in pixels",
                                m_outputConfig.nl);
                        case OutputConfiguration.DescriptorFormat.DontDisplay:
                            return "";
                        default:
                            throw new NotImplementedException();
                    }
                };

                // get var name
                string imageInfoVarName = String.Format(m_outputConfig.varNfImageInfo, imageName);

                // image info header
                textHeader.AppendFormat("extern {0};" + m_outputConfig.nl, imageInfoVarName);

                // image info source
                textSource.AppendFormat("{2} =" + m_outputConfig.nl + "{{" + m_outputConfig.nl +
                                                  "{3}" +
                                                  "{4}" +
                                                  "\t{5}, {0} Image bitmap array{1}" + m_outputConfig.nl +
                                                  "}};" + m_outputConfig.nl,
                                                  m_outputConfig.CommentStart,
                                                  m_outputConfig.CommentEnd,
                                                  imageInfoVarName,
                                                  getImageWidthString(),
                                                  getImageHeigtString(),
                                                  MyExtensions.GetVariableNameFromExpression(bitmapVarName));

            }
        }
        #endregion

        #region Font
        // generate the required output for text
        private static void generateOutputForFont(OutputConfiguration outConfig, Font font, string inputText, StringBuilder textSource, StringBuilder textHeader)
        {
            char[] charactersToGenerate;

            // get the characters we need to generate from the input text, removing duplicates
            charactersToGenerate = getCharactersToGenerate(inputText, outConfig.CodePage, outConfig.generateSpaceCharacterBitmap);

            // do nothing if no chars defined
            if (charactersToGenerate.Length == 0)
            {
                textSource.Clear().Append("No Characters to generate");
                textHeader.Clear();
                return;
            }

            FontDescriptor fontInfo = new FontDescriptor(outConfig, charactersToGenerate, font);

            textSource.Append(fontInfo.TextSource);
            textHeader.Append(fontInfo.TextHeader);
        }

        // get the characters we need to generate
        private static char[] getCharactersToGenerate(string inputText, int codePage, bool generateSpace)
        {
            CodePageInfo cpi = new CodePageInfo(codePage);

            // Expand and remove all ranges from the input text (look for << x - y >>
            // expand the ranges into the input text
            inputText = expandAndRemoveCharacterRanges(inputText, codePage);

            List<char> characterList = new List<char>();
            List<char> CodePageCharacterList = new List<char>(cpi.GetAllValidCharacter());

            // iterate over the characters in the textbox
            characterList = inputText.Aggregate<char, List<char>>(new List<char>(), (list, c) =>
                {
                    if (c == ' ' && !generateSpace) { } // skip - space is not encoded rather generated dynamically by the driver
                    else if (c == '\n' || c == '\r' || c == '\t') { } // dont generate newlines or tab
                    else list.Add(c);
                    return list;
                });

            // remove dublicats and sort
            // remove all charaters not includet in codepage and return
            return CodePageCharacterList                
                .Intersect(characterList)
                .Distinct()
                .OrderBy(p => cpi.GetOffsetFromCharacter(p))
                .ToArray();
        }

        // expand and remove character ranges ( look for << x - y >> )
        private static string expandAndRemoveCharacterRanges(string s, int codePage)
        {
            // create the search pattern
            //String searchPattern = @"<<.*-.*>>";
            String searchPattern = @"<<(?<rangeStart>.*?)-(?<rangeEnd>.*?)>>";

            // create the regex
            Regex regex = new Regex(searchPattern, RegexOptions.Multiline);

            // get matches
            MatchCollection regexMatches = regex.Matches(s.Replace(" ", ""));

            // holds the number of characters removed
            int charactersRemoved = 0;

            CodePageInfo cp = new CodePageInfo(codePage);

            // for each match
            foreach (Match regexMatch in regexMatches)
            {
                // get range start and end
                int rangeStart = 0, rangeEnd = 0;

                // try to parse ranges
                if (TryParseCharacterRangePoint(regexMatch.Groups["rangeStart"].Value, out rangeStart) &&
                    TryParseCharacterRangePoint(regexMatch.Groups["rangeEnd"].Value, out rangeEnd))
                {
                    // remove this from the string
                    s = s.Remove(regexMatch.Index - charactersRemoved, regexMatch.Length);

                    // save the number of chars removed so that we can fixup index (the index
                    // of the match changes as we remove characters)
                    charactersRemoved += regexMatch.Length;

                    // create a string from these values
                    for (int charIndex = rangeStart; charIndex <= rangeEnd; ++charIndex)
                    {
                        // shove this character to a encodet char container
                        char unicodeChar = cp.GetCharacterFromOffset(charIndex);

                        // add this to the string
                        s += unicodeChar;
                    }
                }
            }

            return s;
        }
       
        // try to parse character range
        private static bool TryParseCharacterRangePoint(string s, out int value)
        {
            if (s.StartsWith("0x"))
                return int.TryParse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out value);
            else
                return int.TryParse(s, out value);
        }
        #endregion

        #region fontbitmap
        private void generateOutputForFontImage(OutputConfiguration outConfig, 
            Dictionary<Color, bool> colorList, 
            Size tileSize, 
            int codepage, 
            Bitmap bitmapOriginal, 
            StringBuilder textSource,
            StringBuilder textHeader)
        {
            // the name of the bitmap
            string imageName;
            Color[] backgroundColors;
            BitmapInfo bi;

            textSource.Clear();
            textHeader.Clear();

            if (bitmapOriginal == null || txtImageName.Text == "")
            {
                textSource.Append("No image found ");
                return;
            }

            imageName = MyExtensions.ScrubVariableName(txtImageName.Text);
            backgroundColors = MyExtensions.GetEnabledKeys<Color>(colorList);

            // check if bitmap is assigned
            if (m_currentLoadedBitmap != null)
            {
                //
                // Bitmap manipulation
                //
                bi = new BitmapInfo(m_outputConfig, m_currentLoadedBitmap, colorList);

                // try to manipulate the bitmap

                if (!bi.GenerateManipulatetBitmap(bi.OriginalBorder))
                {
                    // show error
                    MessageBox.Show("No blackground pixels found in bitmap",
                                    "Can't convert bitmap",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                    // stop here, failed to manipulate the bitmap for whatever reason
                    return;
                }

                // according to config
                if (m_outputConfig.addCommentVariableName)
                {
                    // add source file header
                    textSource.AppendFormat("{0}" + m_outputConfig.nl + "{1} Image data for {2}" + m_outputConfig.nl + "{3}" + m_outputConfig.nl + m_outputConfig.nl,
                                                        m_outputConfig.CommentStart, 
                                                        m_outputConfig.CommentBlockMiddle, 
                                                        imageName,
                                                        m_outputConfig.CommentBlockEnd);

                    // add header file header
                    textHeader.AppendFormat("{0}Bitmap info for {1}{2}" + m_outputConfig.nl,
                                                        m_outputConfig.CommentStart, 
                                                        imageName,
                                                        m_outputConfig.CommentEnd);
                }

                // bitmap varname
                string bitmapVarName = String.Format(m_outputConfig.varNfImageBitmap, imageName) + "[]";

                // add to source
                textSource.AppendFormat("{0} =" + m_outputConfig.nl + "{{" + m_outputConfig.nl, bitmapVarName);

                //
                // Bitmap to string
                //
                // page array
                bi.GeneratePageArray();

                // assign pages for fully populated 8 bits
                int pagesPerRow = MyExtensions.ConvertValueByDescriptorFormat(OutputConfiguration.DescriptorFormat.DisplayInBytes, bi.BitmapToGenerate.Width);

                // now convert to string
                bi.GenerateCharacterDataDescriptorAndVisulazer();
                textSource.Append(bi.Descriptor);

                // close
                textSource.AppendFormat("}};" + m_outputConfig.nl + m_outputConfig.nl);

                // according to config
                if (m_outputConfig.addCommentVariableName)
                {
                    // set sizes comment
                    textSource.AppendFormat("{0}Bitmap sizes for {1}{2}" + m_outputConfig.nl,
                                                        m_outputConfig.CommentStart, imageName, m_outputConfig.CommentEnd);
                }

                Func<string> getImageWidthString = () =>
                {
                    const string format = "\t{2}, {0} {3}{1}{4}";
                    // display width in bytes?
                    switch (m_outputConfig.descImgWidth)
                    {
                        case OutputConfiguration.DescriptorFormat.DisplayInBytes:
                            return string.Format(format,
                                m_outputConfig.CommentStart,
                                m_outputConfig.CommentEnd,
                                pagesPerRow,
                                "Image width in bytes (pages)",
                                m_outputConfig.nl);
                        case OutputConfiguration.DescriptorFormat.DisplayInBits:
                            return string.Format(format,
                                m_outputConfig.CommentStart,
                                m_outputConfig.CommentEnd,
                                bi.BitmapToGenerate.Width,
                                "Image width in pixels",
                                m_outputConfig.nl);
                        case OutputConfiguration.DescriptorFormat.DontDisplay:
                            return "";
                        default:
                            throw new NotImplementedException();
                    }
                };

                Func<string> getImageHeigtString = () =>
                {
                    const string format = "\t{2}, {0} {3}{1}{4}";

                    switch (m_outputConfig.descImgHeight)
                    {
                        case OutputConfiguration.DescriptorFormat.DisplayInBytes:
                            return string.Format(format,
                                m_outputConfig.CommentStart,
                                m_outputConfig.CommentEnd,
                                MyExtensions.ConvertValueByDescriptorFormat(OutputConfiguration.DescriptorFormat.DisplayInBytes, bi.BitmapToGenerate.Height),
                                "Image height in bytes (pages)",
                                m_outputConfig.nl);
                        case OutputConfiguration.DescriptorFormat.DisplayInBits:
                            return string.Format(format,
                                m_outputConfig.CommentStart,
                                m_outputConfig.CommentEnd,
                                bi.BitmapToGenerate.Height,
                                "Image height in pixels",
                                m_outputConfig.nl);
                        case OutputConfiguration.DescriptorFormat.DontDisplay:
                            return "";
                        default:
                            throw new NotImplementedException();
                    }
                };

                // get var name
                string imageInfoVarName = String.Format(m_outputConfig.varNfImageInfo, imageName);

                // image info header
                textHeader.AppendFormat("extern {0};" + m_outputConfig.nl, imageInfoVarName);

                // image info source
                textSource.AppendFormat("{2} =" + m_outputConfig.nl + "{{" + m_outputConfig.nl +
                                                  "{3}" +
                                                  "{4}" +
                                                  "\t{5}, {0} Image bitmap array{1}" + m_outputConfig.nl +
                                                  "}};" + m_outputConfig.nl,
                                                  m_outputConfig.CommentStart,
                                                  m_outputConfig.CommentEnd,
                                                  imageInfoVarName,
                                                  getImageWidthString(),
                                                  getImageHeigtString(),
                                                  MyExtensions.GetVariableNameFromExpression(bitmapVarName));

            }
        }

        #endregion

        private void UpdateInputImageFont(object sender, EventArgs e)
        {
            updateBitmap();
            updateBitmapPreview();
        }

        private void pbxBitmap_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = e.Location;
            int w = (int)numericUpDownInputImageTileSizeX.Value;
            int h = (int)numericUpDownInputImageTileSizeY.Value;
            int tilesPerLine = (int)numericUpDownInputImageTilesPerLine.Value;

            p.X -= p.X % w;
            p.Y -= p.Y % h;

            hScrollBarInputImageCharacterPos.Value = p.Y * tilesPerLine / h + p.X / w;
        }

        private void hScrollBarInputImageCharacterPos_ValueChanged(object sender, EventArgs e)
        {
            UpdateInputImageFont(sender, e);
            textBoxInputImageCharacterPos.Text = hScrollBarInputImageCharacterPos.Value.ToString();
        }

        #region

        #endregion
    }
}