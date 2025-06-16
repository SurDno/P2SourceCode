// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.JerboaService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System.Collections;
using System.Xml;
using UnityEngine;

#nullable disable
namespace Engine.Impl.Services
{
  [GameService(new System.Type[] {typeof (JerboaService), typeof (IJerboaService)})]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class JerboaService : IJerboaService, IUpdatable, IInitialisable, ISavesController
  {
    private GameObject prefabInstance;
    private JerboaManager jerboaManager;
    private float quality = 1f;
    private float amount = 0.0f;
    private JerboaColorEnum color = JerboaColorEnum.Default;

    public float Quality
    {
      get => this.quality;
      set
      {
        this.quality = value;
        if (!((UnityEngine.Object) this.jerboaManager != (UnityEngine.Object) null))
          return;
        this.jerboaManager.Quality = this.quality;
      }
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public float Amount
    {
      get => this.amount;
      set
      {
        this.amount = value;
        if (!((UnityEngine.Object) this.jerboaManager != (UnityEngine.Object) null))
          return;
        this.jerboaManager.Weight = this.amount;
      }
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public JerboaColorEnum Color
    {
      get => this.color;
      set
      {
        this.color = value;
        if (!((UnityEngine.Object) this.jerboaManager != (UnityEngine.Object) null))
          return;
        this.jerboaManager.ColorEnum = this.color;
      }
    }

    public void Syncronize()
    {
      if (!((UnityEngine.Object) this.jerboaManager != (UnityEngine.Object) null))
        return;
      this.jerboaManager.Syncronize();
    }

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      if (!((UnityEngine.Object) this.prefabInstance != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.prefabInstance);
      this.prefabInstance = (GameObject) null;
    }

    public void ComputeUpdate()
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      Vector3 position = ((IEntityView) player).Position;
      if ((UnityEngine.Object) this.prefabInstance == (UnityEngine.Object) null && (UnityEngine.Object) ScriptableObjectInstance<ResourceFromCodeData>.Instance.JerboaPrefab != (UnityEngine.Object) null)
      {
        this.prefabInstance = UnityEngine.Object.Instantiate<GameObject>(ScriptableObjectInstance<ResourceFromCodeData>.Instance.JerboaPrefab, position, Quaternion.identity);
        this.jerboaManager = this.prefabInstance.GetComponent<JerboaManager>();
        if ((UnityEngine.Object) this.jerboaManager == (UnityEngine.Object) null)
        {
          Debug.Log((object) "Jerboa prefab doesn't contain JerboaManager component");
        }
        else
        {
          this.jerboaManager.Weight = this.amount;
          this.jerboaManager.ColorEnum = this.color;
          this.jerboaManager.Quality = this.quality;
          this.jerboaManager.Syncronize();
        }
      }
      LocationItemComponent component = player.GetComponent<LocationItemComponent>();
      this.jerboaManager.Visible = component == null || !component.IsIndoor;
      if (!((UnityEngine.Object) this.prefabInstance != (UnityEngine.Object) null))
        return;
      this.prefabInstance.transform.position = position;
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      XmlElement node = element[TypeUtility.GetTypeName(this.GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(this.GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader((XmlNode) node, context);
        ((ISerializeStateLoad) this).StateLoad((IDataReader) reader, this.GetType());
        yield break;
      }
    }

    public void Unload()
    {
      if (!((UnityEngine.Object) this.prefabInstance != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.prefabInstance);
      this.prefabInstance = (GameObject) null;
    }

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<JerboaService>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }
  }
}
