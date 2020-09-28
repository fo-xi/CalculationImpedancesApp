﻿using System;
using System.Numerics;
using System.Collections.Generic;

namespace CalculationImpedancesApp
{
    /// <summary>
    /// A class representing a resistor.
    /// </summary>
    public class Resistor : IElement
    {
        /// <summary>
        /// An event that will fire when an element changes.
        /// </summary>
        public event EventHandler SegmentChanged;

        /// <summary>
        /// A collection that stores sub-segments of the circiut.
        /// </summary>
        public ElementObservableCollectioncs<ISegment> SubSegments { get; } = null;

        /// <summary>
        /// Resistor name.
        /// </summary>
        private double _name;

        /// <summary>
        /// Resistor value.
        /// </summary>
        private double _value;

        /// <summary>
        /// Returns and sets the name of a resistor.
        /// </summary>
        public string Name
        {
	        get
	        {
		        return _name;

	        }
	        set
	        {
		        if (value.Lenght < 0)
		        {
			        throw new ArgumentException($"The {nameof(Value)} cannot be empty!");
		        }
	        }
        }

        /// <summary>
        /// Returns and sets the value of a resistor.
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException($"The {nameof(Value)} cannot be negative!");
                }
                if (value != _value)
                {
                    SegmentChanged?.Invoke(this,
                    new ElementEventArgs($"The resistor changed the {nameof(Value)} to {Value}!"));
                }
                _value = value;
            }
        }

        /// <summary>
        /// Creates a resistor.
        /// </summary>
        /// <param name="name">Resistor name.</param>
        /// <param name="value">Resistor value.</param>
        public Resistor(string name, double value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Resistor impedance calculation.
        /// <param name="frequency">Signal frequency.</param>
        /// </summary>
        public Complex CalculateZ(double frequency)
        {
            Complex result = new Complex(Value, 0);
            return result;
        }

        /// <summary>
        /// Overriding a method that returns the name and value of the capacitor.
        /// </summary>
        public override string ToString()
        {
            return "Resistor: " + Name + " = " + Value;
        }
    }
}
