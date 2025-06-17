using System.Collections.Generic;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Utility;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services.Gizmos
{
  [RuntimeService(typeof (GizmoService))]
  public class GizmoService : IInitialisable, IUpdatable
  {
    private bool initialise;
    private bool draw;
    private Material lineMaterial;
    private GUIStyle textStyle;
    private GizmoRenderBehaviour gizmoRenderBehaviour;
    private bool gizmoRenderBehaviourVisible;
    private List<BoxGizmo> boxes = [];
    private List<LineGizmo> lines = [];
    private List<CircleGizmo> circles = [];
    private List<SectorGizmo> sectors = [];
    private List<EyeSectorGizmo> eyeSectors = [];
    private List<TextGigmo3d> texts3d = [];
    private List<TextGigmo> texts = [];
    private List<PositionTextGigmo> positionTexts = [];

    public bool Visible { get; set; } = true;

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    public void ComputeUpdate()
    {
      bool flag = InstanceByRequest<EngineApplication>.Instance.IsDebug && Visible;
      if (flag != gizmoRenderBehaviourVisible && initialise)
      {
        gizmoRenderBehaviourVisible = flag;
        gizmoRenderBehaviour.enabled = gizmoRenderBehaviourVisible;
      }
      if (!flag)
        return;
      CheckInitialise();
    }

    private void CheckInitialise()
    {
      if (initialise)
        return;
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      if (camera == null)
        return;
      gizmoRenderBehaviour = GizmoRenderBehaviour.Create(camera);
      gizmoRenderBehaviourVisible = true;
      initialise = true;
    }

    public void FireDrawShapes()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsDebug || !Visible || !initialise)
        return;
      draw = true;
      if (lineMaterial == null)
        CreateLineMaterial();
      DrawShapes();
    }

    public void FireDrawTexts(UnityEngine.Camera camera)
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsDebug || !Visible || !initialise)
        return;
      draw = true;
      DrawTexts(camera);
    }

    public void Check()
    {
      if (!draw)
        return;
      FireClear();
    }

    public void DrawBox(Vector3 min, Vector3 max, Color color)
    {
      if (draw)
        FireClear();
      boxes.Add(new BoxGizmo {
        Min = min,
        Max = max,
        Color = color
      });
    }

    public void DrawText3d(Vector3 position, string text, TextCorner corner, Color color)
    {
      if (draw)
        FireClear();
      texts3d.Add(new TextGigmo3d {
        Position = position,
        Content = new GUIContent(text),
        Corner = corner,
        Color = color,
        UsePrevPoint = false
      });
    }

    public void DrawText3d(string text, TextCorner corner, Color color)
    {
      if (draw)
        FireClear();
      texts3d.Add(new TextGigmo3d {
        Position = Vector3.zero,
        Content = new GUIContent(text),
        Corner = corner,
        Color = color,
        UsePrevPoint = true
      });
    }

    public void DrawText(Vector2 position, string text, TextCorner corner, Color color)
    {
      if (draw)
        FireClear();
      positionTexts.Add(new PositionTextGigmo {
        Position = position,
        Content = new GUIContent(text),
        Corner = corner,
        Color = color
      });
    }

    public void DrawText(string text, Color color)
    {
      if (draw)
        FireClear();
      texts.Add(new TextGigmo {
        Content = new GUIContent(text),
        Color = color
      });
    }

    public void DrawLine(Vector3 start, Vector3 end, Color color)
    {
      if (draw)
        FireClear();
      lines.Add(new LineGizmo {
        Start = start,
        End = end,
        Color = color
      });
    }

    public void DrawCircle(Vector3 position, float radius, Color color, bool solid = true)
    {
      if (draw)
        FireClear();
      circles.Add(new CircleGizmo {
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
      if (draw)
        FireClear();
      sectors.Add(new SectorGizmo {
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
      if (draw)
        FireClear();
      eyeSectors.Add(new EyeSectorGizmo {
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
      if (boxes.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (BoxGizmo box in boxes)
        {
          GL.Color(box.Color);
          GizmoUtility.DrawBox(box.Min, box.Max);
        }
      }
      if (lines.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (LineGizmo line in lines)
        {
          GL.Color(line.Color);
          GL.Vertex(line.Start);
          GL.Vertex(line.End);
        }
      }
      if (circles.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (CircleGizmo circle in circles)
        {
          GL.Color(circle.Color);
          GizmoUtility.DrawCircle(circle.Position, circle.Radius, circle.Solid);
        }
      }
      if (sectors.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (SectorGizmo sector in sectors)
        {
          GL.Color(sector.Color);
          GizmoUtility.DrawSector(sector.Position, sector.Radius, sector.StartAngle, sector.EndAngle, sector.Solid, FunctionUtility.DefaultFunction);
        }
      }
      if (eyeSectors.Count != 0)
      {
        if (!flag)
        {
          flag = true;
          lineMaterial.SetPass(0);
          GL.Begin(1);
        }
        foreach (EyeSectorGizmo eyeSector in eyeSectors)
        {
          GL.Color(eyeSector.Color);
          GizmoUtility.DrawSector(eyeSector.Position, eyeSector.Radius, eyeSector.StartAngle, eyeSector.EndAngle, eyeSector.Solid, FunctionUtility.EyeFunction);
        }
      }
      if (!flag)
        return;
      GL.End();
    }

    private void DrawTexts(UnityEngine.Camera camera)
    {
      if (textStyle == null)
      {
        textStyle = new GUIStyle();
        textStyle.font = RuntimeInspectedDrawer.Instance.Skin.font;
        textStyle.fontSize = 9;
        textStyle.fontStyle = FontStyle.Normal;
      }
      if (texts3d.Count != 0)
      {
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.wordWrap = false;
        Rect rect = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
        bool flag = false;
        foreach (TextGigmo3d textGigmo3d in texts3d)
        {
          if (!textGigmo3d.UsePrevPoint)
          {
            Vector3 rhs = textGigmo3d.Position - camera.transform.position;
            flag = Vector3.Dot(camera.transform.forward, rhs) <= 0.0;
            if (!flag)
            {
              Vector3 screenPoint = camera.WorldToScreenPoint(textGigmo3d.Position);
              screenPoint.y = Screen.height - screenPoint.y;
              rect = new Rect(screenPoint.x, screenPoint.y, 0.0f, 0.0f);
            }
            else
              continue;
          }
          else if (flag)
            continue;
          GUIContent content = textGigmo3d.Content;
          Vector2 vector2 = textStyle.CalcSize(content);
          if (vector2.x != 0.0 && vector2.y != 0.0)
          {
            Rect position = textGigmo3d.Corner != TextCorner.Up ? (textGigmo3d.Corner != TextCorner.Down ? (textGigmo3d.Corner != TextCorner.Left ? (textGigmo3d.Corner != TextCorner.Right ? new Rect((float) (rect.x + rect.width / 2.0 - vector2.x / 2.0), (float) (rect.y + rect.height / 2.0 - vector2.y / 2.0), vector2.x, vector2.y) : new Rect(rect.x + rect.width, (float) (rect.y + rect.height / 2.0 - vector2.y / 2.0), vector2.x, vector2.y)) : new Rect(rect.x - vector2.x, (float) (rect.y + rect.height / 2.0 - vector2.y / 2.0), vector2.x, vector2.y)) : new Rect((float) (rect.x + rect.width / 2.0 - vector2.x / 2.0), rect.y + rect.height, vector2.x, vector2.y)) : new Rect((float) (rect.x + rect.width / 2.0 - vector2.x / 2.0), rect.y - vector2.y, vector2.x, vector2.y);
            rect = position;
            textStyle.normal.textColor = Color.black;
            GUI.Label(new Rect(position.x + 1f, position.y + 1f, position.width, position.height), content, textStyle);
            textStyle.normal.textColor = textGigmo3d.Color;
            GUI.Label(position, content, textStyle);
          }
        }
      }
      if (texts.Count != 0)
      {
        textStyle.alignment = TextAnchor.UpperLeft;
        textStyle.wordWrap = true;
        Rect position = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
        foreach (TextGigmo text in texts)
        {
          GUIContent content = text.Content;
          float num = textStyle.CalcHeight(content, position.width);
          textStyle.normal.textColor = Color.black;
          GUI.Label(new Rect(position.x + 1f, position.y + 1f, position.width, position.height), content, textStyle);
          textStyle.normal.textColor = text.Color;
          GUI.Label(position, content, textStyle);
          position.y += num;
        }
      }
      if (positionTexts.Count == 0)
        return;
      textStyle.alignment = TextAnchor.MiddleCenter;
      textStyle.wordWrap = false;
      foreach (PositionTextGigmo positionText in positionTexts)
      {
        Rect rect = new Rect(positionText.Position.x, positionText.Position.y, 0.0f, 0.0f);
        GUIContent content = positionText.Content;
        Vector2 vector2 = textStyle.CalcSize(content);
        if (vector2.x != 0.0 && vector2.y != 0.0)
        {
          Rect position = positionText.Corner != TextCorner.Up ? (positionText.Corner != TextCorner.Down ? (positionText.Corner != TextCorner.Left ? (positionText.Corner != TextCorner.Right ? new Rect((float) (rect.x + rect.width / 2.0 - vector2.x / 2.0), (float) (rect.y + rect.height / 2.0 - vector2.y / 2.0), vector2.x, vector2.y) : new Rect(rect.x + rect.width, (float) (rect.y + rect.height / 2.0 - vector2.y / 2.0), vector2.x, vector2.y)) : new Rect(rect.x - vector2.x, (float) (rect.y + rect.height / 2.0 - vector2.y / 2.0), vector2.x, vector2.y)) : new Rect((float) (rect.x + rect.width / 2.0 - vector2.x / 2.0), rect.y + rect.height, vector2.x, vector2.y)) : new Rect((float) (rect.x + rect.width / 2.0 - vector2.x / 2.0), rect.y - vector2.y, vector2.x, vector2.y);
          if (positionText.Color.a == 1.0)
          {
            textStyle.normal.textColor = Color.black;
            GUI.Label(new Rect(position.x + 1f, position.y + 1f, position.width, position.height), content, textStyle);
            textStyle.normal.textColor = positionText.Color;
            GUI.Label(position, content, textStyle);
          }
          else
          {
            textStyle.normal.textColor = positionText.Color;
            GUI.Label(position, content, textStyle);
          }
        }
      }
    }

    private void CreateLineMaterial()
    {
      lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
      lineMaterial.hideFlags = HideFlags.HideAndDontSave;
      lineMaterial.SetInt("_SrcBlend", 5);
      lineMaterial.SetInt("_DstBlend", 10);
      lineMaterial.SetInt("_Cull", 0);
      lineMaterial.SetInt("_ZWrite", 0);
      lineMaterial.SetInt("_ZTest", 0);
    }

    public void FireClear()
    {
      draw = false;
      boxes.Clear();
      lines.Clear();
      circles.Clear();
      sectors.Clear();
      eyeSectors.Clear();
      texts3d.Clear();
      texts.Clear();
      positionTexts.Clear();
    }
  }
}
