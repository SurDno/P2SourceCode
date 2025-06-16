public class RotateDown : MonoBehaviour
{
  [SerializeField]
  private float gravity = 30f;
  [SerializeField]
  private float drag = 1f;
  [SerializeField]
  private bool limited = false;
  [SerializeField]
  private float minLimit = 0.0f;
  [SerializeField]
  private float maxLimit = 0.0f;
  private float rotation;
  private float velocity;
  private Vector3 forwardBeforeAnimation;
  private bool wasEnabled;

  private void UpdateRotation()
  {
    velocity += forwardBeforeAnimation.y * gravity * Time.deltaTime;
    velocity *= Mathf.Max(0.0f, (float) (1.0 - (double) Time.deltaTime * drag));
    rotation += velocity * Time.deltaTime;
    if (limited)
    {
      if (rotation < (double) minLimit)
      {
        rotation = minLimit;
        if (velocity >= 0.0)
          return;
        velocity = 0.0f;
      }
      else
      {
        if (rotation <= (double) maxLimit)
          return;
        rotation = maxLimit;
        if (velocity > 0.0)
          velocity = 0.0f;
      }
    }
    else
      rotation = Mathf.Repeat(rotation, 360f);
  }

  private void LateUpdate()
  {
    if (wasEnabled)
    {
      UpdateRotation();
    }
    else
    {
      ResetRotation();
      wasEnabled = true;
    }
    this.transform.localEulerAngles = new Vector3(rotation, 0.0f, 0.0f);
  }

  private void OnDisable() => wasEnabled = false;

  private void ResetRotation()
  {
    velocity = 0.0f;
    rotation = Vector3.SignedAngle(this.transform.parent.up, Vector3.up, this.transform.parent.right);
  }

  private void Update() => forwardBeforeAnimation = this.transform.forward;
}
