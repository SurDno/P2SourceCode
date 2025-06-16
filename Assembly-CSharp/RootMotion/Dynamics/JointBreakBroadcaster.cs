// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.JointBreakBroadcaster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  public class JointBreakBroadcaster : MonoBehaviour
  {
    [SerializeField]
    [HideInInspector]
    public PuppetMaster puppetMaster;
    [SerializeField]
    [HideInInspector]
    public int muscleIndex;

    private void OnJointBreak()
    {
      if (!this.enabled)
        return;
      this.puppetMaster.RemoveMuscleRecursive(this.puppetMaster.muscles[this.muscleIndex].joint, true, true, MuscleRemoveMode.Numb);
    }
  }
}
