using Cofe.Serializations.Converters;
using Engine.Common.Weather;
using Engine.Source.Inventory;
using System;
using System.IO;
using UnityEngine;

namespace Engine.Source.Connections
{
  [Serializable]
  public struct IInventoryPlaceholderSerializable : IEngineSerializable
  {
    [SerializeField]
    private string id;

    public Guid Id
    {
      get => DefaultConverter.ParseGuid(this.id);
      set => this.id = value != Guid.Empty ? value.ToString() : "";
    }

    public System.Type Type => typeof (IWeatherSnapshot);

    public IInventoryPlaceholder Value
    {
      get => TemplateUtility.GetTemplate<IInventoryPlaceholder>(this.Id);
      set => this.Id = value != null ? value.Id : Guid.Empty;
    }

    public override int GetHashCode() => string.IsNullOrEmpty(this.id) ? 0 : this.id.GetHashCode();

    public override bool Equals(object a)
    {
      return a is IWeatherSnapshotSerializable && this == (IInventoryPlaceholderSerializable) a;
    }

    public static bool operator ==(
      IInventoryPlaceholderSerializable a,
      IInventoryPlaceholderSerializable b)
    {
      return a.id == b.id;
    }

    public static bool operator !=(
      IInventoryPlaceholderSerializable a,
      IInventoryPlaceholderSerializable b)
    {
      return !(a == b);
    }

    public override string ToString()
    {
      IInventoryPlaceholder inventoryPlaceholder = this.Value;
      return inventoryPlaceholder != null ? Path.GetFileNameWithoutExtension(inventoryPlaceholder.Source) : "";
    }
  }
}
