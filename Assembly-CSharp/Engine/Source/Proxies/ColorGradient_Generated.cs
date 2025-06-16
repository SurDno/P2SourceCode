using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Drawing.Gradient;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ColorGradient))]
  public class ColorGradient_Generated : 
    ColorGradient,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ColorGradient_Generated instance = Activator.CreateInstance<ColorGradient_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ColorGradient_Generated gradientGenerated = (ColorGradient_Generated) target2;
      CloneableObjectUtility.FillListTo(gradientGenerated.alphaKeys, alphaKeys);
      CloneableObjectUtility.FillListTo(gradientGenerated.colorKeys, colorKeys);
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.WriteList(writer, "AlphaKeys", alphaKeys);
      UnityDataWriteUtility.WriteList(writer, "ColorKeys", colorKeys);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      alphaKeys = UnityDataReadUtility.ReadList(reader, "AlphaKeys", alphaKeys);
      colorKeys = UnityDataReadUtility.ReadList(reader, "ColorKeys", colorKeys);
    }
  }
}
