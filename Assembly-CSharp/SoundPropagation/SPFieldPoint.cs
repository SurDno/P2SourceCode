// Decompiled with JetBrains decompiler
// Type: SoundPropagation.SPFieldPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace SoundPropagation
{
  public class SPFieldPoint : MonoBehaviour
  {
    [SerializeField]
    private SPFieldSource sourcePrefab;

    public Vector3 Position { get; private set; }

    private void OnEnable()
    {
      if ((Object) this.sourcePrefab == (Object) null)
        return;
      this.Position = this.transform.position;
      SPFieldSource.AddPoint(this.sourcePrefab, this);
    }

    private void OnDisable()
    {
      if ((Object) this.sourcePrefab == (Object) null)
        return;
      SPFieldSource.RemovePoint(this.sourcePrefab, this);
    }
  }
}
