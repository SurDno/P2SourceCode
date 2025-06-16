using System.Collections;
using UnityEngine;

namespace RootMotion.Dynamics
{
  [HelpURL("http://root-motion.com/puppetmasterdox/html/page11.html")]
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/BehaviourFall")]
  public class BehaviourFall : BehaviourBase
  {
    [LargeHeader("Animation State")]
    [Tooltip("Animation State to crossfade to when this behaviour is activated.")]
    public string stateName = "Falling";
    [Tooltip("The duration of crossfading to 'State Name'. Value is in seconds.")]
    public float transitionDuration = 0.4f;
    [Tooltip("Layer index containing the destination state. If no layer is specified or layer is -1, the first state that is found with the given name or hash will be played.")]
    public int layer;
    [Tooltip("Start time of the current destination state. Value is in seconds. If no explicit fixedTime is specified or fixedTime value is float.NegativeInfinity, the state will either be played from the start if it's not already playing, or will continue playing from its current time and no transition will happen.")]
    public float fixedTime;
    [LargeHeader("Blending")]
    [Tooltip("The layers that will be raycasted against to find colliding objects.")]
    public LayerMask raycastLayers;
    [Tooltip("The parameter in the Animator that blends between catch fall and writhe animations.")]
    public string blendParameter = "FallBlend";
    [Tooltip("The height of the pelvis from the ground at which will blend to writhe animation.")]
    public float writheHeight = 4f;
    [Tooltip("The vertical velocity of the pelvis at which will blend to writhe animation.")]
    public float writheYVelocity = 1f;
    [Tooltip("The speed of blendig between the two falling animations.")]
    public float blendSpeed = 3f;
    [Tooltip("The speed of blending in mapping on activation.")]
    public float blendMappingSpeed = 1f;
    [LargeHeader("Ending")]
    [Tooltip("If false, this behaviour will never end.")]
    public bool canEnd;
    [Tooltip("The minimum time since this behaviour activated before it can end.")]
    public float minTime = 1.5f;
    [Tooltip("If the velocity of the pelvis falls below this value, can end the behaviour.")]
    public float maxEndVelocity = 0.5f;
    [Tooltip("Event triggered when all end conditions are met.")]
    public BehaviourBase.PuppetEvent onEnd;
    private float timer;
    private bool endTriggered;

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page11.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_behaviour_fall.html");
    }

    protected override void OnActivate()
    {
      this.forceActive = true;
      this.StopAllCoroutines();
      this.StartCoroutine(this.SmoothActivate());
    }

    protected override void OnDeactivate() => this.forceActive = false;

    public override void OnReactivate()
    {
      this.timer = 0.0f;
      this.endTriggered = false;
    }

    private IEnumerator SmoothActivate()
    {
      this.timer = 0.0f;
      this.endTriggered = false;
      this.puppetMaster.targetAnimator.CrossFadeInFixedTime(this.stateName, this.transitionDuration, this.layer, this.fixedTime);
      Muscle[] muscleArray1 = this.puppetMaster.muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        m.state.pinWeightMlp = 0.0f;
        m.rigidbody.velocity = m.mappedVelocity;
        m.rigidbody.angularVelocity = m.mappedAngularVelocity;
        m = (Muscle) null;
      }
      muscleArray1 = (Muscle[]) null;
      float fader = 0.0f;
      while ((double) fader < 1.0)
      {
        fader += Time.deltaTime;
        Muscle[] muscleArray2 = this.puppetMaster.muscles;
        for (int index = 0; index < muscleArray2.Length; ++index)
        {
          Muscle m = muscleArray2[index];
          m.state.pinWeightMlp -= Time.deltaTime;
          m.state.mappingWeightMlp += Time.deltaTime * this.blendMappingSpeed;
          m = (Muscle) null;
        }
        muscleArray2 = (Muscle[]) null;
        yield return (object) null;
      }
    }

    protected override void OnFixedUpdate()
    {
      if ((int) this.raycastLayers == -1)
        Debug.LogWarning((object) "BehaviourFall has no layers to raycast to.", (Object) this.transform);
      float blendTarget = this.GetBlendTarget(this.GetGroundHeight());
      this.puppetMaster.targetAnimator.SetFloat(this.blendParameter, Mathf.MoveTowards(this.puppetMaster.targetAnimator.GetFloat(this.blendParameter), blendTarget, Time.deltaTime * this.blendSpeed));
      this.timer += Time.deltaTime;
      if (this.endTriggered || !this.canEnd || (double) this.timer < (double) this.minTime || this.puppetMaster.isBlending || (double) this.puppetMaster.muscles[0].rigidbody.velocity.magnitude >= (double) this.maxEndVelocity)
        return;
      this.endTriggered = true;
      this.onEnd.Trigger(this.puppetMaster);
    }

    protected override void OnLateUpdate()
    {
      this.puppetMaster.targetRoot.position += this.puppetMaster.muscles[0].transform.position - this.puppetMaster.muscles[0].target.position;
      this.GroundTarget(this.raycastLayers);
    }

    public override void Resurrect()
    {
      foreach (Muscle muscle in this.puppetMaster.muscles)
        muscle.state.pinWeightMlp = 0.0f;
    }

    private float GetBlendTarget(float groundHeight)
    {
      if ((double) groundHeight > (double) this.writheHeight)
        return 1f;
      Vector3 vertical = V3Tools.ExtractVertical(this.puppetMaster.muscles[0].rigidbody.velocity, this.puppetMaster.targetRoot.up, 1f);
      float num = vertical.magnitude;
      if ((double) Vector3.Dot(vertical, this.puppetMaster.targetRoot.up) < 0.0)
        num = -num;
      return (double) num > (double) this.writheYVelocity ? 1f : 0.0f;
    }

    private float GetGroundHeight()
    {
      RaycastHit hitInfo = new RaycastHit();
      return Physics.Raycast(this.puppetMaster.muscles[0].rigidbody.position, -this.puppetMaster.targetRoot.up, out hitInfo, 100f, (int) this.raycastLayers) ? hitInfo.distance : float.PositiveInfinity;
    }
  }
}
