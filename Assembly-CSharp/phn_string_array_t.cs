using System;
using System.Collections.Generic;

[Serializable]
public class phn_string_array_t
{
  public string[] phns;

  public phn_string_array_t(string[] _phns) => phns = _phns;

  public phn_string_array_t(List<string> _phns)
  {
    phns = new string[_phns.Count];
    for (int index = 0; index < _phns.Count; ++index)
      phns[index] = _phns[index];
  }
}
