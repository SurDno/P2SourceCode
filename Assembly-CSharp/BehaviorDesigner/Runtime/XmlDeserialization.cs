using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Serializations.Data.Xml;
using Engine.Common.Commons.Converters;
using Engine.Common.Comparers;
using Scripts.Tools.Serializations.Converters;
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
      behaviorSource.EntryTask = null;
      behaviorSource.RootTask = null;
      behaviorSource.DetachedTasks = null;
      behaviorSource.Variables = null;
      BehaviorTreeDataContext.Tasks = new Dictionary<int, Task>(IntComparer.Instance);
      BehaviorTreeDataContext.Variables = new Dictionary<string, SharedVariable>();
      BehaviorTreeDataContext.ContextUnityObjects = taskData.unityObjects;
      try
      {
        BehaviorSourceData behaviorSourceData = DefaultDataReadUtility.ReadSerialize<BehaviorSourceData>(new XmlNodeDataReader(XmlDeserializationCache.GetOrCreateData(taskData.XmlData), context), "Object");
        behaviorSource.SetAllVariables(behaviorSourceData.Variables);
        behaviorSource.EntryTask = behaviorSourceData.EntryTask;
        behaviorSource.RootTask = behaviorSourceData.RootTask;
      }
      catch (Exception ex)
      {
        Debug.LogError("Exception : " + ex + "\r\ncontext : " + context);
      }
      BehaviorTreeDataContext.Tasks = null;
      BehaviorTreeDataContext.Variables = null;
      BehaviorTreeDataContext.ContextUnityObjects = null;
    }
  }
}
