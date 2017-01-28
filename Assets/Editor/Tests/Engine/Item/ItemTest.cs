using UnityEngine;
using NUnit.Framework;
using RoseOfEternity;
using System.Collections;

[TestFixture]
public class ItemTest {

	[Test]
	public void TestGetAttributeSuccess() {

		Attribute a1 = new Attribute ("test_get_attribute", "test", "test", 0.0f, 0.0f, 10.0f);
		AttributeCollection attributes = new AttributeCollection ();

		AttributeEnums.AttributeType type = AttributeEnums.AttributeType.ABILITY_POINTS;
		attributes.Add (type, a1);

		Item i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", attributes);

		Assert.NotNull (i1.GetAttribute (type));
	}

	[Test]
	public void TestGetAttributesFailiure() {
		Item i1 = new Item (0, Item.ItemType.WEAPON, "weapon", "weapon description", "weapon", new AttributeCollection ());

		Assert.Null(i1.GetAttribute(AttributeEnums.AttributeType.ABILITY_POINTS));
	}
}