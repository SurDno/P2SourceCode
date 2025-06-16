using UnityEngine;

namespace FirstPersonController
{
  [RequireComponent(typeof (CharacterController))]
  public class FirstPersonController : MonoBehaviour
  {
    private Camera m_Camera;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    [SerializeField]
    private FOVKick m_FovKick = new FOVKick();
    [SerializeField]
    private float m_GravityMultiplier;
    [SerializeField]
    private CurveControlledBob m_HeadBob = new CurveControlledBob();
    private Vector2 m_Input;
    [SerializeField]
    private bool m_IsWalking;
    private bool m_Jump;
    [SerializeField]
    private LerpControlledBob m_JumpBob = new LerpControlledBob();
    private bool m_Jumping;
    [SerializeField]
    private float m_JumpSpeed;
    [SerializeField]
    private MouseLook m_MouseLook;
    private Vector3 m_MoveDir = Vector3.zero;
    private float m_NextStep;
    private Vector3 m_OriginalCameraPosition;
    private bool m_PreviouslyGrounded;
    [SerializeField]
    private float m_RunSpeed;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float m_RunstepLenghten;
    private float m_StepCycle;
    [SerializeField]
    private float m_StepInterval;
    [SerializeField]
    private float m_StickToGroundForce;
    [SerializeField]
    private bool m_UseFovKick;
    [SerializeField]
    private bool m_UseHeadBob;
    [SerializeField]
    private float m_WalkSpeed;
    private float m_YRotation;

    private void Start()
    {
      this.m_CharacterController = this.GetComponent<CharacterController>();
      this.m_Camera = Camera.main;
      this.m_OriginalCameraPosition = this.m_Camera.transform.localPosition;
      this.m_FovKick.Setup(this.m_Camera);
      this.m_HeadBob.Setup(this.m_Camera, this.m_StepInterval);
      this.m_StepCycle = 0.0f;
      this.m_NextStep = this.m_StepCycle / 2f;
      this.m_Jumping = false;
      this.m_MouseLook.Init(this.transform, this.m_Camera.transform);
    }

    private void Update()
    {
      this.RotateView();
      if (!this.m_Jump)
        this.m_Jump = Input.GetKeyDown(KeyCode.Space);
      if (!this.m_PreviouslyGrounded && this.m_CharacterController.isGrounded)
      {
        this.StartCoroutine(this.m_JumpBob.DoBobCycle());
        this.m_MoveDir.y = 0.0f;
        this.m_Jumping = false;
      }
      if (!this.m_CharacterController.isGrounded && !this.m_Jumping && this.m_PreviouslyGrounded)
        this.m_MoveDir.y = 0.0f;
      this.m_PreviouslyGrounded = this.m_CharacterController.isGrounded;
    }

    private void FixedUpdate()
    {
      float speed;
      this.GetInput(out speed);
      Vector3 vector = this.transform.forward * this.m_Input.y + this.transform.right * this.m_Input.x;
      RaycastHit hitInfo;
      Physics.SphereCast(this.transform.position, this.m_CharacterController.radius, Vector3.down, out hitInfo, this.m_CharacterController.height / 2f, -1, QueryTriggerInteraction.Ignore);
      Vector3 normalized = Vector3.ProjectOnPlane(vector, hitInfo.normal).normalized;
      this.m_MoveDir.x = normalized.x * speed;
      this.m_MoveDir.z = normalized.z * speed;
      if (this.m_CharacterController.isGrounded)
      {
        this.m_MoveDir.y = -this.m_StickToGroundForce;
        if (this.m_Jump)
        {
          this.m_MoveDir.y = this.m_JumpSpeed;
          this.m_Jump = false;
          this.m_Jumping = true;
        }
      }
      else
        this.m_MoveDir += Physics.gravity * this.m_GravityMultiplier * Time.fixedDeltaTime;
      this.m_CollisionFlags = this.m_CharacterController.Move(this.m_MoveDir * Time.fixedDeltaTime);
      this.ProgressStepCycle(speed);
      this.UpdateCameraPosition(speed);
      this.m_MouseLook.UpdateCursorLock();
    }

    private void ProgressStepCycle(float speed)
    {
      if ((double) this.m_CharacterController.velocity.sqrMagnitude > 0.0 && ((double) this.m_Input.x != 0.0 || (double) this.m_Input.y != 0.0))
        this.m_StepCycle += (this.m_CharacterController.velocity.magnitude + speed * (this.m_IsWalking ? 1f : this.m_RunstepLenghten)) * Time.fixedDeltaTime;
      if ((double) this.m_StepCycle <= (double) this.m_NextStep)
        return;
      this.m_NextStep = this.m_StepCycle + this.m_StepInterval;
    }

    private void UpdateCameraPosition(float speed)
    {
      if (!this.m_UseHeadBob)
        return;
      Vector3 localPosition;
      if ((double) this.m_CharacterController.velocity.magnitude > 0.0 && this.m_CharacterController.isGrounded)
      {
        this.m_Camera.transform.localPosition = this.m_HeadBob.DoHeadBob(this.m_CharacterController.velocity.magnitude + speed * (this.m_IsWalking ? 1f : this.m_RunstepLenghten));
        localPosition = this.m_Camera.transform.localPosition with
        {
          y = this.m_Camera.transform.localPosition.y - this.m_JumpBob.Offset()
        };
      }
      else
        localPosition = this.m_Camera.transform.localPosition with
        {
          y = this.m_OriginalCameraPosition.y - this.m_JumpBob.Offset()
        };
      this.m_Camera.transform.localPosition = localPosition;
    }

    private void GetInput(out float speed)
    {
      float x = 0.0f;
      if (Input.GetKey(KeyCode.A))
        x += -1f;
      if (Input.GetKey(KeyCode.D))
        ++x;
      float y = 0.0f;
      if (Input.GetKey(KeyCode.W))
        ++y;
      if (Input.GetKey(KeyCode.S))
        y += -1f;
      bool isWalking = this.m_IsWalking;
      this.m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
      speed = this.m_IsWalking ? this.m_WalkSpeed : this.m_RunSpeed;
      this.m_Input = new Vector2(x, y);
      if ((double) this.m_Input.sqrMagnitude > 1.0)
        this.m_Input.Normalize();
      if (this.m_IsWalking == isWalking || !this.m_UseFovKick || (double) this.m_CharacterController.velocity.sqrMagnitude <= 0.0)
        return;
      this.StopAllCoroutines();
      this.StartCoroutine(!this.m_IsWalking ? this.m_FovKick.FOVKickUp() : this.m_FovKick.FOVKickDown());
    }

    private void RotateView()
    {
      this.m_MouseLook.LookRotation(this.transform, this.m_Camera.transform);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
      if (this.m_CollisionFlags == CollisionFlags.Below || (Object) attachedRigidbody == (Object) null || attachedRigidbody.isKinematic)
        return;
      attachedRigidbody.AddForceAtPosition(this.m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }
  }
}
