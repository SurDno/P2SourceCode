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
    msStart = _start;
    msPreFadeStart = _start;
    msEnd = _end;
    msPostFadeEnd = _end;
    anim = _label;
    weight = _weight;
  }

  public int StartMilli
  {
    get => msStart;
    set
    {
      int num = msStart - msPreFadeStart;
      msStart = value;
      msPreFadeStart = msStart - num;
    }
  }

  public int EndMilli
  {
    get => msEnd;
    set
    {
      int num = msPostFadeEnd - msEnd;
      msEnd = value;
      msPostFadeEnd = msEnd + num;
    }
  }

  public int PostFadeEnd
  {
    get => msPostFadeEnd;
    set => msPostFadeEnd = value;
  }

  public int PreFadeStart
  {
    get => msPreFadeStart;
    set => msPreFadeStart = value;
  }

  public string AnimLabel
  {
    get => anim;
    set => anim = value;
  }

  public float Weight
  {
    get => weight;
    set => weight = value;
  }

  public float MakeLinearBlend(float msTime)
  {
    float num1 = msTime;
    float num2 = 0.0f;
    if (msTime < (double) StartMilli)
    {
      float preFadeStart = PreFadeStart;
      float startMilli = StartMilli;
      num2 = Weight * (float) ((num1 - (double) preFadeStart) / (startMilli - (double) preFadeStart));
    }
    else if (msTime < (double) EndMilli)
      num2 = Weight;
    else if (msTime < (double) PostFadeEnd)
    {
      float endMilli = EndMilli;
      float postFadeEnd = PostFadeEnd;
      num2 = Weight * (float) ((postFadeEnd - (double) num1) / (postFadeEnd - (double) endMilli));
    }
    return num2;
  }

  public float MakeCurveBlend(float msTime)
  {
    float num1 = msTime;
    float num2 = 0.0f;
    if (msTime < (double) StartMilli)
    {
      float preFadeStart = PreFadeStart;
      float startMilli = StartMilli;
      num2 = Weight * Mathf.Sin((float) ((num1 - (double) preFadeStart) / (startMilli - (double) preFadeStart)) * 1.57079637f);
    }
    else if (msTime < (double) EndMilli)
      num2 = Weight;
    else if (msTime < (double) PostFadeEnd)
    {
      float endMilli = EndMilli;
      float postFadeEnd = PostFadeEnd;
      num2 = Weight * Mathf.Sin((float) ((postFadeEnd - (double) num1) / (postFadeEnd - (double) endMilli)) * 1.57079637f);
    }
    return num2;
  }
}
