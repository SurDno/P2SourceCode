// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.Sun_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Sun))]
  public class Sun_Generated : Sun, ICloneable, ICopyable, ISerializeDataWrite, ISerializeDataRead
  {
    public object Clone()
    {
      Sun_Generated instance = Activator.CreateInstance<Sun_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Sun_Generated sunGenerated = (Sun_Generated) target2;
      sunGenerated.brightness = this.brightness;
      sunGenerated.contrast = this.contrast;
      sunGenerated.size = this.size;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Brightness", this.brightness);
      DefaultDataWriteUtility.Write(writer, "Contrast", this.contrast);
      DefaultDataWriteUtility.Write(writer, "Size", this.size);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.brightness = DefaultDataReadUtility.Read(reader, "Brightness", this.brightness);
      this.contrast = DefaultDataReadUtility.Read(reader, "Contrast", this.contrast);
      this.size = DefaultDataReadUtility.Read(reader, "Size", this.size);
    }
  }
}
