using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


abstract public class WorldMapPinButton : MonoBehaviour
{
    [SerializeField] protected WorldMapPin _worldMapPin;

    private PointerEventData _clickData;
    private List<RaycastResult> _clickResults;

    private GraphicRaycaster _graphicRaycaster;

    private void Awake() {
        _graphicRaycaster = GetComponent<GraphicRaycaster>();
        _clickData = new PointerEventData(EventSystem.current);
        _clickResults = new List<RaycastResult>();
    }

    abstract protected void SelectMapPin();

	public void SelectMapPin(InputAction.CallbackContext context)
 	{ 
        if (context.started && this.isActiveAndEnabled) {
            _clickData.position = Mouse.current.position.ReadValue();
            _clickResults.Clear();

            _graphicRaycaster.Raycast(_clickData, _clickResults);

            foreach(RaycastResult result in _clickResults)
            {
                if (result.gameObject == this.gameObject)
                {
                    print(string.Format("SelectMapPin({0}) | currentSelectedGameObject: {1} | isActive: {2}", context, EventSystem.current.currentSelectedGameObject, this.gameObject.activeSelf));
                    SelectMapPin();
                    break;
                }
            }
        }
    }
}