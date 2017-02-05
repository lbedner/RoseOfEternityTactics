using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

using RoseOfEternity;

public class SlotImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject itemBeingDragged;
	private static bool _isDragging;

	private Text _name;
	private Text _description;
	private Text _attribute_1;
	private Text _attribute_2;

	private GameObject _toolTipPanel;
	private Item _item;

	private Vector3 startPosition;
	private Transform startParent;

	/// <summary>
	/// Initialize the specified toolTipPanel and item.
	/// </summary>
	/// <param name="toolTipPanel">Tool tip panel.</param>
	/// <param name="item">Item.</param>
	public void Initialize(GameObject toolTipPanel, Item item) {

		// Setup tool tip panel and text components
		_toolTipPanel = toolTipPanel;

		_name = _toolTipPanel.transform.FindChild ("Name").GetComponent<Text> ();
		_description = _toolTipPanel.transform.FindChild ("Description").GetComponent<Text> ();
		_attribute_1 = _toolTipPanel.transform.FindChild ("Attribute_1").GetComponent<Text> ();
		_attribute_2 = _toolTipPanel.transform.FindChild ("Attribute_2").GetComponent<Text> ();

		_item = item;
		_isDragging = false;
	}

	/// <summary>
	/// Raises the pointer enter event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerEnter(PointerEventData eventData) {
		if (!_toolTipPanel.activeSelf && !_isDragging) {

			// Reposition tool tip
			Vector3 imagePosition = this.gameObject.transform.position;
			Vector3 oldToolTipPosition = _toolTipPanel.transform.position;
			_toolTipPanel.transform.position = new Vector3 (imagePosition.x + 128, imagePosition.y, oldToolTipPosition.z);

			// Set data on tool tip
			SetToolTip ();

			// Display tool tip
			_toolTipPanel.SetActive (true);
		}
	}

	/// <summary>
	/// Raises the pointer exit event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerExit(PointerEventData eventData) {
		if (_toolTipPanel.activeSelf && !_isDragging)
			_toolTipPanel.SetActive (false);
	}

	/// <summary>
	/// Sets the tool tip.
	/// </summary>
	public void SetToolTip() {

		_name.text = _item.Name;
		_description.text = _item.Description;

		if (_item.Type == Item.ItemType.WEAPON) {

			Attribute attribute = _item.GetAttribute (AttributeEnums.AttributeType.STRENGTH);
			_attribute_1.text = string.Format ("{0}: {1}", attribute.Name, attribute.CurrentValue);

			attribute = _item.GetAttribute (AttributeEnums.AttributeType.CRITICAL_CHANCE);
			_attribute_2.text = string.Format ("{0}: {1}%", attribute.Name, attribute.CurrentValue);
		} else if (_item.Type == Item.ItemType.ARMOR) {
			Attribute attribute = _item.GetAttribute (AttributeEnums.AttributeType.DEFENSE);
			_attribute_1.text = string.Format ("{0}: {1}", attribute.Name, attribute.CurrentValue);

			attribute = _item.GetAttribute (AttributeEnums.AttributeType.DODGE_CHANCE);
			_attribute_2.text = string.Format ("{0}: {1}%", attribute.Name, attribute.CurrentValue);
		} 
		else if (_item.Type == Item.ItemType.CONSUMABLE) {
			_attribute_1.text = "";
			_attribute_2.text = "";
		}
	}

	/// <summary>
	/// Raises the begin drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnBeginDrag(PointerEventData eventData) {
		_toolTipPanel.SetActive (false);
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		_isDragging = true;
		//GetComponent<Canvas> ().sortingOrder = 2;
		//transform.parent = transform.parent;

	}

	/// <summary>
	/// Raises the drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnDrag(PointerEventData eventData) {
		transform.position = Input.mousePosition;
	}

	/// <summary>
	/// Raises the end drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnEndDrag(PointerEventData eventData) {
		itemBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
		if (transform.parent == startParent)
			transform.position = startPosition;
		else
			transform.position = transform.parent.position;
		_isDragging = false;
		//GetComponent<Canvas> ().sortingOrder = 1;
	}
}