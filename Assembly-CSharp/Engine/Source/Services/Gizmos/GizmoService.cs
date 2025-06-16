using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Utility;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Services.Gizmos
{
  [RuntimeService(new System.Type[] {typeof (GizmoService)})]
  public class GizmoService : IInitialisable, IUpdatable
  {
    private bool initialise;
    private bool draw;
    private Material lineMaterial;
    private GUIStyle textStyle;
    private GizmoRenderBehaviour gizmoRenderBehaviour;
    private bool gizmoRenderBehaviourVisible;
    private List<BoxGizmo> boxes = new List<BoxGizmo>();
    private List<LineGizmo> lines = new List<LineGizmo>();
    private List<CircleGizmo> circles = new List<CircleGizmo>();
    private List<SectorGizmo> sectors = new List<SectorGizmo>();
    private List<EyeSectorGizmo> eyeSectors = new List<EyeSectorGizmo>();
    private List<TextGigmo3d> texts3d = new List<TextGigmo3d>();
    private List<TextGigmo> texts = new List<TextGigmo>();
    private List<PositionTextGigmo> positionTexts = new List<PositionTextGigmo>();

    public bool Visible { get; set; } = true;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void ComputeUpdate()
    {
      bool flag = InstanceByRequest<EngineApplication>.Instance.IsDebug && this.Visible;
      if (flag != this.gizmoRenderBehaviourVisible && this.initialise)
      {
        this.gizmoRenderBehaviourVisible = flag;
        this.gizmoRenderBehaviour.enabled = this.gizmoRenderBehaviourVisible;
      }
      if (!flag)
        return;
      this.CheckInitialise();
    }

    private void CheckInitialise()
    {
      if (this.initialise)
        return;
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      if ((UnityEngine.Object) camera == (UnityEngine.Object) null)
        return;
      this.gizmoRenderBehaviour = GizmoRenderBehaviour.Create(camera);
      this.gizmoRenderBehaviourVisible = true;
      this.initialise = true;
    }

    public void FireDrawShapes()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsDebug || !this.Visible || !this.initialise)
        return;
      this.draw = true;
      if ((UnityEngine.Object) this.lineMaterial == (UnityEngine.Object) null)
        this.CreateLineMaterial();
      this.DrawShapes();
    }

    public void FireDrawTexts(UnityEngine.Camera camera)
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsDebug || !this.Visible || !this.initialise)
        return;
      this.draw = true;
      this.DrawTexts(camera);
    }

    public void Check()
    {
      if (!this.draw)
        return;
      this.FireClear();
    }

    public void DrawBox(Vector3 min, Vector3 max, Color color)
    {
      if (this.draw)
        this.FireClear();
      this.boxes.Add(new BoxGizmo()
      {
        Min = min,
        Max = max,
        Color = color
      });
    }

    public void DrawText3d(Vector3 position, string text, TextCorner corner, Color color)
    {
      if (this.draw)
        this.FireClear();
      this.texts3d.Add(new TextGigmo3d()
      {
        Position = position,
        Content = new GUIContent(text),
        Corner = corner,
        Color = color,
        UsePrevPoint = false
      });
    }

    public void DrawText3d(string text, TextCorner corner, Color color)
    {
      if (this.draw)
        this.FireClear();
      this.texts3d.Add(new TextGigmo3d()
      {
        Position = Vector3.zero,
        Content = new GUIContent(text),
        Corner = corner,
        Color = color,
        UsePrevPoint = true
      });
    }

    public void DrawText(Vector2 position, string text, TextCorner corner, Color color)
    {
      if (this.draw)
        this.FireClear();
      this.positionTexts.Add(new PositionTextGigmo()
      {
        Position = position,
        Content = new GUIContent(text),
        Corner = corner,
        Color = color
      });
    }

    public void DrawText(string text, Color color)
    {
      if (this.draw)
        this.FireClear();
      this.texts.Add(new TextGigmo()
      {
        Content = new GUIContent(text),
        Color = color
      });
    }

    public void DrawLine(Vector3 start, Vector3 end, Color color)
    {
      if (this.draw)
        this.FireClear();
      this.lines.Add(new LineGizmo()
      {
        Start = start,
        End = end,
        Color = color
      });
    }

    public void DrawCircle(Vector3 position, float radius, Color color, bool solid = true)
    {
      if (this.draw)
        this.FireClear();
      this.circles.Add(new CircleGizmo()
      {
        Position = position,
        Radius = radius,
        Color = color,
        Solid = solid
      });
    }

    public void DrawSector(
      Vector3 position,
      float radius,
      float startAngle,
      float endAngle,
      Color color,
      bool solid = true)
    {
      if (this.draw)
        this.FireClear();
      this.sectors.Add(new SectorGizmo()
      {
        Position = position,
        Radius = radius,
        StartAngle = startAngle,
        EndAngle = endAngle,
        Color = color,
        Solid = solid
      });
    }

    public void DrawEyeSector(
      Vector3 position,
      float radius,
      float startAngle,
      float endAngle,
      Color color,
      bool solid = true)
    {
      if (this.draw)
        this.FireClear();
      this.eyeSectors.Add(new EyeSectorGizmo()
      {
        Position = position,
        Radius = radius,
        StartAngle = startAngle,
        EndAngle = endAngle,
        Color = color,
        Solid = solid
      });
    }

    private void DrawShapes()
    {
      bool flag = false;
      if (this.boxes.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          this.lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (BoxGizmo box in this.boxes)
        {
          GL.Color(box.Color);
          GizmoUtility.DrawBox(box.Min, box.Max);
        }
      }
      if (this.lines.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          this.lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (LineGizmo line in this.lines)
        {
          GL.Color(line.Color);
          GL.Vertex(line.Start);
          GL.Vertex(line.End);
        }
      }
      if (this.circles.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          this.lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (CircleGizmo circle in this.circles)
        {
          GL.Color(circle.Color);
          GizmoUtility.DrawCircle(circle.Position, circle.Radius, circle.Solid);
        }
      }
      if (this.sectors.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          this.lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (SectorGizmo sector in this.sectors)
        {
          GL.Color(sector.Color);
          GizmoUtility.DrawSector(sector.Position, sector.Radius, sector.StartAngle, sector.EndAngle, sector.Solid, new Func<float, float, float>(FunctionUtility.DefaultFunction));
        }
      }
      if (this.eyeSectors.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          this.lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (EyeSectorGizmo eyeSector in this.eyeSectors)
        {
          GL.Color(eyeSector.Color);
          GizmoUtility.DrawSector(eyeSector.Position, eyeSector.Radius, eyeSector.StartAngle, eyeSector.EndAngle, eyeSector.Solid, new Func<float, float, float>(FunctionUtility.EyeFunction));
        }
      }
      if (!flag)
        return;
      GL.End();
    }

    private void DrawTexts(UnityEngine.Camera camera)
    {
      if (this.textStyle == null)
      {
        this.textStyle = new GUIStyle();
        this.textStyle.font = RuntimeInspectedDrawer.Instance.Skin.font;
        this.textStyle.fontSize = 9;
        this.textStyle.fontStyle = FontStyle.Normal;
      }
      if (this.texts3d.Count != 0)
      {
        this.textStyle.alignment = TextAnchor.MiddleCenter;
        this.textStyle.wordWrap = false;
        Rect rect = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
        bool flag = false;
        foreach (TextGigmo3d textGigmo3d in this.texts3d)
        {
          if (!textGigmo3d.UsePrevPoint)
          {
            Vector3 rhs = textGigmo3d.Position - camera.transform.position;
            flag = (double) Vector3.Dot(camera.transform.forward, rhs) <= 0.0;
            if (!flag)
            {
              Vector3 screenPoint = camera.WorldToScreenPoint(textGigmo3d.Position);
              screenPoint.y = (float) Screen.height - screenPoint.y;
              rect = new Rect(screenPoint.x, screenPoint.y, 0.0f, 0.0f);
            }
            else
              continue;
          }
          else if (flag)
            continue;
          GUIContent content = textGigmo3d.Content;
          Vector2 vector2 = this.textStyle.CalcSize(content);
          if ((double) vector2.x != 0.0 && (double) vector2.y != 0.0)
          {
            Rect position = textGigmo3d.Corner != TextCorner.Up ? (textGigmo3d.Corner != TextCorner.Down ? (textGigmo3d.Corner != TextCorner.Left ? (textGigmo3d.Corner != TextCorner.Right ? new Rect((float) ((double) rect.x + (double) rect.width / 2.0 - (double) vector2.x / 2.0), (float) ((double) rect.y + (double) rect.height / 2.0 - (double) vector2.y / 2.0), vector2.x, vector2.y) : new Rect(rect.x + rect.width, (float) ((double) rect.y + (double) rect.height / 2.0 - (double) vector2.y / 2.0), vector2.x, vector2.y)) : new Rect(rect.x - vector2.x, (float) ((double) rect.y + (double) rect.height / 2.0 - (double) vector2.y / 2.0), vector2.x, vector2.y)) : new Rect((float) ((double) rect.x + (double) rect.width / 2.0 - (double) vector2.x / 2.0), rect.y + rect.height, vector2.x, vector2.y)) : new Rect((float) ((double) rect.x + (double) rect.width / 2.0 - (double) vector2.x / 2.0), rect.y - vector2.y, vector2.x, vector2.y);
            rect = position;
            this.textStyle.normal.textColor = Color.black;
            GUI.Label(new Rect(position.x + 1f, position.y + 1f, position.width, position.height), content, this.textStyle);
            this.textStyle.normal.textColor = textGigmo3d.Color;
            GUI.Label(position, content, this.textStyle);
          }
        }
      }
      if (this.texts.Count != 0)
      {
        this.textStyle.alignment = TextAnchor.UpperLeft;
        this.textStyle.wordWrap = true;
        Rect position = new Rect(0.0f, 0.0f, (float) Screen.width, (float) Screen.height);
        foreach (TextGigmo text in this.texts)
        {
          GUIContent content = text.Content;
          float num = this.textStyle.CalcHeight(content, position.width);
          this.textStyle.normal.textColor = Color.black;
          GUI.Label(new Rect(position.x + 1f, position.y + 1f, position.width, position.height), content, this.textStyle);
          this.textStyle.normal.textColor = text.Color;
          GUI.Label(position, content, this.textStyle);
          position.y += num;
        }
      }
      if (this.positionTexts.Count == 0)
        return;
      this.textStyle.alignment = TextAnchor.MiddleCenter;
      this.textStyle.wordWrap = false;
      foreach (PositionTextGigmo positionText in this.positionTexts)
      {
        Rect rect = new Rect(positionText.Position.x, positionText.Position.y, 0.0f, 0.0f);
        GUIContent content = positionText.Content;
        Vector2 vector2 = this.textStyle.CalcSize(content);
        if ((double) vector2.x != 0.0 && (double) vector2.y != 0.0)
        {
          Rect position = positionText.Corner != TextCorner.Up ? (positionText.Corner != TextCorner.Down ? (positionText.Corner != TextCorner.Left ? (positionText.Corner != TextCorner.Right ? new Rect((float) ((double) rect.x + (double) rect.width / 2.0 - (double) vector2.x / 2.0), (float) ((double) rect.y + (double) rect.height / 2.0 - (double) vector2.y / 2.0), vector2.x, vector2.y) : new Rect(rect.x + rect.width, (float) ((double) rect.y + (double) rect.height / 2.0 - (double) vector2.y / 2.0), vector2.x, vector2.y)) : new Rect(rect.x - vector2.x, (float) ((double) rect.y + (double) rect.height / 2.0 - (double) vector2.y / 2.0), vector2.x, vector2.y)) : new Rect((float) ((double) rect.x + (double) rect.width / 2.0 - (double) vector2.x / 2.0), rect.y + rect.height, vector2.x, vector2.y)) : new Rect((float) ((double) rect.x + (double) rect.width / 2.0 - (double) vector2.x / 2.0), rect.y - vector2.y, vector2.x, vector2.y);
          if ((double) positionText.Color.a == 1.0)
          {
            this.textStyle.normal.textColor = Color.black;
            GUI.Label(new Rect(position.x + 1f, position.y + 1f, position.width, position.height), content, this.textStyle);
            this.textStyle.normal.textColor = positionText.Color;
            GUI.Label(position, content, this.textStyle);
          }
          else
          {
            this.textStyle.normal.textColor = positionText.Color;
            GUI.Label(position, content, this.textStyle);
          }
        }
      }
    }

    private void CreateLineMaterial()
    {
      this.lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
      this.lineMaterial.hideFlags = HideFlags.HideAndDontSave;
      this.lineMaterial.SetInt("_SrcBlend", 5);
      this.lineMaterial.SetInt("_DstBlend", 10);
      this.lineMaterial.SetInt("_Cull", 0);
      this.lineMaterial.SetInt("_ZWrite", 0);
      this.lineMaterial.SetInt("_ZTest", 0);
    }

    public void FireClear()
    {
      this.draw = false;
      this.boxes.Clear();
      this.lines.Clear();
      this.circles.Clear();
      this.sectors.Clear();
      this.eyeSectors.Clear();
      this.texts3d.Clear();
      this.texts.Clear();
      this.positionTexts.Clear();
    }
  }
}
