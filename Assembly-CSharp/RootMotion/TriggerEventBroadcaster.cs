// Decompiled with JetBrains decompiler
// Type: RootMotion.TriggerEventBroadcaster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion
{
  public class TriggerEventBroadcaster : MonoBehaviour
  {
    public GameObject target;

    private void OnTriggerEnter(Collider collider)
    {
      if (!((Object) this.target != (Object) null))
        return;
      this.target.SendMessage(nameof (OnTriggerEnter), (object) collider, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerStay(Collider collider)
    {
      if (!((Object) this.target != (Object) null))
        return;
      this.target.SendMessage(nameof (OnTriggerStay), (object) collider, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerExit(Collider collider)
    {
      if (!((Object) this.target != (Object) null))
        return;
      this.target.SendMessage(nameof (OnTriggerExit), (object) collider, SendMessageOptions.DontRequireReceiver);
    }
  }
}
