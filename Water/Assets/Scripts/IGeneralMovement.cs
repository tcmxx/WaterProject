using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGeneralMovement {

	/// <summary>
	/// Move the specified horizontal.
	/// </summary>
	/// <param name="horizontal">Horizontal. -1 means left, 0 means stop, 1 means right</param>
	void Move(int horizontal);

	void Jump();
}
