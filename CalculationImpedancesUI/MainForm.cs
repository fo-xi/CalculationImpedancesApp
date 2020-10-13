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
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace CalculationImpedancesUI
{
	public partial class MainForm : Form
	{
		//TODO: RSDN
		/// <summary>
		/// All program data.
		/// </summary>
		Project project = new Project();

		//TODO: Зачем публично?
		public readonly List<string> Type = new List<string>
		{
			"",
			"Resistor",
			"Inductor",
			"Capacitor",
		};

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{

			CircuitSelectionComboBox.DataSource = null;
			CircuitSelectionComboBox.DataSource = project.Circuits;
			CircuitSelectionComboBox.DisplayMember = "Name";

			TypeComboBox.DataSource = Type;

			foreach (var i in project.Circuits)
			{
				i.SegmentChanged += ShowMessage;
			}
		}

		private void FillCircuitNodes() 
		{
			CircuitsTreeView.Nodes.Clear();
			var circuit = project.SelectedCircuit;
			SegmentTreeNode mainCircuitNode= new SegmentTreeNode
			{
				Text = circuit.Name,
			};
			CircuitsTreeView.Nodes.Add(mainCircuitNode);
			foreach (var subSegment in circuit.SubSegments)
			{
				SegmentTreeNode subSegmentNode = new SegmentTreeNode
				{
					Text = subSegment is IElement ? subSegment.ToString(): subSegment.Name,
					Segment = subSegment
				};
				if(!(subSegmentNode.Segment is IElement))
				{
					FillTreeNode(subSegmentNode, subSegment);
				}
				CircuitsTreeView.Nodes[0].Nodes.Add(subSegmentNode);
			}
			CircuitsTreeView.ExpandAll();
		}

		private void FillTreeNode(SegmentTreeNode circuitNode, ISegment segment)
		{
			if (segment is IElement)
			{
				SegmentTreeNode elementNode = new SegmentTreeNode
				{
					Text = segment.ToString(),
					Segment = segment
				};
				circuitNode.Nodes.Add(elementNode);
			}
			else
			{
				foreach (var subSegment in segment.SubSegments)
				{
					SegmentTreeNode segmentNode = new SegmentTreeNode
					{
						Text = subSegment is IElement ? subSegment.ToString() : subSegment.Name,
						Segment = subSegment
					};
					circuitNode.Nodes.Add(segmentNode);
					if (!(subSegment is IElement))
					{
						FillTreeNode(segmentNode, subSegment);
					}
				}
			}
		}

		private void CircuitSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e)

		{
			var selectedIndexCircuit = CircuitSelectionComboBox.SelectedIndex;
			if (selectedIndexCircuit != -1)
			{
				project.SelectedCircuit = project.Circuits[selectedIndexCircuit];
			}

			Calculate();
			FillCircuitNodes();
		}
		
		private void AddCircuitButton_Click(object sender, EventArgs e)
		{
			var circuit = new CircuitForm();
			circuit.ShowDialog();
			if (circuit.DialogResult == DialogResult.OK)
			{
				project.Circuits.Add(circuit.Circiut);
                //TODO: Дубль
				CircuitSelectionComboBox.DataSource = null;
				CircuitSelectionComboBox.DataSource = project.Circuits;
				CircuitSelectionComboBox.DisplayMember = "Name";
			}
		}

		private void EditCircuitButton_Click(object sender, EventArgs e)
		{
			var selectedIndex = CircuitSelectionComboBox.SelectedIndex;
			if (selectedIndex == -1)
			{
				MessageBox.Show("Select a circuit from the list", "Warning",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{

				var circuit = new CircuitForm();
				var selectedCircuit = project.Circuits[selectedIndex];
				circuit.Circiut = selectedCircuit;
				circuit.ShowDialog();
				if (circuit.DialogResult == DialogResult.OK)
				{
					project.Circuits[selectedIndex].Name = circuit.Circiut.Name;
                    //TODO: Дубль
					CircuitSelectionComboBox.DataSource = null;
					CircuitSelectionComboBox.DataSource = project.Circuits;
					CircuitSelectionComboBox.DisplayMember = "Name";
				}
			}

			Calculate();
		}

		private void RemoveCircuitButton_Click(object sender, EventArgs e)
		{
			var selectedIndex = CircuitSelectionComboBox.SelectedIndex;
			if (selectedIndex == -1)
			{
				MessageBox.Show("Select a circuit from the list", "Warning",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			
            //TODO: RSDN
			DialogResult result = MessageBox.Show("Do you really want to remove this circuit?",
				"Remove circuit", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (result == DialogResult.OK)
			{
				var selectedCircuit = project.Circuits[selectedIndex];
				project.Circuits.Remove(selectedCircuit);
                //TODO: Дубль
				CircuitSelectionComboBox.DataSource = null;
				CircuitSelectionComboBox.DataSource = project.Circuits;
				CircuitSelectionComboBox.DisplayMember = "Name";
			}
		}

		private void CalculateButton_Click(object sender, EventArgs e)
		{
			if (FrequencyTextBox.Text.Length != 0)
			{
				try
				{
					project.Frequencies.Add(double.Parse(FrequencyTextBox.Text));
					FrequenciesListBox.Items.Add(double.Parse(FrequencyTextBox.Text));
				}
				catch
				{
					MessageBox.Show("Incorrect Value", "Warning",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}

			Calculate();
		}

		private void Calculate()
		{
            project.Results = project.SelectedCircuit.CalculateZ(project.Frequencies);
			ImpedanceValues();
			ResultsListBox.DataSource = null;
			ResultsListBox.DataSource = project.ImpedanceValues;
		}

		private void RemoveFrequencyButton_Click(object sender, EventArgs e)
		{
			var selectedIndex = FrequenciesListBox.SelectedIndex;
			if (selectedIndex == -1)
			{
				MessageBox.Show("Select a frequency from the list", "Warning",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			DialogResult result = MessageBox.Show("Do you really want to remove this frequency?",
				"Remove frequency", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (result == DialogResult.OK)
			{
				var selectedFrequency = project.Frequencies[selectedIndex];
				project.Frequencies.Remove(selectedFrequency);
				FrequenciesListBox.Items.RemoveAt(selectedIndex);
			}
		}

		private void AddElementButton_Click(object sender, EventArgs e)
		{
			//TODO: Дубль
			var selectedIndex = CircuitsTreeView.SelectedNode as SegmentTreeNode;
			if (selectedIndex == null)
			{
				MessageBox.Show("Select a element from the list", "Warning",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (selectedIndex == CircuitsTreeView.Nodes[0])
			{
                //TODO: Дубль
				var element = CreateElement();
				if (element == null)
				{
					return;
				}
				project.SelectedCircuit.SubSegments.Add(element);
				selectedIndex.Nodes.Add(new SegmentTreeNode
				{
					Text = element.ToString(),
					Segment = element
				});
			}
			else if (selectedIndex.Segment is IElement)
			{
				var parent = selectedIndex.Parent as SegmentTreeNode;
				//TODO: Дубль
				var element = CreateElement();
				if (element == null)
				{
					return;
				}

				parent.Segment.SubSegments.Add(element);
				parent.Nodes.Add(new SegmentTreeNode
				{
					Text = element.ToString(),
					Segment = element
				});
			}
			else
			{
                //TODO: Дубль
				var element = CreateElement();
				if (element == null)
				{
					return;
				}
				selectedIndex.Segment.SubSegments.Add(element);
				selectedIndex.Nodes.Add(new SegmentTreeNode
				{
					Text = element.ToString(),
					Segment = element
				});
			}
			TypeComboBox.Text = "";
			NameTextBox.Text = "";
			ValueTextBox.Text = "";
			Calculate();
		}

        private void EditElementButton_Click(object sender, EventArgs e)
        {
            //TODO: Дубль
			var selectedIndex = CircuitsTreeView.SelectedNode as SegmentTreeNode;
            if (selectedIndex == null)
            {
                MessageBox.Show("Select a element from the list", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var parent = selectedIndex.Parent as SegmentTreeNode;
            //TODO: Дубль
			var element = CreateElement();
            if (element == null)
            {
                return;
            }
            parent.Segment.SubSegments.Remove(selectedIndex.Segment);
            parent.Segment.SubSegments.Add(element);
            parent.Nodes.Remove(selectedIndex);
            parent.Nodes.Add(new SegmentTreeNode
            {
                Text = element.ToString(),
                Segment = element
            });
            Calculate();
        }

        private void RemoveElementButton_Click(object sender, EventArgs e)
		{
            //TODO: Дубль
			var selectedIndex = CircuitsTreeView.SelectedNode as SegmentTreeNode;
			if (selectedIndex == null)
			{
				MessageBox.Show("Select a element from the list", "Warning",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (selectedIndex == CircuitsTreeView.Nodes[0])
			{
				MessageBox.Show("Can't delete root element", "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
                //TODO: Дубль
				var parent = selectedIndex.Parent as SegmentTreeNode;
				var element = selectedIndex.Segment;
				if (parent.Segment == null)
				{
					project.SelectedCircuit.SubSegments.Remove(element);
				}
				else
				{
					parent.Segment.SubSegments.Remove(element);
				}
				parent.Nodes.Remove(selectedIndex);
			}
			Calculate();
		}

		private IElement CreateElement()
		{
			IElement segment = null;
			try
			{
				var name = NameTextBox.Text;
				var value = double.Parse(ValueTextBox.Text);

				switch (TypeComboBox.SelectedIndex)
				{
					case 1:
					{
						segment = new Resistor(name, value);
						break;
					}
					case 2:
					{
						segment = new Inductor(name, value);
						break;
					}
					case 3:
					{
						segment = new Capacitor(name, value);
						break;
					}
				}
			}
			catch (FormatException exception)
			{
				MessageBox.Show(exception.Message, "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return segment;
		}

		private void CircuitsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
            //TODO: Дубль
			var selectedIndex = CircuitsTreeView.SelectedNode as SegmentTreeNode;
			if (selectedIndex == null)
			{
				MessageBox.Show("Select a element from the list", "Warning",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (selectedIndex.Segment is Resistor)
			{
				TypeComboBox.Text = "Resistor";
			}
			else if (selectedIndex.Segment is Inductor)
			{
				TypeComboBox.Text = "Inductor";
			}
			else if (selectedIndex.Segment is Capacitor)
			{
				TypeComboBox.Text = "Capacitor";
			}
			else
			{
				TypeComboBox.Text = "";
			}

			if (selectedIndex.Segment is IElement element)
			{
				NameTextBox.Text = selectedIndex.Segment.Name;
				ValueTextBox.Text = element.Value.ToString();
				EditElementButton.Enabled = true;
			}
			else
			{
				TypeComboBox.Text = "";
				NameTextBox.Text = "";
				ValueTextBox.Text = "";
				EditElementButton.Enabled = false;
			}
		}

		private void ShowMessage(object sender, EventArgs e)
		{
			var message = e as ElementEventArgs;
			MessageBox.Show(message.Message, "Information",
				MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void ImpedanceValues()
		{
			project.ImpedanceValues = new List<string>();
			foreach (var i in project.Results)
			{
				project.ImpedanceValues.Add($"{Math.Round(i.Real, 3)} " +
                                            $"+ {Math.Round(i.Imaginary, 3)}*j");
			}
		}

		private void CircuitsTreeView_ItemDrag(object sender, ItemDragEventArgs e)
		{
			DoDragDrop(e.Item, DragDropEffects.Move);
		}

		private void CircuitsTreeView_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void CircuitsTreeView_DragDrop(object sender, DragEventArgs e)
		{
			//TODO: Подозрительно много комментариев...
			// Получаем координаты объекта, к которому перетаскиваем выбранный нами объект 
			Point targetPoint = CircuitsTreeView.PointToClient(new Point(e.X, e.Y));

			// Извлекает узел из места падения (куда перетаскиваем)
			SegmentTreeNode targetNode = CircuitsTreeView.GetNodeAt(targetPoint) as SegmentTreeNode;

			// Вабранная нами node, которую мы перетаскиваем (что перетаскиваем)
			SegmentTreeNode draggedNode = e.Data.GetData(typeof(SegmentTreeNode)) as SegmentTreeNode;

			// Проверка на пустоту
			if (draggedNode == null)
			{
				return;
			}

			// Если пользователь попал в пустоту
			if (targetNode == null)
			{
				UpdateTreeView(draggedNode, targetNode);
				draggedNode.Remove();
				CircuitsTreeView.Nodes[0].Nodes.Add(draggedNode);
				draggedNode.Expand();
			}
			else
			{
				TreeNode parentNode = targetNode;

				// Если перетаскиваемый узел не равен сам себе и не равен нулю 
				if (!draggedNode.Equals(targetNode) && targetNode != null)
				{
					bool canDrop = true;

					// Поднимаемся вверх от узла, на который мы упали,
					// чтобы узнать, является ли targetNode нашим родителем
					while (canDrop && (parentNode != null))
					{
						canDrop = !Object.ReferenceEquals(draggedNode, parentNode);
						parentNode = parentNode.Parent;
					}

					// Это допустимое место падения?
					if (canDrop)
					{
						if (targetNode.Segment is IElement)
						{
							return;
						}
						UpdateTreeView(draggedNode, targetNode);
						draggedNode.Remove();
						targetNode.Nodes.Add(draggedNode);

					}
					targetNode.Expand();
					
				}
			}
			CircuitsTreeView.SelectedNode = draggedNode;
		}

		private void UpdateTreeView(SegmentTreeNode draggedNode, SegmentTreeNode targetNode)
		{
			var parent = draggedNode.Parent as SegmentTreeNode;
			if (parent.Segment == null)
			{
				project.SelectedCircuit.SubSegments.Remove(draggedNode.Segment);
			}
			else
			{
				parent.Segment.SubSegments.Remove(draggedNode.Segment);
			}

			if (targetNode == null || targetNode.Segment == null)
			{
				project.SelectedCircuit.SubSegments.Add(draggedNode.Segment);
			}
			else
			{
				targetNode.Segment.SubSegments.Add(draggedNode.Segment);
			}
		}

        private void AddSegmentButton_Click(object sender, EventArgs e)
        {
            var segmentForm = new SegmentForm();
            segmentForm.ShowDialog();
            if (segmentForm.DialogResult == DialogResult.OK)
            {
                //TODO: Дубль
				var selectedIndex = CircuitsTreeView.SelectedNode as SegmentTreeNode;
                if (selectedIndex == null)
                {
                    MessageBox.Show("Select a element from the list", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (selectedIndex == CircuitsTreeView.Nodes[0])
                {
                    //TODO: Дубль
					var segment = segmentForm.Segment;
                    project.SelectedCircuit.SubSegments.Add(segment);
                    selectedIndex.Nodes.Add(new SegmentTreeNode
                    {
                        Text = segmentForm.Segment.Name,
                        Segment = segment
                    });
                }
                else if (selectedIndex.Segment is IElement)
                {
                    MessageBox.Show("Segment cannot be created from element", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    //TODO: Дубль
					var segment = segmentForm.Segment;
                    selectedIndex.Segment.SubSegments.Add(segment);
                    selectedIndex.Nodes.Add(new SegmentTreeNode
                    {
                        Text = segmentForm.Segment.Name,
                        Segment = segment
                    });
                }
            }
        }
	}
}

