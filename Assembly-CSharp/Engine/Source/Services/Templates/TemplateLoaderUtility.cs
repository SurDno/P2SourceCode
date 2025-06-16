using System;
using System.Collections.Generic;
using System.IO;
using AssetDatabases;
using Engine.Common;
using Engine.Source.Commons;

namespace Engine.Source.Services.Templates
{
  public static class TemplateLoaderUtility
  {
    public static IObject LoadObject(string path)
    {
      TextAsset context = AssetDatabaseService.Instance.Load<TextAsset>(path);
      if ((UnityEngine.Object) context != (UnityEngine.Object) null && context.bytes != null)
      {
        using (MemoryStream memoryStream = new MemoryStream(context.bytes))
        {
          IObject @object = SerializeUtility.Deserialize<IObject>(memoryStream, path);
          if (@object != null)
            return @object;
          Debug.LogError((object) ("Error deserialize template, path : " + path), (UnityEngine.Object) context);
          return null;
        }
      }

      Debug.LogError((object) ("Error load template, path : " + path), (UnityEngine.Object) context);
      return null;
    }

    public static void AddTemplateImpl(
      IObject template,
      string asset,
      Dictionary<Guid, IObject> items,
      Dictionary<Guid, string> names)
    {
      if (template is ITemplateSetter templateSetter)
        templateSetter.IsTemplate = true;
      items.Add(template.Id, template);
    }
  }
}
