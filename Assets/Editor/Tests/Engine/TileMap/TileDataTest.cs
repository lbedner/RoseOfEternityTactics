using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class TileDataTest {

	[Test]
	public void TestSwapUnits() {
		GameObject testContainer = new GameObject ();
		testContainer.AddComponent<Ally> ();
		Ally unit = testContainer.GetComponent<Ally> ();
		Vector3 position = new Vector3 (0.0f, 0.0f, 0.0f);
			
		TileData tileData1 = new TileData (TileData.TerrainTypeEnum.DESERT, true, "", 0, 0, 0, 0, position, position, "", "unit1");
		tileData1.Unit = unit;
		TileData tileData2 = new TileData (TileData.TerrainTypeEnum.DESERT, true, "", 0, 0, 0, 0, position, position, "");

		Assert.NotNull (tileData1.Unit);
		Assert.Null (tileData2.Unit);

		tileData1.SwapUnits (tileData2);

		Assert.Null (tileData1.Unit);
		Assert.NotNull (tileData2.Unit);
	}
}