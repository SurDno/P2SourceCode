// Decompiled with JetBrains decompiler
// Type: XMLMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class XMLMarker
{
  private int msStart;
  private int msEnd;
  private int msPreFadeStart;
  private int msPostFadeEnd;
  private float weight;
  public string anim = "";

  public XMLMarker(string _label, int _start, int _end, float _weight)
  {
    this.msStart = _start;
    this.msPreFadeStart = _start;
    this.msEnd = _end;
    this.msPostFadeEnd = _end;
    this.anim = _label;
    this.weight = _weight;
  }

  public int StartMilli
  {
    get => this.msStart;
    set
    {
      int num = this.msStart - this.msPreFadeStart;
      this.msStart = value;
      this.msPreFadeStart = this.msStart - num;
    }
  }

  public int EndMilli
  {
    get => this.msEnd;
    set
    {
      int num = this.msPostFadeEnd - this.msEnd;
      this.msEnd = value;
      this.msPostFadeEnd = this.msEnd + num;
    }
  }

  public int PostFadeEnd
  {
    get => this.msPostFadeEnd;
    set => this.msPostFadeEnd = value;
  }

  public int PreFadeStart
  {
    get => this.msPreFadeStart;
    set => this.msPreFadeStart = value;
  }

  public string AnimLabel
  {
    get => this.anim;
    set => this.anim = value;
  }

  public float Weight
  {
    get => this.weight;
    set => this.weight = value;
  }

  public float MakeLinearBlend(float msTime)
  {
    float num1 = msTime;
    float num2 = 0.0f;
    if ((double) msTime < (double) this.StartMilli)
    {
      float preFadeStart = (float) this.PreFadeStart;
      float startMilli = (float) this.StartMilli;
      num2 = this.Weight * (float) (((double) num1 - (double) preFadeStart) / ((double) startMilli - (double) preFadeStart));
    }
    else if ((double) msTime < (double) this.EndMilli)
      num2 = this.Weight;
    else if ((double) msTime < (double) this.PostFadeEnd)
    {
      float endMilli = (float) this.EndMilli;
      float postFadeEnd = (float) this.PostFadeEnd;
      num2 = this.Weight * (float) (((double) postFadeEnd - (double) num1) / ((double) postFadeEnd - (double) endMilli));
    }
    return num2;
  }

  public float MakeCurveBlend(float msTime)
  {
    float num1 = msTime;
    float num2 = 0.0f;
    if ((double) msTime < (double) this.StartMilli)
    {
      float preFadeStart = (float) this.PreFadeStart;
      float startMilli = (float) this.StartMilli;
      num2 = this.Weight * Mathf.Sin((float) (((double) num1 - (double) preFadeStart) / ((double) startMilli - (double) preFadeStart)) * 1.57079637f);
    }
    else if ((double) msTime < (double) this.EndMilli)
      num2 = this.Weight;
    else if ((double) msTime < (double) this.PostFadeEnd)
    {
      float endMilli = (float) this.EndMilli;
      float postFadeEnd = (float) this.PostFadeEnd;
      num2 = this.Weight * Mathf.Sin((float) (((double) postFadeEnd - (double) num1) / ((double) postFadeEnd - (double) endMilli)) * 1.57079637f);
    }
    return num2;
  }
}
