using UnityEngine;

public class JerboaCharacter : MonoBehaviour
{
  private Animator jerboaAnimator;
  public bool jumpStart;
  public float groundCheckDistance = 0.6f;
  public float groundCheckOffset = 0.01f;
  public bool isGrounded = true;
  public float jumpSpeed = 1f;
  private Rigidbody jerboaRigid;
  public float forwardSpeed;
  public float turnSpeed;
  public float walkMode = 1f;
  public float jumpStartTime;

  private void Start()
  {
    jerboaAnimator = GetComponent<Animator>();
    jerboaRigid = GetComponent<Rigidbody>();
  }

  private void FixedUpdate()
  {
    CheckGroundStatus();
    Move();
    jumpStartTime += Time.deltaTime;
  }

  public void Attack() => jerboaAnimator.SetTrigger(nameof (Attack));

  public void Hit() => jerboaAnimator.SetTrigger(nameof (Hit));

  public void Grooming() => jerboaAnimator.SetTrigger(nameof (Grooming));

  public void Death() => jerboaAnimator.SetBool("IsLived", false);

  public void Rebirth() => jerboaAnimator.SetBool("IsLived", true);

  public void StandUp() => jerboaAnimator.SetBool("SitDown", false);

  public void Sitdown() => jerboaAnimator.SetBool("SitDown", true);

  public void Gallop() => walkMode = 2f;

  public void Walk() => walkMode = 1f;

  public void Jump()
  {
    if (!isGrounded)
      return;
    jerboaAnimator.SetBool("JumpStart", true);
    jumpStart = true;
    jumpStartTime = 0.0f;
    isGrounded = false;
    jerboaAnimator.SetBool("IsGrounded", false);
  }

  private void CheckGroundStatus()
  {
    isGrounded = Physics.Raycast(transform.position + transform.up * groundCheckOffset, Vector3.down, out RaycastHit _, groundCheckDistance);
    if (jumpStart && jumpStartTime > 0.15000000596046448)
    {
      jumpStart = false;
      jerboaAnimator.SetBool("JumpStart", false);
      jerboaRigid.AddForce((transform.up + transform.forward * forwardSpeed) * jumpSpeed, ForceMode.Impulse);
      jerboaAnimator.applyRootMotion = false;
      jerboaAnimator.SetBool("IsGrounded", false);
    }
    if (isGrounded && !jumpStart && jumpStartTime > 0.30000001192092896)
    {
      jerboaAnimator.applyRootMotion = true;
      jerboaAnimator.SetBool("IsGrounded", true);
    }
    else if (!jumpStart)
    {
      jerboaAnimator.applyRootMotion = false;
      jerboaAnimator.SetBool("IsGrounded", false);
    }
  }

  public void Move()
  {
    jerboaAnimator.SetFloat("Forward", forwardSpeed * walkMode);
    jerboaAnimator.SetFloat("Turn", turnSpeed);
  }
}
