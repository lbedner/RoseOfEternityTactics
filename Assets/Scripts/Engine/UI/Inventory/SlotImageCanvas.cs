using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

using EternalEngine;

public class SlotImageCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

	public static GameObject itemBeingDragged;

	private static bool _isDragging;

	private Canvas _canvas;
	private CanvasGroup _canvasGroup;

	private InventoryController _inventoryController;

	private Transform _unequippedToolTipNameAndTierPanel;
	private Text _unequippedToolTipTier;
	private Text _unequippedToolTipName;
	private Text _unequippedToolTipDescription;
	private Text _unequippedToolTipAttribute1;
	private Text _unequippedToolTipAttribute2;

	private Transform _equippedToolTipNameAndTierPanel;
	private Text _equippedToolTipTier;
	private Text _equippedToolTipName;
	private Text _equippedToolTipDescription;
	private Text _equippedToolTipAttribute1;
	private Text _equippedToolTipAttribute2;

	private GameObject _unequippedToolTipPanel;
	private GameObject _equippedToolTipPanel;

	private Item _item;
	private Item _secondaryItem = null;

	private Vector3 startPosition;
	private Transform startParent;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		_canvas = transform.GetComponent<Canvas> ();
		_canvasGroup = transform.GetComponent<CanvasGroup> ();
		_inventoryController = GameObject.Find ("Inventory").GetComponent<InventoryController> ();
	}

	/// <summary>
	/// Instantiates the instance.
	/// </summary>
	/// <returns>The instance.</returns>
	/// <param name="slotImageCanvasPrefab">Slot image canvas prefab.</param>
	/// <param name="item">Item.</param>
	/// <param name="slotPanel">Slot panel.</param>
	/// <param name="toolTipPanel">Tool tip panel.</param>
	/// <param name="toolTipPanelSecondary">Tool tip panel secondary.</param>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	public static SlotImageCanvas InstantiateInstance(
		Canvas slotImageCanvasPrefab,
		Item item,
		GameObject slotPanel,
		GameObject toolTipPanel,
		GameObject toolTipPanelSecondary,
		float width,
		float height
	) {

		// Create canvas for slot image
        Canvas canvas = Instantiate (slotImageCanvasPrefab) as Canvas;

        // Resize canvas
        //RectTransform rectTransform = (RectTransform)canvas.transform;
        //rectTransform.sizeDelta = new Vector2(width, height);
		canvas.transform.SetParent (slotPanel.transform, false);

		// Setup background image
		Image image = canvas.transform.Find("SlotBackgroundImage").GetComponent<Image>();
		image.transform.SetParent (canvas.transform, false);
		image.color = item.TierColor;
		RectTransform rectTransform = (RectTransform)image.transform;
		rectTransform.sizeDelta = new Vector2 (width * 0.85f, height * 0.85f);

		// Setup slot image
		image = canvas.transform.Find("SlotImage").GetComponent<Image>();
		image.transform.SetParent (canvas.transform, false);

		// Set icon on slot image
		image.sprite = item.Icon;

		// Resize image size
		rectTransform = (RectTransform)image.transform;
		rectTransform.sizeDelta = new Vector2 (width, height);

		// Pass tool tip info
		SlotImageCanvas slotImage = canvas.transform.GetComponent<SlotImageCanvas> ();
		slotImage.Initialize (toolTipPanel, toolTipPanelSecondary, item);

		return slotImage;
	}

	/// <summary>
	/// Initialize the tool tip panels and item.
	/// </summary>
	/// <param name="uneqippedToolTipPanel">Uneqipped tool tip panel.</param>
	/// <param name="equippedToolTipPanel">Equipped tool tip panel.</param>
	/// <param name="item">Item.</param>
	public void Initialize(GameObject uneqippedToolTipPanel, GameObject equippedToolTipPanel, Item item) {

		// Setup tool tip panel and text components
		_unequippedToolTipPanel = uneqippedToolTipPanel;
		_equippedToolTipPanel = equippedToolTipPanel;

		_unequippedToolTipNameAndTierPanel = _unequippedToolTipPanel.transform.Find ("NameAndTier");
		_unequippedToolTipName = _unequippedToolTipNameAndTierPanel.Find("Name").GetComponent<Text> ();
		_unequippedToolTipTier = _unequippedToolTipNameAndTierPanel.Find("Tier").GetComponent<Text> ();
		_unequippedToolTipDescription = _unequippedToolTipPanel.transform.Find ("Description").GetComponent<Text> ();
		_unequippedToolTipAttribute1 = _unequippedToolTipPanel.transform.Find ("Attribute_1").GetComponent<Text> ();
		_unequippedToolTipAttribute2 = _unequippedToolTipPanel.transform.Find ("Attribute_2").GetComponent<Text> ();

		_equippedToolTipNameAndTierPanel = _equippedToolTipPanel.transform.Find ("NameAndTier");
		_equippedToolTipName = _equippedToolTipNameAndTierPanel.Find("Name").GetComponent<Text> ();
		_equippedToolTipTier = _equippedToolTipNameAndTierPanel.Find("Tier").GetComponent<Text> ();
		_equippedToolTipDescription = _equippedToolTipPanel.transform.Find ("Description").GetComponent<Text> ();
		_equippedToolTipAttribute1 = _equippedToolTipPanel.transform.Find ("Attribute_1").GetComponent<Text> ();
		_equippedToolTipAttribute2 = _equippedToolTipPanel.transform.Find ("Attribute_2").GetComponent<Text> ();

		_item = item;

		_isDragging = false;
	}

	/// <summary>
	/// Gets the item.
	/// </summary>
	/// <returns>The item.</returns>
	public Item GetItem() {
		return _item;
	}


	// ----------------------------- TOOL TIP EVENTS ----------------------------- //

	/// <summary>
	/// Raises the pointer enter event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerEnter(PointerEventData eventData) {
		PositionToolTips ();
	}

	/// <summary>
	/// Positions the tool tips.
	/// </summary>
	private void PositionToolTips() {
		if (!_unequippedToolTipPanel.activeSelf && !_isDragging) {

			GameObject leftToolTipPanel = _unequippedToolTipPanel;
			GameObject rightToolTipPanel = null;

			Color unequippedAttribute1Color = Color.black;
			Color unequippedAttribute2Color = Color.black;

			// Determine if the right panel needs to be displayed (i.e. we're doing a comparison)
			InventorySlots.SlotType slot = _item.SlotType;
			Slot equipmentSlot = _inventoryController.GetEquipmentSlot (slot);
			if (equipmentSlot != null && equipmentSlot.SlotImage != null && equipmentSlot != GetSlot()) {
				_secondaryItem = equipmentSlot.SlotImage.GetComponent<SlotImageCanvas> ().GetItem ();

				leftToolTipPanel = _equippedToolTipPanel;
				rightToolTipPanel = _unequippedToolTipPanel;

				// Determine colors for attributes
				if (_item.Type == Item.ItemType.WEAPON || _item.Type == Item.ItemType.MONSTER_ATTACK) {
					unequippedAttribute1Color = GetToolTipColorByAttribute(_item, _secondaryItem, AttributeEnums.AttributeType.DAMAGE);
					unequippedAttribute2Color = GetToolTipColorByAttribute(_item, _secondaryItem, AttributeEnums.AttributeType.CRITICAL_CHANCE);
				} else if (_item.Type == Item.ItemType.ARMOR) {
					unequippedAttribute1Color = GetToolTipColorByAttribute(_item, _secondaryItem, AttributeEnums.AttributeType.ARMOR);
					unequippedAttribute2Color = GetToolTipColorByAttribute(_item, _secondaryItem, AttributeEnums.AttributeType.DODGE_CHANCE);
				} 

			}

			// Position the tool tip panels
			Vector3 imagePosition = this.gameObject.transform.position;
			Vector3 oldToolTipPosition = leftToolTipPanel.transform.position;
			leftToolTipPanel.transform.position = new Vector3 (imagePosition.x + 128, imagePosition.y, oldToolTipPosition.z);
			if (rightToolTipPanel != null)
				rightToolTipPanel.transform.position = new Vector3 (imagePosition.x + 328, imagePosition.y, oldToolTipPosition.z);

			// Set the data on the tool tip(s)
			SetUnequippedToolTip(unequippedAttribute1Color, unequippedAttribute2Color);
			if (rightToolTipPanel != null)
				SetEquippedToolTip ();

			// Display tool tip(s)
			leftToolTipPanel.SetActive(true);
			if (rightToolTipPanel != null)
				rightToolTipPanel.SetActive(true);
		}
	}

	/// <summary>
	/// Raises the pointer exit event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerExit(PointerEventData eventData) {
		if (!_isDragging) {
			if (_unequippedToolTipPanel.activeSelf)
				_unequippedToolTipPanel.SetActive (false);
			if (_equippedToolTipPanel.activeSelf)
				_equippedToolTipPanel.SetActive (false);
		}
	}

	/// <summary>
	/// Sets the unequipped item tool tip.
	/// </summary>
	/// <param name="attribute1Color">Attribute1 color.</param>
	/// <param name="attribute2Color">Attribute2 color.</param>
	public void SetUnequippedToolTip(Color attribute1Color, Color attribute2Color) {
		SetToolTip (_item, _unequippedToolTipNameAndTierPanel, _unequippedToolTipName, _unequippedToolTipTier, _unequippedToolTipDescription, _unequippedToolTipAttribute1, _unequippedToolTipAttribute2, attribute1Color, attribute2Color);
	}

	/// <summary>
	/// Sets the equipped item tool tip.
	/// </summary>
	public void SetEquippedToolTip() {
		Color defaultAttributeColor = Color.black;
		SetToolTip (_secondaryItem, _equippedToolTipNameAndTierPanel, _equippedToolTipName, _equippedToolTipTier, _equippedToolTipDescription, _equippedToolTipAttribute1, _equippedToolTipAttribute2, defaultAttributeColor, defaultAttributeColor);
	}

	/// <summary>
	/// Gets the tool tip color by attribute.
	/// </summary>
	/// <returns>The tool tip color by attribute.</returns>
	/// <param name="item">Item.</param>
	/// <param name="equippedItem">Equipped item.</param>
	/// <param name="type">Type.</param>
	private Color GetToolTipColorByAttribute(Item item, Item equippedItem, AttributeEnums.AttributeType type) {
		Attribute attribute1 = item.GetAttribute (type);
		Attribute attribute2 = equippedItem.GetAttribute (type);
		if (attribute1.CurrentValue < attribute2.CurrentValue)
			return _inventoryController.negativeToolTipAttributeColor;
		else if (attribute1.CurrentValue > attribute2.CurrentValue)
			return _inventoryController.positiveToolTipAttributeColor;
		return Color.black;
	}

	/// <summary>
	/// Sets the tool tip.
	/// </summary>
	/// <param name="item">Item.</param>
	/// <param name="name">Name.</param>
	/// <param name="description">Description.</param>
	/// <param name="attribute1">Attribute1.</param>
	/// <param name="attribute2">Attribute2.</param>
	/// <param name="attribute1Color">Attribute1 color.</param>
	/// <param name="attribute2Color">Attribute2 color.</param>
	private void SetToolTip(Item item, Transform nameAndTierPanel, Text name, Text tier, Text description, Text attribute1, Text attribute2, Color attribute1Color, Color attribute2Color) {

		name.text = item.Name;
		tier.text = string.Format ("[{0}]", item.TierName);
		nameAndTierPanel.GetComponent<Image> ().color = item.TierColor;
		description.text = item.Description;

		if ((item.Type == Item.ItemType.WEAPON || _item.Type == Item.ItemType.MONSTER_ATTACK) && item.SlotType != InventorySlots.SlotType.AMMO) {
			Attribute attribute = item.GetAttribute (AttributeEnums.AttributeType.DAMAGE);
			attribute1.text = string.Format ("{0}: {1}", attribute.Name, attribute.CurrentValue);
			attribute1.color = attribute1Color;

			attribute = item.GetAttribute (AttributeEnums.AttributeType.CRITICAL_CHANCE);
			attribute2.text = string.Format ("{0}: {1}%", attribute.Name, attribute.CurrentValue);
			attribute2.color = attribute2Color;
		} else if (item.Type == Item.ItemType.ARMOR) {
			Attribute attribute = item.GetAttribute (AttributeEnums.AttributeType.ARMOR);
			attribute1.text = string.Format ("{0}: {1}", attribute.Name, attribute.CurrentValue);
			attribute1.color = attribute1Color;

			attribute = item.GetAttribute (AttributeEnums.AttributeType.DODGE_CHANCE);
			attribute2.text = string.Format ("{0}: {1}%", attribute.Name, attribute.CurrentValue);
			attribute2.color = attribute2Color;
		} else if (item.Type == Item.ItemType.CONSUMABLE) {
			attribute1.text = string.Format ("Use: {0}", description.text);
			attribute1.color = Color.black;
			attribute2.text = "";
			description.text = "";
		} else if ((item.Type == Item.ItemType.WEAPON || _item.Type == Item.ItemType.MONSTER_ATTACK) && item.SlotType == InventorySlots.SlotType.AMMO) {
			attribute1.text = string.Format ("Use: {0}", description.text);
			attribute1.color = Color.black;
			attribute2.text = "";
			description.text = "";
		}
	}

	// ----------------------------- DRAG AND DROP EVENTS ----------------------------- //

	/// <summary>
	/// Raises the begin drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnBeginDrag(PointerEventData eventData) {
		_unequippedToolTipPanel.SetActive (false);
		_equippedToolTipPanel.SetActive (false);
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;
		_canvasGroup.blocksRaycasts = false;
		_isDragging = true;
		_inventoryController.SetEquippedItemsPanelsActive(_item);
		_canvas.sortingOrder += 1;

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
		_canvasGroup.blocksRaycasts = true;
		if (transform.parent == startParent)
			transform.position = startPosition;
		else {
			transform.position = transform.parent.position;

			// Play SFX
			PlaySFX();
		}
		_isDragging = false;
		_inventoryController.SetEquippedItemsPanelsActive ();
		_unequippedToolTipPanel.SetActive (false);
		_equippedToolTipPanel.SetActive (false);
		_canvas.sortingOrder -= 1;
	}


	// ----------------------------- POINTER CLICK EVENTS ----------------------------- //

	/// <summary>
	/// Raises the pointer click event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick(PointerEventData eventData) {

		// Check for double click
		if (eventData.clickCount == 2) {

			Slot oldSlot = GetSlot ();

			// This item is in inventory
			if (oldSlot.slotType == InventorySlots.SlotType.ANY) {

				// Get item slot and see if it's filled
				InventorySlots.SlotType slotType = _item.SlotType;
				Slot equipmentSlot = _inventoryController.GetEquipmentSlot (slotType);
				if (equipmentSlot != null) {
					GameObject slotImageCanvasObject = equipmentSlot.SlotImage;

					// If there is an item in the equipment slot, swap it out
					if (slotImageCanvasObject != null) {
						SlotImageCanvas slotImageCanvas = equipmentSlot.SlotImage.GetComponent<SlotImageCanvas> ();
						SwapSlots (this, slotImageCanvas, oldSlot, equipmentSlot);
					}
					else {
						// Drop item into slot
						DropInSlot (equipmentSlot);
					}

					// Play SFX
					PlaySFX ();
				}
			}
			// This item is equipped
			else if (oldSlot.slotType != InventorySlots.SlotType.ANY) {
				Slot newSlot = _inventoryController.GetFirstOpenSlot ();
				if (newSlot != null) {

					// Drop item into slot
					DropInSlot (newSlot);

					// Play SFX
					PlaySFX ();
				}
			}
		}
	}

	/// <summary>
	/// Drops item into the slot.
	/// </summary>
	/// <param name="slot">Slot.</param>
	public void DropInSlot(Slot newSlot) {

		// Get old slot
		Slot oldSlot = GetSlot();

		// Update UI
		UpdateUI(this, newSlot);

		// Update inventory
		_inventoryController.UpdateInventory(_item, oldSlot, newSlot);
	}

	/// <summary>
	/// Gets the slot.
	/// </summary>
	/// <returns>The slot.</returns>
	public Slot GetSlot() {
		return transform.parent.GetComponent<Slot> ();
	}

	/// <summary>
	/// Swaps the slots of 2 items.
	/// </summary>
	/// <param name="slotImageCanvas1">Slot image canvas1.</param>
	/// <param name="slotImageCanvas2">Slot image canvas2.</param>
	/// <param name="slotImageCanvasSlot1">Slot image canvas slot1.</param>
	/// <param name="slotImageCanvasSlot2">Slot image canvas slot2.</param>
	public void SwapSlots(SlotImageCanvas slotImageCanvas1, SlotImageCanvas slotImageCanvas2, Slot slotImageCanvasSlot1, Slot slotImageCanvasSlot2) {

		// Update UI
		UpdateUI (slotImageCanvas1, slotImageCanvasSlot2);
		UpdateUI (slotImageCanvas2, slotImageCanvasSlot1);

		// Sawp Inventory
		_inventoryController.SwapInventory (
			slotImageCanvas1.GetItem (),
			slotImageCanvas2.GetItem (),
			slotImageCanvasSlot1,
			slotImageCanvasSlot2
		);		
	}

	/// <summary>
	/// Updates the UI of the slot image's location.
	/// </summary>
	/// <param name="slotImageCanvas">Slot image canvas.</param>
	/// <param name="newSlot">New slot.</param>
	private void UpdateUI(SlotImageCanvas slotImageCanvas, Slot newSlot) {

		// Make slot the parent of the slot canvas/image
		slotImageCanvas.transform.SetParent (newSlot.transform, false);

		// Get height of slot and set the image to that same height just to be safe
		RectTransform rectTransform = (RectTransform)newSlot.transform;
		float height = rectTransform.rect.height;
		float width = rectTransform.rect.width;

		//rectTransform = (RectTransform)slotImageCanvas.transform.GetChild (0);
		rectTransform = (RectTransform) slotImageCanvas.transform.Find("SlotImage");
		rectTransform.sizeDelta = new Vector2 (width, height);

		rectTransform = (RectTransform) slotImageCanvas.transform.Find("SlotBackgroundImage");
		rectTransform.sizeDelta = new Vector2 (width * 0.85f, height * 0.85f);
	}

	/// <summary>
	/// Plays the SFX when moving items around.
	/// </summary>
	private void PlaySFX() {
		switch (_item.Type) {
		case Item.ItemType.WEAPON:
		case Item.ItemType.MONSTER_ATTACK:
			_inventoryController.PlayMetalSFX ();
			break;
		case Item.ItemType.ARMOR:
			_inventoryController.PlayLeatherSFX ();
			break;
		case Item.ItemType.CONSUMABLE:
			_inventoryController.PlayRingSFX ();
			break;
		}
	}

	/// <summary>
	/// Determines whether this instance is equipped in the specified slotType.
	/// </summary>
	/// <returns><c>true</c> if this instance is equipped in the specified slotType; otherwise, <c>false</c>.</returns>
	/// <param name="slotType">Slot type.</param>
	private bool IsEquipped(InventorySlots.SlotType slotType) {
		return _inventoryController.GetEquipmentSlot (slotType) == GetSlot ();
	}		
}