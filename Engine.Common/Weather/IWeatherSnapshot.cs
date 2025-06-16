// Decompiled with JetBrains decompiler
// Type: Engine.Common.Weather.IWeatherSnapshot
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Binders;
using Engine.Common.Blenders;

#nullable disable
namespace Engine.Common.Weather
{
  [Sample("ISnapshot")]
  public interface IWeatherSnapshot : IBlendable<IWeatherSnapshot>, IObject
  {
  }
}
