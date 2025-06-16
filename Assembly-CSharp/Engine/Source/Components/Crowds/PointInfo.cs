// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Crowds.PointInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Movable;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components.Crowds
{
  public class PointInfo
  {
    [Inspected]
    public Vector3 Position;
    [Inspected]
    public Quaternion Rotation;
    [Inspected]
    public IEntity Region;
    [Inspected]
    public AreaEnum Area;
    [Inspected]
    public IEntity Entity;
    [Inspected]
    public List<IParameter> States = new List<IParameter>();
    [Inspected]
    public bool OnNavMesh;
    [Inspected]
    public IEntity EntityPoint;

    public GameObject GameObject
    {
      get
      {
        return this.EntityPoint != null ? ((IEntityView) this.EntityPoint).GameObject : (GameObject) null;
      }
    }
  }
}
