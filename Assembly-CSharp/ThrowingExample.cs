// Decompiled with JetBrains decompiler
// Type: ThrowingExample
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ThrowingExample : MonoBehaviour
{
  [SerializeField]
  private KeyCode key;
  [SerializeField]
  private GameObject prefab;
  [SerializeField]
  private LayerMask layerMask;

  private void Update()
  {
    if (!Input.GetKeyDown(this.key))
      return;
    this.Throw();
  }

  private void Throw()
  {
    Vector3 position = this.transform.position;
    Vector3 forward = this.transform.forward;
    RaycastHit hitInfo;
    if (!Physics.Raycast(position + forward, forward, out hitInfo, 50f, (int) this.layerMask, QueryTriggerInteraction.Ignore))
      return;
    GameObject gameObject = Object.Instantiate<GameObject>(this.prefab);
    gameObject.transform.position = hitInfo.point;
    gameObject.transform.rotation = Quaternion.LookRotation(forward);
  }
}
