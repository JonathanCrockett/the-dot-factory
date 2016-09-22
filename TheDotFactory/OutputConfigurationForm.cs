using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheDotFactory
{
    public partial class OutputConfigurationForm : Form
    {
        // output configuration manager
        private OutputConfigurationManager m_outputConfigurationManager;

        // flag indicating whether user loaded a preset configration and then changed
        // the settings for it without saving yet
        private bool m_presetConfigurationModified = false;

        // whther or not we're currently loading a configuration
        bool m_loadingOutputConfigurationToForm = false;

        // populate the fields
        void populateControls()
        {
            // set datasources
            cbxPaddingHoriz.DataSource = Enum.GetNames(typeof(OutputConfiguration.PaddingRemoval));
            cbxPaddingVert.DataSource = Enum.GetNames(typeof(OutputConfiguration.PaddingRemoval));
            cbxCommentStyle.DataSource = Enum.GetNames(typeof(OutputConfiguration.CommentStyle));
            cbxBitLayout.DataSource = Enum.GetNames(typeof(OutputConfiguration.BitLayout));
            cbxByteFormat.DataSource = Enum.GetNames(typeof(OutputConfiguration.ByteFormat));
            cbxRotation.DataSource = OutputConfiguration.Rotation.GetNames();

            // display string arrays
            cbxCharWidthFormat.Items.AddRange(OutputConfiguration.DescriptorFormatDisplayString);
            cbxCharHeightFormat.Items.AddRange(OutputConfiguration.DescriptorFormatDisplayString);
            cbxFontHeightFormat.Items.AddRange(OutputConfiguration.DescriptorFormatDisplayString);
            cbxImgWidthFormat.Items.AddRange(OutputConfiguration.DescriptorFormatDisplayString);
            cbxImgHeightFormat.Items.AddRange(OutputConfiguration.DescriptorFormatDisplayString);

            cbxCharacterEncoding.Items.AddRange(CodePageInfo.GetEncoderNameList());

            // add leading
            cbxByteLeadingChar.Items.Add(OutputConfiguration.ByteLeadingStringBinary);
            cbxByteLeadingChar.Items.Add(OutputConfiguration.ByteLeadingStringHex);

            txtBmpVisualizerChar.Items.AddRange(OutputConfiguration.CommentVisualizerChar);

            // re-populate dropdown
            m_outputConfigurationManager.comboBoxPopulate(cbxOutputConfigurations);
        }

        // output configuration to form
        void loadOutputConfigurationToForm(OutputConfiguration outputConfig)
        {
            // set flag
            m_loadingOutputConfigurationToForm = true;
            
            // load combo boxes
            cbxPaddingHoriz.SelectedIndex = (int)outputConfig.paddingRemovalHorizontal;
            cbxPaddingVert.SelectedIndex = (int)outputConfig.paddingRemovalVertical;
            cbxCommentStyle.SelectedIndex = (int)outputConfig.commentStyle;
            cbxBitLayout.SelectedIndex = (int)outputConfig.bitLayout;
            cbxByteOrderMsbFirst.Checked = outputConfig.byteOrderMsbFirst;
            cbxByteFormat.SelectedIndex = (int)outputConfig.byteFormat;
            cbxRotation.SelectedIndex = (int)outputConfig.rotation;
            cbxCharWidthFormat.SelectedIndex = (int)outputConfig.descCharWidth;
            cbxCharHeightFormat.SelectedIndex = (int)outputConfig.descCharHeight;
            cbxFontHeightFormat.SelectedIndex = (int)outputConfig.descFontHeight;
            cbxImgWidthFormat.SelectedIndex = (int)outputConfig.descImgWidth;
            cbxImgHeightFormat.SelectedIndex = (int)outputConfig.descImgHeight;

            // text boxes
            cbxByteLeadingChar.Text = outputConfig.byteLeadingString;
            txtSpacePixels.Text = outputConfig.spaceGenerationPixels.ToString();
            txtLookupBlocksNewAfterCharCount.Text = outputConfig.lookupBlocksNewAfterCharCount.ToString();
            cbxOutputConfigurations.Text = outputConfig.displayName;
            txtVarNfFontBitmaps.Text = outputConfig.varNfBitmaps;
            txtVarNfCharInfo.Text = outputConfig.varNfCharInfo;
            txtVarNfFontInfo.Text = outputConfig.varNfFontInfo;
            txtVarNfImageBitmap.Text = outputConfig.varNfImageBitmap;
            txtVarNfImageInfo.Text = outputConfig.varNfImageInfo;

            // load check boxes
            cbxFlipHoriz.Checked = outputConfig.flipHorizontal;
            cbxFlipVert.Checked = outputConfig.flipVertical;
            cbxCommentVarName.Checked = outputConfig.addCommentVariableName;
            cbxCommentCharVisual.Checked = outputConfig.addCommentCharVisualizer;
            cbxCommentCharDesc.Checked = outputConfig.addCommentCharDescriptor;
            cbxGenerateSpaceBitmap.Checked = outputConfig.generateSpaceCharacterBitmap;
            cbxGenerateLookupArray.Checked = outputConfig.generateLookupArray;
            txtBmpVisualizerChar.Text = "" + outputConfig.bmpVisualizerChar + outputConfig.bmpVisualizerCharEmpty;
            cbxGenerateLookupBlocks.Checked = outputConfig.generateLookupBlocks;
            cbxCharacterEncoding.Text = CodePageInfo.GetCodepageName(outputConfig.CodePage);
            cbxAddCodePage.Checked = outputConfig.addCodePage;

            // radio buttons
            // -- wrap          
            rbnLineWrapAtColumn.Checked = (outputConfig.lineWrap == OutputConfiguration.LineWrap.AtColumn);
            rbnLineWrapAtBitmap.Checked = !rbnLineWrapAtColumn.Checked;

            // clear flag
            m_loadingOutputConfigurationToForm = false;
        }

        // output configuration to form
        void loadFormToOutputConfiguration(ref OutputConfiguration outputConfig)
        {
            // load combo boxes
            outputConfig.paddingRemovalHorizontal = (OutputConfiguration.PaddingRemoval)Enum.Parse(typeof(OutputConfiguration.PaddingRemoval), cbxPaddingHoriz.Text);
            outputConfig.paddingRemovalVertical = (OutputConfiguration.PaddingRemoval)Enum.Parse(typeof(OutputConfiguration.PaddingRemoval), cbxPaddingVert.Text);
            outputConfig.commentStyle = (OutputConfiguration.CommentStyle)Enum.Parse(typeof(OutputConfiguration.CommentStyle), cbxCommentStyle.Text);
            outputConfig.bitLayout = (OutputConfiguration.BitLayout)Enum.Parse(typeof(OutputConfiguration.BitLayout), cbxBitLayout.Text);
            outputConfig.byteFormat = (OutputConfiguration.ByteFormat)Enum.Parse(typeof(OutputConfiguration.ByteFormat), cbxByteFormat.Text);
            outputConfig.rotation = OutputConfiguration.Rotation.Parse(cbxRotation.Text);
            outputConfig.descCharWidth = (OutputConfiguration.DescriptorFormat)Array.IndexOf(OutputConfiguration.DescriptorFormatDisplayString, cbxCharWidthFormat.Text);
            outputConfig.descCharHeight = (OutputConfiguration.DescriptorFormat)Array.IndexOf(OutputConfiguration.DescriptorFormatDisplayString, cbxCharHeightFormat.Text);
            outputConfig.descFontHeight = (OutputConfiguration.DescriptorFormat)Array.IndexOf(OutputConfiguration.DescriptorFormatDisplayString, cbxFontHeightFormat.Text);
            outputConfig.descImgWidth = (OutputConfiguration.DescriptorFormat)Array.IndexOf(OutputConfiguration.DescriptorFormatDisplayString, cbxImgWidthFormat.Text);
            outputConfig.descImgHeight = (OutputConfiguration.DescriptorFormat)Array.IndexOf(OutputConfiguration.DescriptorFormatDisplayString, cbxImgHeightFormat.Text);

            // text boxes
            outputConfig.byteLeadingString = cbxByteLeadingChar.Text;
            outputConfig.spaceGenerationPixels = int.Parse(txtSpacePixels.Text);
            outputConfig.lookupBlocksNewAfterCharCount = int.Parse(txtLookupBlocksNewAfterCharCount.Text);
            outputConfig.varNfBitmaps = txtVarNfFontBitmaps.Text;
            outputConfig.varNfCharInfo = txtVarNfCharInfo.Text;
            outputConfig.varNfFontInfo = txtVarNfFontInfo.Text;
            outputConfig.varNfImageBitmap = txtVarNfImageBitmap.Text;
            outputConfig.varNfImageInfo = txtVarNfImageInfo.Text;

            // load check boxes
            outputConfig.flipHorizontal = cbxFlipHoriz.Checked;
            outputConfig.flipVertical = cbxFlipVert.Checked;
            outputConfig.addCommentVariableName = cbxCommentVarName.Checked;
            outputConfig.addCommentCharVisualizer = cbxCommentCharVisual.Checked;
            outputConfig.addCommentCharDescriptor = cbxCommentCharDesc.Checked;
            outputConfig.generateSpaceCharacterBitmap = cbxGenerateSpaceBitmap.Checked;
            outputConfig.generateLookupArray = cbxGenerateLookupArray.Checked;
            outputConfig.byteOrderMsbFirst = cbxByteOrderMsbFirst.Checked;

            outputConfig.bmpVisualizerChar = (txtBmpVisualizerChar.Text.Length >= 1) ? 
                    txtBmpVisualizerChar.Text[0] :
                     OutputConfiguration.CommentVisualizerCharDefault;

            outputConfig.bmpVisualizerCharEmpty = (txtBmpVisualizerChar.Text.Length >= 2) ? 
                txtBmpVisualizerChar.Text[1] :
                OutputConfiguration.CommendVisualizerCharEmptyDefault;

            outputConfig.CodePage = CodePageInfo.GetCodepage(cbxCharacterEncoding.Text);
            outputConfig.addCodePage = cbxAddCodePage.Checked;
            outputConfig.generateLookupBlocks = cbxGenerateLookupBlocks.Checked;

            // radio buttons
            // -- wrap
            if (rbnLineWrapAtColumn.Checked) outputConfig.lineWrap = OutputConfiguration.LineWrap.AtColumn;
            else outputConfig.lineWrap = OutputConfiguration.LineWrap.AtBitmap;
        }

        private void setControlTooltip(Control control, string tooltipString)
        {
            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(control, tooltipString);
        }

        public OutputConfigurationForm(ref OutputConfigurationManager outputConfigurationManager)
        {
            // set ocm
            m_outputConfigurationManager = outputConfigurationManager;

            InitializeComponent();
            populateControls();

            // set tooltips
            setControlTooltip(btnUpdateConfig, "Save updated config to preset");
            setControlTooltip(btnSaveNewConfig, "Save as new preset");
            setControlTooltip(btnDeleteConfig, "Delete preset");
        }

        // populate an output configuration
        public int getOutputConfiguration(int displayedOutputConfigurationIndex)
        {
            // load no preset
            cbxOutputConfigurations.SelectedIndex = -1;

            // can never be modifying preset when just displayed
            modifyingPresetConfigurationExit();
            
            // check if we need to display an OC
            if (displayedOutputConfigurationIndex != -1)
            {
                // get the configuration
                OutputConfiguration oc = m_outputConfigurationManager.configurationGetAtIndex(displayedOutputConfigurationIndex);

                // copy the object from the repository to the working copy
                m_outputConfigurationManager.workingOutputConfiguration = oc.clone();
            }
            else
            {
                // clear out display name so that when this is loaded into cbx it doesn't
                // select a preset
                m_outputConfigurationManager.workingOutputConfiguration.displayName = "";
            }

            // set index in cbx
            cbxOutputConfigurations.SelectedIndex = displayedOutputConfigurationIndex;
            
            // load the configuration of the working output configuration
            loadOutputConfigurationToForm(m_outputConfigurationManager.workingOutputConfiguration);

            // show self
            ShowDialog();

            // load current state of form to working output configuration
            loadFormToOutputConfiguration(ref m_outputConfigurationManager.workingOutputConfiguration);

            // are we in modifying state?
            if (!m_presetConfigurationModified)
            {
                // nope, simply return the preset index
                return cbxOutputConfigurations.SelectedIndex;
            }
            else
            {
                // user modified a preset and didn't save - switch back to no preset
                return -1;
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            // close self
            Close();
        }

        private void cbxByteFormat_TextChanged(object sender, EventArgs e)
        {
            // set leading string acccordingly
            switch((OutputConfiguration.ByteFormat)cbxByteFormat.SelectedIndex )
            {
                case OutputConfiguration.ByteFormat.Hex:
                    // set hex leading only if set to binary
                    if (cbxByteLeadingChar.Text == OutputConfiguration.ByteLeadingStringBinary)
                        cbxByteLeadingChar.Text = OutputConfiguration.ByteLeadingStringHex;
                    break;
                case OutputConfiguration.ByteFormat.Binary:
                    // set hex binary only if set to hex
                    if (cbxByteLeadingChar.Text == OutputConfiguration.ByteLeadingStringHex)
                        cbxByteLeadingChar.Text = OutputConfiguration.ByteLeadingStringBinary;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void OutputConfigurationForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSaveNewConfig_Click(object sender, EventArgs e)
        {
            // no focus
            gbxPadding.Focus();
            
            // exit modifying
            modifyingPresetConfigurationExit();
            
            // get name of new configuration
            InputBoxDialog ib = new InputBoxDialog();
                ib.FormPrompt = "Enter preset name";
                ib.FormCaption = "New preset configuration";
                ib.DefaultValue = "";

            // show the dialog
            if (ib.ShowDialog() == DialogResult.OK)
            {
                // close dialog
                ib.Close();

                // create a new output configuration
                OutputConfiguration oc = new OutputConfiguration();

                // load current form to config
                loadFormToOutputConfiguration(ref oc);

                // set display name
                oc.displayName = ib.InputResponse;

                // save new configuration to end of list
                m_outputConfigurationManager.configurationAdd(ref oc);

                // re-populate dropdown
                m_outputConfigurationManager.comboBoxPopulate(cbxOutputConfigurations);

                // set selected index
                cbxOutputConfigurations.SelectedIndex = cbxOutputConfigurations.Items.Count - 1;

                // re-save 
                m_outputConfigurationManager.saveToFile("OutputConfigs.xml");
            }
        }

        private void cbxOutputConfigurations_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check that we haven't reverted to no selection
            if (cbxOutputConfigurations.SelectedIndex != -1)
            {
                // get the configuration
                OutputConfiguration oc = m_outputConfigurationManager.configurationGetAtIndex(cbxOutputConfigurations.SelectedIndex);

                // copy the object from the repository to the working copy
                m_outputConfigurationManager.workingOutputConfiguration = oc.clone();

                // load to form
                loadOutputConfigurationToForm(m_outputConfigurationManager.workingOutputConfiguration);
            }
        }

        private void btnDeleteConfig_Click(object sender, EventArgs e)
        {
            // no focus
            gbxPadding.Focus();

            // remove current 
            m_outputConfigurationManager.configurationDelete(cbxOutputConfigurations.SelectedIndex);

            // re-populate dropdown
            m_outputConfigurationManager.comboBoxPopulate(cbxOutputConfigurations);

            // re-save 
            m_outputConfigurationManager.saveToFile("OutputConfigs.xml");

            // check if any configurations left in manager
            if (m_outputConfigurationManager.configurationCountGet() > 0)
            {
                // just get the first
                OutputConfiguration oc = m_outputConfigurationManager.configurationGetAtIndex(0);

                // to form
                loadOutputConfigurationToForm(oc);
            }
            else
            {
                // clear text
                cbxOutputConfigurations.Text = "";
            }
        }

        private void btnUpdateConfig_Click(object sender, EventArgs e)
        {
            // no focus
            gbxPadding.Focus();
            
            // exit modifying
            modifyingPresetConfigurationExit();

            // get the configuration reference at index
            OutputConfiguration updatedOutputConfiguration = m_outputConfigurationManager.configurationGetAtIndex(cbxOutputConfigurations.SelectedIndex);
            
            // load current form to the configuration
            loadFormToOutputConfiguration(ref updatedOutputConfiguration);

            // re-save 
            m_outputConfigurationManager.saveToFile("OutputConfigs.xml");
        }

        // enter modifying state
        private void modifyingPresetConfigurationEnter()
        {
            // enter modified state
            m_presetConfigurationModified = true;

            // enable edit button, to allow user to modify his changes
            btnUpdateConfig.Enabled = true;

            // update name of preset to indicate modified
            cbxOutputConfigurations.Font = new Font(cbxOutputConfigurations.Font, FontStyle.Italic);
        }

        // exit modifying state
        private void modifyingPresetConfigurationExit()
        {
            // enter modified state
            m_presetConfigurationModified = false;

            // enable edit button, to allow user to modify his changes
            btnUpdateConfig.Enabled = false;

            // update name of preset to indicate modified
            cbxOutputConfigurations.Font = new Font(cbxOutputConfigurations.Font, FontStyle.Regular);
        }

        private void onOutputConfigurationFormChange(object sender, EventArgs e)
        {
            // check if a preset is selected
            if (!m_loadingOutputConfigurationToForm && cbxOutputConfigurations.SelectedIndex != -1)
            {
                // when user has changed a preset, enter modifying state
                modifyingPresetConfigurationEnter();
            }
        }
    }
}
