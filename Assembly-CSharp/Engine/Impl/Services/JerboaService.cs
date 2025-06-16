using System.Collections;
using System.Xml;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Inspectors;
using JerboaAnimationInstancing;

namespace Engine.Impl.Services
{
  [GameService(typeof (JerboaService), typeof (IJerboaService))]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class JerboaService : IJerboaService, IUpdatable, IInitialisable, ISavesController
  {
    private GameObject prefabInstance;
    private JerboaManager jerboaManager;
    private float quality = 1f;
    private float amount;
    private JerboaColorEnum color = JerboaColorEnum.Default;

    public float Quality
    {
      get => quality;
      set
      {
        quality = value;
        if (!((UnityEngine.Object) jerboaManager != (UnityEngine.Object) null))
          return;
        jerboaManager.Quality = quality;
      }
    }

    [StateSaveProxy]
    [StateLoadProxy()]
    [Inspected(Mutable = true)]
    public float Amount
    {
      get => amount;
      set
      {
        amount = value;
        if (!((UnityEngine.Object) jerboaManager != (UnityEngine.Object) null))
          return;
        jerboaManager.Weight = amount;
      }
    }

    [StateSaveProxy]
    [StateLoadProxy()]
    [Inspected(Mutable = true)]
    public JerboaColorEnum Color
    {
      get => color;
      set
      {
        color = value;
        if (!((UnityEngine.Object) jerboaManager != (UnityEngine.Object) null))
          return;
        jerboaManager.ColorEnum = color;
      }
    }

    public void Syncronize()
    {
      if (!((UnityEngine.Object) jerboaManager != (UnityEngine.Object) null))
        return;
      jerboaManager.Syncronize();
    }

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
      if (!((UnityEngine.Object) prefabInstance != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) prefabInstance);
      prefabInstance = (GameObject) null;
    }

    public void ComputeUpdate()
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      Vector3 position = ((IEntityView) player).Position;
      if ((UnityEngine.Object) prefabInstance == (UnityEngine.Object) null && (UnityEngine.Object) ScriptableObjectInstance<ResourceFromCodeData>.Instance.JerboaPrefab != (UnityEngine.Object) null)
      {
        prefabInstance = UnityEngine.Object.Instantiate<GameObject>(ScriptableObjectInstance<ResourceFromCodeData>.Instance.JerboaPrefab, position, Quaternion.identity);
        jerboaManager = prefabInstance.GetComponent<JerboaManager>();
        if ((UnityEngine.Object) jerboaManager == (UnityEngine.Object) null)
        {
          Debug.Log((object) "Jerboa prefab doesn't contain JerboaManager component");
        }
        else
        {
          jerboaManager.Weight = amount;
          jerboaManager.ColorEnum = color;
          jerboaManager.Quality = quality;
          jerboaManager.Syncronize();
        }
      }
      LocationItemComponent component = player.GetComponent<LocationItemComponent>();
      jerboaManager.Visible = component == null || !component.IsIndoor;
      if (!((UnityEngine.Object) prefabInstance != (UnityEngine.Object) null))
        return;
      prefabInstance.transform.position = position;
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      XmlElement node = element[TypeUtility.GetTypeName(GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader(node, context);
        ((ISerializeStateLoad) this).StateLoad(reader, GetType());
        yield break;
      }
    }

    public void Unload()
    {
      if (!((UnityEngine.Object) prefabInstance != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) prefabInstance);
      prefabInstance = (GameObject) null;
    }

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
    }
  }
}
