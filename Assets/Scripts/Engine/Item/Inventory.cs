using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class Inventory {
	private List<Item> _items = new List<Item> ();

	/// <summary>
	/// Add the specified item.
	/// </summary>
	/// <param name="item">Item.</param>
	public void Add(Item item) {
		_items.Add (item);
	}

	/// <summary>
	/// Remove the specified item.
	/// </summary>
	/// <param name="item">Item.</param>
	public void Remove(Item item) {
		if (_items.Contains(item))
			_items.Remove (item);
	}

	/// <summary>
	/// Gets the first item by name.
	/// </summary>
	/// <returns>The first item by name.</returns>
	/// <param name="name">Name.</param>
	public Item GetFirstItemByName(string name) {
		foreach (Item item in _items)
			if (item.Name == name)
				return item;
		return null;
	}

	/// <summary>
	/// Gets list of items by name.
	/// </summary>
	/// <returns>The items by name.</returns>
	/// <param name="name">Name.</param>
	public List<Item> GetItemsByName(string name) {
		List<Item> items = new List<Item> ();

		foreach (Item item in _items)
			if (item.Name == name)
				items.Add (item);

		return items;
	}

	/// <summary>
	/// Number of items in the inventory.
	/// </summary>
	public int Count() {
		return _items.Count;
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Inventory"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Inventory"/>.</returns>
	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();
		foreach (Item item in _items)
			sb.Append (item.ToString ());
		return sb.ToString ();		
	}
}