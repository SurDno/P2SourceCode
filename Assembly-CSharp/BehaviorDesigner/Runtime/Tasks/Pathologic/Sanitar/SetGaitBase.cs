// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.Sanitar.SetGaitBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic.Sanitar
{
  public abstract class SetGaitBase : Action
  {
    protected EngineBehavior behavior;

    public abstract EngineBehavior.GaitType GetGait();

    public override TaskStatus OnUpdate()
    {
      if ((Object) this.behavior == (Object) null)
      {
        this.behavior = this.gameObject.GetComponent<EngineBehavior>();
        if ((Object) this.behavior == (Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (BehaviorComponent).Name + " engine component"), (Object) this.gameObject);
          return TaskStatus.Failure;
        }
      }
      this.behavior.Gait = this.GetGait();
      return TaskStatus.Success;
    }
  }
}
