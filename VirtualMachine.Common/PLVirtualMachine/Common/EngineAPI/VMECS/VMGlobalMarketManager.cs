using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("GlobalMarketManager", null)]
  public class VMGlobalMarketManager : VMComponent
  {
    public const string ComponentName = "GlobalMarketManager";

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    [Method("Set base price for template", "Template:Storable,Price", "")]
    public virtual void SetBaseItemTradePrice(IBlueprintRef template, float price)
    {
    }

    [Method("Set base prices for template", "Template:Storable,Price,Buy price", "")]
    public virtual void SetBaseItemTradeBuyPrice(
      IBlueprintRef template,
      float price,
      float buyPrice)
    {
    }

    [Method("Set current price factor for template", "Template:Storable,Price factor", "")]
    public virtual void SetItemTradePriceFactor(IBlueprintRef template, float priceFactor)
    {
    }

    [Method("Set current price factors for template", "Template:Storable,Price factor,Buy price factor", "")]
    public virtual void SetItemTradeBuyPriceFactor(
      IBlueprintRef template,
      float priceFactor,
      float buyPriceFactor)
    {
    }

    [Method("", ",,", "")]
    public virtual void SetBaseItemTradePrices(
      string itemTemplatesNames,
      string itemTemplatesPrices,
      string itemTemplatesBuyPrices)
    {
    }

    [Method("", ",,", "")]
    public virtual void SetBaseItemTradePriceFactors(
      string itemTemplatesNames,
      string itemTemplatesFactors,
      string itemTemplatesBuyFactors)
    {
    }

    [Method("", ",", "")]
    public virtual void SetItemStackCount(
      string itemTemplatesNames,
      string itemTemplatesStackCountValues)
    {
    }

    public virtual float GetCurrentItemTradeGlobalPrice(string templateName, bool buyPrice = false)
    {
      return 0.0f;
    }

    public static VMGlobalMarketManager Instance { get; protected set; }
  }
}
