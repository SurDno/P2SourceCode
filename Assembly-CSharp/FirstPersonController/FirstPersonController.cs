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
    private FOVKick m_FovKick = new();
    [SerializeField]
    private float m_GravityMultiplier;
    [SerializeField]
    private CurveControlledBob m_HeadBob = new();
    private Vector2 m_Input;
    [SerializeField]
    private bool m_IsWalking;
    private bool m_Jump;
    [SerializeField]
    private LerpControlledBob m_JumpBob = new();
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
      m_CharacterController = GetComponent<CharacterController>();
      m_Camera = Camera.main;
      m_OriginalCameraPosition = m_Camera.transform.localPosition;
      m_FovKick.Setup(m_Camera);
      m_HeadBob.Setup(m_Camera, m_StepInterval);
      m_StepCycle = 0.0f;
      m_NextStep = m_StepCycle / 2f;
      m_Jumping = false;
      m_MouseLook.Init(transform, m_Camera.transform);
    }

    private void Update()
    {
      RotateView();
      if (!m_Jump)
        m_Jump = Input.GetKeyDown(KeyCode.Space);
      if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
      {
        StartCoroutine(m_JumpBob.DoBobCycle());
        m_MoveDir.y = 0.0f;
        m_Jumping = false;
      }
      if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
        m_MoveDir.y = 0.0f;
      m_PreviouslyGrounded = m_CharacterController.isGrounded;
    }

    private void FixedUpdate()
    {
      GetInput(out float speed);
      Vector3 vector = transform.forward * m_Input.y + transform.right * m_Input.x;
      Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out RaycastHit hitInfo, m_CharacterController.height / 2f, -1, QueryTriggerInteraction.Ignore);
      Vector3 normalized = Vector3.ProjectOnPlane(vector, hitInfo.normal).normalized;
      m_MoveDir.x = normalized.x * speed;
      m_MoveDir.z = normalized.z * speed;
      if (m_CharacterController.isGrounded)
      {
        m_MoveDir.y = -m_StickToGroundForce;
        if (m_Jump)
        {
          m_MoveDir.y = m_JumpSpeed;
          m_Jump = false;
          m_Jumping = true;
        }
      }
      else
        m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
      m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
      ProgressStepCycle(speed);
      UpdateCameraPosition(speed);
      m_MouseLook.UpdateCursorLock();
    }

    private void ProgressStepCycle(float speed)
    {
      if (m_CharacterController.velocity.sqrMagnitude > 0.0 && (m_Input.x != 0.0 || m_Input.y != 0.0))
        m_StepCycle += (m_CharacterController.velocity.magnitude + speed * (m_IsWalking ? 1f : m_RunstepLenghten)) * Time.fixedDeltaTime;
      if (m_StepCycle <= (double) m_NextStep)
        return;
      m_NextStep = m_StepCycle + m_StepInterval;
    }

    private void UpdateCameraPosition(float speed)
    {
      if (!m_UseHeadBob)
        return;
      Vector3 localPosition;
      if (m_CharacterController.velocity.magnitude > 0.0 && m_CharacterController.isGrounded)
      {
        m_Camera.transform.localPosition = m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude + speed * (m_IsWalking ? 1f : m_RunstepLenghten));
        localPosition = m_Camera.transform.localPosition with
        {
          y = m_Camera.transform.localPosition.y - m_JumpBob.Offset()
        };
      }
      else
        localPosition = m_Camera.transform.localPosition with
        {
          y = m_OriginalCameraPosition.y - m_JumpBob.Offset()
        };
      m_Camera.transform.localPosition = localPosition;
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
      bool isWalking = m_IsWalking;
      m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
      speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
      m_Input = new Vector2(x, y);
      if (m_Input.sqrMagnitude > 1.0)
        m_Input.Normalize();
      if (m_IsWalking == isWalking || !m_UseFovKick || m_CharacterController.velocity.sqrMagnitude <= 0.0)
        return;
      StopAllCoroutines();
      StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
    }

    private void RotateView()
    {
      m_MouseLook.LookRotation(transform, m_Camera.transform);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
      if (m_CollisionFlags == CollisionFlags.Below || attachedRigidbody == null || attachedRigidbody.isKinematic)
        return;
      attachedRigidbody.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }
  }
}
