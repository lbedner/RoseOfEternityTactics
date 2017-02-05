using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquippedItemsController : MonoBehaviour {
	public GameObject rightHandPanel;
	public GameObject leftHandPanel;
	public GameObject chestPanel;
	public GameObject armsPanel;
	public GameObject legsPanel;

	public Text damageText;
	public Text armorText;
	public Text hitPercentText;
	public Text dodgePercentText;

	public Canvas slotImageCanvasPrefab;
	public GameObject toolTipPanel;
	public GameObject toolTipPanelSecondary;

	private Color _originalColor;

	private InventorySlots _inventorySlots;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		print ("EquippedItemsController.Start()");
		_originalColor = rightHandPanel.GetComponent<Image> ().color;
	}

	/// <summary>
	/// Sets the equipped items.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void SetEquippedItems(Unit unit) {
		_inventorySlots = unit.GetInventorySlots ();

		// Delete existing slot image (if exists) before adding new one.
		// TODO: Clearly, this is terrible. Come up with better solution.
		ClearAllSlotImages ();
		ClearAttributeText ();

		// Iterate over weapons/armor and add
		var items = new List<Item>();
		foreach (Item item in _inventorySlots.GetInventorySlots().Values) {
			SetAllAttributesText (item);

			GameObject slotPanel = GetPanelBySlot (item.SlotType).transform.FindChild("Slot").gameObject;

			RectTransform rectTransform = (RectTransform)slotPanel.transform;
			float height = rectTransform.rect.height;
			float width = rectTransform.rect.width;

			// Create slot images
			SlotImageCanvas.InstantiateInstance (
				slotImageCanvasPrefab,
				item,
				slotPanel,
				toolTipPanel,
				toolTipPanelSecondary,
				width,
				height
			);
		}
	}

	/// <summary>
	/// Adds the item.
	/// </summary>
	/// <param name="item">Item.</param>
	public void AddItem(Item item) {
		_inventorySlots.Add (item);
		IncrementAllAttributesText (item);
	}

	/// <summary>
	/// Removes the item.
	/// </summary>
	/// <param name="slotType">Slot type.</param>
	public void RemoveItem(InventorySlots.SlotType slotType) {
		Item item = _inventorySlots.Remove (slotType);
		DecrementAllAttributesText (item);
	}

	/// <summary>
	/// Gets the item.
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="slotType">Slot type.</param>
	public Item GetItem(InventorySlots.SlotType slotType) {
		return _inventorySlots.Get(slotType);
	}

	/// <summary>
	/// Sets the panels active/inactive depending on the item.
	/// </summary>
	/// <param name="item">Item.</param>
	public void SetPanelsActive(Item item) {
		InventorySlots.SlotType slotType = item.SlotType;

		SetPanelActive (rightHandPanel, CanEquip (rightHandPanel, slotType));
		SetPanelActive (leftHandPanel, CanEquip (leftHandPanel, slotType));
		SetPanelActive (chestPanel, CanEquip (chestPanel, slotType));
		SetPanelActive (armsPanel, CanEquip (armsPanel, slotType));
		SetPanelActive (legsPanel, CanEquip (legsPanel, slotType));
	}

	/// <summary>
	/// Sets all the panels active.
	/// </summary>
	public void SetPanelsActive() {
		SetPanelActive (rightHandPanel, true);
		SetPanelActive (leftHandPanel, true);
		SetPanelActive (chestPanel, true);
		SetPanelActive (armsPanel, true);
		SetPanelActive (legsPanel, true);
	}

	/// <summary>
	/// Gets the slot if available.
	/// </summary>
	/// <returns>The slot if available.</returns>
	/// <param name="slot">Slot.</param>
	public Slot GetSlotIfAvailable(InventorySlots.SlotType slot) {
		GameObject panel = GetPanelBySlot (slot);
		if (panel != null) {
			Slot slotObject = GetSlot(slot);
			return slotObject.SlotImage == null ? slotObject : null;
		}
		else
			return null;				
	}

	/// <summary>
	/// Gets the slot.
	/// </summary>
	/// <returns>The slot.</returns>
	/// <param name="slot">Slot.</param>
	public Slot GetSlot(InventorySlots.SlotType slot) {
		GameObject panel = GetPanelBySlot (slot);
		if (panel != null)
			return panel.transform.FindChild ("Slot").GetComponent<Slot> ();
		return null;
	}

	/// <summary>
	/// Clears all slot images.
	/// </summary>
	public void ClearAllSlotImages() {
		ClearSlotImage (rightHandPanel);
		ClearSlotImage (leftHandPanel);
		ClearSlotImage (chestPanel);
		ClearSlotImage (armsPanel);
		ClearSlotImage (legsPanel);
	}

	/// <summary>
	/// Clears the slot image.
	/// </summary>
	/// <param name="slotPanel">Slot panel.</param>
	private void ClearSlotImage(GameObject slotPanel) {
		Slot slot = slotPanel.transform.FindChild("Slot").GetComponent<Slot>();
		if (slot.SlotImage)
			Destroy (slot.SlotImage);
	}
		
	/// <summary>
	/// Gets the panel by slot.
	/// </summary>
	/// <returns>The panel by slot.</returns>
	/// <param name="slot">Slot.</param>
	private GameObject GetPanelBySlot(InventorySlots.SlotType slot) {
		switch (slot) {
		case InventorySlots.SlotType.RIGHT_HAND:
			return rightHandPanel;
		case InventorySlots.SlotType.LEFT_HAND:
			return leftHandPanel;
		case InventorySlots.SlotType.BODY:
			return chestPanel;
		case InventorySlots.SlotType.HANDS:
			return armsPanel;
		case InventorySlots.SlotType.FEET:
			return legsPanel;
		default:
			return null;
		}
	}
		
	/// <summary>
	/// Determines whether this instance can equip an item in the specified slot.
	/// </summary>
	/// <returns><c>true</c> if this instance can equip the item in the specified slot; otherwise, <c>false</c>.</returns>
	/// <param name="panel">Panel.</param>
	/// <param name="slot">Slot.</param>
	private bool CanEquip(GameObject panel, InventorySlots.SlotType slotType) {
		return slotType == panel.transform.FindChild ("Slot").GetComponent<Slot> ().slotType;
	}

    /// <summary>
    /// Sets the panel's active state.
    /// </summary>
    /// <param name="panel">Panel.</param>
    /// <param name="active">If set to <c>true</c> active.</param>
	private void SetPanelActive(GameObject panel, bool active) {
		Image image = panel.GetComponent<Image> ();
		if (active)
			image.color = _originalColor;
		else
			image.color = Color.gray;
	}

	/// <summary>
	/// Increments all attributes text.
	/// </summary>
	/// <param name="item">Item.</param>
	private void IncrementAllAttributesText(Item item) {

		// Iterate over all attributes of item
		foreach (var attribute in item.GetAttributeCollection().GetAttributes()) {
			Text attributeText = GetAttributeText (attribute.Key);
			if (attributeText != null)
				IncrementAttributeText (attributeText, attribute.Value.CurrentValue);
		}		
	}

	/// <summary>
	/// Decrements all attributes text.
	/// </summary>
	/// <param name="item">Item.</param>
	private void DecrementAllAttributesText(Item item) {

		// Iterate over all attributes of item
		foreach (var attribute in item.GetAttributeCollection().GetAttributes()) {
			Text attributeText = GetAttributeText (attribute.Key);
			if (attributeText != null)
				DecrementAttributeText (attributeText, attribute.Value.CurrentValue);
		}
	}

	/// <summary>
	/// Sets all attributes text.
	/// </summary>
	/// <param name="item">Item.</param>
	private void SetAllAttributesText(Item item) {

		// Iterate over all attributes of item
		foreach (var attribute in item.GetAttributeCollection().GetAttributes()) {
			Text attributeText = GetAttributeText (attribute.Key);
			if (attributeText != null)
				IncrementAttributeText(attributeText, attribute.Value.CurrentValue);
		}
	}


	/// <summary>
	/// Gets the attribute text.
    /// Only certain attributes are supported.
	/// </summary>
	/// <returns>The attribute text.</returns>
	/// <param name="type">Type.</param>
	private Text GetAttributeText(AttributeEnums.AttributeType type) {

		// Look for certain attributes
		Text attributeText = null;
		switch (type) {
		case AttributeEnums.AttributeType.ARMOR:
			attributeText = armorText;
			break;
		case AttributeEnums.AttributeType.DAMAGE:
			attributeText = damageText;
			break;
		case AttributeEnums.AttributeType.DODGE_CHANCE:
			attributeText = dodgePercentText;
			break;
		case AttributeEnums.AttributeType.HIT_PERCENT:
			attributeText = hitPercentText;
			break;
		}
		return attributeText;
	}

	/// <summary>
	/// Increments the attribute text value
	/// </summary>
	/// <param name="attributeText">Attribute text.</param>
	/// <param name="newValue">New value.</param>
	private void IncrementAttributeText(Text attributeText, float newValue) {

		// Get current value
		float currentValue = float.Parse(attributeText.text);
		attributeText.text = (currentValue += newValue).ToString ();
	}

	/// <summary>
	/// Decrements the attribute text value.
	/// </summary>
	/// <param name="attributeText">Attribute text.</param>
	/// <param name="newValue">New value.</param>
	private void DecrementAttributeText(Text attributeText, float newValue) {

		// Get current value
		float currentValue = int.Parse(attributeText.text);
		attributeText.text = (currentValue -= newValue).ToString ();
	}

	/// <summary>
	/// Clears the attribute text.
	/// </summary>
	private void ClearAttributeText() {
		armorText.text = "0";
		damageText.text = "0";
		dodgePercentText.text = "0";
		hitPercentText.text = "0";
	}
}