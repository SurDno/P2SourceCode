using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetDatabases;
using Cofe.Utility;
using Engine.Assets.Internal.Reference;
using Engine.Common;
using Engine.Common.Comparers;
using Engine.Common.Services;
using Engine.Impl;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Services.Templates
{
  [Depend(typeof (IFactory))]
  [RuntimeService(typeof (ITemplateService))]
  public class RuntimeTemplateService : ITemplateService, IAsyncInitializable, IInitialisable
  {
    private Dictionary<Guid, IObject> items = new Dictionary<Guid, IObject>(GuidComparer.Instance);

    [Inspected]
    private int Count => items.Count;

    public void Initialise()
    {
      IEnumerator enumerator = AsyncInitialize();
      while (enumerator.MoveNext())
      {
        object current = enumerator.Current;
      }
    }

    public int AsyncCount => TemplateLoader.Instance.AsyncCount;

    public IEnumerator AsyncInitialize()
    {
      Dictionary<Guid, string> names = null;
      Clear();
      InitialiseEngineProgressService progressService = ServiceLocator.GetService<InitialiseEngineProgressService>();
      progressService.Update("RuntimeTemplateLoader", "Prepare");
      yield return TemplateLoader.Instance.Load(items, names);
      progressService.Update(TypeUtility.GetTypeName(GetType()), "Extract Inventory Templates");
      ExtractInventoryTemplates(names);
      progressService.Update(TypeUtility.GetTypeName(GetType()), "Extract Templates");
      ExtractTemplates(names);
      progressService.Update(TypeUtility.GetTypeName(GetType()), "Complete");
    }

    public void Terminate() => Clear();

    private void ExtractTemplates(Dictionary<Guid, string> names)
    {
      foreach (SceneObject sceneObject in GetTemplates<SceneObject>().ToList())
      {
        sceneObject.Name = Path.GetFileNameWithoutExtension(sceneObject.Source);
        foreach (SceneObjectItem sceneObjectItem in sceneObject.Items)
          ExtractTemplate(sceneObject, sceneObjectItem, names);
      }
    }

    private void ExtractInventoryTemplates(Dictionary<Guid, string> names)
    {
      foreach (IEntity entity in GetTemplates<IEntity>().ToList())
      {
        StorageComponent component = entity.GetComponent<StorageComponent>();
        if (component != null)
        {
          foreach (TemplateInfo inventoryTemplateInfo in component.InventoryTemplateInfos)
          {
            IEntity template1 = GetTemplate<IEntity>(inventoryTemplateInfo.InventoryTemplate.Id);
            if (template1 != null)
            {
              Entity template2 = (Entity) ServiceLocator.GetService<Factory>().Instantiate(typeof (IEntity), inventoryTemplateInfo.Id, template1);
              template2.Name = template1.Name;
              TemplateLoaderUtility.AddTemplateImpl(template2, nameof (ExtractInventoryTemplates), items, names);
            }
          }
        }
      }
    }

    private void ExtractTemplate(
      IScene sceneObject,
      SceneObjectItem sceneObjectItem,
      Dictionary<Guid, string> names)
    {
      foreach (SceneObjectItem sceneObjectItem1 in sceneObjectItem.Items)
        ExtractTemplate(sceneObject, sceneObjectItem1, names);
      string path = AssetDatabaseService.Instance.GetPath(sceneObject.Id);
      Guid id1 = sceneObjectItem.Origin.Id;
      IEntity template1 = sceneObjectItem.Origin.Value;
      if (template1 != null)
      {
        Guid id2 = sceneObjectItem.Id;
        if (items.TryGetValue(id2, out IObject _))
          return;
        Entity template2 = (Entity) ServiceLocator.GetService<Factory>().Instantiate(typeof (IEntity), id2, template1);
        template2.Name = sceneObjectItem.PreserveName;
        TemplateLoaderUtility.AddTemplateImpl(template2, nameof (ExtractTemplate), items, names);
      }
      else if (id1 != Guid.Empty)
        Debug.LogError((object) ("Origin template not found : " + id1 + " , scene context : " + path));
      Entity entity = (Entity) sceneObjectItem.Template.Value;
      Guid id3 = sceneObjectItem.Template.Id;
      if (entity != null)
        entity.Name = sceneObjectItem.PreserveName;
      else if (id3 != Guid.Empty)
        Debug.LogError((object) ("Template not found : " + id3 + " , scene context : " + path));
      if (!(id1 != Guid.Empty) || !(id3 != Guid.Empty))
        return;
      Debug.LogError((object) ("Template and Origin at the same time, origin : " + id1 + " , template : " + id3 + " , scene context : " + path));
    }

    public IObject GetTemplate(Type type, Guid id)
    {
      IObject template;
      items.TryGetValue(id, out template);
      return template;
    }

    public T GetTemplate<T>(Guid id) where T : class, IObject
    {
      return GetTemplate(typeof (T), id) as T;
    }

    public IEnumerable<IObject> GetTemplates(Type type)
    {
      foreach (KeyValuePair<Guid, IObject> keyValuePair in items)
      {
        KeyValuePair<Guid, IObject> item = keyValuePair;
        IObject result = item.Value;
        if (TypeUtility.IsAssignableFrom(type, result.GetType()))
          yield return result;
        result = null;
        item = new KeyValuePair<Guid, IObject>();
      }
    }

    public IEnumerable<T> GetTemplates<T>() where T : class, IObject
    {
      foreach (KeyValuePair<Guid, IObject> keyValuePair in items)
      {
        KeyValuePair<Guid, IObject> item = keyValuePair;
        if (item.Value is T result)
          yield return result;
        result = default (T);
        item = new KeyValuePair<Guid, IObject>();
      }
    }

    private void Clear() => items.Clear();
  }
}
