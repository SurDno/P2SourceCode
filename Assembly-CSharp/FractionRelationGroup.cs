using System;
using System.Collections.Generic;
using Engine.Common.Commons;

[Serializable]
public class FractionRelationGroup
{
  [SerializeField]
  public FractionRelationEnum Relation;
  [SerializeField]
  public List<FractionEnum> Fractions;
}
