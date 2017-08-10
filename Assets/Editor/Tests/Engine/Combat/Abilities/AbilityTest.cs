using UnityEngine;
using System.Collections;

using NUnit.Framework;

using EternalEngine;

[TestFixture]
public class AbilityTest {

	private AttributeCollection _attributeCollection;
	private AbilityCollection _abilityCollection;

	private Ability _ability1;

	[SetUp]
	public void Setup() {
		Attribute range = new Attribute (AttributeEnums.AttributeType.RANGE, "range", "rng", "range", 4.0f, 1.0f, 100.0f);
		Attribute aoeRange = new Attribute (AttributeEnums.AttributeType.AOE_RANGE, "aoeRange", "aoerng", "aoerange", 1.0f, 1.0f, 100.0f);
		_attributeCollection = new AttributeCollection ();
		_attributeCollection.Add (range);
		_attributeCollection.Add (aoeRange);

		_ability1 = new Ability (0, Ability.AbilityType.MAGIC, "test_magic", "test_magic", "test_magic", "iconPath", "vfxPath", 1, 0, Ability.TargetTypeEnum.DEFAULT, _attributeCollection);
	}

	[Test]
	public void TestDeepCopy() {
		Ability shallowCopy = _ability1;
		Assert.AreSame (_ability1, shallowCopy);

		Ability deepCopy = _ability1.DeepCopy ();
		deepCopy.GetAttributeCollection ().Get (AttributeEnums.AttributeType.RANGE).Increment (1.0f);

		// Test that all values are different in deep copied version
		Assert.AreNotSame(_ability1, deepCopy);
	}

	[Test]
	public void TestGetRange() {
		Assert.AreEqual (4, _ability1.GetRange ());
	}

	[Test]
	public void TestGetAOERange() {
		Assert.AreEqual (1, _ability1.GetAOERange ());
	}
}