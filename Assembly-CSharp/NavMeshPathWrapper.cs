// Decompiled with JetBrains decompiler
// Type: NavMeshPathWrapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using UnityEngine.AI;

#nullable disable
public class NavMeshPathWrapper
{
  private NavMeshPath path;

  public NavMeshPathWrapper(NavMeshPath path) => this.path = path;

  [Inspected]
  private int CornersCount => this.path.corners.Length;

  [Inspected]
  private NavMeshPathStatus Status => this.path.status;
}
