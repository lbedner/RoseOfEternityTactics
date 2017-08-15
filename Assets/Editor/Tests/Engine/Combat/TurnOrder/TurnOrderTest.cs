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
		_turnOrder.AddUnit (_unit1);
		Assert.AreEqual (1, _turnOrder.GetTurnOrderLength ());
	}

	[Test]
	public void TestInsertUnit() {

		// Add a unit normally, and make sure the index is as it should be
		_turnOrder.AddUnit(_unit1);
		List<Unit> units = _turnOrder.GetAllUnits();
		Assert.AreEqual(0, units.IndexOf(_unit1));

		// Insert a new unit at the beginning and confirm the indexes get shifted
		_turnOrder.InsertUnit(_unit2, 0);
		units = _turnOrder.GetAllUnits();
		Assert.AreEqual(0, units.IndexOf(_unit2));
		Assert.AreEqual(1, units.IndexOf(_unit1));
	}

	[Test]
	public void TestRemoveUnit() {
		_turnOrder.AddUnit (_unit1);
		_turnOrder.RemoveUnit (_unit1);
		Assert.AreEqual (0, _turnOrder.GetTurnOrderLength ());
	}

	[Test]
	public void TestGetNextUp() {
		Assert.IsNull (_turnOrder.GetNextUp ());

		_turnOrder.AddUnit (_unit1);
		Assert.AreEqual (_unit1, _turnOrder.GetNextUp ());
	}

	[Test]
	public void TestGetTurnOrderLength() {
		_turnOrder.AddUnit (_unit1);
		_turnOrder.AddUnit (_unit1);
		_turnOrder.AddUnit (_unit1);
		_turnOrder.AddUnit (_unit1);

		Assert.AreEqual (4, _turnOrder.GetTurnOrderLength ());
	}

	[Test]
	public void TestGetAllUnits() {
		_turnOrder.AddUnit (_unit1);
		_turnOrder.AddUnit (_unit2);

		List<Unit> units = _turnOrder.GetAllUnits ();

		Assert.AreEqual (2, units.Count);
	}
}