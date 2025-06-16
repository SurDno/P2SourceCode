// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.WeatherController_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Impl.Services;
using Engine.Source.Services;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WeatherController))]
  public class WeatherController_Generated : 
    WeatherController,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveSerialize<WeatherInfo>(writer, "WeatherInfo", this.WeatherInfo);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.WeatherInfo = DefaultStateLoadUtility.ReadSerialize<WeatherInfo>(reader, "WeatherInfo");
    }
  }
}
