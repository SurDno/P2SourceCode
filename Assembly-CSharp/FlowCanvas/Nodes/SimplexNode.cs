using System;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [SpoofAOT]
  public abstract class SimplexNode
  {
    [NonSerialized]
    private string _name;
    [NonSerialized]
    private string _description;

    public virtual string name
    {
      get
      {
        if (string.IsNullOrEmpty(_name))
        {
          NameAttribute attribute = GetType().RTGetAttribute<NameAttribute>(false);
          _name = attribute != null ? attribute.name : GetType().FriendlyName().SplitCamelCase();
        }
        return _name;
      }
    }

    public virtual string description
    {
      get
      {
        if (string.IsNullOrEmpty(_description))
        {
          DescriptionAttribute attribute = GetType().RTGetAttribute<DescriptionAttribute>(false);
          _description = attribute != null ? attribute.description : "No Description";
        }
        return _description;
      }
    }

    protected ParameterInfo[] parameters
    {
      get
      {
        return GetType().GetMethod("Invoke", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).GetParameters();
      }
    }

    public void RegisterPorts(FlowNode node)
    {
      OnRegisterPorts(node);
      foreach (PropertyInfo property in GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
      {
        if (property.CanRead && !property.GetGetMethod().IsVirtual)
          node.AddPropertyOutput(property, this);
      }
    }

    protected abstract void OnRegisterPorts(FlowNode node);

    public virtual void OnGraphStarted()
    {
    }

    public virtual void OnGraphPaused()
    {
    }

    public virtual void OnGraphUnpaused()
    {
    }

    public virtual void OnGraphStoped()
    {
    }
  }
}
