// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Crowds.CrowdPointInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Movable;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components.Crowds
{
  public struct CrowdPointInfo
  {
    [Inspected]
    public GameObject GameObject;
    [Inspected]
    public int Radius;
    [Inspected]
    public Vector3 CenterPoint;
    [Inspected]
    public Vector3 Position;
    [Inspected]
    public Quaternion Rotation;
    [Inspected]
    public AreaEnum Area;
    [Inspected]
    public IEntity EntityPoint;
    [Inspected]
    public bool OnNavMesh;
    [Inspected]
    public bool NotReady;
  }
}
