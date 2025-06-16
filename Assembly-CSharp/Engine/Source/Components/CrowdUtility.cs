using System.Collections.Generic;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components.Crowds;

namespace Engine.Source.Components
{
  public static class CrowdUtility
  {
    public static List<CrowdPointInfo> Points = new List<CrowdPointInfo>(1024);

    public static void SetAsCrowd(IEntity entity)
    {
      ((Entity) entity).DontSave = true;
      if (entity.Childs == null)
        return;
      foreach (Entity child in entity.Childs)
        child.DontSave = true;
    }
  }
}
