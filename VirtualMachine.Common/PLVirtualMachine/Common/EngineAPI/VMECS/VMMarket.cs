using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Market", typeof (IMarketComponent))]
  [Depended("Storage")]
  public class VMMarket : VMEngineComponent<IMarketComponent>
  {
    public const string ComponentName = "Market";
    protected Dictionary<string, PriceInfo> marketPricesTable = new Dictionary<string, PriceInfo>();
    protected PriceInfo defaultPriceInfo;
    public static float DEFAULT_ITEM_PRICE;

    [Property("Enabled", "", false, true)]
    public bool MarketEnabled
    {
      get => this.Component.IsEnabled;
      set => this.Component.IsEnabled = value;
    }

    [Method("", ",,", "")]
    [SpecialFunction(ESpecialFunctionName.SFN_SET_MULTI_STORABLES_FIXED_PRICES)]
    public void SetMultiStorablesFixedPrices(
      string itemTemplatesNames,
      string priceValuesInfo,
      string buyPriceValuesInfo)
    {
      string[] strArray1 = itemTemplatesNames.Split(';');
      string[] strArray2 = priceValuesInfo.Split(';');
      string[] strArray3 = buyPriceValuesInfo.Split(';');
      if (strArray1.Length == 0)
        return;
      VMGameComponent instance = VMGameComponent.Instance;
      float priceValue = instance.ReadFloatParamValue(strArray2[0]);
      float buyPriceValue = instance.ReadFloatParamValue(strArray3[0]);
      for (int index = 0; index < strArray1.Length; ++index)
        this.SetStorablePriceByTemplateName(strArray1[index], priceValue, buyPriceValue);
    }

    [Method("", ",,", "")]
    public void SetMultiStorablesPricesFactor(
      string itemTemplatesNames,
      string sPriceFactorValues,
      string sBuyPriceFactorValues)
    {
      string[] strArray1 = itemTemplatesNames.Split(';');
      string[] strArray2 = sPriceFactorValues.Split(';');
      string[] strArray3 = sBuyPriceFactorValues.Split(';');
      if (strArray1.Length == 0)
        return;
      VMGameComponent instance = VMGameComponent.Instance;
      float priceFactorValue = instance.ReadFloatParamValue(strArray2[0]);
      float buyPriceFactorValue = instance.ReadFloatParamValue(strArray3[0]);
      for (int index = 0; index < strArray1.Length; ++index)
        this.SetStorablePriceFactorByTemplateName(strArray1[index], priceFactorValue, buyPriceFactorValue);
    }

    public void SetStorablePriceByTemplateName(
      string storableTemplateName,
      float priceValue,
      float buyPriceValue)
    {
      if ("Pathologic" == storableTemplateName)
      {
        if (this.defaultPriceInfo == null)
          this.defaultPriceInfo = new PriceInfo();
        this.defaultPriceInfo.Price = priceValue;
        this.defaultPriceInfo.BuyPrice = buyPriceValue;
      }
      if (this.marketPricesTable.ContainsKey(storableTemplateName))
      {
        PriceInfo priceInfo = this.marketPricesTable[storableTemplateName];
        priceInfo.Price = priceValue;
        priceInfo.BuyPrice = buyPriceValue;
      }
      else
        this.marketPricesTable.Add(storableTemplateName, new PriceInfo(storableTemplateName)
        {
          Price = priceValue,
          BuyPrice = buyPriceValue
        });
      this.OnModify();
    }

    public void SetStorablePriceFactorByTemplateName(
      string storableTemplateName,
      float priceFactorValue,
      float buyPriceFactorValue)
    {
      if ("Pathologic" == storableTemplateName)
      {
        if (this.defaultPriceInfo == null)
          this.defaultPriceInfo = new PriceInfo();
        this.defaultPriceInfo.PriceFactor = priceFactorValue;
        this.defaultPriceInfo.BuyPriceFactor = buyPriceFactorValue;
      }
      if (this.marketPricesTable.ContainsKey(storableTemplateName))
      {
        PriceInfo priceInfo = this.marketPricesTable[storableTemplateName];
        priceInfo.PriceFactor = priceFactorValue;
        priceInfo.BuyPriceFactor = buyPriceFactorValue;
      }
      else
        this.marketPricesTable.Add(storableTemplateName, new PriceInfo(storableTemplateName)
        {
          PriceFactor = priceFactorValue,
          BuyPriceFactor = buyPriceFactorValue
        });
      this.OnModify();
    }

    public PriceInfo GetTraderPriceInfoByTemplateName(string templateName)
    {
      if (this.marketPricesTable.ContainsKey(templateName))
        return this.marketPricesTable[templateName];
      return this.defaultPriceInfo != null ? this.defaultPriceInfo : new PriceInfo(templateName);
    }

    [Method("Set fixed price", "Template:Storable,Price", "")]
    public void SetStorablePriceByTemplate([Template] IEntity template, float priceValue)
    {
      if (template == null)
        Logger.AddError(string.Format("Storable template for market {0} price setting is not defined !", (object) this.Component.Owner.Name));
      else
        this.SetStorablePriceByTemplateName(template.Name, priceValue, priceValue);
    }

    [Method("Set fixed prices", "Template:Storable,Price,BuyPrice", "")]
    public void SetStorablePricesByTemplate(
      [Template] IEntity template,
      float priceValue,
      float buyPriceValue)
    {
      if (template == null)
        Logger.AddError(string.Format("Storable template for market {0} price setting is not defined !", (object) this.Component.Owner.Name));
      else
        this.SetStorablePriceByTemplateName(template.Name, priceValue, buyPriceValue);
    }

    private void FillPrices()
    {
      VMMarket.FillPrices(this, this.Component.Owner.GetComponent<IStorageComponent>());
      VMMarket.FillPrices(this, ServiceCache.Simulation.Player.GetComponent<IStorageComponent>());
    }

    public static void FillPrices(VMMarket market, IStorageComponent storage)
    {
      for (int index = 0; index < storage.Items.Count<IStorableComponent>(); ++index)
      {
        IStorableComponent storableComponent = storage.Items.ElementAt<IStorableComponent>(index);
        if (storableComponent != null && !storableComponent.IsDisposed)
        {
          string name = storableComponent.Owner.Name;
          PriceInfo itemTradePrice = VMMarket.CalculateItemTradePrice(market, name);
          storableComponent.Invoice = new Invoice()
          {
            BuyPrice = itemTradePrice.BuyPrice,
            SellPrice = itemTradePrice.Price
          };
        }
      }
    }

    private static PriceInfo CalculateItemTradePrice(VMMarket market, string templateName)
    {
      PriceInfo infoByTemplateName = market.GetTraderPriceInfoByTemplateName(templateName);
      float num1 = infoByTemplateName.Price;
      float num2 = infoByTemplateName.BuyPrice;
      if ((double) infoByTemplateName.Price < -9.9999999747524271E-07)
        num1 = VMGlobalMarketManager.Instance.GetCurrentItemTradeGlobalPrice(templateName);
      if ((double) infoByTemplateName.BuyPrice > -9.9999999747524271E-07 || (double) infoByTemplateName.BuyPriceFactor > 9.9999999747524271E-07)
      {
        if ((double) infoByTemplateName.BuyPrice < -9.9999999747524271E-07)
          num2 = VMGlobalMarketManager.Instance.GetCurrentItemTradeGlobalPrice(templateName, true);
      }
      else
        num2 = 0.0f;
      return new PriceInfo(templateName)
      {
        Price = num1 * infoByTemplateName.PriceFactor,
        BuyPrice = num2 * infoByTemplateName.BuyPriceFactor
      };
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.OnFillPrices -= new Action(this.FillPrices);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.OnFillPrices += new Action(this.FillPrices);
    }
  }
}
