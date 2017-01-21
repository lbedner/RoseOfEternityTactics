using UnityEngine;
using NUnit.Framework;
using RoseOfEternity;
using System.Collections;

[TestFixture]
public class AttributeTest {

	private Attribute _a1;
	private Attribute _a2;

	[SetUp]
	public void Setup() {
		_a1 = new Attribute ("increment", "test", "test", 0.0f, 0.0f, 10.0f);
		_a2 = new Attribute ("decrement", "test", "test", 10.0f, 0.0f, 10.0f);
	}

	[Test]
	public void TestIncrement() {

		// Test normal increment
		_a1.Increment (1.0f);
		Assert.AreEqual (1.0f, _a1.CurrentValue);

		// Test that you can't go outside the boundaries of the min/max
		_a1.Increment(100.0f);
		Assert.AreEqual (10.0f, _a1.CurrentValue);
	}

	[Test]
	public void TestDecrement() {

		// Test normal decrement
		_a2.Decrement(1.0f);
		Assert.AreEqual (9.0f, _a2.CurrentValue);

		// Test that you can't go outside the boundaries of min/max
		_a2.Decrement(100.0f);
		Assert.AreEqual (0.0f, _a2.CurrentValue);
	}
}