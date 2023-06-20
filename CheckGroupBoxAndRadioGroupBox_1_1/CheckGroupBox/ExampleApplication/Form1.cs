using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

using UIToolbox;

namespace ExampleApplication
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void checkGroupBox1_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			if(checkBox.Checked)
				Trace.WriteLine("checkGroupBox1 was checked");
			else
				Trace.WriteLine("checkGroupBox1 was unchecked");
		}
	}
}