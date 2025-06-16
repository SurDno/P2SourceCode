// Decompiled with JetBrains decompiler
// Type: CrowdPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Movable;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class CrowdPoint : MonoBehaviour
{
  [SerializeField]
  [FormerlySerializedAs("Area")]
  private AreaEnum area;
  [SerializeField]
  private bool onNavMesh;

  public AreaEnum Area => this.area;

  public bool OnNavMesh => this.onNavMesh;
}
