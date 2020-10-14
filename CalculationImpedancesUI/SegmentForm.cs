﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalculationImpedancesApp;

namespace CalculationImpedancesUI
{
	public partial class SegmentForm : Form
	{
		//TODO: RSDN? (+)
		/// <summary>
		/// New circuit segment.
		/// </summary>
		public ISegment NewSegment { get; set; }
		public SegmentForm()
		{
			InitializeComponent();
		}

		//TODO: Почему публично? (+)
		private readonly List<string> SegmentType = new List<string>
		{
			"Serial",
			"Parallel"
		};

		private void OKButton_Click(object sender, EventArgs e)
		{
			switch (SegmentComboBox.SelectedIndex)
			{
				case 0:
				{
					NewSegment = new SerialCircuit(new SegmentsObservableCollection());
					break;
				}
				case 1:
				{
					NewSegment = new ParallelCircuit(new SegmentsObservableCollection());
					break;
				}
			}
            this.DialogResult = DialogResult.OK;
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

        private void SegmentForm_Load(object sender, EventArgs e)
        {
            SegmentComboBox.DataSource = SegmentType;
		}
    }
}
