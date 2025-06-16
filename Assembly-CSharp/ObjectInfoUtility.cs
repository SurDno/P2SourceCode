using AssetDatabases;
using Cofe.Utility;
using Engine.Common;
using Engine.Source.Commons;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ObjectInfoUtility
{
  private static StringBuilder tmp = new StringBuilder(2048);
  private static Thread thread = Thread.CurrentThread;

  public static StringBuilder GetStream()
  {
    if (ObjectInfoUtility.thread != Thread.CurrentThread)
    {
      UnityEngine.Debug.LogException(new Exception("Get Stream from wrong thread"));
      return new StringBuilder();
    }
    ObjectInfoUtility.tmp.Clear();
    return ObjectInfoUtility.tmp;
  }

  public static string GetHierarchyPath(this IEntity entity)
  {
    return ObjectInfoUtility.GetStream().GetHierarchyPath(entity).ToString();
  }

  public static string GetInfo(this object target)
  {
    return ObjectInfoUtility.GetStream().GetInfo(target).ToString();
  }

  public static string GetInfo(this IObject obj)
  {
    return ObjectInfoUtility.GetStream().GetInfo(obj).ToString();
  }

  public static string GetInfo(this UnityEngine.Object obj)
  {
    return ObjectInfoUtility.GetStream().GetInfo(obj).ToString();
  }

  public static string GetFullName(this GameObject go)
  {
    return ObjectInfoUtility.GetStream().GetFullName(go).ToString();
  }

  public static string GetStackTrace() => ObjectInfoUtility.GetStream().GetStackTrace(2).ToString();

  public static StringBuilder GetStackTrace(this StringBuilder info, int startIndex = 1)
  {
    StackTrace stackTrace = new StackTrace(true);
    for (int index = startIndex; index < stackTrace.FrameCount; ++index)
    {
      StackFrame frame = stackTrace.GetFrame(index);
      string fileName = frame.GetFileName();
      if (fileName != null)
      {
        info.Append("   ");
        info.Append(fileName);
        info.Append(" : ");
        info.Append(frame.GetFileLineNumber());
        info.Append("\n");
      }
    }
    return info;
  }

  public static StringBuilder GetHierarchyPath(this StringBuilder info, IEntity entity)
  {
    if (entity == null)
    {
      info.Append("[null]");
      return info;
    }
    if (entity.IsTemplate)
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(AssetDatabaseService.Instance.GetPath(entity.Id));
      info.Append(withoutExtension);
      return info;
    }
    info.Append("Simulation");
    info.GetHierarchyPathInner(entity);
    return info;
  }

  private static StringBuilder GetHierarchyPathInner(this StringBuilder info, IEntity entity)
  {
    if (entity.Parent != null)
      info.GetHierarchyPathInner(entity.Parent);
    info.Append("/");
    info.Append(entity.Name ?? "");
    return info;
  }

  public static StringBuilder GetInfo(this StringBuilder info, object target)
  {
    if (target == null)
    {
      info.Append("[null]");
      return info;
    }
    UnityEngine.Object object1 = target as UnityEngine.Object;
    if (object1 != (UnityEngine.Object) null)
    {
      info.GetInfo(object1);
      return info;
    }
    if (target is IObject object2)
    {
      info.GetInfo(object2);
      return info;
    }
    info.Append(target.ToString());
    return info;
  }

  private static StringBuilder GetInfo(this StringBuilder info, IObject obj)
  {
    if (obj == null)
    {
      info.Append("[null]");
      return info;
    }
    info.Append("[");
    if (obj.IsTemplate)
    {
      info.Append("template");
      info.Append(" , id : ");
      info.Append((object) obj.Id);
      info.Append(" , name : ");
      info.Append(Path.GetFileNameWithoutExtension(obj.Source));
      info.Append(" , source : ");
      info.Append(obj.Source);
    }
    else
    {
      info.Append("instance");
      info.Append(" , id : ");
      info.Append((object) obj.Id);
      info.Append(" , name : ");
      info.Append(obj.Name);
      if (obj is Entity entity)
      {
        info.Append(" , hierarchy path : ");
        info.GetHierarchyPath((IEntity) entity);
        string context = entity.Context;
        info.Append(" , context : ");
        info.Append(context ?? "[null]");
      }
    }
    if (obj.TemplateId != Guid.Empty)
    {
      info.Append(" , template : ");
      info.GetInfo(obj.Template);
    }
    info.Append("]");
    return info;
  }

  public static StringBuilder GetInfo(this StringBuilder info, UnityEngine.Object obj)
  {
    if (obj == (UnityEngine.Object) null)
    {
      info.Append("[null]");
      return info;
    }
    info.Append("type : ");
    info.Append(TypeUtility.GetTypeName(((object) obj).GetType()));
    info.Append(" , to string : ");
    info.Append(((object) obj).ToString());
    GameObject go = obj as GameObject;
    if ((UnityEngine.Object) go != (UnityEngine.Object) null)
    {
      info.Append(" , path : ");
      info.GetFullName(go);
      return info;
    }
    Component component = obj as Component;
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return info;
    info.Append(" , path : ");
    info.GetFullName(component.gameObject);
    return info;
  }

  public static StringBuilder GetFullName(this StringBuilder info, GameObject go)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      info.Append("[null]");
      return info;
    }
    Scene scene = go.scene;
    if (scene.IsValid())
    {
      info.Append("[");
      info.Append(scene.name);
      info.Append("]");
    }
    info.GetFullNameInner(go);
    return info;
  }

  public static StringBuilder GetFullNameInner(this StringBuilder info, GameObject go)
  {
    if ((UnityEngine.Object) go.transform.parent != (UnityEngine.Object) null)
      info.GetFullNameInner(go.transform.parent.gameObject);
    info.Append("/");
    info.Append(go.name);
    return info;
  }
}
