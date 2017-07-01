using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

public class AbilityCollection {
	public bool HasTalent { get; private set; }
	public bool HasMagic { get; private set; }

	public int Count { get { return _abilities.Count; } }

	private Dictionary<int, Ability> _abilities = new Dictionary<int, Ability>();

	/// <summary>
	/// Gets or sets the abilities.
	/// Used purely for deserialization.
	/// </summary>
	/// <value>The abilities.</value>
	[JsonProperty] private List<Ability> Abilities { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="AbilityCollection"/> class.
	/// </summary>
	public AbilityCollection() {
		HasTalent = false;
		HasMagic = false;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AbilityCollection"/> class.
	/// </summary>
	/// <param name="abilities">Abilities.</param>
	[JsonConstructor]
	private AbilityCollection(List<Ability> abilities) {

		// Convert to a proper dictionary
		foreach (var ability in abilities)
			Add (ability);		
	}

	/// <summary>
	/// Returns a deep copied instance.
	/// </summary>
	/// <returns>The copy.</returns>
	public AbilityCollection DeepCopy() {
		AbilityCollection deepCopiedInstance = new AbilityCollection ();
		foreach (var ability in _abilities.Values)
			deepCopiedInstance.Add (ability.DeepCopy());
		return deepCopiedInstance;
	}

	/// <summary>
	/// Add the specified ability.
	/// </summary>
	/// <param name="ability">Ability.</param>
	public void Add(Ability ability) {
		_abilities.Add (ability.Id, ability);

		// Set ability type flags based on passed in abilities
		Ability.AbilityType type = ability.Type;
		if (type == Ability.AbilityType.TALENT) {
			if (!HasTalent)
				HasTalent = true;
		}
		else if (type == Ability.AbilityType.MAGIC) {
			if (!HasMagic)
				HasMagic = true;
		}
	}

	/// <summary>
	/// Get by the specified ID.
	/// </summary>
	/// <param name="id">Identifier.</param>
	public Ability Get(int id) {
		return _abilities [id];
	}

	/// <summary>
	/// Gets the abilities.
	/// </summary>
	/// <returns>The abilities.</returns>
	public Dictionary<int, Ability> GetAbilities() {
		return _abilities;
	}

	/// <summary>
	/// Gets the attack ability.
	/// </summary>
	/// <returns>The attack ability.</returns>
	public Ability GetAttackAbility() {
		return GetAbilitiesByType (Ability.AbilityType.ATTACK) [0];
	}

	/// <summary>
	/// Gets the magic abilities.
	/// </summary>
	/// <returns>The magic abilities.</returns>
	public List<Ability> GetMagicAbilities() {
		return GetAbilitiesByType (Ability.AbilityType.MAGIC);
	}

	/// <summary>
	/// Gets the talent abilities.
	/// </summary>
	/// <returns>The talent abilities.</returns>
	public List<Ability> GetTalentAbilities() {
		return GetAbilitiesByType (Ability.AbilityType.TALENT);
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="AbilityCollection"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="AbilityCollection"/>.</returns>
	public override string ToString ()
	{
		StringBuilder sb = new StringBuilder ();
		foreach (var ability in _abilities.Values)
			sb.Append (ability.ToString ());
		return sb.ToString ();
	}

	/// <summary>
	/// Gets the abilities by type
	/// </summary>
	/// <returns>The abilities by type.</returns>
	/// <param name="type">Type.</param>
	private List<Ability> GetAbilitiesByType(Ability.AbilityType type) {

		// Create tmp dictionary for filtering by type
		var tmpAbilityDict = new Dictionary<int, Ability>();
		foreach (var ability in _abilities)
			if (ability.Value.Type == type)
				tmpAbilityDict.Add (ability.Key, ability.Value);

		// Sort by id
		var sortedList = tmpAbilityDict.Keys.ToList();
		sortedList.Sort ();

		// Create final sorted ability list
		var abilityList = new List<Ability>();
		foreach (var id in sortedList)
			abilityList.Add (_abilities [id]);
		return abilityList;
	}
}