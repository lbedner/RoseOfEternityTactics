public class WorldMapPinConfirmButton : WorldMapPinButton
{
    protected override void SelectMapPin()
    {
        print("WorldMapPinConfirmButton.SelectMapPin()");
        _worldMapPin.LoadScene();
	}
}