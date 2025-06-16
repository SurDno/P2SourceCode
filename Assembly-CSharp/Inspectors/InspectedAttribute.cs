using System;
using System.Reflection;
using Cofe.Meta;
using Cofe.Utility;

namespace Inspectors
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
  public class InspectedAttribute : MemberAttribute
  {
    public static readonly Guid DrawEditInspectedId = Guid.NewGuid();
    public static readonly Guid DrawRuntimeInspectedId = Guid.NewGuid();
    public static readonly Guid HasInspectedId = Guid.NewGuid();
    public static readonly Guid HeaderInspectedId = Guid.NewGuid();
    private ExecuteMode mode = ExecuteMode.Runtime;

    public bool Mutable { get; set; }

    public bool Header { get; set; }

    public Type Type { get; set; }

    public string Name { get; set; }

    public ExecuteMode Mode
    {
      get => mode;
      set => mode = value;
    }

    public override void ComputeMember(Container container, MemberInfo member)
    {
      container.GetHandler(HasInspectedId).AddHandle((target, data) => ((IHasInspected) data).HasInspected(mode));
      FieldInfo field = member as FieldInfo;
      if (field != null)
      {
        ComputeField(container, field);
      }
      else
      {
        PropertyInfo property = member as PropertyInfo;
        if (property != null)
        {
          ComputeProperty(container, property);
        }
        else
        {
          MethodInfo method = member as MethodInfo;
          if (!(method != null))
            return;
          ComputeMethod(container, method);
        }
      }
    }

    private void ComputeMethod(Container container, MethodInfo method)
    {
      ComputeHandle handle = (target, data) =>
      {
        InspectedContext inspectedContext = (InspectedContext) data;
        string name1;
        if (!Name.IsNullOrEmpty())
        {
          name1 = Name;
        }
        else
        {
          string name2 = method.Name;
          if (name2.StartsWith("_"))
            name2 = name2.Substring(1);
          name1 = name2.ToUpperFirstCharacter();
        }
        Type type1 = Type;
        if ((object) type1 == null)
          type1 = typeof (DefaultMethodDrawer);
        Type type2 = type1;
        inspectedContext.Provider.DrawInspected(name1, type2, null, true, target, method, parameters => method.Invoke(target, (object[]) parameters));
      };
      if ((mode & ExecuteMode.Runtime) != 0)
        container.GetHandler(DrawRuntimeInspectedId).AddHandle(handle);
      if ((mode & ExecuteMode.Edit) == 0)
        return;
      container.GetHandler(DrawEditInspectedId).AddHandle(handle);
    }

    private void ComputeProperty(Container container, PropertyInfo property)
    {
      if (property.GetGetMethod(true) == null)
        return;
      if (Header)
        container.GetHandler(HeaderInspectedId).AddHandle((target, data) =>
        {
          IInspectedProvider inspectedProvider = (IInspectedProvider) data;
          object obj = property.GetValue(target, null);
          if (obj == null)
            return;
          inspectedProvider.SetHeader(obj.ToString());
        });
      bool hasMutator = property.GetSetMethod(true) != null;
      ComputeHandle handle = (target, data) =>
      {
        InspectedContext provider = (InspectedContext) data;
        object obj = property.GetValue(target, null);
        string name1;
        if (!Name.IsNullOrEmpty())
        {
          name1 = Name;
        }
        else
        {
          string name2 = property.Name;
          if (name2.StartsWith("_"))
            name2 = name2.Substring(1);
          name1 = name2.ToUpperFirstCharacter();
        }
        provider.Provider.DrawInspected(name1, property.PropertyType, obj, Mutable & hasMutator, target, property, value2 =>
        {
          if (property.CanWrite)
            property.SetValue(target, value2, null);
          Action<object> setter = provider.Setter;
          if (setter == null)
            return;
          setter(target);
        });
      };
      if ((mode & ExecuteMode.Runtime) != 0)
        container.GetHandler(DrawRuntimeInspectedId).AddHandle(handle);
      if ((mode & ExecuteMode.Edit) == 0)
        return;
      container.GetHandler(DrawEditInspectedId).AddHandle(handle);
    }

    private void ComputeField(Container container, FieldInfo field)
    {
      if (Header)
        container.GetHandler(HeaderInspectedId).AddHandle((target, data) =>
        {
          IInspectedProvider inspectedProvider = (IInspectedProvider) data;
          object obj = field.GetValue(target);
          if (obj == null)
            return;
          inspectedProvider.SetHeader(obj.ToString());
        });
      ComputeHandle handle = (target, data) =>
      {
        InspectedContext provider = (InspectedContext) data;
        object obj = field.GetValue(target);
        string name1;
        if (!Name.IsNullOrEmpty())
        {
          name1 = Name;
        }
        else
        {
          string name2 = field.Name;
          if (name2.StartsWith("_"))
            name2 = name2.Substring(1);
          name1 = name2.ToUpperFirstCharacter();
        }
        provider.Provider.DrawInspected(name1, field.FieldType, obj, Mutable, target, field, value2 =>
        {
          field.SetValue(target, value2);
          Action<object> setter = provider.Setter;
          if (setter == null)
            return;
          setter(target);
        });
      };
      if ((mode & ExecuteMode.Runtime) != 0)
        container.GetHandler(DrawRuntimeInspectedId).AddHandle(handle);
      if ((mode & ExecuteMode.Edit) == 0)
        return;
      container.GetHandler(DrawEditInspectedId).AddHandle(handle);
    }
  }
}
