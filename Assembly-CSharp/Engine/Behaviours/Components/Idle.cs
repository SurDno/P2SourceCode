using System;
using Engine.Impl.Tasks;
using Engine.Source.Commons;

namespace Engine.Behaviours.Components
{
  public class Idle : MonoBehaviour
  {
    private Animator animator;
    private MecanimKinds.MovableIdleStateKind animatorState = MecanimKinds.MovableIdleStateKind.Unknown;
    private AnimatorOverrideController animatorOverrideController;
    [SerializeField]
    [FormerlySerializedAs("_Clips")]
    private Clip[] clips;
    private bool isInitialized;
    private MecanimKinds.MovableIdleStateKind localState = MecanimKinds.MovableIdleStateKind.Unknown;
    private Parameter[] parameters;

    public Animator Animator
    {
      get => animator;
      protected set => animator = value;
    }

    private void OnPauseEvent()
    {
      if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
        return;
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        animator.SetFloat("Mecanim.Speed", 0.0f);
      else
        animator.SetFloat("Mecanim.Speed", 1f);
    }

    private void Initialize()
    {
      if (isInitialized)
        return;
      animator = this.GetComponent<Animator>();
      if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
        return;
      animatorOverrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
      if ((UnityEngine.Object) animatorOverrideController == (UnityEngine.Object) null)
        return;
      this.parameters = new Parameter[animator.parameterCount];
      AnimatorControllerParameter[] parameters = animator.parameters;
      for (int index = 0; index < this.parameters.Length; ++index)
      {
        Parameter parameter = new Parameter();
        parameter.CopyFrom(animator, parameters[index]);
        this.parameters[index] = parameter;
      }
      isInitialized = true;
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += OnPauseEvent;
      OnPauseEvent();
    }

    private void Update()
    {
      Initialize();
      if ((UnityEngine.Object) animator == (UnityEngine.Object) null || (UnityEngine.Object) animatorOverrideController == (UnityEngine.Object) null || !animator.isInitialized || !isInitialized)
        return;
      if (true & animator.GetInteger("Movable.State.Control") == 1)
      {
        animatorState = (MecanimKinds.MovableIdleStateKind) animator.GetInteger("Movable.Idle.Current");
        if (localState == MecanimKinds.MovableIdleStateKind.Unknown)
        {
          SetClip(MecanimKinds.MovableIdleStateKind.Primary);
        }
        else
        {
          if (localState != animatorState || animator.IsInTransition(0))
            return;
          switch (localState)
          {
            case MecanimKinds.MovableIdleStateKind.Primary:
              SetClip(MecanimKinds.MovableIdleStateKind.Secondary);
              break;
            case MecanimKinds.MovableIdleStateKind.Secondary:
              SetClip(MecanimKinds.MovableIdleStateKind.Primary);
              break;
          }
        }
      }
      else
      {
        localState = MecanimKinds.MovableIdleStateKind.Unknown;
        animatorState = MecanimKinds.MovableIdleStateKind.Unknown;
      }
    }

    private void SetClip(MecanimKinds.MovableIdleStateKind state)
    {
      localState = state;
      Clip clip = new Clip();
      bool flag = false;
      float num1 = UnityEngine.Random.value;
      float num2 = 0.0f;
      for (int index = 0; index < this.clips.Length; ++index)
      {
        clip = this.clips[index];
        if (num1 - (double) num2 > 0.0 && num1 - (double) num2 - clip.Probability <= 0.0)
        {
          flag = true;
          break;
        }
        num2 += clip.Probability;
      }
      if (!flag)
        return;
      AnimatorOverrideController animatorController = animator.runtimeAnimatorController as AnimatorOverrideController;
      if ((UnityEngine.Object) animatorController == (UnityEngine.Object) null)
        return;
      string str = null;
      switch (state)
      {
        case MecanimKinds.MovableIdleStateKind.Primary:
          str = "Movable_Idle_Primary";
          break;
        case MecanimKinds.MovableIdleStateKind.Secondary:
          str = "Movable_Idle_Secondary";
          break;
      }
      AnimationClipPair[] clips = animatorController.clips;
      for (int index = 0; index < clips.Length; ++index)
      {
        if (clips[index].originalClip.name == str)
          clips[index].overrideClip = clip.Animation;
      }
      for (int index = 0; index < parameters.Length; ++index)
      {
        Parameter parameter = new Parameter();
        parameter.CopyFrom(animator);
        parameters[index] = parameter;
      }
      AnimatorStateInfo[] animatorStateInfoArray = new AnimatorStateInfo[animator.layerCount];
      for (int layerIndex = 0; layerIndex < animator.layerCount; ++layerIndex)
        animatorStateInfoArray[layerIndex] = animator.GetCurrentAnimatorStateInfo(layerIndex);
      animatorController.clips = clips;
      animator.Update(0.0f);
      for (int index = 0; index < parameters.Length; ++index)
      {
        Parameter parameter = new Parameter();
        parameter.CopyTo(animator);
        parameters[index] = parameter;
      }
      for (int layer = 0; layer < animator.layerCount; ++layer)
        animator.Play(animatorStateInfoArray[layer].fullPathHash, layer, animatorStateInfoArray[layer].normalizedTime);
      animator.Update(0.0f);
    }

    private struct Parameter
    {
      public string Name;
      public AnimatorControllerParameterType Type;
      public float Float;
      public int Int;
      public bool Bool;

      public void CopyTo(Animator Animator)
      {
        switch (Type)
        {
          case AnimatorControllerParameterType.Float:
            Animator.SetFloat(Name, Float);
            break;
          case AnimatorControllerParameterType.Int:
            Animator.SetInteger(Name, Int);
            break;
          case AnimatorControllerParameterType.Bool:
            Animator.SetBool(Name, Bool);
            break;
          case AnimatorControllerParameterType.Trigger:
            if (Bool)
            {
              Animator.SetTrigger(Name);
              break;
            }
            Animator.ResetTrigger(Name);
            break;
        }
      }

      public void CopyFrom(Animator Animator)
      {
        switch (Type)
        {
          case AnimatorControllerParameterType.Float:
            Float = Animator.GetFloat(Name);
            break;
          case AnimatorControllerParameterType.Int:
            Int = Animator.GetInteger(Name);
            break;
          case AnimatorControllerParameterType.Bool:
            Bool = Animator.GetBool(Name);
            break;
          case AnimatorControllerParameterType.Trigger:
            Bool = Animator.GetBool(Name);
            break;
        }
      }

      public void CopyFrom(Animator Animator, AnimatorControllerParameter Parameter)
      {
        Type = Parameter.type;
        Name = Parameter.name;
        CopyFrom(Animator);
      }
    }

    [Serializable]
    public struct Clip
    {
      public AnimationClip Animation;
      public float Probability;
    }
  }
}
