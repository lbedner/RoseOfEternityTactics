using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using EternalEngine;

[TestFixture]
public class AttributeCollectionTest {

	private Attribute _a1;
	private Attribute _a2;
	private AttributeCollection _collection;
	private AttributeEnums.AttributeType _experienceType;
	private AttributeEnums.AttributeType _levelType;

	[SetUp]
	public void Setup() {
		_experienceType = AttributeEnums.AttributeType.EXPERIENCE;
		_levelType = AttributeEnums.AttributeType.LEVEL;
		_a1 = new Attribute (_experienceType, "EXP", "EXP", "", 0, 0, 100);
		_a2 = new Attribute (_levelType, "LVL", "LVL", "", 0, 1, 10);
	}

	[Test]
	public void TestAddAndCount() {

		_collection = new AttributeCollection();

		// Test adding separate attributes
		_collection.Add (_a1.Type, _a1);
		_collection.Add (_a2);

		Assert.AreEqual (2, _collection.Count());

		// Test that you cannot add the same attributes more than once
		_collection.Add (_a1.Type, _a1);
		_collection.Add (_a2.Type, _a2);

		Assert.AreEqual (2, _collection.Count());
	}

	[Test]
	public void TestGetSuccess() {

		_collection = new AttributeCollection();

		// Add some test attributes
		_collection.Add (_a1);
		_collection.Add (_a2.Type, _a2);

		// Fetch attributes
		Attribute actual1 = _collection.Get (_experienceType);
		Attribute actual2 = _collection.Get (_levelType);

		// Make sure attributes are the same
		Assert.AreEqual (_a1, actual1);
		Assert.AreEqual (_a2, actual2);
	}

	[Test]
	public void TestGetFailure() {

		_collection = new AttributeCollection();

		// Fetch attributes (that don't exist yet)
		Attribute actual1 = _collection.Get (_experienceType);
		Attribute actual2 = _collection.Get (_levelType);

		// Test that attributes are null
		Assert.IsNull (actual1);
		Assert.IsNull (actual2);
	}

	[Test]
	public void TestDeepCopy() {

		_collection = new AttributeCollection();
		_collection.Add (_a1.Type, _a1);
		_collection.Add (_a2);

		// Make sure shallow copies are the same
		AttributeCollection shallowCopy = _collection;
		Assert.AreSame (_collection, shallowCopy);

		foreach (var attribute in _collection.GetAttributes())
			Assert.AreEqual (attribute.Value.CurrentValue, shallowCopy.Get (attribute.Key).CurrentValue);

		// Make sure deep copies are different
		AttributeCollection deepCopy = _collection.DeepCopy ();
		deepCopy.Get (_a1.Type).CurrentValue = 40;
		deepCopy.Get (_a2.Type).CurrentValue = 4;

		Assert.AreNotSame (_collection, deepCopy);

		foreach (var attribute in _collection.GetAttributes())
			Assert.AreNotEqual (attribute.Value.CurrentValue, deepCopy.Get (attribute.Key).CurrentValue);
	}

	[Test]
	public void TestGetFromGlobalCollection() {

		float experienceValue = 10.0f;
		float levelValue = 3.0f;

		// Create test incoming attribute dictionary
		var incomingAttributeDictionary = new Dictionary<AttributeEnums.AttributeType, float> () {
			{_experienceType, experienceValue},
			{_levelType, levelValue}
		};

		// Create "Global" dictionary
		_collection = new AttributeCollection();
		_collection.Add (_a1);
		_collection.Add (_a2);

		// Create new local collection
		AttributeCollection localCollection = new AttributeCollection();

		// Populate local collection with values from global one
		localCollection = AttributeCollection.GetFromGlobalCollection(incomingAttributeDictionary, _collection, localCollection);

		Attribute localExperienceAttribute = localCollection.Get (_experienceType);
		Attribute localLevelAttribute = localCollection.Get (_levelType);

		// Verify that the deep copy worked
		Assert.AreNotSame(localExperienceAttribute, _collection.Get(_experienceType));
		Assert.AreNotSame(localLevelAttribute, _collection.Get(_levelType));

		// Verify the current values
		Assert.AreEqual(experienceValue, localExperienceAttribute.CurrentValue);
		Assert.AreEqual(levelValue, localLevelAttribute.CurrentValue);
	}
}