// Decompiled with JetBrains decompiler
// Type: Engine.Assets.Objects.StepsAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Engine.Assets.Objects
{
  [Serializable]
  public class StepsAction
  {
    public StepsEvent[] Events;
    public float MaxThesholdIntensity;
    public float MinThesholdIntensity;
    public PhysicMaterial Material;
  }
}
