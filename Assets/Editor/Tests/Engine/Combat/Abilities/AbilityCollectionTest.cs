using UnityEngine;
using System.Collections;

using NUnit.Framework;

using EternalEngine;

[TestFixture]
public class AbilityCollectionTest {

	private AttributeCollection _attributeCollection;
	private AbilityCollection _abilityCollection;

	private Ability _ability1;
	private Ability _ability2;
	private Ability _ability3;
	private Ability _ability4;
	private Ability _ability5;

	[SetUp]
	public void Setup() {
		Attribute range = new Attribute (AttributeEnums.AttributeType.RANGE, "range", "rng", "range", 4.0f, 1.0f, 100.0f);
		Attribute aoeRange = new Attribute (AttributeEnums.AttributeType.AOE_RANGE, "aoeRange", "aoerng", "aoerange", 1.0f, 1.0f, 100.0f);
		_attributeCollection = new AttributeCollection ();
		_attributeCollection.Add (range);
		_attributeCollection.Add (aoeRange);

		_ability1 = new Ability (0, Ability.AbilityType.MAGIC, "test_magic", "test_magic", "test_magic", "iconPath", "vfxPath", 1, 0, Ability.TargetTypeEnum.DEFAULT, _attributeCollection);
		_ability2 = new Ability (1, Ability.AbilityType.TALENT, "testTalent", "testTalent", "testTalent", "iconPath", "vfxPath", 1, 0, Ability.TargetTypeEnum.DEFAULT, _attributeCollection);
		_ability3 = new Ability (2, Ability.AbilityType.ATTACK, "testAttack", "testAttack", "testAttack", "iconPath", "vfxPath", 1, 0, Ability.TargetTypeEnum.DEFAULT, _attributeCollection);
		_ability4 = new Ability (3, Ability.AbilityType.MAGIC, "testMagic2", "testMagic2", "testMagic2", "iconPath", "vfxPath", 1, 0, Ability.TargetTypeEnum.DEFAULT, _attributeCollection);
		_ability5 = new Ability (4, Ability.AbilityType.TALENT, "testTalent2", "testTalent2", "testTalent2", "iconPath", "vfxPath", 1, 0, Ability.TargetTypeEnum.DEFAULT, _attributeCollection);
	}

	[Test]
	public void TestDeepCopy() {
		_abilityCollection = new AbilityCollection ();
		_abilityCollection.Add (_ability1);
		_abilityCollection.Add (_ability2);
		_abilityCollection.Add (_ability3);

		// Make sure shallow copy is the same
		AbilityCollection shallowCopy = _abilityCollection;
		Assert.AreSame (_abilityCollection, shallowCopy);

		// Make sure deep copy is different
		AbilityCollection deepCopy = _abilityCollection.DeepCopy();
		Assert.AreNotSame (_abilityCollection, deepCopy);
	}

	[Test]
	public void TestAdd() {
		_abilityCollection = new AbilityCollection ();

		Assert.AreEqual (0, _abilityCollection.Count);
		_abilityCollection.Add (_ability1);
		Assert.AreEqual (1, _abilityCollection.Count);
	}

	[Test]
	public void TestGet() {
		_abilityCollection = new AbilityCollection ();
		_abilityCollection.Add (_ability1);

		Ability ability = _abilityCollection.Get (0);

		Assert.AreSame (_ability1, ability);
	}

	[Test]
	public void TestGetAbilities() {
		_abilityCollection = new AbilityCollection ();
		_abilityCollection.Add (_ability1);
		_abilityCollection.Add (_ability2);
		_abilityCollection.Add (_ability3);

		var abilities = _abilityCollection.GetAbilities ();
		Assert.AreEqual (3, abilities.Count);

		Assert.AreSame (_ability1, abilities [0]);
		Assert.AreSame (_ability2, abilities [1]);
		Assert.AreSame (_ability3, abilities [2]);
	}

	[Test]
	public void TestGetAttackAbility() {
		_abilityCollection = new AbilityCollection ();
		_abilityCollection.Add (_ability1);
		_abilityCollection.Add (_ability2);
		_abilityCollection.Add (_ability3);

		Assert.AreSame (_ability3, _abilityCollection.GetAttackAbility ());
	}

	[Test]
	public void TestGetMagicAbilities() {
		_abilityCollection = new AbilityCollection ();
		_abilityCollection.Add (_ability1);
		_abilityCollection.Add (_ability2);
		_abilityCollection.Add (_ability3);
		_abilityCollection.Add (_ability4);
		_abilityCollection.Add (_ability5);

		var abilities = _abilityCollection.GetMagicAbilities ();

		Assert.AreEqual (2, abilities.Count);
		Assert.AreSame (_ability1, abilities [0]);
		Assert.AreSame (_ability4, abilities [1]);
	}

	[Test]
	public void TestGetTalentAbilities() {
		_abilityCollection = new AbilityCollection ();
		_abilityCollection.Add (_ability1);
		_abilityCollection.Add (_ability2);
		_abilityCollection.Add (_ability3);
		_abilityCollection.Add (_ability4);
		_abilityCollection.Add (_ability5);

		var abilities = _abilityCollection.GetTalentAbilities ();

		Assert.AreEqual (2, abilities.Count);
		Assert.AreSame (_ability2, abilities [0]);
		Assert.AreSame (_ability5, abilities [1]);
	}
}