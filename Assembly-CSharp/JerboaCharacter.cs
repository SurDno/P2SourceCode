// Decompiled with JetBrains decompiler
// Type: JerboaCharacter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class JerboaCharacter : MonoBehaviour
{
  private Animator jerboaAnimator;
  public bool jumpStart = false;
  public float groundCheckDistance = 0.6f;
  public float groundCheckOffset = 0.01f;
  public bool isGrounded = true;
  public float jumpSpeed = 1f;
  private Rigidbody jerboaRigid;
  public float forwardSpeed;
  public float turnSpeed;
  public float walkMode = 1f;
  public float jumpStartTime = 0.0f;

  private void Start()
  {
    this.jerboaAnimator = this.GetComponent<Animator>();
    this.jerboaRigid = this.GetComponent<Rigidbody>();
  }

  private void FixedUpdate()
  {
    this.CheckGroundStatus();
    this.Move();
    this.jumpStartTime += Time.deltaTime;
  }

  public void Attack() => this.jerboaAnimator.SetTrigger(nameof (Attack));

  public void Hit() => this.jerboaAnimator.SetTrigger(nameof (Hit));

  public void Grooming() => this.jerboaAnimator.SetTrigger(nameof (Grooming));

  public void Death() => this.jerboaAnimator.SetBool("IsLived", false);

  public void Rebirth() => this.jerboaAnimator.SetBool("IsLived", true);

  public void StandUp() => this.jerboaAnimator.SetBool("SitDown", false);

  public void Sitdown() => this.jerboaAnimator.SetBool("SitDown", true);

  public void Gallop() => this.walkMode = 2f;

  public void Walk() => this.walkMode = 1f;

  public void Jump()
  {
    if (!this.isGrounded)
      return;
    this.jerboaAnimator.SetBool("JumpStart", true);
    this.jumpStart = true;
    this.jumpStartTime = 0.0f;
    this.isGrounded = false;
    this.jerboaAnimator.SetBool("IsGrounded", false);
  }

  private void CheckGroundStatus()
  {
    this.isGrounded = Physics.Raycast(this.transform.position + this.transform.up * this.groundCheckOffset, Vector3.down, out RaycastHit _, this.groundCheckDistance);
    if (this.jumpStart && (double) this.jumpStartTime > 0.15000000596046448)
    {
      this.jumpStart = false;
      this.jerboaAnimator.SetBool("JumpStart", false);
      this.jerboaRigid.AddForce((this.transform.up + this.transform.forward * this.forwardSpeed) * this.jumpSpeed, ForceMode.Impulse);
      this.jerboaAnimator.applyRootMotion = false;
      this.jerboaAnimator.SetBool("IsGrounded", false);
    }
    if (this.isGrounded && !this.jumpStart && (double) this.jumpStartTime > 0.30000001192092896)
    {
      this.jerboaAnimator.applyRootMotion = true;
      this.jerboaAnimator.SetBool("IsGrounded", true);
    }
    else if (!this.jumpStart)
    {
      this.jerboaAnimator.applyRootMotion = false;
      this.jerboaAnimator.SetBool("IsGrounded", false);
    }
  }

  public void Move()
  {
    this.jerboaAnimator.SetFloat("Forward", this.forwardSpeed * this.walkMode);
    this.jerboaAnimator.SetFloat("Turn", this.turnSpeed);
  }
}
