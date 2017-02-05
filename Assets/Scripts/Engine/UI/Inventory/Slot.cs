using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {

	public InventorySlots.SlotType slotType = InventorySlots.SlotType.ANY;

	/// <summary>
	/// Gets the slot image.
	/// </summary>
	/// <value>The slot image.</value>
	public GameObject SlotImage {
		get {
			if (transform.childCount > 0) {
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

	/// <summary>
	/// Gets or sets the index.
	/// </summary>
	/// <value>The index.</value>
	public int Index { get; set; }

	/// <summary>
	/// Raises the drop event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnDrop (PointerEventData eventData) {

		// If slot is empty, drop new item into it
		if (!SlotImage) {

			// Potentially only allow certain item types
			SlotImageCanvas slotImageCanvas = SlotImageCanvas.itemBeingDragged.GetComponent<SlotImageCanvas> ();
			if (IsSlotCompatible(slotImageCanvas))
				slotImageCanvas.DropInSlot (this);
		}
		// If slot is filled, swap items
		else {
			SlotImageCanvas targetSlotImageCanvas = SlotImage.GetComponent<SlotImageCanvas> ();
			SlotImageCanvas sourceSlotImageCanvas = SlotImageCanvas.itemBeingDragged.GetComponent<SlotImageCanvas> ();
			Slot sourceSlot = sourceSlotImageCanvas.GetSlot ();

			if (IsSlotCompatible(sourceSlotImageCanvas))
				targetSlotImageCanvas.SwapSlots (targetSlotImageCanvas, sourceSlotImageCanvas, this, sourceSlot);
		}
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Slot"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Slot"/>.</returns>
	public override string ToString ()
	{
		return string.Format ("[Slot: SlotImage={0}, SlotType={1}]", SlotImage, slotType);
	}

	/// <summary>
	/// Determines whether this slot is compatible with the item trying to be added to it.
	/// </summary>
	/// <returns><c>true</c> if this instance is slot compatible the specified slotImageCanvas; otherwise, <c>false</c>.</returns>
	/// <param name="slotImageCanvas">Slot image canvas.</param>
	private bool IsSlotCompatible(SlotImageCanvas slotImageCanvas) {
		return (slotType == InventorySlots.SlotType.ANY || slotType == slotImageCanvas.GetItem ().SlotType);
	}
}