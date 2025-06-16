// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.SimplexNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using ParadoxNotion.Design;
using System;
using System.Reflection;

#nullable disable
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
        if (string.IsNullOrEmpty(this._name))
        {
          NameAttribute attribute = ReflectionTools.RTGetAttribute<NameAttribute>(this.GetType(), false);
          this._name = attribute != null ? attribute.name : this.GetType().FriendlyName().SplitCamelCase();
        }
        return this._name;
      }
    }

    public virtual string description
    {
      get
      {
        if (string.IsNullOrEmpty(this._description))
        {
          DescriptionAttribute attribute = ReflectionTools.RTGetAttribute<DescriptionAttribute>(this.GetType(), false);
          this._description = attribute != null ? attribute.description : "No Description";
        }
        return this._description;
      }
    }

    protected ParameterInfo[] parameters
    {
      get
      {
        return this.GetType().GetMethod("Invoke", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).GetParameters();
      }
    }

    public void RegisterPorts(FlowNode node)
    {
      this.OnRegisterPorts(node);
      foreach (PropertyInfo property in this.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
      {
        if (property.CanRead && !property.GetGetMethod().IsVirtual)
          node.AddPropertyOutput(property, (object) this);
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
