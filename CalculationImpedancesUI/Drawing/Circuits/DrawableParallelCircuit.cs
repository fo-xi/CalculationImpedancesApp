﻿using System.Drawing;
using CalculationImpedancesApp;
using CalculationImpedancesUI.Drawing;

//TODO: Несоответствие дефолтному namespace (+)
namespace CalculationImpedancesUI.Circuits
{
	//TODO: RSDN (+)
	/// <summary>
	/// The class contains methods for working with parallel segment.
	/// </summary>
	public class DrawableParallelCircuit : DrawableSegmentBase
	{
		/// <summary>
		/// Create a parallel segment.
		/// </summary>
		/// <param name="segment">A circuit segment.</param>
		public DrawableParallelCircuit(ISegment segment) : base(segment)
		{ 
		}

		//TODO: XML (+)
        /// <summary>
        /// Calculating the size for a parallel circuit segment.
        /// </summary>
        /// <returns></returns>
		public override Size CalculateSize()
		{
			//TODO: Дубль (+)
            Height = 0;
            Width = 0;
			foreach (DrawableSegmentBase segment in Nodes)
			{
				var calculateSize = segment.CalculateSize();
				Height += calculateSize.Height + Distance;
				if (calculateSize.Width > Width)
				{
					Width = calculateSize.Width;
				}
			}

			Height -= Distance;
			SizeSegment = new Size(Width + Distance, Height);
			return SizeSegment;
		}

		//TODO: XML (+)
        /// <summary>
        /// Finding the coordinates of a parallel circuit segment.
        /// </summary>
		public override void FindCoordinate()
		{
			foreach (DrawableSegmentBase segment in Nodes)
			{
				//TODO: RSDN - длины строк (+)
				int middle = (SizeSegment.Width / 2) 
                     - (segment.SizeSegment.Width / 2);
                int distance = 10;
				var prevNode = segment.PrevNode as DrawableSegmentBase;
				if (prevNode == null)
				{
					segment.StartCoordinate = 
                        new Point(StartCoordinate.X + middle, StartCoordinate.Y);
				}
				else
				{
					segment.StartCoordinate = 
                        new Point(StartCoordinate.X + middle,
						prevNode.StartCoordinate.Y + prevNode.SizeSegment.Height + distance);
				}

				if (!(segment is DrawableElement))
				{
					segment.FindCoordinate();
				}

				segment.CalculateСonnectСoordinate();
			}
		}

		//TODO: XML (+)
        /// <summary>
        /// Draws a parallel segment of the circuit.
        /// </summary>
		public override void Draw()
		{
			foreach (DrawableSegmentBase node in Nodes)
			{
				node.Draw();
				//TODO: RSDN - длины строк (+)
				var leftConnect = 
                    new Point(LeftСonnectСoordinate.X, node.LeftСonnectСoordinate.Y);
				var rightConnect = 
                    new Point(RightСonnectСoordinate.X, node.RightСonnectСoordinate.Y);

				if (Index != 0 || Nodes.Count > 0)
				{
					Graphics.DrawLine(Pen, leftConnect, node.LeftСonnectСoordinate);
				}

				Graphics.DrawLine(Pen, rightConnect, node.RightСonnectСoordinate);
			}

			if (Nodes.Count == 0)
			{
				return;
			}

			var firstNode = Nodes[0] as DrawableSegmentBase;
			var lastNode = Nodes[Nodes.Count - 1] as DrawableSegmentBase;
			//TODO: RSDN - длины строк (+)
			var firstPointRightConnection = 
                new Point(RightСonnectСoordinate.X, firstNode.RightСonnectСoordinate.Y);
			var lastPointRightConnection = 
                new Point(RightСonnectСoordinate.X, lastNode.RightСonnectСoordinate.Y);

			var firstPointLeftConnection = 
                new Point(LeftСonnectСoordinate.X, firstNode.RightСonnectСoordinate.Y);
			var lastPointLeftConnection = 
                new Point(LeftСonnectСoordinate.X, lastNode.RightСonnectСoordinate.Y);

			Graphics.DrawLine(Pen, firstPointRightConnection, lastPointRightConnection);
			Graphics.DrawLine(Pen, firstPointLeftConnection, lastPointLeftConnection);
		}
	}
}
