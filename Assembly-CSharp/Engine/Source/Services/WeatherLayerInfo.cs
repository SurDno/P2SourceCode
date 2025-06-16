// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.WeatherLayerInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Common.Weather;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Services
{
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class WeatherLayerInfo
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public WeatherLayer Layer;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public float Opacity;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public Guid SnapshotTemplateId;
  }
}
