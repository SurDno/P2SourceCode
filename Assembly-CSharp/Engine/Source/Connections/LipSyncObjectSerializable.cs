using Cofe.Serializations.Converters;
using Engine.Source.Commons;
using System;
using System.IO;
using UnityEngine;

namespace Engine.Source.Connections
{
  [Serializable]
  public struct LipSyncObjectSerializable : IEngineSerializable
  {
    [SerializeField]
    private string id;

    public Guid Id
    {
      get => DefaultConverter.ParseGuid(this.id);
      set => this.id = value != Guid.Empty ? value.ToString() : "";
    }

    public System.Type Type => typeof (LipSyncObject);

    public LipSyncObject Value
    {
      get => TemplateUtility.GetTemplate<LipSyncObject>(this.Id);
      set => this.Id = value != null ? value.Id : Guid.Empty;
    }

    public override int GetHashCode() => string.IsNullOrEmpty(this.id) ? 0 : this.id.GetHashCode();

    public override bool Equals(object a)
    {
      return a is LipSyncObjectSerializable objectSerializable && this == objectSerializable;
    }

    public static bool operator ==(LipSyncObjectSerializable a, LipSyncObjectSerializable b)
    {
      return a.id == b.id;
    }

    public static bool operator !=(LipSyncObjectSerializable a, LipSyncObjectSerializable b)
    {
      return !(a == b);
    }

    public override string ToString()
    {
      LipSyncObject lipSyncObject = this.Value;
      return lipSyncObject != null ? Path.GetFileNameWithoutExtension(lipSyncObject.Source) : "";
    }
  }
}
