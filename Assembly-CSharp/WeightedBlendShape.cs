using System;

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
