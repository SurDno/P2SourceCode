using UnityEngine;

namespace InputServices;

public interface ICursorController {
	bool Visible { get; set; }

	bool Free { get; set; }

	Vector2 Position { get; }

	void Move(float diffX, float diffY);
}