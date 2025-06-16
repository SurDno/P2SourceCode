using Engine.Common.Commons;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FractionRelationGroup
{
  [SerializeField]
  public FractionRelationEnum Relation;
  [SerializeField]
  public List<FractionEnum> Fractions;
}
