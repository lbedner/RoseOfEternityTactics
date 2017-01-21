using UnityEngine;
using NUnit.Framework;
using RoseOfEternity;
using System.Collections;

[TestFixture]
public class AttributeCollectionTest {

	private Attribute _a1;
	private Attribute _a2;
	private AttributeCollection _collection;

	[SetUp]
	public void Setup() {
		_a1 = new Attribute ("EXP", "EXP", "", 0, 0, 100);
		_a2 = new Attribute ("LVL", "LVL", "", 0, 1, 10);
	}

	[Test]
	public void TestAddAndCount() {

		_collection = new AttributeCollection();

		// Test adding separate attributes
		_collection.Add (AttributeEnums.AttributeType.EXPERIENCE, _a1);
		_collection.Add (AttributeEnums.AttributeType.LEVEL, _a2);

		Assert.AreEqual (2, _collection.Count());

		// Test that you cannot add the same attributes more than once
		_collection.Add (AttributeEnums.AttributeType.EXPERIENCE, _a1);
		_collection.Add (AttributeEnums.AttributeType.LEVEL, _a2);

		Assert.AreEqual (2, _collection.Count());
	}

	[Test]
	public void TestGetSuccess() {

		_collection = new AttributeCollection();

		// Add some test attributes
		_collection.Add (AttributeEnums.AttributeType.EXPERIENCE, _a1);
		_collection.Add (AttributeEnums.AttributeType.LEVEL, _a2);

		// Fetch attributes
		Attribute actual1 = _collection.Get (AttributeEnums.AttributeType.EXPERIENCE);
		Attribute actual2 = _collection.Get (AttributeEnums.AttributeType.LEVEL);

		// Make sure attributes are the same
		Assert.AreEqual (_a1, actual1);
		Assert.AreEqual (_a2, actual2);
	}

	[Test]
	public void TestGetFailure() {

		_collection = new AttributeCollection();

		// Fetch attributes (that don't exist yet)
		Attribute actual1 = _collection.Get (AttributeEnums.AttributeType.EXPERIENCE);
		Attribute actual2 = _collection.Get (AttributeEnums.AttributeType.LEVEL);

		// Test that attributes are null
		Assert.IsNull (actual1);
		Assert.IsNull (actual2);
	}
}