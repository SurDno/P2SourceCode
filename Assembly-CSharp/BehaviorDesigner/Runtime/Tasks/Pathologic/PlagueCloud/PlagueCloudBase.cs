// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.PlagueCloud.PlagueCloudBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic.PlagueCloud
{
  public abstract class PlagueCloudBase : Action
  {
    private global::PlagueCloud[] clouds;

    public abstract global::PlagueCloud.VisibilityType GetVisibilityType();

    public override void OnStart()
    {
      if (this.clouds != null)
        return;
      this.clouds = this.gameObject.GetComponentsInChildren<global::PlagueCloud>();
      if (this.clouds != null)
        return;
      Debug.LogError((object) (this.gameObject.name + ": doesn't contain " + typeof (global::PlagueCloud).Name + " unity component"), (Object) this.gameObject);
    }

    public override TaskStatus OnUpdate()
    {
      if (this.clouds == null)
        return TaskStatus.Failure;
      for (int index = 0; index < this.clouds.Length; ++index)
        this.clouds[index].Visibility = this.GetVisibilityType();
      return TaskStatus.Success;
    }
  }
}
