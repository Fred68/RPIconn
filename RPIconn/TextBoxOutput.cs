using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPIconn
	{
	public partial class TextBoxOutput:Form
		{


		public TextBoxOutput()
			{
			InitializeComponent();
			tbInnerText.Dock = DockStyle.Fill;
			tbInnerText.Multiline = true;
			tbInnerText.ReadOnly = true;
			tbInnerText.ScrollBars = ScrollBars.Vertical;
			}

		public string InnerText
			{
			get {return tbInnerText.Text; }
			set {	
				tbInnerText.Text = value;
				tbInnerText.TabStop = false;	// Unselect text
				}
			}

		}
	}
