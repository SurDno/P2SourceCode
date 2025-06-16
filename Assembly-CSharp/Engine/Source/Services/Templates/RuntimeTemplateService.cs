// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Templates.RuntimeTemplateService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.Templates
{
  [Depend(typeof (IFactory))]
  [RuntimeService(new System.Type[] {typeof (ITemplateService)})]
  public class RuntimeTemplateService : ITemplateService, IAsyncInitializable, IInitialisable
  {
    private Dictionary<Guid, IObject> items = new Dictionary<Guid, IObject>((IEqualityComparer<Guid>) GuidComparer.Instance);

    [Inspected]
    private int Count => this.items.Count;

    public void Initialise()
    {
      IEnumerator enumerator = this.AsyncInitialize();
      while (enumerator.MoveNext())
      {
        object current = enumerator.Current;
      }
    }

    public int AsyncCount => TemplateLoader.Instance.AsyncCount;

    public IEnumerator AsyncInitialize()
    {
      Dictionary<Guid, string> names = (Dictionary<Guid, string>) null;
      this.Clear();
      InitialiseEngineProgressService progressService = ServiceLocator.GetService<InitialiseEngineProgressService>();
      progressService.Update("RuntimeTemplateLoader", "Prepare");
      yield return (object) TemplateLoader.Instance.Load(this.items, names);
      progressService.Update(TypeUtility.GetTypeName(this.GetType()), "Extract Inventory Templates");
      this.ExtractInventoryTemplates(names);
      progressService.Update(TypeUtility.GetTypeName(this.GetType()), "Extract Templates");
      this.ExtractTemplates(names);
      progressService.Update(TypeUtility.GetTypeName(this.GetType()), "Complete");
    }

    public void Terminate() => this.Clear();

    private void ExtractTemplates(Dictionary<Guid, string> names)
    {
      foreach (SceneObject sceneObject in this.GetTemplates<SceneObject>().ToList<SceneObject>())
      {
        sceneObject.Name = Path.GetFileNameWithoutExtension(sceneObject.Source);
        foreach (SceneObjectItem sceneObjectItem in sceneObject.Items)
          this.ExtractTemplate((IScene) sceneObject, sceneObjectItem, names);
      }
    }

    private void ExtractInventoryTemplates(Dictionary<Guid, string> names)
    {
      foreach (IEntity entity in this.GetTemplates<IEntity>().ToList<IEntity>())
      {
        StorageComponent component = entity.GetComponent<StorageComponent>();
        if (component != null)
        {
          foreach (TemplateInfo inventoryTemplateInfo in component.InventoryTemplateInfos)
          {
            IEntity template1 = this.GetTemplate<IEntity>(inventoryTemplateInfo.InventoryTemplate.Id);
            if (template1 != null)
            {
              Entity template2 = (Entity) ServiceLocator.GetService<Factory>().Instantiate(typeof (IEntity), inventoryTemplateInfo.Id, (object) template1);
              template2.Name = template1.Name;
              TemplateLoaderUtility.AddTemplateImpl((IObject) template2, nameof (ExtractInventoryTemplates), this.items, names);
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
        this.ExtractTemplate(sceneObject, sceneObjectItem1, names);
      string path = AssetDatabaseService.Instance.GetPath(sceneObject.Id);
      Guid id1 = sceneObjectItem.Origin.Id;
      IEntity template1 = sceneObjectItem.Origin.Value;
      if (template1 != null)
      {
        Guid id2 = sceneObjectItem.Id;
        if (this.items.TryGetValue(id2, out IObject _))
          return;
        Entity template2 = (Entity) ServiceLocator.GetService<Factory>().Instantiate(typeof (IEntity), id2, (object) template1);
        template2.Name = sceneObjectItem.PreserveName;
        TemplateLoaderUtility.AddTemplateImpl((IObject) template2, nameof (ExtractTemplate), this.items, names);
      }
      else if (id1 != Guid.Empty)
        Debug.LogError((object) ("Origin template not found : " + (object) id1 + " , scene context : " + path));
      Entity entity = (Entity) sceneObjectItem.Template.Value;
      Guid id3 = sceneObjectItem.Template.Id;
      if (entity != null)
        entity.Name = sceneObjectItem.PreserveName;
      else if (id3 != Guid.Empty)
        Debug.LogError((object) ("Template not found : " + (object) id3 + " , scene context : " + path));
      if (!(id1 != Guid.Empty) || !(id3 != Guid.Empty))
        return;
      Debug.LogError((object) ("Template and Origin at the same time, origin : " + (object) id1 + " , template : " + (object) id3 + " , scene context : " + path));
    }

    public IObject GetTemplate(System.Type type, Guid id)
    {
      IObject template;
      this.items.TryGetValue(id, out template);
      return template;
    }

    public T GetTemplate<T>(Guid id) where T : class, IObject
    {
      return this.GetTemplate(typeof (T), id) as T;
    }

    public IEnumerable<IObject> GetTemplates(System.Type type)
    {
      foreach (KeyValuePair<Guid, IObject> keyValuePair in this.items)
      {
        KeyValuePair<Guid, IObject> item = keyValuePair;
        IObject result = item.Value;
        if (TypeUtility.IsAssignableFrom(type, result.GetType()))
          yield return result;
        result = (IObject) null;
        item = new KeyValuePair<Guid, IObject>();
      }
    }

    public IEnumerable<T> GetTemplates<T>() where T : class, IObject
    {
      foreach (KeyValuePair<Guid, IObject> keyValuePair in this.items)
      {
        KeyValuePair<Guid, IObject> item = keyValuePair;
        if (item.Value is T result)
          yield return result;
        result = default (T);
        item = new KeyValuePair<Guid, IObject>();
      }
    }

    private void Clear() => this.items.Clear();
  }
}
