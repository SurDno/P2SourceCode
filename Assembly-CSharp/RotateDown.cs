using UnityEngine;

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
  private float rotation = 0.0f;
  private float velocity = 0.0f;
  private Vector3 forwardBeforeAnimation;
  private bool wasEnabled;

  private void UpdateRotation()
  {
    this.velocity += this.forwardBeforeAnimation.y * this.gravity * Time.deltaTime;
    this.velocity *= Mathf.Max(0.0f, (float) (1.0 - (double) Time.deltaTime * (double) this.drag));
    this.rotation += this.velocity * Time.deltaTime;
    if (this.limited)
    {
      if ((double) this.rotation < (double) this.minLimit)
      {
        this.rotation = this.minLimit;
        if ((double) this.velocity >= 0.0)
          return;
        this.velocity = 0.0f;
      }
      else
      {
        if ((double) this.rotation <= (double) this.maxLimit)
          return;
        this.rotation = this.maxLimit;
        if ((double) this.velocity > 0.0)
          this.velocity = 0.0f;
      }
    }
    else
      this.rotation = Mathf.Repeat(this.rotation, 360f);
  }

  private void LateUpdate()
  {
    if (this.wasEnabled)
    {
      this.UpdateRotation();
    }
    else
    {
      this.ResetRotation();
      this.wasEnabled = true;
    }
    this.transform.localEulerAngles = new Vector3(this.rotation, 0.0f, 0.0f);
  }

  private void OnDisable() => this.wasEnabled = false;

  private void ResetRotation()
  {
    this.velocity = 0.0f;
    this.rotation = Vector3.SignedAngle(this.transform.parent.up, Vector3.up, this.transform.parent.right);
  }

  private void Update() => this.forwardBeforeAnimation = this.transform.forward;
}
