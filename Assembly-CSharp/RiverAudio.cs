using UnityEngine;

public class RiverAudio : MonoBehaviour
{
  public Transform Source;
  public Collider2D PlayerCollider;
  [Space]
  public float Smoothness = 0.25f;
  public float Height;
  private Collider2D riverCollider;
  private Vector2 sourcePosition;
  private Vector2 sourceVelocity = Vector2.zero;

  private Vector2 SourcePosition
  {
    get => this.sourcePosition;
    set
    {
      this.sourcePosition = value;
      this.Source.position = new Vector3(this.sourcePosition.x, this.Height, this.sourcePosition.y);
    }
  }

  private Vector2 PlayerPosition
  {
    get
    {
      if ((Object) GameCamera.Instance.CameraTransform == (Object) null)
        return Vector2.zero;
      Vector3 position = GameCamera.Instance.CameraTransform.position;
      return new Vector2(position.x, position.z);
    }
  }

  private Vector2 ClosestPoint(Vector2 playerPosition)
  {
    this.PlayerCollider.transform.localPosition = new Vector3(playerPosition.x, playerPosition.y, 0.0f);
    ColliderDistance2D colliderDistance2D = Physics2D.Distance(this.riverCollider, this.PlayerCollider);
    return !colliderDistance2D.isValid || colliderDistance2D.isOverlapped ? playerPosition : colliderDistance2D.pointA;
  }

  private void Start()
  {
    this.riverCollider = this.GetComponent<Collider2D>();
    if ((Object) this.Source == (Object) null || (Object) this.PlayerCollider == (Object) null || (Object) this.riverCollider == (Object) null)
    {
      if ((Object) this.Source != (Object) null)
        this.Source.gameObject.SetActive(false);
      this.enabled = false;
    }
    else
      this.SourcePosition = this.ClosestPoint(this.PlayerPosition);
  }

  private void Update()
  {
    Vector2 playerPosition = this.PlayerPosition;
    Vector2 target = this.ClosestPoint(playerPosition);
    Vector2 vector2_1 = Vector2.SmoothDamp(this.SourcePosition, target, ref this.sourceVelocity, this.Smoothness, 1000f, Time.deltaTime);
    float magnitude1 = (target - playerPosition).magnitude;
    Vector2 vector2_2 = vector2_1 - playerPosition;
    float magnitude2 = vector2_2.magnitude;
    if ((double) magnitude2 < (double) magnitude1)
    {
      float num = magnitude1 / magnitude2;
      Vector2 vector2_3 = vector2_2 * num;
      vector2_1 = playerPosition + vector2_3;
    }
    this.SourcePosition = vector2_1;
  }
}
