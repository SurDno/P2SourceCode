using BehaviorDesigner.Runtime.Tasks;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Engine.Common.Commons.Converters;
using Engine.Common.Comparers;
using Scripts.Tools.Serializations.Converters;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
  public static class XmlDeserialization
  {
    public static void LoadXml(
      TaskSerializationData taskData,
      BehaviorSource behaviorSource,
      string context)
    {
      behaviorSource.EntryTask = (Task) null;
      behaviorSource.RootTask = (Task) null;
      behaviorSource.DetachedTasks = (List<Task>) null;
      behaviorSource.Variables = (List<SharedVariable>) null;
      BehaviorTreeDataContext.Tasks = new Dictionary<int, Task>((IEqualityComparer<int>) IntComparer.Instance);
      BehaviorTreeDataContext.Variables = new Dictionary<string, SharedVariable>();
      BehaviorTreeDataContext.ContextUnityObjects = taskData.unityObjects;
      try
      {
        BehaviorSourceData behaviorSourceData = DefaultDataReadUtility.ReadSerialize<BehaviorSourceData>((IDataReader) new XmlNodeDataReader((XmlNode) XmlDeserializationCache.GetOrCreateData(taskData.XmlData), context), "Object");
        behaviorSource.SetAllVariables(behaviorSourceData.Variables);
        behaviorSource.EntryTask = behaviorSourceData.EntryTask;
        behaviorSource.RootTask = behaviorSourceData.RootTask;
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ("Exception : " + ex.ToString() + "\r\ncontext : " + context));
      }
      BehaviorTreeDataContext.Tasks = (Dictionary<int, Task>) null;
      BehaviorTreeDataContext.Variables = (Dictionary<string, SharedVariable>) null;
      BehaviorTreeDataContext.ContextUnityObjects = (List<UnityEngine.Object>) null;
    }
  }
}
