using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class TileDataTest {

	[Test]
	public void TestSwapUnits() {
		GameObject testContainer = new GameObject ();
		testContainer.AddComponent<Ally> ();
		Ally unit = testContainer.GetComponent<Ally> ();
			
		TileData tileData1 = new TileData (TileData.TerrainTypeEnum.DESERT, true, "", 0, 0, 0, 0, unit);
		TileData tileData2 = new TileData (TileData.TerrainTypeEnum.DESERT, true, "", 0, 0, 0, 0, null);

		Assert.NotNull (tileData1.Unit);
		Assert.Null (tileData2.Unit);

		tileData1.SwapUnits (tileData2);

		Assert.Null (tileData1.Unit);
		Assert.NotNull (tileData2.Unit);
	}
}