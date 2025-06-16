// Decompiled with JetBrains decompiler
// Type: StateSetters.StateSetterItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace StateSetters
{
  [Serializable]
  public class StateSetterItem
  {
    public string Type;
    public string StringValue1;
    public float FloatValue1;
    public float FloatValue2;
    public int IntValue1;
    public int IntValue2;
    public bool BoolValue1;
    public UnityEngine.Object ObjectValue1;
    public Color ColorValue1;
    public Color ColorValue2;

    public void Apply(float value) => StateSetterService.Apply(this, value);
  }
}
