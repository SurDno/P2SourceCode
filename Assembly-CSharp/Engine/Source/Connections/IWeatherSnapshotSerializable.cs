// Decompiled with JetBrains decompiler
// Type: Engine.Source.Connections.IWeatherSnapshotSerializable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Converters;
using Engine.Common.Weather;
using System;
using System.IO;
using UnityEngine;

#nullable disable
namespace Engine.Source.Connections
{
  [Serializable]
  public struct IWeatherSnapshotSerializable : IEngineSerializable
  {
    [SerializeField]
    private string id;

    public Guid Id
    {
      get => DefaultConverter.ParseGuid(this.id);
      set => this.id = value != Guid.Empty ? value.ToString() : "";
    }

    public System.Type Type => typeof (IWeatherSnapshot);

    public IWeatherSnapshot Value
    {
      get => TemplateUtility.GetTemplate<IWeatherSnapshot>(this.Id);
      set => this.Id = value != null ? value.Id : Guid.Empty;
    }

    public override int GetHashCode() => string.IsNullOrEmpty(this.id) ? 0 : this.id.GetHashCode();

    public override bool Equals(object a)
    {
      return a is IWeatherSnapshotSerializable snapshotSerializable && this == snapshotSerializable;
    }

    public static bool operator ==(IWeatherSnapshotSerializable a, IWeatherSnapshotSerializable b)
    {
      return a.id == b.id;
    }

    public static bool operator !=(IWeatherSnapshotSerializable a, IWeatherSnapshotSerializable b)
    {
      return !(a == b);
    }

    public override string ToString()
    {
      IWeatherSnapshot weatherSnapshot = this.Value;
      return weatherSnapshot != null ? Path.GetFileNameWithoutExtension(weatherSnapshot.Source) : "";
    }
  }
}
