using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.Objects;
using System;
using System.Xml;

namespace PLVirtualMachine.Dynamic.Components
{
  public class StorageCommand : 
    IStorageCommand,
    ISerializeStateSave,
    IDynamicLoadSerializable,
    INeedSave
  {
    private Guid targetStorageEntityLoadedId = Guid.Empty;

    public void Initialize(
      EStorageCommandType commandType,
      VMStorage storage,
      VMWorldObject itemLogicObject,
      bool dropIfBusy = false)
    {
      this.Initialize(commandType, storage, itemLogicObject, 1, (OperationTagsInfo) null, (OperationMultiTagsInfo) null, dropIfBusy: dropIfBusy);
    }

    public void Initialize(
      EStorageCommandType commandType,
      VMStorage storage,
      VMWorldObject itemLogicObject,
      int itemsToAdd,
      OperationTagsInfo containerTypes,
      OperationMultiTagsInfo containerTags,
      CombinationItemParams ciParams = null,
      bool dropIfBusy = false)
    {
      this.StorageCommandType = commandType;
      this.TargetStorage = storage;
      this.TargetItemTemplate = itemLogicObject;
      this.ItemsCount = itemsToAdd;
      this.MaxItemsCount = itemsToAdd;
      this.ContainerTypesInfo = containerTypes;
      this.ContainerTagsInfo = containerTags;
      this.CombinationParams = ciParams;
      this.DropIfBusyMode = dropIfBusy;
    }

    public void Initialize(
      EStorageCommandType commandType,
      VMStorage storage,
      VMWorldObject itemLogicObject,
      int itemsToAdd,
      int maxItemsToAdd)
    {
      this.StorageCommandType = commandType;
      this.TargetStorage = storage;
      this.TargetItemTemplate = itemLogicObject;
      this.ItemsCount = itemsToAdd;
      this.MaxItemsCount = maxItemsToAdd;
      this.ContainerTypesInfo = (OperationTagsInfo) null;
      this.ContainerTagsInfo = (OperationMultiTagsInfo) null;
      this.CombinationParams = (CombinationItemParams) null;
      this.DropIfBusyMode = false;
    }

    public bool NeedSave
    {
      get
      {
        return this.TargetStorage != null && this.TargetStorage.Parent != null && !this.TargetStorage.Parent.IsDisposed;
      }
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.SaveEnum<EStorageCommandType>(writer, "StorageCommandType", this.StorageCommandType);
      if (this.TargetStorage != null)
        SaveManagerUtility.Save(writer, "TargetStorage", this.TargetStorage.Parent.EngineGuid);
      if (this.TargetItemTemplate != null)
        SaveManagerUtility.Save(writer, "TargetItemTemplate", this.TargetItemTemplate.BaseGuid);
      SaveManagerUtility.Save(writer, "ItemsCount", this.ItemsCount);
      SaveManagerUtility.Save(writer, "MaxItemsCount", this.MaxItemsCount);
      if (this.ContainerTypesInfo != null)
        SaveManagerUtility.Save(writer, "ContainerTypesInfo", this.ContainerTypesInfo.ToString());
      if (this.ContainerTagsInfo != null)
        SaveManagerUtility.Save(writer, "ContainerTagsInfo", this.ContainerTagsInfo.ToString());
      SaveManagerUtility.SaveDynamicSerializable(writer, "CombinationParams", (ISerializeStateSave) this.CombinationParams);
      SaveManagerUtility.Save(writer, "DropIfBusyMode", this.DropIfBusyMode);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "StorageCommandType")
          this.StorageCommandType = VMSaveLoadManager.ReadEnum<EStorageCommandType>((XmlNode) childNode);
        else if (childNode.Name == "TargetStorage")
          this.targetStorageEntityLoadedId = VMSaveLoadManager.ReadGuid((XmlNode) childNode);
        else if (childNode.Name == "TargetItemTemplate")
        {
          ulong id = VMSaveLoadManager.ReadUlong((XmlNode) childNode);
          if (id != 0UL)
          {
            IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(id);
            if (objectByGuid != null)
              this.TargetItemTemplate = (VMWorldObject) objectByGuid;
          }
        }
        else if (childNode.Name == "ItemsCount")
          this.ItemsCount = VMSaveLoadManager.ReadInt((XmlNode) childNode);
        else if (childNode.Name == "MaxItemsCount")
          this.MaxItemsCount = VMSaveLoadManager.ReadInt((XmlNode) childNode);
        else if (childNode.Name == "DropIfBusyMode")
          this.DropIfBusyMode = VMSaveLoadManager.ReadBool((XmlNode) childNode);
        else if (childNode.Name == "ContainerTypesInfo")
        {
          string innerText = childNode.InnerText;
          this.ContainerTypesInfo = new OperationTagsInfo();
          this.ContainerTypesInfo.Read(innerText);
        }
        else if (childNode.Name == "ContainerTagsInfo")
        {
          string innerText = childNode.InnerText;
          this.ContainerTagsInfo = new OperationMultiTagsInfo();
          this.ContainerTagsInfo.Read(innerText);
        }
        else if (childNode.Name == "CombinationParams")
        {
          this.CombinationParams = new CombinationItemParams();
          this.CombinationParams.LoadFromXML(childNode);
        }
      }
    }

    public void AfterSaveLoading()
    {
      if (!(this.targetStorageEntityLoadedId != Guid.Empty))
        return;
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(this.targetStorageEntityLoadedId);
      if (entityByEngineGuid != null)
      {
        VMComponent componentByName = entityByEngineGuid.GetComponentByName("Storage");
        if (componentByName != null)
          this.TargetStorage = (VMStorage) componentByName;
        else
          Logger.AddError(string.Format("SaveLoad error: storage component not found in target storage entity {0} at storage group command loading", (object) entityByEngineGuid.Name));
      }
      else
        Logger.AddError(string.Format("SaveLoad error: target storage entity id={0} not found at storage group command loading", (object) this.targetStorageEntityLoadedId));
    }

    public void Clear()
    {
      this.TargetStorage = (VMStorage) null;
      this.TargetItemTemplate = (VMWorldObject) null;
      this.ContainerTypesInfo = (OperationTagsInfo) null;
      this.ContainerTagsInfo = (OperationMultiTagsInfo) null;
      this.CombinationParams = (CombinationItemParams) null;
    }

    public EStorageCommandType StorageCommandType { get; private set; }

    public VMStorage TargetStorage { get; private set; }

    public VMWorldObject TargetItemTemplate { get; private set; }

    public int ItemsCount { get; private set; }

    public int MaxItemsCount { get; private set; }

    public OperationTagsInfo ContainerTypesInfo { get; private set; }

    public OperationMultiTagsInfo ContainerTagsInfo { get; private set; }

    public CombinationItemParams CombinationParams { get; private set; }

    public bool DropIfBusyMode { get; private set; }
  }
}
