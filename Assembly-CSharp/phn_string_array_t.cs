// Decompiled with JetBrains decompiler
// Type: phn_string_array_t
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
[Serializable]
public class phn_string_array_t
{
  public string[] phns;

  public phn_string_array_t(string[] _phns) => this.phns = _phns;

  public phn_string_array_t(List<string> _phns)
  {
    this.phns = new string[_phns.Count];
    for (int index = 0; index < _phns.Count; ++index)
      this.phns[index] = _phns[index];
  }
}
