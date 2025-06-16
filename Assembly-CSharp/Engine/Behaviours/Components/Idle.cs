using Engine.Impl.Tasks;
using Engine.Source.Commons;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Behaviours.Components
{
  public class Idle : MonoBehaviour
  {
    private Animator animator;
    private MecanimKinds.MovableIdleStateKind animatorState = MecanimKinds.MovableIdleStateKind.Unknown;
    private AnimatorOverrideController animatorOverrideController;
    [SerializeField]
    [FormerlySerializedAs("_Clips")]
    private Idle.Clip[] clips;
    private bool isInitialized;
    private MecanimKinds.MovableIdleStateKind localState = MecanimKinds.MovableIdleStateKind.Unknown;
    private Idle.Parameter[] parameters;

    public Animator Animator
    {
      get => this.animator;
      protected set => this.animator = value;
    }

    private void OnPauseEvent()
    {
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        return;
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        this.animator.SetFloat("Mecanim.Speed", 0.0f);
      else
        this.animator.SetFloat("Mecanim.Speed", 1f);
    }

    private void Initialize()
    {
      if (this.isInitialized)
        return;
      this.animator = this.GetComponent<Animator>();
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        return;
      this.animatorOverrideController = this.animator.runtimeAnimatorController as AnimatorOverrideController;
      if ((UnityEngine.Object) this.animatorOverrideController == (UnityEngine.Object) null)
        return;
      this.parameters = new Idle.Parameter[this.animator.parameterCount];
      AnimatorControllerParameter[] parameters = this.animator.parameters;
      for (int index = 0; index < this.parameters.Length; ++index)
      {
        Idle.Parameter parameter = new Idle.Parameter();
        parameter.CopyFrom(this.animator, parameters[index]);
        this.parameters[index] = parameter;
      }
      this.isInitialized = true;
      InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += new Action(this.OnPauseEvent);
      this.OnPauseEvent();
    }

    private void Update()
    {
      this.Initialize();
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null || (UnityEngine.Object) this.animatorOverrideController == (UnityEngine.Object) null || !this.animator.isInitialized || !this.isInitialized)
        return;
      if (true & this.animator.GetInteger("Movable.State.Control") == 1)
      {
        this.animatorState = (MecanimKinds.MovableIdleStateKind) this.animator.GetInteger("Movable.Idle.Current");
        if (this.localState == MecanimKinds.MovableIdleStateKind.Unknown)
        {
          this.SetClip(MecanimKinds.MovableIdleStateKind.Primary);
        }
        else
        {
          if (this.localState != this.animatorState || this.animator.IsInTransition(0))
            return;
          switch (this.localState)
          {
            case MecanimKinds.MovableIdleStateKind.Primary:
              this.SetClip(MecanimKinds.MovableIdleStateKind.Secondary);
              break;
            case MecanimKinds.MovableIdleStateKind.Secondary:
              this.SetClip(MecanimKinds.MovableIdleStateKind.Primary);
              break;
          }
        }
      }
      else
      {
        this.localState = MecanimKinds.MovableIdleStateKind.Unknown;
        this.animatorState = MecanimKinds.MovableIdleStateKind.Unknown;
      }
    }

    private void SetClip(MecanimKinds.MovableIdleStateKind state)
    {
      this.localState = state;
      Idle.Clip clip = new Idle.Clip();
      bool flag = false;
      float num1 = UnityEngine.Random.value;
      float num2 = 0.0f;
      for (int index = 0; index < this.clips.Length; ++index)
      {
        clip = this.clips[index];
        if ((double) num1 - (double) num2 > 0.0 && (double) num1 - (double) num2 - (double) clip.Probability <= 0.0)
        {
          flag = true;
          break;
        }
        num2 += clip.Probability;
      }
      if (!flag)
        return;
      AnimatorOverrideController animatorController = this.animator.runtimeAnimatorController as AnimatorOverrideController;
      if ((UnityEngine.Object) animatorController == (UnityEngine.Object) null)
        return;
      string str = (string) null;
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
      for (int index = 0; index < this.parameters.Length; ++index)
      {
        Idle.Parameter parameter = new Idle.Parameter();
        parameter.CopyFrom(this.animator);
        this.parameters[index] = parameter;
      }
      AnimatorStateInfo[] animatorStateInfoArray = new AnimatorStateInfo[this.animator.layerCount];
      for (int layerIndex = 0; layerIndex < this.animator.layerCount; ++layerIndex)
        animatorStateInfoArray[layerIndex] = this.animator.GetCurrentAnimatorStateInfo(layerIndex);
      animatorController.clips = clips;
      this.animator.Update(0.0f);
      for (int index = 0; index < this.parameters.Length; ++index)
      {
        Idle.Parameter parameter = new Idle.Parameter();
        parameter.CopyTo(this.animator);
        this.parameters[index] = parameter;
      }
      for (int layer = 0; layer < this.animator.layerCount; ++layer)
        this.animator.Play(animatorStateInfoArray[layer].fullPathHash, layer, animatorStateInfoArray[layer].normalizedTime);
      this.animator.Update(0.0f);
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
        switch (this.Type)
        {
          case AnimatorControllerParameterType.Float:
            Animator.SetFloat(this.Name, this.Float);
            break;
          case AnimatorControllerParameterType.Int:
            Animator.SetInteger(this.Name, this.Int);
            break;
          case AnimatorControllerParameterType.Bool:
            Animator.SetBool(this.Name, this.Bool);
            break;
          case AnimatorControllerParameterType.Trigger:
            if (this.Bool)
            {
              Animator.SetTrigger(this.Name);
              break;
            }
            Animator.ResetTrigger(this.Name);
            break;
        }
      }

      public void CopyFrom(Animator Animator)
      {
        switch (this.Type)
        {
          case AnimatorControllerParameterType.Float:
            this.Float = Animator.GetFloat(this.Name);
            break;
          case AnimatorControllerParameterType.Int:
            this.Int = Animator.GetInteger(this.Name);
            break;
          case AnimatorControllerParameterType.Bool:
            this.Bool = Animator.GetBool(this.Name);
            break;
          case AnimatorControllerParameterType.Trigger:
            this.Bool = Animator.GetBool(this.Name);
            break;
        }
      }

      public void CopyFrom(Animator Animator, AnimatorControllerParameter Parameter)
      {
        this.Type = Parameter.type;
        this.Name = Parameter.name;
        this.CopyFrom(Animator);
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
