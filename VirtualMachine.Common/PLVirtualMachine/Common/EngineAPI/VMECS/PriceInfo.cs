using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using System.Xml;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  public class PriceInfo : ISerializeStateSave, IDynamicLoadSerializable
  {
    private float buyPriceValue = -1f;
    private string templateName = "";

    public PriceInfo()
    {
    }

    public PriceInfo(string templateName) => this.templateName = templateName;

    public string TemplateName => this.templateName;

    public float Price { get; set; } = -1f;

    public float BuyPrice
    {
      get => this.buyPriceValue;
      set
      {
        this.buyPriceValue = value;
        if ((double) this.buyPriceValue <= 9.9999999747524271E-07 || (double) this.BuyPriceFactor >= 9.9999999747524271E-07)
          return;
        this.BuyPriceFactor = 1f;
      }
    }

    public float PriceFactor { get; set; } = 1f;

    public float BuyPriceFactor { get; set; }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "TemplateName", this.templateName);
      SaveManagerUtility.Save(writer, "Price", this.Price);
      SaveManagerUtility.Save(writer, "BuyPrice", this.buyPriceValue);
      SaveManagerUtility.Save(writer, "PriceFactor", this.PriceFactor);
      SaveManagerUtility.Save(writer, "BuyPriceFactor", this.BuyPriceFactor);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      foreach (XmlElement childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name == "TemplateName")
          this.templateName = childNode.InnerText;
        else if (childNode.Name == "Price")
          this.Price = StringUtility.ToSingle(childNode.InnerText);
        else if (childNode.Name == "BuyPrice")
          this.buyPriceValue = StringUtility.ToSingle(childNode.InnerText);
        else if (childNode.Name == "PriceFactor")
          this.PriceFactor = StringUtility.ToSingle(childNode.InnerText);
        else if (childNode.Name == "BuyPriceFactor")
          this.BuyPriceFactor = StringUtility.ToSingle(childNode.InnerText);
      }
    }
  }
}
