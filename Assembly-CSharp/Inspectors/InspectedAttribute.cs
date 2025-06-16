using Cofe.Meta;
using Cofe.Utility;
using System;
using System.Reflection;

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
      get => this.mode;
      set => this.mode = value;
    }

    public override void ComputeMember(Container container, MemberInfo member)
    {
      container.GetHandler(InspectedAttribute.HasInspectedId).AddHandle((ComputeHandle) ((target, data) => ((IHasInspected) data).HasInspected(this.mode)));
      FieldInfo field = member as FieldInfo;
      if (field != (FieldInfo) null)
      {
        this.ComputeField(container, field);
      }
      else
      {
        PropertyInfo property = member as PropertyInfo;
        if (property != (PropertyInfo) null)
        {
          this.ComputeProperty(container, property);
        }
        else
        {
          MethodInfo method = member as MethodInfo;
          if (!(method != (MethodInfo) null))
            return;
          this.ComputeMethod(container, method);
        }
      }
    }

    private void ComputeMethod(Container container, MethodInfo method)
    {
      ComputeHandle handle = (ComputeHandle) ((target, data) =>
      {
        InspectedContext inspectedContext = (InspectedContext) data;
        string name1;
        if (!this.Name.IsNullOrEmpty())
        {
          name1 = this.Name;
        }
        else
        {
          string name2 = method.Name;
          if (name2.StartsWith("_"))
            name2 = name2.Substring(1);
          name1 = name2.ToUpperFirstCharacter();
        }
        Type type1 = this.Type;
        if ((object) type1 == null)
          type1 = typeof (DefaultMethodDrawer);
        Type type2 = type1;
        inspectedContext.Provider.DrawInspected(name1, type2, (object) null, true, target, (MemberInfo) method, (Action<object>) (parameters => method.Invoke(target, (object[]) parameters)));
      });
      if ((this.mode & ExecuteMode.Runtime) != 0)
        container.GetHandler(InspectedAttribute.DrawRuntimeInspectedId).AddHandle(handle);
      if ((this.mode & ExecuteMode.Edit) == 0)
        return;
      container.GetHandler(InspectedAttribute.DrawEditInspectedId).AddHandle(handle);
    }

    private void ComputeProperty(Container container, PropertyInfo property)
    {
      if (property.GetGetMethod(true) == (MethodInfo) null)
        return;
      if (this.Header)
        container.GetHandler(InspectedAttribute.HeaderInspectedId).AddHandle((ComputeHandle) ((target, data) =>
        {
          IInspectedProvider inspectedProvider = (IInspectedProvider) data;
          object obj = property.GetValue(target, (object[]) null);
          if (obj == null)
            return;
          inspectedProvider.SetHeader(obj.ToString());
        }));
      bool hasMutator = property.GetSetMethod(true) != (MethodInfo) null;
      ComputeHandle handle = (ComputeHandle) ((target, data) =>
      {
        InspectedContext provider = (InspectedContext) data;
        object obj = property.GetValue(target, (object[]) null);
        string name1;
        if (!this.Name.IsNullOrEmpty())
        {
          name1 = this.Name;
        }
        else
        {
          string name2 = property.Name;
          if (name2.StartsWith("_"))
            name2 = name2.Substring(1);
          name1 = name2.ToUpperFirstCharacter();
        }
        provider.Provider.DrawInspected(name1, property.PropertyType, obj, this.Mutable & hasMutator, target, (MemberInfo) property, (Action<object>) (value2 =>
        {
          if (property.CanWrite)
            property.SetValue(target, value2, (object[]) null);
          Action<object> setter = provider.Setter;
          if (setter == null)
            return;
          setter(target);
        }));
      });
      if ((this.mode & ExecuteMode.Runtime) != 0)
        container.GetHandler(InspectedAttribute.DrawRuntimeInspectedId).AddHandle(handle);
      if ((this.mode & ExecuteMode.Edit) == 0)
        return;
      container.GetHandler(InspectedAttribute.DrawEditInspectedId).AddHandle(handle);
    }

    private void ComputeField(Container container, FieldInfo field)
    {
      if (this.Header)
        container.GetHandler(InspectedAttribute.HeaderInspectedId).AddHandle((ComputeHandle) ((target, data) =>
        {
          IInspectedProvider inspectedProvider = (IInspectedProvider) data;
          object obj = field.GetValue(target);
          if (obj == null)
            return;
          inspectedProvider.SetHeader(obj.ToString());
        }));
      ComputeHandle handle = (ComputeHandle) ((target, data) =>
      {
        InspectedContext provider = (InspectedContext) data;
        object obj = field.GetValue(target);
        string name1;
        if (!this.Name.IsNullOrEmpty())
        {
          name1 = this.Name;
        }
        else
        {
          string name2 = field.Name;
          if (name2.StartsWith("_"))
            name2 = name2.Substring(1);
          name1 = name2.ToUpperFirstCharacter();
        }
        provider.Provider.DrawInspected(name1, field.FieldType, obj, this.Mutable, target, (MemberInfo) field, (Action<object>) (value2 =>
        {
          field.SetValue(target, value2);
          Action<object> setter = provider.Setter;
          if (setter == null)
            return;
          setter(target);
        }));
      });
      if ((this.mode & ExecuteMode.Runtime) != 0)
        container.GetHandler(InspectedAttribute.DrawRuntimeInspectedId).AddHandle(handle);
      if ((this.mode & ExecuteMode.Edit) == 0)
        return;
      container.GetHandler(InspectedAttribute.DrawEditInspectedId).AddHandle(handle);
    }
  }
}
