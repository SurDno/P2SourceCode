using System;
using System.Collections;
using System.Xml;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using Inspectors;

namespace Engine.Source.Services
{
  [GameService(typeof (InterfaceBlockingService), typeof (IInterfaceBlockingService))]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class InterfaceBlockingService : IInterfaceBlockingService, ISavesController
  {
    [StateSaveProxy]
    [StateLoadProxy]
    protected bool mapInterfaceBlocked;
    [StateSaveProxy]
    [StateLoadProxy]
    protected bool mindMapInterfaceBlocked;
    [StateSaveProxy]
    [StateLoadProxy]
    protected bool inventoryInterfaceBlocked;
    [StateSaveProxy]
    [StateLoadProxy]
    protected bool statsInterfaceBlocked;
    [StateSaveProxy]
    [StateLoadProxy()]
    protected bool boundsInterfaceBlocked;

    public event Action OnBlockChanged;

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
      mapInterfaceBlocked = false;
      mindMapInterfaceBlocked = false;
      inventoryInterfaceBlocked = false;
      statsInterfaceBlocked = false;
      boundsInterfaceBlocked = false;
    }

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
    }

    [Inspected(Mutable = true)]
    public bool BlockMapInterface
    {
      get => mapInterfaceBlocked;
      set
      {
        mapInterfaceBlocked = value;
        OnInvalidate();
      }
    }

    [Inspected(Mutable = true)]
    public bool BlockMindMapInterface
    {
      get => mindMapInterfaceBlocked;
      set
      {
        mindMapInterfaceBlocked = value;
        OnInvalidate();
      }
    }

    [Inspected(Mutable = true)]
    public bool BlockStatsInterface
    {
      get => statsInterfaceBlocked;
      set
      {
        statsInterfaceBlocked = value;
        OnInvalidate();
      }
    }

    [Inspected(Mutable = true)]
    public bool BlockInventoryInterface
    {
      get => inventoryInterfaceBlocked;
      set
      {
        inventoryInterfaceBlocked = value;
        OnInvalidate();
      }
    }

    [Inspected(Mutable = true)]
    public bool BlockBoundsInterface
    {
      get => boundsInterfaceBlocked;
      set
      {
        boundsInterfaceBlocked = value;
        OnInvalidate();
      }
    }

    private void OnInvalidate()
    {
      Action onBlockChanged = OnBlockChanged;
      if (onBlockChanged == null)
        return;
      onBlockChanged();
    }
  }
}
