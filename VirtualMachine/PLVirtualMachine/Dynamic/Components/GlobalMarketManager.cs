using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMGlobalMarketManager))]
  public class GlobalMarketManager : 
    VMGlobalMarketManager,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    private Dictionary<string, ItemTradePriceInfo> tradePricesTable;
    private bool loaded;

    public override string GetComponentTypeName() => nameof (GlobalMarketManager);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }

    public override void Initialize(VMBaseEntity parent)
    {
      base.Initialize(parent);
      if (Instance == null)
        Instance = this;
      else
        Logger.AddError("Global Market Manager component creation dublicate!");
    }

    public static void ResetInstance()
    {
      Instance = null;
    }

    public override void SetBaseItemTradePrices(
      string itemTemplatesNames,
      string itemTemplatesPrices,
      string itemTemplatesBuyPrices)
    {
      if (tradePricesTable == null)
      {
        Logger.AddError("Market prices table not inited!!!");
      }
      else
      {
        string[] strArray1 = itemTemplatesNames.Split(';');
        string[] strArray2 = itemTemplatesPrices.Split(';');
        string[] strArray3 = itemTemplatesBuyPrices.Split(';');
        if (strArray1.Length != strArray2.Length)
          Logger.AddError(string.Format("Set prices items count not corresponded to prices count at {0}", DynamicFSM.CurrentStateInfo));
        if (strArray1.Length == 0)
          return;
        IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
        float num1 = 0.0f;
        if ("" != strArray2[0])
          num1 = GameComponent.ReadContextFloatParamValue(methodExecInitiator, strArray2[0]);
        else
          Logger.AddError(string.Format("Cannot set base price for {0}, price not defined ", DynamicFSM.CurrentStateInfo));
        float num2 = 0.0f;
        if ("" != strArray3[0])
          num2 = GameComponent.ReadContextFloatParamValue(methodExecInitiator, strArray3[0]);
        else
          Logger.AddError(string.Format("Cannot set base buy price for {0}, buy price not defined ", DynamicFSM.CurrentStateInfo));
        for (int index = 0; index < strArray1.Length; ++index)
        {
          string key = strArray1[index];
          if (!tradePricesTable.ContainsKey(key))
          {
            Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", key, DynamicFSM.CurrentStateInfo));
          }
          else
          {
            tradePricesTable[key].Price = num1;
            tradePricesTable[key].BuyPrice = num2;
          }
        }
        OnModify();
      }
    }

    public override void SetBaseItemTradePriceFactors(
      string itemTemplatesNames,
      string itemTemplatesFactors,
      string itemTemplatesBuyFactors)
    {
      if (tradePricesTable == null)
      {
        Logger.AddError("Market prices table not inited!!!");
      }
      else
      {
        string[] strArray1 = itemTemplatesNames.Split(';');
        string[] strArray2 = itemTemplatesFactors.Split(';');
        string[] strArray3 = itemTemplatesBuyFactors.Split(';');
        if (strArray1.Length != strArray2.Length)
          Logger.AddError(string.Format("Set prices items count not corresponded to prices count at {0}", DynamicFSM.CurrentStateInfo));
        if (strArray1.Length == 0)
          return;
        IDynamicGameObjectContext methodExecInitiator = VMEngineAPIManager.LastMethodExecInitiator;
        float num1 = GameComponent.ReadContextFloatParamValue(methodExecInitiator, strArray2[0]);
        float num2 = GameComponent.ReadContextFloatParamValue(methodExecInitiator, strArray3[0]);
        for (int index = 0; index < strArray1.Length; ++index)
        {
          string key = strArray1[index];
          if (!tradePricesTable.ContainsKey(key))
          {
            Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", key, DynamicFSM.CurrentStateInfo));
          }
          else
          {
            tradePricesTable[key].PriceFactor = num1;
            tradePricesTable[key].BuyPriceFactor = num2;
          }
        }
        OnModify();
      }
    }

    public override void SetBaseItemTradePrice(IBlueprintRef template, float price)
    {
      if (tradePricesTable == null)
      {
        Logger.AddError("Market prices table not inited!!!");
      }
      else
      {
        string name = template.Blueprint.Name;
        if (!tradePricesTable.ContainsKey(name))
        {
          Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", name, DynamicFSM.CurrentStateInfo));
        }
        else
        {
          tradePricesTable[name].Price = price;
          tradePricesTable[name].BuyPrice = price;
          OnModify();
        }
      }
    }

    public override void SetBaseItemTradeBuyPrice(
      IBlueprintRef template,
      float price,
      float buyPrice)
    {
      if (tradePricesTable == null)
      {
        Logger.AddError("Market prices table not inited!!!");
      }
      else
      {
        string name = template.Blueprint.Name;
        if (!tradePricesTable.ContainsKey(name))
        {
          Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", name, DynamicFSM.CurrentStateInfo));
        }
        else
        {
          tradePricesTable[name].Price = price;
          tradePricesTable[name].BuyPrice = buyPrice;
          OnModify();
        }
      }
    }

    public override void SetItemTradePriceFactor(IBlueprintRef template, float priceFactor)
    {
      if (tradePricesTable == null)
      {
        Logger.AddError("Market prices table not inited!!!");
      }
      else
      {
        string name = template.Blueprint.Name;
        if (!tradePricesTable.ContainsKey(name))
        {
          Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", name, DynamicFSM.CurrentStateInfo));
        }
        else
        {
          tradePricesTable[name].PriceFactor = priceFactor;
          tradePricesTable[name].BuyPriceFactor = priceFactor;
          OnModify();
        }
      }
    }

    public override void SetItemTradeBuyPriceFactor(
      IBlueprintRef template,
      float priceFactor,
      float buyPriceFactor)
    {
      if (tradePricesTable == null)
      {
        Logger.AddError("Market prices table not inited!!!");
      }
      else
      {
        string name = template.Blueprint.Name;
        if (!tradePricesTable.ContainsKey(name))
        {
          Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", name, DynamicFSM.CurrentStateInfo));
        }
        else
        {
          tradePricesTable[name].PriceFactor = priceFactor;
          tradePricesTable[name].BuyPriceFactor = buyPriceFactor;
          OnModify();
        }
      }
    }

    public override void SetItemStackCount(
      string itemTemplatesNames,
      string itemTemplatesStackCountValues)
    {
    }

    public override float GetCurrentItemTradeGlobalPrice(string templateName, bool buyPrice = false)
    {
      if (tradePricesTable == null)
      {
        Logger.AddError("Market prices table not inited!!!");
        return 0.0f;
      }
      string key = templateName;
      if (!tradePricesTable.ContainsKey(key))
        key = "Pathologic";
      if (!tradePricesTable.ContainsKey(key))
      {
        Logger.AddError(string.Format("Item template with name {0} not found in prices table at {1}", templateName, DynamicFSM.CurrentStateInfo));
        return 0.0f;
      }
      return buyPrice ? tradePricesTable[key].ResultBuyPrice : tradePricesTable[key].ResultPrice;
    }

    public void InitTradePricesTable(IEnumerable<IBlueprint> allTemplates)
    {
      if (tradePricesTable == null)
        tradePricesTable = new Dictionary<string, ItemTradePriceInfo>();
      tradePricesTable.Clear();
      tradePricesTable.Add("Pathologic", new ItemTradePriceInfo());
      foreach (IBlueprint allTemplate in allTemplates)
      {
        if ((allTemplate.GetCategory() == EObjectCategory.OBJECT_CATEGORY_ITEM || allTemplate.GetCategory() == EObjectCategory.OBJECT_CATEGORY_OTHERS) && allTemplate.IsFunctionalSupport("Storable"))
        {
          ItemTradePriceInfo itemTradePriceInfo = new ItemTradePriceInfo((IWorldBlueprint) allTemplate);
          if (!tradePricesTable.ContainsKey(allTemplate.Name))
            tradePricesTable.Add(allTemplate.Name, itemTradePriceInfo);
          else
            Logger.AddError(string.Format("Storable name {0} is dublicated !", allTemplate.Name));
        }
      }
      loaded = true;
    }

    public override void StateSave(IDataWriter writer)
    {
      base.StateSave(writer);
      SaveManagerUtility.SaveDynamicSerializableList(writer, "TradePricesTable", tradePricesTable.Values);
    }

    public override void LoadFromXML(XmlElement xmlNode)
    {
      base.LoadFromXML(xmlNode);
      for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i1];
        if (childNode1.Name == "TradePricesTable")
        {
          tradePricesTable = new Dictionary<string, ItemTradePriceInfo>(childNode1.ChildNodes.Count);
          for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
          {
            XmlElement childNode2 = (XmlElement) childNode1.ChildNodes[i2];
            ItemTradePriceInfo itemTradePriceInfo = new ItemTradePriceInfo();
            itemTradePriceInfo.LoadFromXML(childNode2);
            if (itemTradePriceInfo.ItemTemplate != null)
              tradePricesTable.Add(itemTradePriceInfo.ItemTemplate.Name, itemTradePriceInfo);
          }
        }
      }
    }

    public override void OnCreate()
    {
      if (loaded)
        return;
      InitTradePricesTable(((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).Blueprints);
    }

    public override void Clear() => loaded = false;
  }
}
