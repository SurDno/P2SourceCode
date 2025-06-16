using System;
using System.Collections.Generic;
using System.Reflection;
using Cofe.Meta;

namespace Inspectors
{
  public class InspectedProvider : IInspectedProvider, IExpandedProvider
  {
    private static HashSet<string> expanded = new HashSet<string>();
    private IInspectedDrawer drawer;

    public InspectedProvider(IInspectedDrawer drawer) => this.drawer = drawer;

    public void DrawInspected(
      string name,
      Type type,
      object value,
      bool mutable,
      object target,
      MemberInfo member,
      Action<object> setter)
    {
      GUI.enabled = mutable;
      InspectedDrawerService.DrawerHandle drawer1 = InspectedDrawerService.GetDrawer(type);
      if (drawer1 != null)
      {
        drawer1(name, type, value, mutable, this, drawer, target, member, setter);
      }
      else
      {
        if (type.IsGenericType)
        {
          drawer1 = InspectedDrawerService.GetDrawer(type.GetGenericTypeDefinition());
          if (drawer1 != null)
            drawer1(name, type, value, mutable, this, drawer, target, member, setter);
        }
        if (drawer1 == null)
        {
          InspectedDrawerService.DrawerHandle drawer2 = InspectedDrawerService.GetDrawer(typeof (object));
          if (drawer2 != null)
            drawer2(name, type, value, mutable, this, drawer, target, member, setter);
        }
      }
      GUI.enabled = true;
    }

    public void Draw(object target, Action<object> setter)
    {
      drawer.IndentLevel = 0;
      DeepName = "";
      MetaService.Compute(target, DrawId, new InspectedContext {
        Provider = this,
        Setter = setter
      });
    }

    public void SetHeader(string name)
    {
      if (string.IsNullOrEmpty(ElementName))
        ElementName = name;
      else
        ElementName = ElementName + " | " + name;
    }

    public void SetExpanded(string name, bool value)
    {
      if (value)
        expanded.Add(name);
      else
        expanded.Remove(name);
    }

    public bool GetExpanded(string name) => expanded.Contains(name);

    public string DeepName { get; set; }

    public object ContextObject { get; set; }

    public string ElementName { get; set; }

    public Guid DrawId
    {
      get
      {
        return Application.isPlaying ? InspectedAttribute.DrawRuntimeInspectedId : InspectedAttribute.DrawEditInspectedId;
      }
    }

    public Guid NameId => InspectedAttribute.HeaderInspectedId;

    public int ContextIndex { get; set; }

    public Action ContextItemMenu { get; set; }
  }
}
