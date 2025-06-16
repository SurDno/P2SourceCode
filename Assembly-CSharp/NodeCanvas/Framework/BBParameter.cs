using ParadoxNotion;
using ParadoxNotion.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

    public static BBParameter CreateInstance(System.Type t, Blackboard bb)
    {
      if (t == (System.Type) null)
        return (BBParameter) null;
      BBParameter instance = (BBParameter) Activator.CreateInstance(typeof (BBParameter<>).RTMakeGenericType(new System.Type[1]
      {
        t
      }));
      instance.bb = bb;
      return instance;
    }

    public static void SetBBFields(object o, Blackboard bb)
    {
      List<BBParameter> objectBbParameters = BBParameter.GetObjectBBParameters(o);
      for (int index = 0; index < objectBbParameters.Count; ++index)
        objectBbParameters[index].bb = bb;
    }

    public static List<BBParameter> GetObjectBBParameters(object o)
    {
      List<BBParameter> objectBbParameters = new List<BBParameter>();
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
      get => this._targetVariableID;
      set => this._targetVariableID = value;
    }

    public Variable varRef
    {
      get => this._varRef;
      set
      {
        if (this._varRef == value)
          return;
        this._varRef = value;
        this.Bind(value);
      }
    }

    public Blackboard bb
    {
      get => this._bb;
      set
      {
        if (!((UnityEngine.Object) this._bb != (UnityEngine.Object) value))
          return;
        this._bb = value;
        this.varRef = (UnityEngine.Object) value != (UnityEngine.Object) null ? this.ResolveReference(this._bb, true) : (Variable) null;
      }
    }

    public string name
    {
      get => this._name;
      set
      {
        if (!(this._name != value))
          return;
        this._name = value;
        this.varRef = value != null ? this.ResolveReference(this.bb, false) : (Variable) null;
      }
    }

    public bool useBlackboard
    {
      get => this.name != null;
      set
      {
        if (!value)
          this.name = (string) null;
        if (!value || this.name != null)
          return;
        this.name = string.Empty;
      }
    }

    public bool isNone => this.name == string.Empty;

    public bool isNull => object.Equals(this.objectValue, (object) null);

    public System.Type refType => this.varRef?.varType;

    public object value
    {
      get => this.objectValue;
      set => this.objectValue = value;
    }

    protected abstract object objectValue { get; set; }

    public abstract System.Type varType { get; }

    protected abstract void Bind(Variable data);

    private Variable ResolveReference(Blackboard targetBlackboard, bool useID)
    {
      string name = this.name;
      Variable variable = (Variable) null;
      if ((UnityEngine.Object) targetBlackboard == (UnityEngine.Object) null)
        return (Variable) null;
      if (useID && this.targetVariableID != null)
        variable = targetBlackboard.GetVariableByID(this.targetVariableID);
      if (variable == null && !string.IsNullOrEmpty(name))
        variable = targetBlackboard.GetVariable(name, this.varType);
      return variable;
    }

    public Variable PromoteToVariable(Blackboard targetBB)
    {
      if (string.IsNullOrEmpty(this.name))
      {
        this.varRef = (Variable) null;
        return (Variable) null;
      }
      string name = this.name;
      string str = (UnityEngine.Object) targetBB != (UnityEngine.Object) null ? targetBB.name : string.Empty;
      if ((UnityEngine.Object) targetBB == (UnityEngine.Object) null)
      {
        this.varRef = (Variable) null;
        Debug.LogError((object) string.Format("Parameter '{0}' failed to promote to a variable, because Blackboard named '{1}' could not be found.", (object) name, (object) str));
        return (Variable) null;
      }
      this.varRef = targetBB.AddVariable(name, this.varType);
      if (this.varRef == null)
        Debug.LogError((object) string.Format("Parameter {0} (of type '{1}') failed to promote to a Variable in Blackboard '{2}'.", (object) name, (object) this.varType.FriendlyName(), (object) str));
      return this.varRef;
    }

    public override string ToString()
    {
      if (this.isNone)
        return "<b>NONE</b>";
      if (this.useBlackboard)
        return string.Format("<b>${0}</b>", (object) this.name);
      if (this.isNull)
        return "<b>NULL</b>";
      if (this.objectValue is IList)
        return string.Format("<b>{0}</b>", (object) this.varType.FriendlyName());
      return this.objectValue is IDictionary ? string.Format("<b>{0}</b>", (object) this.varType.FriendlyName()) : string.Format("<b>{0}</b>", (object) this.objectValue.ToStringAdvanced());
    }
  }
}
