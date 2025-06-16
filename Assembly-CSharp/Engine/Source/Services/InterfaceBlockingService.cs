using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using Inspectors;
using System;
using System.Collections;
using System.Xml;

namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (InterfaceBlockingService), typeof (IInterfaceBlockingService)})]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class InterfaceBlockingService : IInterfaceBlockingService, ISavesController
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected bool mapInterfaceBlocked;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected bool mindMapInterfaceBlocked;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected bool inventoryInterfaceBlocked;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected bool statsInterfaceBlocked;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    protected bool boundsInterfaceBlocked;

    public event Action OnBlockChanged;

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
      this.mapInterfaceBlocked = false;
      this.mindMapInterfaceBlocked = false;
      this.inventoryInterfaceBlocked = false;
      this.statsInterfaceBlocked = false;
      this.boundsInterfaceBlocked = false;
    }

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<InterfaceBlockingService>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }

    [Inspected(Mutable = true)]
    public bool BlockMapInterface
    {
      get => this.mapInterfaceBlocked;
      set
      {
        this.mapInterfaceBlocked = value;
        this.OnInvalidate();
      }
    }

    [Inspected(Mutable = true)]
    public bool BlockMindMapInterface
    {
      get => this.mindMapInterfaceBlocked;
      set
      {
        this.mindMapInterfaceBlocked = value;
        this.OnInvalidate();
      }
    }

    [Inspected(Mutable = true)]
    public bool BlockStatsInterface
    {
      get => this.statsInterfaceBlocked;
      set
      {
        this.statsInterfaceBlocked = value;
        this.OnInvalidate();
      }
    }

    [Inspected(Mutable = true)]
    public bool BlockInventoryInterface
    {
      get => this.inventoryInterfaceBlocked;
      set
      {
        this.inventoryInterfaceBlocked = value;
        this.OnInvalidate();
      }
    }

    [Inspected(Mutable = true)]
    public bool BlockBoundsInterface
    {
      get => this.boundsInterfaceBlocked;
      set
      {
        this.boundsInterfaceBlocked = value;
        this.OnInvalidate();
      }
    }

    private void OnInvalidate()
    {
      Action onBlockChanged = this.OnBlockChanged;
      if (onBlockChanged == null)
        return;
      onBlockChanged();
    }
  }
}
