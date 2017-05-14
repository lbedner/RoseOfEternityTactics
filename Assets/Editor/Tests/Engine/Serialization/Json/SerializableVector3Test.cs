using UnityEngine;
using NUnit.Framework;

public class SerializableVector3Test {

	[Test]
	public void TestImplicitOperators() {
		Vector3 vector3 = new Vector3 (0.0f, 0.1f, 0.2f);
		SerializableVector3 serializableVector3 = new SerializableVector3(0.3f, 0.4f, 0.5f);

		SerializableVector3 implicitSerializableVector3 = vector3;
		Assert.AreEqual (vector3.x, implicitSerializableVector3.x);
		Assert.AreEqual (vector3.y, implicitSerializableVector3.y);
		Assert.AreEqual (vector3.z, implicitSerializableVector3.z);

		Vector3 implicitVector3 = serializableVector3;
		Assert.AreEqual (serializableVector3.x, implicitVector3.x);
		Assert.AreEqual (serializableVector3.y, implicitVector3.y);
		Assert.AreEqual (serializableVector3.z, implicitVector3.z);
	}
}