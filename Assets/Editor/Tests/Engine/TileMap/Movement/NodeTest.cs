using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class NodeTest {

	private Node _n1;
	private Node _n2;

	[SetUp]
	public void Setup() {
		_n1 = new Node ();
		_n2 = new Node ();
	}

	[Test]
	public void TestDistanceTo() {

		// Horizontal
		SetCoordinates (0, 0, 1, 0);
		Assert.AreEqual (1.0f, _n1.DistanceTo(_n2));

		// Diaganol
		SetCoordinates (0, 0, 1, 1);
		Assert.AreEqual (1.41421354f, _n1.DistanceTo(_n2));

		// Vertial
		SetCoordinates (0, 0, 0, 1);
		Assert.AreEqual (1.0f, _n1.DistanceTo(_n2));
	}

	[Test]
	public void TestDistanceToFailure() {
		SetCoordinates (0, 0, 5, 10);

		Assert.AreEqual (0.0f, _n1.DistanceTo (null));
	}

	private void SetCoordinates(int n1_x, int n1_z, int n2_x, int n2_z) {
		_n1.x = n1_x;
		_n1.z = n1_z;

		_n2.x = n2_x;
		_n2.z = n2_z;
	}
}