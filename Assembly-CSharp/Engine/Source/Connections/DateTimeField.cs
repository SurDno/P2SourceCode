// Decompiled with JetBrains decompiler
// Type: Engine.Source.Connections.DateTimeField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Connections
{
  [Serializable]
  public struct DateTimeField : ISerializationCallbackReceiver
  {
    [SerializeField]
    private string data;

    public DateTime Value { get; set; }

    public void OnAfterDeserialize()
    {
      DateTime result;
      if (this.data != null && DefaultConverter.TryParseDateTime(this.data, out result))
        this.Value = result;
      else
        this.Value = DateTime.MinValue;
    }

    public void OnBeforeSerialize()
    {
      if (this.Value != DateTime.MinValue)
        this.data = DefaultConverter.ToString(this.Value);
      else
        this.data = "";
    }
  }
}
