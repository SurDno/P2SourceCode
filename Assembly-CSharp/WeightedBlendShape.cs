// Decompiled with JetBrains decompiler
// Type: WeightedBlendShape
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[Serializable]
public class WeightedBlendShape
{
  public string blendName;
  public float weight;
  public int blendIdx = -1;

  public WeightedBlendShape()
  {
    this.blendName = "";
    this.weight = 0.0f;
  }

  public WeightedBlendShape(string _blendName, float _weight)
  {
    this.blendName = _blendName;
    this.weight = _weight;
  }

  public WeightedBlendShape(string _blendName, float _weight, int _blendIdx)
  {
    this.blendName = _blendName;
    this.weight = _weight;
    this.blendIdx = _blendIdx;
  }
}
