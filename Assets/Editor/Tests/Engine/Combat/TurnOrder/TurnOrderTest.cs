using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class TurnOrderTest {

	private TurnOrder _turnOrder;
	private Ally _unit1;
	private Enemy _unit2;

	[SetUp]
	public void Setup() {
		_turnOrder = new TurnOrder ();

		GameObject testContainer = new GameObject ();
		testContainer.AddComponent<Ally> ();
		testContainer.AddComponent<Enemy> ();

		_unit1 = testContainer.GetComponent<Ally>();
		_unit2 = testContainer.GetComponent<Enemy> ();
	}

	[Test]
	public void TestAddUnit() {
		_turnOrder.AddCombatant (_unit1);
		Assert.AreEqual (1, _turnOrder.GetTurnOrderLength ());
	}

	[Test]
	public void TestRemoveUnit() {
		_turnOrder.AddCombatant (_unit1);
		_turnOrder.RemoveCombatant (_unit1);
		Assert.AreEqual (0, _turnOrder.GetTurnOrderLength ());
	}

	[Test]
	public void TestGetNextUp() {
		Assert.IsNull (_turnOrder.GetNextUp ());

		_turnOrder.AddCombatant (_unit1);
		Assert.AreEqual (_unit1, _turnOrder.GetNextUp ());
	}

	[Test]
	public void TestGetTurnOrderSlotNumber() {
		_turnOrder.AddCombatant (_unit1);
		_turnOrder.AddCombatant (_unit2);

		Assert.AreEqual (1, _turnOrder.GetTurnOrderSlotNumber (_unit1));
		Assert.AreEqual (2, _turnOrder.GetTurnOrderSlotNumber (_unit2));
	}		

	[Test]
	public void TestFinishTurn() {
		_turnOrder.AddCombatant (_unit1);
		_turnOrder.AddCombatant (_unit2);

		_turnOrder.FinishTurn (_unit1);

		Assert.AreEqual (1, _turnOrder.GetTurnOrderSlotNumber (_unit2));
		Assert.AreEqual (2, _turnOrder.GetTurnOrderSlotNumber (_unit1));
	}

	[Test]
	public void TestGetTurnOrderLength() {
		_turnOrder.AddCombatant (_unit1);
		_turnOrder.AddCombatant (_unit1);
		_turnOrder.AddCombatant (_unit1);
		_turnOrder.AddCombatant (_unit1);

		Assert.AreEqual (4, _turnOrder.GetTurnOrderLength ());
	}

	[Test]
	public void TestGetAllUnits() {
		_turnOrder.AddCombatant (_unit1);
		_turnOrder.AddCombatant (_unit2);

		List<Unit> units = _turnOrder.GetAllUnits ();

		Assert.AreEqual (2, units.Count);
	}
}