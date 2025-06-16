using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Storable", typeof (IStorableComponent))]
  public class VMStorable : VMEngineComponent<IStorableComponent>
  {
    public const string ComponentName = "Storable";
    private ITextRef objectTooltip;
    private ITextRef objectDescription;
    private ITextRef objectTitle;
    private ITextRef objectSpecialDescription;
    private string storeTag;
    private static Dictionary<string, IEntity> StorableTemplateNamesDict = new Dictionary<string, IEntity>();

    [Property("Storable class", "", true, "Pathologic")]
    public string StorableClass { get; set; }

    [Property("Tag", "", true)]
    public string StoreTag
    {
      get => this.storeTag;
      set
      {
        if (value == null)
          return;
        this.storeTag = value;
      }
    }

    [Property("Default stack count", "", true, 1)]
    public int DefaultStackCount
    {
      get => this.Component.Max;
      set => this.Component.Max = value;
    }

    public static IStorableComponent MakeStorableByTemplate(
      VMBaseEntity storageEntity,
      IEntity template)
    {
      if (template.GetComponent<IStorableComponent>() == null)
      {
        Logger.AddError(string.Format("Invalid storable object template {0}: there is no storable component!", (object) template.Name));
        return (IStorableComponent) null;
      }
      try
      {
        if (!VMStorable.StorableTemplateNamesDict.ContainsKey(template.Name))
          VMStorable.StorableTemplateNamesDict.Add(template.Name, template);
        IStorableComponent component = EngineAPIManager.Instance.CreateWorldTemplateDynamicChildInstance(template, (VMBaseEntity) null, ServiceCache.Simulation.Storables).Instance.GetComponent<IStorableComponent>();
        Invoice invoice;
        invoice.SellPrice = 0.0f;
        invoice.BuyPrice = VMMarket.DEFAULT_ITEM_PRICE;
        component.Invoice = invoice;
        return component;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Set storable {0} market data error: {1}", (object) template.Name, (object) ex));
      }
      return (IStorableComponent) null;
    }

    public static void MakeStorableByInstance(
      VMStorage storage,
      IStorableComponent storable,
      IEntity template)
    {
      IEntity owner = storable.Owner;
      if (template.GetComponent<IStorableComponent>() == null)
        Logger.AddError(string.Format("Invalid storable object template {0}: there is no storable component!", (object) template.Name));
      else
        EngineAPIManager.Instance.CreateWorldTemplateDynamicChildInstance(template, owner);
    }

    public static IEntity GetEngTemplateByStorableName(string storableName)
    {
      return VMStorable.StorableTemplateNamesDict.ContainsKey(storableName) ? VMStorable.StorableTemplateNamesDict[storableName] : (IEntity) null;
    }

    [Property("Title", "", true)]
    [SpecialProperty(ESpecialPropertyName.SPN_STORABLE_TITLE)]
    public ITextRef Title
    {
      get => this.objectTitle;
      set
      {
        this.objectTitle = value;
        this.Component.Title = EngineAPIManager.CreateEngineTextInstance(this.objectTitle);
      }
    }

    [Property("Tooltip", "", true)]
    [SpecialProperty(ESpecialPropertyName.SPN_STORABLE_TOOLTIP)]
    public ITextRef Tooltip
    {
      get => this.objectTooltip;
      set
      {
        this.objectTooltip = value;
        this.Component.Tooltip = EngineAPIManager.CreateEngineTextInstance(this.objectTooltip);
      }
    }

    [Property("Description", "", true)]
    [SpecialProperty(ESpecialPropertyName.SPN_STORABLE_DESCRIPTION)]
    public ITextRef Description
    {
      get => this.objectDescription;
      set
      {
        this.objectDescription = value;
        this.Component.Description = EngineAPIManager.CreateEngineTextInstance(this.objectDescription);
      }
    }

    [Property("SpecialDescription", "", true)]
    [SpecialProperty(ESpecialPropertyName.SPN_STORABLE_SPECIALDESCRIPTION)]
    public ITextRef SpecialDescription
    {
      get => this.objectSpecialDescription;
      set
      {
        this.objectSpecialDescription = value;
        this.Component.SpecialDescription = EngineAPIManager.CreateEngineTextInstance(this.objectSpecialDescription);
      }
    }

    public static void DoSetStorableTextField(
      IStorableComponent item,
      string textFieldName,
      LocalizedText textInstance)
    {
      switch (textFieldName)
      {
        case "title":
          item.Title = textInstance;
          break;
        case "description":
          item.Description = textInstance;
          break;
        case "specialdescription":
          item.SpecialDescription = textInstance;
          break;
        case "tooltip":
          item.Tooltip = textInstance;
          break;
      }
    }

    public static void ClearAll() => VMStorable.StorableTemplateNamesDict.Clear();
  }
}
