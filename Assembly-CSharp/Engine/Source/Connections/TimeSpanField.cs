// Decompiled with JetBrains decompiler
// Type: Engine.Source.Connections.TimeSpanField
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
  public struct TimeSpanField : ISerializationCallbackReceiver
  {
    [SerializeField]
    private string data;

    public TimeSpan Value { get; set; }

    public void OnAfterDeserialize()
    {
      TimeSpan result;
      if (this.data != null && DefaultConverter.TryParseTimeSpan(this.data, out result))
        this.Value = result;
      else
        this.Value = TimeSpan.Zero;
    }

    public void OnBeforeSerialize()
    {
      if (this.Value != TimeSpan.Zero)
        this.data = DefaultConverter.ToString(this.Value);
      else
        this.data = "";
    }
  }
}
