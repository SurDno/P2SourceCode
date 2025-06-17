using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (NodeData))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class NodeData : IStub, ISerializeDataWrite, ISerializeDataRead
  {
    private object nodeDesigner;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    private Vector2 offset;
    private string friendlyName = "";
    private string comment = "";
    private bool isBreakpoint;
    private Texture icon;
    private Color color;
    private bool collapsed;
    private int colorIndex;
    private List<string> watchedFieldNames;
    private List<FieldInfo> watchedFields;
    private float pushTime = -1f;
    private float popTime = -1f;
    private float interruptTime = -1f;
    private bool isReevaluating;
    private TaskStatus executionStatus = TaskStatus.Inactive;

    public void DataWrite(IDataWriter writer)
    {
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "Offset", offset);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      offset = BehaviorTreeDataReadUtility.ReadUnity(reader, "Offset", offset);
    }

    public object NodeDesigner
    {
      get => nodeDesigner;
      set => nodeDesigner = value;
    }

    public Vector2 Offset
    {
      get => offset;
      set => offset = value;
    }

    public string FriendlyName
    {
      get => friendlyName;
      set => friendlyName = value;
    }

    public string Comment
    {
      get => comment;
      set => comment = value;
    }

    public bool IsBreakpoint
    {
      get => isBreakpoint;
      set => isBreakpoint = value;
    }

    public Texture Icon
    {
      get => icon;
      set => icon = value;
    }

    public Color Color
    {
      get => color;
      set => color = value;
    }

    public bool Collapsed
    {
      get => collapsed;
      set => collapsed = value;
    }

    public int ColorIndex
    {
      get => colorIndex;
      set => colorIndex = value;
    }

    public List<string> WatchedFieldNames
    {
      get => watchedFieldNames;
      set => watchedFieldNames = value;
    }

    public List<FieldInfo> WatchedFields
    {
      get => watchedFields;
      set => watchedFields = value;
    }

    public float PushTime
    {
      get => pushTime;
      set => pushTime = value;
    }

    public float PopTime
    {
      get => popTime;
      set => popTime = value;
    }

    public float InterruptTime
    {
      get => interruptTime;
      set => interruptTime = value;
    }

    public bool IsReevaluating
    {
      get => isReevaluating;
      set => isReevaluating = value;
    }

    public TaskStatus ExecutionStatus
    {
      get => executionStatus;
      set => executionStatus = value;
    }

    public void InitWatchedFields(Task task)
    {
      if (watchedFieldNames == null || watchedFieldNames.Count <= 0)
        return;
      watchedFields = [];
      for (int index = 0; index < watchedFieldNames.Count; ++index)
      {
        FieldInfo field = task.GetType().GetField(watchedFieldNames[index], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
          watchedFields.Add(field);
      }
    }

    public void CopyFrom(NodeData nodeData, Task task)
    {
      nodeDesigner = nodeData.NodeDesigner;
      offset = nodeData.Offset;
      comment = nodeData.Comment;
      isBreakpoint = nodeData.IsBreakpoint;
      collapsed = nodeData.Collapsed;
      if (nodeData.WatchedFields == null || nodeData.WatchedFields.Count <= 0)
        return;
      watchedFields = [];
      watchedFieldNames = [];
      for (int index = 0; index < nodeData.watchedFields.Count; ++index)
      {
        FieldInfo field = task.GetType().GetField(nodeData.WatchedFields[index].Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
          watchedFields.Add(field);
          watchedFieldNames.Add(field.Name);
        }
      }
    }

    public bool ContainsWatchedField(FieldInfo field)
    {
      return watchedFields != null && watchedFields.Contains(field);
    }

    public void AddWatchedField(FieldInfo field)
    {
      if (watchedFields == null)
      {
        watchedFields = [];
        watchedFieldNames = [];
      }
      watchedFields.Add(field);
      watchedFieldNames.Add(field.Name);
    }

    public void RemoveWatchedField(FieldInfo field)
    {
      if (watchedFields == null)
        return;
      watchedFields.Remove(field);
      watchedFieldNames.Remove(field.Name);
    }

    private static Vector2 StringToVector2(string vector2String)
    {
      string[] strArray = vector2String.Substring(1, vector2String.Length - 2).Split(',');
      return new Vector3(float.Parse(strArray[0]), float.Parse(strArray[1]));
    }
  }
}
