using Engine.Source.Utility;

namespace Engine.Source.Behaviours.Controllers
{
  public class PlayerStepsController : StepsController
  {
    [Tooltip("Влияет на частоту проигрывания шагов.")]
    [SerializeField]
    private float playerStepSize = 3f;
    private CharacterController characterController;
    private bool footLeft;
    private float footDistance;

    protected override AudioMixerGroup FootAudioMixer
    {
      get => ScriptableObjectInstance<GameSettingsData>.Instance.ProtagonistFootMixer;
    }

    protected override AudioMixerGroup FootEffectsAudioMixer
    {
      get => ScriptableObjectInstance<GameSettingsData>.Instance.ProtagonistFootEffectsMixer;
    }

    protected override void Awake()
    {
      base.Awake();
      characterController = this.GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
      if (!PlayerUtility.IsPlayerCanControlling)
        return;
      if ((Object) characterController == (Object) null)
      {
        Debug.LogError((object) "{1} needs to have charackter controller if used as player");
      }
      else
      {
        if (!characterController.isGrounded)
          return;
        footDistance += characterController.velocity.magnitude * Time.deltaTime;
        float num = Mathf.Sin(6.28318548f * footDistance / playerStepSize);
        if (num < 0.0 && !footLeft)
        {
          OnStep("Skeleton.Humanoid.Foot_Left", false);
          footLeft = true;
        }
        else if (num > 0.0 && footLeft)
        {
          OnStep("Skeleton.Humanoid.Foot_Right", false);
          footLeft = false;
        }
      }
    }
  }
}
