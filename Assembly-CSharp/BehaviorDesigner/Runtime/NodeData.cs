using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System;
using System.Collections.Generic;
using System.Reflection;
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private Vector2 offset;
    private string friendlyName = "";
    private string comment = "";
    private bool isBreakpoint = false;
    private Texture icon;
    private Color color;
    private bool collapsed = false;
    private int colorIndex = 0;
    private List<string> watchedFieldNames = (List<string>) null;
    private List<FieldInfo> watchedFields = (List<FieldInfo>) null;
    private float pushTime = -1f;
    private float popTime = -1f;
    private float interruptTime = -1f;
    private bool isReevaluating = false;
    private TaskStatus executionStatus = TaskStatus.Inactive;

    public void DataWrite(IDataWriter writer)
    {
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "Offset", this.offset);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.offset = BehaviorTreeDataReadUtility.ReadUnity(reader, "Offset", this.offset);
    }

    public object NodeDesigner
    {
      get => this.nodeDesigner;
      set => this.nodeDesigner = value;
    }

    public Vector2 Offset
    {
      get => this.offset;
      set => this.offset = value;
    }

    public string FriendlyName
    {
      get => this.friendlyName;
      set => this.friendlyName = value;
    }

    public string Comment
    {
      get => this.comment;
      set => this.comment = value;
    }

    public bool IsBreakpoint
    {
      get => this.isBreakpoint;
      set => this.isBreakpoint = value;
    }

    public Texture Icon
    {
      get => this.icon;
      set => this.icon = value;
    }

    public Color Color
    {
      get => this.color;
      set => this.color = value;
    }

    public bool Collapsed
    {
      get => this.collapsed;
      set => this.collapsed = value;
    }

    public int ColorIndex
    {
      get => this.colorIndex;
      set => this.colorIndex = value;
    }

    public List<string> WatchedFieldNames
    {
      get => this.watchedFieldNames;
      set => this.watchedFieldNames = value;
    }

    public List<FieldInfo> WatchedFields
    {
      get => this.watchedFields;
      set => this.watchedFields = value;
    }

    public float PushTime
    {
      get => this.pushTime;
      set => this.pushTime = value;
    }

    public float PopTime
    {
      get => this.popTime;
      set => this.popTime = value;
    }

    public float InterruptTime
    {
      get => this.interruptTime;
      set => this.interruptTime = value;
    }

    public bool IsReevaluating
    {
      get => this.isReevaluating;
      set => this.isReevaluating = value;
    }

    public TaskStatus ExecutionStatus
    {
      get => this.executionStatus;
      set => this.executionStatus = value;
    }

    public void InitWatchedFields(Task task)
    {
      if (this.watchedFieldNames == null || this.watchedFieldNames.Count <= 0)
        return;
      this.watchedFields = new List<FieldInfo>();
      for (int index = 0; index < this.watchedFieldNames.Count; ++index)
      {
        FieldInfo field = task.GetType().GetField(this.watchedFieldNames[index], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != (FieldInfo) null)
          this.watchedFields.Add(field);
      }
    }

    public void CopyFrom(NodeData nodeData, Task task)
    {
      this.nodeDesigner = nodeData.NodeDesigner;
      this.offset = nodeData.Offset;
      this.comment = nodeData.Comment;
      this.isBreakpoint = nodeData.IsBreakpoint;
      this.collapsed = nodeData.Collapsed;
      if (nodeData.WatchedFields == null || nodeData.WatchedFields.Count <= 0)
        return;
      this.watchedFields = new List<FieldInfo>();
      this.watchedFieldNames = new List<string>();
      for (int index = 0; index < nodeData.watchedFields.Count; ++index)
      {
        FieldInfo field = task.GetType().GetField(nodeData.WatchedFields[index].Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != (FieldInfo) null)
        {
          this.watchedFields.Add(field);
          this.watchedFieldNames.Add(field.Name);
        }
      }
    }

    public bool ContainsWatchedField(FieldInfo field)
    {
      return this.watchedFields != null && this.watchedFields.Contains(field);
    }

    public void AddWatchedField(FieldInfo field)
    {
      if (this.watchedFields == null)
      {
        this.watchedFields = new List<FieldInfo>();
        this.watchedFieldNames = new List<string>();
      }
      this.watchedFields.Add(field);
      this.watchedFieldNames.Add(field.Name);
    }

    public void RemoveWatchedField(FieldInfo field)
    {
      if (this.watchedFields == null)
        return;
      this.watchedFields.Remove(field);
      this.watchedFieldNames.Remove(field.Name);
    }

    private static Vector2 StringToVector2(string vector2String)
    {
      string[] strArray = vector2String.Substring(1, vector2String.Length - 2).Split(',');
      return (Vector2) new Vector3(float.Parse(strArray[0]), float.Parse(strArray[1]));
    }
  }
}
