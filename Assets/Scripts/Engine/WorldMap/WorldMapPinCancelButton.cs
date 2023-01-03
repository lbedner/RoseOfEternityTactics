public class WorldMapPinCancelButton : WorldMapPinButton
{
    protected override void SelectMapPin()
    {
        _worldMapPin.DeactivateUI();
        _worldMapPin.OnButtonPressed(false);
    }
}