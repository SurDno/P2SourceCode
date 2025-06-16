using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConvertingNamesItem
{
  [SerializeField]
  public string animatorClipName;
  [SerializeField]
  public List<string> sourceClipNames;
}
