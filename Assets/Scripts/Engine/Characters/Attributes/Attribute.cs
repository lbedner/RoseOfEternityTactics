using UnityEngine;
using System.Collections;

namespace RoseOfEternity {
	
	[System.Serializable]
	public class Attribute {

		[SerializeField] private string _name;
		[SerializeField] private string _shortName;
		[SerializeField] private string _toolTip;
		[SerializeField] private float _minimumValue;
		[SerializeField] private float _currentValue;

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
			_name = name;
			_shortName = shortName;
			_toolTip = toolTip;
			_minimumValue = minimumValue;
			MaximumValue = maximumValue;
			CurrentValue = currentValue;
		}

		/// <summary>
		/// Gets or sets the current value. When setting, value will be clamped to the min/max.
		/// </summary>
		/// <value>The current value.</value>
		public float CurrentValue { get { return _currentValue; } set { _currentValue = Mathf.Clamp (value, _minimumValue, MaximumValue); }}

		/// <summary>
		/// Gets or sets the maximum value.
		/// </summary>
		/// <value>The maximum value.</value>
		public float MaximumValue { get; set; }

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
	}
}