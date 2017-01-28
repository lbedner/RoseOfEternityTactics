using UnityEngine;
using System.Collections;

namespace RoseOfEternity {
	
	[System.Serializable]
	public class Attribute {

		private float _currentValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="RoseOfEternity.Attribute"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="shortName">Short name.</param>
		/// <param name="toolTip">Tool tip.</param>
		/// <param name="currentValue">Current value.</param>
		/// <param name="minimumValue">Minimum value.</param>
		/// <param name="maximumValue">Maximum value.</param>
		public Attribute(string name, string shortName, string toolTip, float currentValue, float minimumValue, float maximumValue) {
			Name = name;
			ShortName = shortName;
			ToolTip = toolTip;
			MinimumValue = minimumValue;
			MaximumValue = maximumValue;
			_currentValue = currentValue;
		}

		// Properties
		public string Name { get; private set; }
		public string ShortName { get; private set; }
		public string ToolTip { get; private set; }
		public float MinimumValue { get; private set; }
		public float MaximumValue { get; set; }

		/// <summary>
		/// Gets or sets the current value. When setting, value will be clamped to the min/max.
		/// </summary>
		/// <value>The current value.</value>
		public float CurrentValue { get { return _currentValue; } set { _currentValue = Mathf.Clamp (value, MinimumValue, MaximumValue); }}

		/// <summary>
		/// Increment the current value.
		/// </summary>
		/// <param name="incrementValue">Increment value.</param>
		public void Increment(float incrementValue) {
			CurrentValue += incrementValue;
		}

		/// <summary>
		/// Decrement the current value.
		/// </summary>
		/// <param name="decrementValue">Decrement value.</param>
		public void Decrement(float decrementValue) {
			CurrentValue -= decrementValue;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoseOfEternity.Attribute"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoseOfEternity.Attribute"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[Attribute: Name={0}, CurrentValue={1}]", Name, CurrentValue);
		}
	}
}