// Decompiled with JetBrains decompiler
// Type: Inspectors.IInspectedProvider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

#nullable disable
namespace Inspectors
{
  public interface IInspectedProvider : IExpandedProvider
  {
    string ElementName { get; set; }

    void DrawInspected(
      string name,
      Type type,
      object value,
      bool mutable,
      object target,
      MemberInfo member,
      Action<object> setter);

    Guid DrawId { get; }

    Guid NameId { get; }

    void SetHeader(string name);

    int ContextIndex { get; set; }

    Action ContextItemMenu { get; set; }

    object ContextObject { get; set; }
  }
}
