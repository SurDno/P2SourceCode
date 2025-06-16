// Decompiled with JetBrains decompiler
// Type: Pathologic.Prototype.TragedianSquare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

#nullable disable
namespace Pathologic.Prototype
{
  public class TragedianSquare : MonoBehaviour
  {
    private Vector3 _lastPosition;
    public Transform CapsuleTransform;
    public bool TragedianA;

    private void Start()
    {
      this._lastPosition = Vector3.zero;
      if (this.TragedianA)
        this.GetComponent<Animator>().SetBool("TragedianA", true);
      else
        this.GetComponent<Animator>().SetBool("TragedianB", true);
    }

    private void Update()
    {
      this.GetComponent<CapsuleCollider>().center = this.gameObject.transform.InverseTransformPoint(this.CapsuleTransform.position);
      if ((double) (this._lastPosition - this.CapsuleTransform.position).magnitude <= 0.30000001192092896)
        return;
      this.GetComponent<NavMeshObstacle>().center = this.GetComponent<CapsuleCollider>().center;
      this._lastPosition = this.CapsuleTransform.position;
    }
  }
}
