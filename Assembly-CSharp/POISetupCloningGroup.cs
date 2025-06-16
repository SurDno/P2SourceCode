using System;
using System.Collections.Generic;

[Serializable]
public class POISetupCloningGroup
{
  [SerializeField]
  public POISetup Base;
  [SerializeField]
  public List<POISetup> Clones;
}
