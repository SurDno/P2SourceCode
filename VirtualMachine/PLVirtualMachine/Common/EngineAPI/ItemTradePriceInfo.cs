using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using System.Xml;

namespace PLVirtualMachine.Common.EngineAPI
{
  public class ItemTradePriceInfo : ISerializeStateSave, IDynamicLoadSerializable
  {
    private IWorldBlueprint itemTemplate;
    private float priceValue = 1f;
    private float buyPriceValue = 1f;
    private float priceFactor = 1f;
    private float buyPriceFactor = 1f;

    public ItemTradePriceInfo()
    {
    }

    public ItemTradePriceInfo(IWorldBlueprint item) => this.itemTemplate = item;

    public IWorldBlueprint ItemTemplate
    {
      get => this.itemTemplate;
      set => this.itemTemplate = value;
    }

    public float Price
    {
      get => this.priceValue;
      set => this.priceValue = value;
    }

    public float BuyPrice
    {
      get => this.buyPriceValue;
      set => this.buyPriceValue = value;
    }

    public float PriceFactor
    {
      get => this.priceFactor;
      set => this.priceFactor = value;
    }

    public float BuyPriceFactor
    {
      get => this.buyPriceFactor;
      set => this.buyPriceFactor = value;
    }

    public float ResultPrice => this.priceValue * this.priceFactor;

    public float ResultBuyPrice => this.buyPriceValue * this.buyPriceFactor;

    public void StateSave(IDataWriter writer)
    {
      if (this.itemTemplate != null)
        SaveManagerUtility.Save(writer, "TemplateGuid", this.itemTemplate.BaseGuid);
      SaveManagerUtility.Save(writer, "Price", this.priceValue);
      SaveManagerUtility.Save(writer, "BuyPrice", this.buyPriceValue);
      SaveManagerUtility.Save(writer, "PriceFactor", this.priceFactor);
      SaveManagerUtility.Save(writer, "BuyPriceFactor", this.buyPriceFactor);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      foreach (XmlElement childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name == "TemplateGuid")
        {
          ulong uint64 = StringUtility.ToUInt64(childNode.InnerText);
          IBlueprint blueprintByGuid = IStaticDataContainer.StaticDataContainer.GameRoot.GetBlueprintByGuid(uint64);
          if (blueprintByGuid != null)
          {
            if (typeof (IWorldBlueprint).IsAssignableFrom(blueprintByGuid.GetType()))
              this.itemTemplate = (IWorldBlueprint) blueprintByGuid;
            else
              Logger.AddError(string.Format("SaveLoad error: cannot load item trade price info, item template id {0} is invalid", (object) uint64));
          }
          else
            Logger.AddError(string.Format("SaveLoad error: cannot load item trade price info, item template id {0} not found", (object) uint64));
        }
        else if (childNode.Name == "Price")
          this.priceValue = StringUtility.ToSingle(childNode.InnerText);
        else if (childNode.Name == "BuyPrice")
          this.buyPriceValue = StringUtility.ToSingle(childNode.InnerText);
        else if (childNode.Name == "PriceFactor")
          this.priceFactor = StringUtility.ToSingle(childNode.InnerText);
        else if (childNode.Name == "BuyPriceFactor")
          this.buyPriceFactor = StringUtility.ToSingle(childNode.InnerText);
      }
    }

    public bool NeedToSave() => true;
  }
}
