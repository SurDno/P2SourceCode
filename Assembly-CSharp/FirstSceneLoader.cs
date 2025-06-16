using System;
using System.Collections.Generic;
using Engine.Behaviours;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.HierarchyServices;
using Engine.Source.Commons;

public class FirstSceneLoader : MonoBehaviour
{
  private void Start()
  {
    InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent += OnViewEnabledEvent;
  }

  private void OnDestroy()
  {
    InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent -= OnViewEnabledEvent;
  }

  private void OnViewEnabledEvent(bool enabled)
  {
    SceneObjectContainer container = SceneObjectContainer.GetContainer(this.gameObject.scene);
    if ((UnityEngine.Object) container == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) (typeof (SceneObjectContainer).Name + " not found, path : " + this.gameObject.scene.path + " , root count : " + (object) this.gameObject.scene.GetRootGameObjects().Length), (UnityEngine.Object) this);
    }
    else
    {
      Guid id = container.GetId(this.gameObject);
      if (id == Guid.Empty)
      {
        Debug.LogWarning((object) "GameObject not found", (UnityEngine.Object) this);
      }
      else
      {
        IEntity entity1 = null;
        foreach (HierarchyItem hierarchyItem in ServiceLocator.GetService<HierarchyService>().MainContainer.Items)
        {
          if (hierarchyItem.Reference != null && hierarchyItem.Reference.Id == id)
          {
            entity1 = hierarchyItem.Template;
            break;
          }
        }
        if (entity1 == null)
        {
          Debug.LogWarning((object) "Template not found", (UnityEngine.Object) this);
        }
        else
        {
          IEntity entity2 = null;
          IEnumerable<IEntity> childs = ServiceLocator.GetService<ISimulation>().Hierarchy.Childs;
          if (childs != null)
          {
            foreach (IEntity entity3 in childs)
            {
              if (entity3.TemplateId == entity1.Id)
              {
                entity2 = entity3;
                break;
              }
            }
          }
          if (entity2 == null)
            Debug.LogWarning((object) "Entity not found", (UnityEngine.Object) this);
          else
            ((IEntityViewSetter) entity2).GameObject = enabled ? this.gameObject : (GameObject) null;
        }
      }
    }
  }
}
