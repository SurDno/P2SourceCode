using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Framework
{
  [SpoofAOT]
  [Serializable]
  public abstract class BBParameter
  {
    [SerializeField]
    private string _name;
    [SerializeField]
    private string _targetVariableID;
    [NonSerialized]
    private Blackboard _bb;
    [NonSerialized]
    private Variable _varRef;

    public static BBParameter CreateInstance(Type t, Blackboard bb)
    {
      if (t == null)
        return null;
      BBParameter instance = (BBParameter) Activator.CreateInstance(typeof (BBParameter<>).RTMakeGenericType([
        t
      ]));
      instance.bb = bb;
      return instance;
    }

    public static void SetBBFields(object o, Blackboard bb)
    {
      List<BBParameter> objectBbParameters = GetObjectBBParameters(o);
      for (int index = 0; index < objectBbParameters.Count; ++index)
        objectBbParameters[index].bb = bb;
    }

    public static List<BBParameter> GetObjectBBParameters(object o)
    {
      List<BBParameter> objectBbParameters = [];
      if (o == null)
        return objectBbParameters;
      foreach (FieldInfo field in o.GetType().RTGetFields())
      {
        if (typeof (BBParameter).RTIsAssignableFrom(field.FieldType))
        {
          object instance = field.GetValue(o);
          if (instance == null && field.FieldType != typeof (BBParameter))
          {
            instance = Activator.CreateInstance(field.FieldType);
            field.SetValue(o, instance);
          }
          if (instance != null)
            objectBbParameters.Add((BBParameter) instance);
        }
        else if (typeof (IList).RTIsAssignableFrom(field.FieldType) && !field.FieldType.IsArray && typeof (BBParameter).RTIsAssignableFrom(field.FieldType.RTGetGenericArguments()[0]) && field.GetValue(o) is IList list)
        {
          for (int index = 0; index < list.Count; ++index)
          {
            object instance = list[index];
            if (instance == null && field.FieldType != typeof (BBParameter))
            {
              instance = Activator.CreateInstance(field.FieldType.RTGetGenericArguments()[0]);
              list[index] = instance;
            }
            if (instance != null)
              objectBbParameters.Add((BBParameter) instance);
          }
        }
      }
      return objectBbParameters;
    }

    private string targetVariableID
    {
      get => _targetVariableID;
      set => _targetVariableID = value;
    }

    public Variable varRef
    {
      get => _varRef;
      set
      {
        if (_varRef == value)
          return;
        _varRef = value;
        Bind(value);
      }
    }

    public Blackboard bb
    {
      get => _bb;
      set
      {
        if (!(_bb != value))
          return;
        _bb = value;
        varRef = value != null ? ResolveReference(_bb, true) : null;
      }
    }

    public string name
    {
      get => _name;
      set
      {
        if (!(_name != value))
          return;
        _name = value;
        varRef = value != null ? ResolveReference(bb, false) : null;
      }
    }

    public bool useBlackboard
    {
      get => name != null;
      set
      {
        if (!value)
          name = null;
        if (!value || name != null)
          return;
        name = string.Empty;
      }
    }

    public bool isNone => name == string.Empty;

    public bool isNull => Equals(objectValue, null);

    public Type refType => varRef?.varType;

    public object value
    {
      get => objectValue;
      set => objectValue = value;
    }

    protected abstract object objectValue { get; set; }

    public abstract Type varType { get; }

    protected abstract void Bind(Variable data);

    private Variable ResolveReference(Blackboard targetBlackboard, bool useID)
    {
      string name = this.name;
      Variable variable = null;
      if (targetBlackboard == null)
        return null;
      if (useID && targetVariableID != null)
        variable = targetBlackboard.GetVariableByID(targetVariableID);
      if (variable == null && !string.IsNullOrEmpty(name))
        variable = targetBlackboard.GetVariable(name, varType);
      return variable;
    }

    public Variable PromoteToVariable(Blackboard targetBB)
    {
      if (string.IsNullOrEmpty(this.name))
      {
        varRef = null;
        return null;
      }
      string name = this.name;
      string str = targetBB != null ? targetBB.name : string.Empty;
      if (targetBB == null)
      {
        varRef = null;
        Debug.LogError(string.Format("Parameter '{0}' failed to promote to a variable, because Blackboard named '{1}' could not be found.", name, str));
        return null;
      }
      varRef = targetBB.AddVariable(name, varType);
      if (varRef == null)
        Debug.LogError(string.Format("Parameter {0} (of type '{1}') failed to promote to a Variable in Blackboard '{2}'.", name, varType.FriendlyName(), str));
      return varRef;
    }

    public override string ToString()
    {
      if (isNone)
        return "<b>NONE</b>";
      if (useBlackboard)
        return string.Format("<b>${0}</b>", name);
      if (isNull)
        return "<b>NULL</b>";
      if (objectValue is IList)
        return string.Format("<b>{0}</b>", varType.FriendlyName());
      return objectValue is IDictionary ? string.Format("<b>{0}</b>", varType.FriendlyName()) : string.Format("<b>{0}</b>", objectValue.ToStringAdvanced());
    }
  }
}
