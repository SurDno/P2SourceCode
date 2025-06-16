using SRDebugger.Services;
using SRF;
using SRF.Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Controls
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (RectTransform))]
  [RequireComponent(typeof (CanvasRenderer))]
  public class ProfilerGraphControl : Graphic
  {
    public ProfilerGraphControl.VerticalAlignments VerticalAlignment = ProfilerGraphControl.VerticalAlignments.Bottom;
    private static readonly float[] ScaleSteps = new float[9]
    {
      0.005f,
      1f / 160f,
      0.008333334f,
      0.01f,
      0.0166666675f,
      0.0333333351f,
      0.05f,
      0.0833333358f,
      0.166666672f
    };
    public bool FloatingScale;
    public bool TargetFpsUseApplication;
    public bool DrawAxes = true;
    public int TargetFps = 60;
    public bool Clip = true;
    public const float DataPointMargin = 2f;
    public const float DataPointVerticalMargin = 2f;
    public const float DataPointWidth = 4f;
    public int VerticalPadding = 10;
    public const int LineCount = 4;
    public Color[] LineColours = new Color[0];
    private IProfilerService _profilerService;
    private ProfilerGraphAxisLabel[] _axisLabels;
    private Rect _clipBounds;
    private readonly List<Vector3> _meshVertices = new List<Vector3>();
    private readonly List<Color32> _meshVertexColors = new List<Color32>();
    private readonly List<int> _meshTriangles = new List<int>();

    protected override void Awake()
    {
      base.Awake();
      this._profilerService = SRServiceManager.GetService<IProfilerService>();
    }

    protected void Update() => this.SetVerticesDirty();

    [Obsolete]
    protected override void OnPopulateMesh(Mesh m)
    {
      this._meshVertices.Clear();
      this._meshVertexColors.Clear();
      this._meshTriangles.Clear();
      float width = this.rectTransform.rect.width;
      float height = this.rectTransform.rect.height;
      this._clipBounds = new Rect(0.0f, 0.0f, width, height);
      int num1 = this.TargetFps;
      if (Application.isPlaying && this.TargetFpsUseApplication && Application.targetFrameRate > 0)
        num1 = Application.targetFrameRate;
      float frameTime = 1f / (float) num1;
      int index1 = -1;
      float num2 = this.FloatingScale ? this.CalculateMaxFrameTime() : 1f / (float) num1;
      if (this.FloatingScale)
      {
        for (int index2 = 0; index2 < ProfilerGraphControl.ScaleSteps.Length; ++index2)
        {
          float scaleStep = ProfilerGraphControl.ScaleSteps[index2];
          if ((double) num2 < (double) scaleStep * 1.1000000238418579)
          {
            frameTime = scaleStep;
            index1 = index2;
            break;
          }
        }
        if (index1 < 0)
        {
          index1 = ProfilerGraphControl.ScaleSteps.Length - 1;
          frameTime = ProfilerGraphControl.ScaleSteps[index1];
        }
      }
      else
      {
        for (int index3 = 0; index3 < ProfilerGraphControl.ScaleSteps.Length; ++index3)
        {
          float scaleStep = ProfilerGraphControl.ScaleSteps[index3];
          if ((double) num2 > (double) scaleStep)
            index1 = index3;
        }
      }
      float verticalScale = (height - (float) (this.VerticalPadding * 2)) / frameTime;
      int visibleDataPointCount = this.CalculateVisibleDataPointCount();
      int bufferCurrentSize = this.GetFrameBufferCurrentSize();
      for (int index4 = 0; index4 < bufferCurrentSize && index4 < visibleDataPointCount; ++index4)
      {
        ProfilerFrame frame = this.GetFrame(bufferCurrentSize - index4 - 1);
        this.DrawDataPoint((float) ((double) width - 4.0 * (double) index4 - 4.0 - (double) width / 2.0), verticalScale, frame);
      }
      if (this.DrawAxes)
      {
        if (!this.FloatingScale)
          this.DrawAxis(frameTime, frameTime * verticalScale, this.GetAxisLabel(0));
        int num3 = 2;
        int index5 = 0;
        if (!this.FloatingScale)
          ++index5;
        for (int index6 = index1; index6 >= 0 && index5 < num3; --index6)
        {
          this.DrawAxis(ProfilerGraphControl.ScaleSteps[index6], ProfilerGraphControl.ScaleSteps[index6] * verticalScale, this.GetAxisLabel(index5));
          ++index5;
        }
      }
      m.Clear();
      m.SetVertices(this._meshVertices);
      m.SetColors(this._meshVertexColors);
      m.SetTriangles(this._meshTriangles, 0);
    }

    protected void DrawDataPoint(float xPosition, float verticalScale, ProfilerFrame frame)
    {
      float x = Mathf.Min(this._clipBounds.width / 2f, (float) ((double) xPosition + 4.0 - 2.0));
      float num1 = 0.0f;
      for (int index1 = 0; index1 < 4; ++index1)
      {
        int index2 = index1;
        float num2 = 0.0f;
        if (index1 == 0)
          num2 = (float) frame.CustomTime;
        else if (index1 == 1)
          num2 = (float) frame.UpdateTime;
        else if (index1 == 2)
          num2 = (float) frame.RenderTime;
        else if (index1 == 3)
          num2 = (float) frame.OtherTime;
        float f = num2 * verticalScale;
        if (!f.ApproxZero() && (double) f - 4.0 >= 0.0)
        {
          float y1 = (float) ((double) num1 + 2.0 - (double) this.rectTransform.rect.height / 2.0);
          if (this.VerticalAlignment == ProfilerGraphControl.VerticalAlignments.Top)
            y1 = (float) ((double) this.rectTransform.rect.height / 2.0 - (double) num1 - 2.0);
          float y2 = (float) ((double) y1 + (double) f - 2.0);
          if (this.VerticalAlignment == ProfilerGraphControl.VerticalAlignments.Top)
            y2 = (float) ((double) y1 - (double) f + 2.0);
          Color lineColour = this.LineColours[index2];
          this.AddRect(new Vector3(Mathf.Max((float) (-(double) this._clipBounds.width / 2.0), xPosition), y1), new Vector3(Mathf.Max((float) (-(double) this._clipBounds.width / 2.0), xPosition), y2), new Vector3(x, y2), new Vector3(x, y1), lineColour);
          num1 += f;
        }
      }
    }

    protected void DrawAxis(float frameTime, float yPosition, ProfilerGraphAxisLabel label)
    {
      float x1 = (float) (-(double) this.rectTransform.rect.width * 0.5);
      float x2 = -x1;
      float y1 = (float) ((double) yPosition - (double) this.rectTransform.rect.height * 0.5 + 0.5);
      float y2 = (float) ((double) yPosition - (double) this.rectTransform.rect.height * 0.5 - 0.5);
      Color c = new Color(1f, 1f, 1f, 0.4f);
      this.AddRect(new Vector3(x1, y2), new Vector3(x1, y1), new Vector3(x2, y1), new Vector3(x2, y2), c);
      if (!((UnityEngine.Object) label != (UnityEngine.Object) null))
        return;
      label.SetValue(frameTime, yPosition);
    }

    protected void AddRect(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br, Color c)
    {
      this._meshVertices.Add(tl);
      this._meshVertices.Add(tr);
      this._meshVertices.Add(bl);
      this._meshVertices.Add(br);
      this._meshTriangles.Add(this._meshVertices.Count - 4);
      this._meshTriangles.Add(this._meshVertices.Count - 3);
      this._meshTriangles.Add(this._meshVertices.Count - 1);
      this._meshTriangles.Add(this._meshVertices.Count - 2);
      this._meshTriangles.Add(this._meshVertices.Count - 1);
      this._meshTriangles.Add(this._meshVertices.Count - 3);
      this._meshVertexColors.Add((Color32) c);
      this._meshVertexColors.Add((Color32) c);
      this._meshVertexColors.Add((Color32) c);
      this._meshVertexColors.Add((Color32) c);
    }

    protected ProfilerFrame GetFrame(int i) => this._profilerService.FrameBuffer[i];

    protected int CalculateVisibleDataPointCount()
    {
      return Mathf.RoundToInt(this.rectTransform.rect.width / 4f);
    }

    protected int GetFrameBufferCurrentSize() => this._profilerService.FrameBuffer.Size;

    protected int GetFrameBufferMaxSize() => this._profilerService.FrameBuffer.Capacity;

    protected float CalculateMaxFrameTime()
    {
      int bufferCurrentSize = this.GetFrameBufferCurrentSize();
      int num = Mathf.Min(this.CalculateVisibleDataPointCount(), bufferCurrentSize);
      double maxFrameTime = 0.0;
      for (int index = 0; index < num; ++index)
      {
        ProfilerFrame frame = this.GetFrame(bufferCurrentSize - index - 1);
        if (frame.FrameTime > maxFrameTime)
          maxFrameTime = frame.FrameTime;
      }
      return (float) maxFrameTime;
    }

    private ProfilerGraphAxisLabel GetAxisLabel(int index)
    {
      if (this._axisLabels == null || !Application.isPlaying)
        this._axisLabels = this.GetComponentsInChildren<ProfilerGraphAxisLabel>();
      if (this._axisLabels.Length > index)
        return this._axisLabels[index];
      Debug.LogWarning((object) "[SRDebugger.Profiler] Not enough axis labels in pool");
      return (ProfilerGraphAxisLabel) null;
    }

    public enum VerticalAlignments
    {
      Top,
      Bottom,
    }
  }
}
