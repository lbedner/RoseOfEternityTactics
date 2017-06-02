using UnityEngine;
using System;
using System.Collections;

public class InfoEventArgs<T> : EventArgs {
	public T info;

	/// <summary>
	/// Initializes a new instance of the <see cref="InfoEventArgs`1"/> class.
	/// </summary>
	public InfoEventArgs() 
	{
		info = default(T);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InfoEventArgs`1"/> class.
	/// </summary>
	/// <param name="info">Info.</param>
	public InfoEventArgs (T info)
	{
		this.info = info;
	}
}