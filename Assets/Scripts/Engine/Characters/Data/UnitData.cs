using UnityEngine;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using EternalEngine;

public class UnitData {
	
	public enum UnitType {
		PLAYER,
		ENEMY,
		ALLY,
	}

	[JsonProperty] public string ResRef { get; set; }
	[JsonProperty] public string FirstName { get; set; }
	[JsonProperty] public string LastName { get; set; }
	[JsonProperty] public string Class { get; set; }
	[JsonProperty] public string PortraintLocation { get; set; }
	[JsonProperty] public string Sprite { get; set; }
	[JsonProperty] [JsonConverter(typeof(StringEnumConverter))] public UnitType Type { get; set; }

	[JsonIgnore] public AttributeCollection AttributeCollection { get; set; }
	[JsonIgnore] public InventorySlots InventorySlots { get; set; }
	[JsonIgnore] public AbilityCollection AbilityCollection { get; set; }

	[JsonIgnore] public Sprite Portrait { get; private set; }

	[JsonProperty] private Dictionary<AttributeEnums.AttributeType, float> Attributes {
		get {
			var attributeCurrentValues = new Dictionary<AttributeEnums.AttributeType, float> ();
			foreach (var item in AttributeCollection.GetAttributes())
				attributeCurrentValues.Add (item.Key, item.Value.CurrentValue);
			return attributeCurrentValues;
		}
	}

	[JsonProperty] private Dictionary<InventorySlots.SlotType, int> Inventory {
		get {
			var itemsInSlots = new Dictionary<InventorySlots.SlotType, int> ();
			foreach (var item in InventorySlots.GetInventorySlots())
				itemsInSlots.Add (item.Key, item.Value.Id);
			return itemsInSlots;
		}
	}

	[JsonProperty] private List<int> Abilities {
		get {
			var abilityIds = new List<int> ();
			foreach (var ability in AbilityCollection.GetAbilities().Values)
				abilityIds.Add (ability.Id);
			return abilityIds;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnitData"/> class.
	/// </summary>
	public UnitData() {}

	/// <summary>
	/// Returns a deep copied instance.
	/// </summary>
	/// <returns>The copy.</returns>
	public UnitData DeepCopy() {
		return new UnitData(ResRef, FirstName, LastName, Class, Portrait, Sprite, Type, AttributeCollection.DeepCopy(), InventorySlots.DeepCopy(), AbilityCollection.DeepCopy());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnitData"/> class.
	/// </summary>
	/// <param name="resRef">Res reference.</param>
	/// <param name="firstName">First name.</param>
	/// <param name="lastName">Last name.</param>
	/// <param name="class">Class.</param>
	/// <param name="portrait">Portrait.</param>
	/// <param name="sprite">Sprite.</param>
	/// <param name="unitType">Unit type.</param>
	/// <param name="attributeCollection">Attribute collection.</param>
	/// <param name="inventorySlots">Inventory slots.</param>
	/// <param name="abilityCollection">Ability collection.</param>
	private UnitData(string resRef, string firstName, string lastName, string @class, Sprite portrait, string sprite, UnitType unitType, AttributeCollection attributeCollection, InventorySlots inventorySlots, AbilityCollection abilityCollection) {
		ResRef = resRef;
		FirstName = firstName;
		LastName = lastName;
		Class = @class;
		Portrait = portrait;
		Sprite = sprite;
		Type = unitType;
		AttributeCollection = attributeCollection;
		InventorySlots = inventorySlots;
		AbilityCollection = abilityCollection;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnitData"/> class.
	/// </summary>
	/// <param name="resRef">Res reference.</param>
	/// <param name="firstName">First name.</param>
	/// <param name="lastName">Last name.</param>
	/// <param name="class">Class.</param>
	/// <param name="portraitLocation">Portrait location.</param>
	/// <param name="sprite">Sprite.</param>
	/// <param name="unitType">Unit type.</param>
	/// <param name="attributes">Attributes.</param>
	/// <param name="inventory">Inventory.</param>
	/// <param name="abilityIds">Ability identifiers.</param>
	[JsonConstructor]
	private UnitData(string resRef, string firstName, string lastName, string @class, string portraitLocation, string sprite, UnitType unitType, Dictionary<AttributeEnums.AttributeType, float> attributes, Dictionary<InventorySlots.SlotType, int> inventory, List<int> abilities) {
		ResRef = resRef;
		FirstName = firstName;
		LastName = lastName;
		Class = @class;
		PortraintLocation = portraitLocation;
		Sprite = sprite;
		Type = unitType;

		// Set attributes in their collection. If attribute already exists, set value,
		// else, grab from global collection and set new attribute and value
		AttributeCollection globalAttributeCollection = AttributeManager.Instance.GlobalAttributeCollection;
		if (AttributeCollection == null)
			AttributeCollection = new AttributeCollection ();
		AttributeCollection = AttributeCollection.GetFromGlobalCollection (attributes, globalAttributeCollection, AttributeCollection);

		// Create inventory slots for unit
		InventorySlots = new InventorySlots();
		foreach (var inventorySlot in inventory) {

			// Get item from global inventory
			Item item = ItemManager.Instance.GlobalInventory.GetById(inventorySlot.Value).DeepCopy();
			InventorySlots.Add (inventorySlot.Key, item);
		}

		// Abilities
		AbilityCollection = new AbilityCollection ();
		foreach (var id in abilities) {

			// Get ability from global instance
			Ability ability = AbilityManager.Instance.GlobalAbilityCollection.Get(id).DeepCopy();
			AbilityCollection.Add (ability);
		}

		// Load the portrait
		Portrait = Resources.Load<Sprite>(PortraintLocation);
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="UnitData"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="UnitData"/>.</returns>
	public override string ToString ()
	{
		return string.Format ("[UnitData: FirstName={0}, LastName={1}, Class={2}, Portrait={3}, PortraintLocation={4}, AttributeCollection={5}, InventorySlots={6}], AbilityCollection={7}", FirstName, LastName, Class, Portrait, PortraintLocation, AttributeCollection, InventorySlots, AbilityCollection);
	}
}