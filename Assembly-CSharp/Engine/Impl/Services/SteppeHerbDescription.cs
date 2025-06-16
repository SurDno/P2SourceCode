using System;
using Engine.Source.Connections;
using UnityEngine;

namespace Engine.Impl.Services
{
  [Serializable]
  public class SteppeHerbDescription
  {
    public IEntitySerializable Entity;
    public GameObject PointsPrefab;
  }
}
