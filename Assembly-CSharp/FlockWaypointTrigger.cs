// Decompiled with JetBrains decompiler
// Type: FlockWaypointTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FlockWaypointTrigger : MonoBehaviour
{
  public float _timer = 1f;
  public FlockChild _flockChild;

  public void Start()
  {
    if ((Object) this._flockChild == (Object) null)
      this._flockChild = this.transform.parent.GetComponent<FlockChild>();
    float num = Random.Range(this._timer, this._timer * 3f);
    this.InvokeRepeating("Trigger", num, num);
  }

  public void Trigger() => this._flockChild.Wander(0.0f);
}
