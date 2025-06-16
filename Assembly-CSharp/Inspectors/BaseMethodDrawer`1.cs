// Decompiled with JetBrains decompiler
// Type: Inspectors.BaseMethodDrawer`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Inspectors
{
  public abstract class BaseMethodDrawer<T> : IMethodDrawer where T : class, IMethodDrawer, new()
  {
    public static readonly T Instance = new T();

    public abstract void DrawInspected(
      string name,
      Type type,
      object value,
      bool mutable,
      IInspectedProvider context,
      IInspectedDrawer drawer,
      Action<object> setter);
  }
}
