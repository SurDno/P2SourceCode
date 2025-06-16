using System;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime
{
  [Serializable]
  public class TaskSerializationData
  {
    [SerializeField]
    public string XmlData;
    [SerializeField]
    public List<UnityEngine.Object> unityObjects = new List<UnityEngine.Object>();
  }
}
