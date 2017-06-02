using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour {
	public virtual State CurrentState
	{
		get { return _currentState; }
		set { Transition (value); }
	}
	protected State _currentState;
	protected bool _inTransition;

	public virtual T GetState<T> () where T : State {
		T target = GetComponent<T>();
		if (target == null)
			target = gameObject.AddComponent<T>();
		return target;
	}

	public virtual void ChangeState<T> () where T : State {
		CurrentState = GetState<T>();
	}

	/// <summary>
	/// Transition to the specified newState.
	/// </summary>
	/// <param name="newState">New state.</param>
	protected virtual void Transition (State newState) {
		if (_currentState == newState || _inTransition)
			return;

		_inTransition = true;

		if (_currentState != null)
			_currentState.Exit();

		_currentState = newState;

		if (_currentState != null)
			_currentState.Enter();

		_inTransition = false;
	}
}