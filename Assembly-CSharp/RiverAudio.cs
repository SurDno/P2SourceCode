using UnityEngine;

public class RiverAudio : MonoBehaviour {
	public Transform Source;
	public Collider2D PlayerCollider;
	[Space] public float Smoothness = 0.25f;
	public float Height;
	private Collider2D riverCollider;
	private Vector2 sourcePosition;
	private Vector2 sourceVelocity = Vector2.zero;

	private Vector2 SourcePosition {
		get => sourcePosition;
		set {
			sourcePosition = value;
			Source.position = new Vector3(sourcePosition.x, Height, sourcePosition.y);
		}
	}

	private Vector2 PlayerPosition {
		get {
			if (GameCamera.Instance.CameraTransform == null)
				return Vector2.zero;
			var position = GameCamera.Instance.CameraTransform.position;
			return new Vector2(position.x, position.z);
		}
	}

	private Vector2 ClosestPoint(Vector2 playerPosition) {
		PlayerCollider.transform.localPosition = new Vector3(playerPosition.x, playerPosition.y, 0.0f);
		var colliderDistance2D = Physics2D.Distance(riverCollider, PlayerCollider);
		return !colliderDistance2D.isValid || colliderDistance2D.isOverlapped
			? playerPosition
			: colliderDistance2D.pointA;
	}

	private void Start() {
		riverCollider = GetComponent<Collider2D>();
		if (Source == null || PlayerCollider == null || riverCollider == null) {
			if (Source != null)
				Source.gameObject.SetActive(false);
			enabled = false;
		} else
			SourcePosition = ClosestPoint(PlayerPosition);
	}

	private void Update() {
		var playerPosition = PlayerPosition;
		var target = ClosestPoint(playerPosition);
		var vector2_1 =
			Vector2.SmoothDamp(SourcePosition, target, ref sourceVelocity, Smoothness, 1000f, Time.deltaTime);
		var magnitude1 = (target - playerPosition).magnitude;
		var vector2_2 = vector2_1 - playerPosition;
		var magnitude2 = vector2_2.magnitude;
		if (magnitude2 < (double)magnitude1) {
			var num = magnitude1 / magnitude2;
			var vector2_3 = vector2_2 * num;
			vector2_1 = playerPosition + vector2_3;
		}

		SourcePosition = vector2_1;
	}
}