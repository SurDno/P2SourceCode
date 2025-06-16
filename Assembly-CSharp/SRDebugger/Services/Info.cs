// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.Info
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRF;
using System;

#nullable disable
namespace SRDebugger.Services
{
  public class Info : ISystemInfo
  {
    private Func<object> _valueGetter;

    public string Title { get; set; }

    public object Value
    {
      get
      {
        try
        {
          return this._valueGetter();
        }
        catch (Exception ex)
        {
          return (object) "Error ({0})".Fmt((object) ex.GetType().Name);
        }
      }
    }

    public bool IsPrivate { get; set; }

    public static Info Create(string name, Func<object> getter, bool isPrivate = false)
    {
      return new Info()
      {
        Title = name,
        _valueGetter = getter,
        IsPrivate = isPrivate
      };
    }

    public static Info Create(string name, object value, bool isPrivate = false)
    {
      return new Info()
      {
        Title = name,
        _valueGetter = (Func<object>) (() => value),
        IsPrivate = isPrivate
      };
    }
  }
}
