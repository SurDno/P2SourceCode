using Engine.Source.Utility;
using UnityEngine;
using UnityEngine.Audio;

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
      this.characterController = this.GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
      if (!PlayerUtility.IsPlayerCanControlling)
        return;
      if ((Object) this.characterController == (Object) null)
      {
        Debug.LogError((object) "{1} needs to have charackter controller if used as player");
      }
      else
      {
        if (!this.characterController.isGrounded)
          return;
        this.footDistance += this.characterController.velocity.magnitude * Time.deltaTime;
        float num = Mathf.Sin(6.28318548f * this.footDistance / this.playerStepSize);
        if ((double) num < 0.0 && !this.footLeft)
        {
          this.OnStep("Skeleton.Humanoid.Foot_Left", false);
          this.footLeft = true;
        }
        else if ((double) num > 0.0 && this.footLeft)
        {
          this.OnStep("Skeleton.Humanoid.Foot_Right", false);
          this.footLeft = false;
        }
      }
    }
  }
}
