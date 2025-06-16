// Decompiled with JetBrains decompiler
// Type: ProtagonistLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ProtagonistLight : MonoBehaviour
{
  private void LateUpdate()
  {
    ProtagonistShadersSettings instance = MonoBehaviourInstance<ProtagonistShadersSettings>.Instance;
    if ((Object) instance == (Object) null)
      return;
    Vector3 position = this.transform.parent.position;
    this.transform.position = instance.ProtagonistToWorld(position);
  }
}
