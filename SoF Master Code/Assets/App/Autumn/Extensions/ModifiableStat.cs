using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Modifiable Stat manages a single stat and allow others to add/remove functions to modify the stat
/// </summary>
public class ModifiableStat<T> {
    /// <summary>
    /// The original stat value
    /// </summary>
    public T Original;

    /// <summary>
    /// The current stat value
    /// </summary>
    public T Current;

	/// <summary>
	/// Get the current stat value
	/// </summary>
	/// <returns></returns>
	public T Get () {
		return Current;
	}

    public ModifiableStat(T original = default(T))
    {
        Original = Current = original;
    }

    public void Reset()
    {
        Current = Original;
    }
    

}
