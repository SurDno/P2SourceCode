// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.WeatherSmoothBlender_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Blenders;
using Engine.Source.Commons;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WeatherSmoothBlender))]
  public class WeatherSmoothBlender_Generated : WeatherSmoothBlender, ICloneable, ICopyable
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<WeatherSmoothBlender_Generated>(this);
    }

    public void CopyTo(object target2) => ((EngineObject) target2).name = this.name;
  }
}
