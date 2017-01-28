using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class InventorySlots  {

	/// <summary>
	/// All available slots.
	/// </summary>
	public enum Slot {
		RIGHT_HAND,
		LEFT_HAND,
		HEAD,
		BODY,
		FEET,
		HANDS,
		BACK,
		RING_1,
		RING_2,
		AMULET,
	}

	private Dictionary<Slot, Item> _items = new Dictionary<Slot, Item>();

	/// <summary>
	/// Add the item to the specified slot.
	/// If an item already exists in the slot, remove it and put back into the inventory.
	/// </summary>
	/// <param name="slot">Slot.</param>
	/// <param name="inventory">Inventory.</param>
	/// <param name="item">Item.</param>
	public void Add(Slot slot, Inventory inventory, Item item) {
		if (_items.ContainsKey (slot)) {

			// Remove currently equipped item and put back into inventory
			Remove (slot, inventory);
		}

		// Remove item from inventory
		inventory.Remove(item);

		// Add item to slot
		_items.Add (slot, item);
	}

	/// <summary>
	/// Remove the item from the specified slot and put back into the specified inventory.
	/// </summary>
	/// <param name="slot">Slot.</param>
	/// <param name="inventory">Inventory.</param>
	public void Remove(Slot slot, Inventory inventory) {
		if (_items.ContainsKey(slot)) {

			// Remove item from item slot
			Item item = _items[slot];
			_items.Remove (slot);

			// Put item back into inventory
			inventory.Add(item);
		}
	}

	/// <summary>
	/// Get an item by the specified slot.
	/// </summary>
	/// <param name="slot">Slot.</param>
	public Item Get(Slot slot) {
		if (_items.ContainsKey (slot))
			return _items [slot];
		return null;
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="InventorySlots"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="InventorySlots"/>.</returns>
	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();
		foreach (Item item in _items.Values)
			sb.Append (item.ToString ());
		return sb.ToString ();
	}
}