using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryController : MonoBehaviour {

	public GameObject slotsPanel;
	public GameObject slotPanelPrefab;
	public Canvas slotImageCanvasPrefab;
	public GameObject toolTipPanel;
	public GameObject toolTipPanelSecondary;

	// SFX
	public AudioSource metal;
	public AudioSource leather;
	public AudioSource ring;

	public int rows = 8;
	public int columns = 4;
	public float slotResolution = 64;

	// Tool tip colors
	public Color positiveToolTipAttributeColor;
	public Color negativeToolTipAttributeColor;

	public EquippedItemsController _equippedItemsController;

	private Inventory _inventory;

	private Slot[] _slots;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		_inventory = ItemLoader.GetMockInventoryItems ();
		ResizePanels ();
		AddSlots ();
	}

	/// <summary>
	/// Plays the metal SFX.
	/// </summary>
	public void PlayMetalSFX() {
		metal.PlayOneShot (metal.clip);
	}

	/// <summary>
	/// Plays the leather SFX.
	/// </summary>
	public void PlayLeatherSFX() {
		leather.PlayOneShot (leather.clip);
	}

	/// <summary>
	/// Plays the ring SFX.
	/// </summary>
	public void PlayRingSFX() {
		ring.PlayOneShot(ring.clip);
	}

	/// <summary>
	/// Sets the equipped items panels active.
	/// </summary>
	/// <param name="item">Item.</param>
	public void SetEquippedItemsPanelsActive(Item item) {
		_equippedItemsController.SetPanelsActive(item);
	}

	/// <summary>
	/// Sets the equipped items panels active.
	/// </summary>
	public void SetEquippedItemsPanelsActive() {
		_equippedItemsController.SetPanelsActive ();
	}

	/// <summary>
	/// Gets the equipment slot if available.
	/// </summary>
	/// <returns>The equipment slot if available.</returns>
	/// <param name="slotType">Slot type.</param>
	public Slot GetEquipmentSlotIfAvailable(InventorySlots.SlotType slotType) {
		return _equippedItemsController.GetSlotIfAvailable (slotType);
	}

	/// <summary>
	/// Gets the equipment slot.
	/// </summary>
	/// <returns>The equipment slot.</returns>
	/// <param name="slotType">Slot type.</param>
	public Slot GetEquipmentSlot(InventorySlots.SlotType slotType) {
		return _equippedItemsController.GetSlot (slotType);
	}

	/// <summary>
	/// Updates the inventory for equipment slots and normal inventory slots.
	/// </summary>
	/// <param name="item">Item.</param>
	/// <param name="oldSlot">Old slot.</param>
	/// <param name="newSlot">New slot.</param>
	public void UpdateInventory(Item item, Slot oldSlot, Slot newSlot) {

		// Handle items from the old slots
		RemoveItem(item, oldSlot);

		// Add items to new slots
		AddItem(item, newSlot);
	}

	/// <summary>
	/// Swaps the inventory.
	/// </summary>
	/// <param name="item1">Item1.</param>
	/// <param name="item2">Item2.</param>
	/// <param name="item1Slot">Item1 slot.</param>
	/// <param name="item2Slot">Item2 slot.</param>
	public void SwapInventory(Item item1, Item item2, Slot item1Slot, Slot item2Slot) {

		// Remove items from slots
		RemoveItem (item1, item1Slot);
		RemoveItem (item2, item2Slot);

		// Add items to slots
		AddItem (item1, item2Slot);
		AddItem (item2, item1Slot);		
	}

	/// <summary>
	/// Gets the first open slot.
	/// </summary>
	/// <returns>The first open slot.</returns>
	public Slot GetFirstOpenSlot() {
		foreach (Slot slot in _slots) {
			if (!slot.SlotImage)
				return slot;
		}
		return null;
	}

	/// <summary>
	/// Adds the item.
	/// </summary>
	/// <param name="item">Item.</param>
	/// <param name="slot">Slot.</param>
	private void AddItem(Item item, Slot slot) {
		if (slot.slotType != InventorySlots.SlotType.ANY)
			_equippedItemsController.AddItem (item);
		else if (slot.slotType == InventorySlots.SlotType.ANY) {
			_inventory.Upsert (item, slot.Index);
		}
	}

	/// <summary>
	/// Removes the item.
	/// </summary>
	/// <param name="item">Item.</param>
	/// <param name="slot">Slot.</param>
	private void RemoveItem(Item item, Slot slot) {
		if (slot.slotType != InventorySlots.SlotType.ANY)
			_equippedItemsController.RemoveItem (slot.slotType);
		else if (slot.slotType == InventorySlots.SlotType.ANY) {
			_inventory.Remove (item);
		}
	}

	/// <summary>
	/// Resizes the panels.
	/// </summary>
	private void ResizePanels() {

		// Set width/height based off # of columns/rows and slot resolution
		int width = (int) (columns * slotResolution);
		int height = (int) (rows * slotResolution);

		// Resize inventory panel
		RectTransform rectTransform = (RectTransform)transform;
		rectTransform.sizeDelta = new Vector2 (width, height);

		// Resize slots panel
		rectTransform = slotsPanel.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2 (width, height);

		// Set cell size in grid layout
		GridLayoutGroup layoutGroup = slotsPanel.GetComponent<GridLayoutGroup>();
		layoutGroup.cellSize = new Vector2 (slotResolution, slotResolution);
	}

	/// <summary>
	/// Adds the inventory slots.
	/// </summary>
	private void AddSlots() {
		int numSlots = rows * columns;
		_slots = new Slot[numSlots];
		int itemCount = _inventory.Count ();
		for (int index = 0; index < numSlots; index++) {

			// Add Slot Panel
			GameObject slotPanel = Instantiate (slotPanelPrefab);
			slotPanel.transform.SetParent (slotsPanel.transform, false);
			slotPanel.name = string.Format ("Slot {0}", index);

			Slot slotPanelClass = slotPanel.GetComponent<Slot> ();
			slotPanelClass.Index = index;

			_slots [index] = slotPanelClass;

			// Set slot image for all inventory items
			if (itemCount > 0 && _inventory.Get (index) != null) {
				
				Item item = _inventory.Get (index);
				SlotImageCanvas.InstantiateInstance(slotImageCanvasPrefab, item, slotPanel, toolTipPanel, toolTipPanelSecondary, slotResolution, slotResolution);
				itemCount--;
			}
			else {
				// Add null values to inventory
				_inventory.Add (null);
			}
		}
	}
}