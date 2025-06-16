// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Detectablies.DetectableCandidatInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.Detectablies
{
  public struct DetectableCandidatInfo
  {
    [Inspected]
    public DetectableComponent Detectable;
    [Inspected]
    public ILocationItemComponent LocationItem;
    [Inspected]
    public GameObject GameObject;
    [Inspected]
    public Vector3 Offset;
  }
}
