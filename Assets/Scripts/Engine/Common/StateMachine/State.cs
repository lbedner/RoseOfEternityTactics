using UnityEngine;

public abstract class State : MonoBehaviour {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public virtual void Enter () {
		AddListeners ();
	}

	/// <summary>
	/// Removes listeners when the state is exited.
	/// </summary>
	public virtual void Exit () {
		RemoveListeners ();
	}

	/// <summary>
	/// Removes listeners when the state is destroyed.
	/// </summary>
	protected virtual void OnDestroy() {
		RemoveListeners ();
	}

	/// <summary>
	/// Adds the listeners.
	/// </summary>
	protected virtual void AddListeners () {}

	/// <summary>
	/// Removes the listeners.
	/// </summary>
	protected virtual void RemoveListeners () {}	
}