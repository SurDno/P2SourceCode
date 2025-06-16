// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.PortAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using ParadoxNotion;
using System;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class PortAttribute : MemberAttribute
  {
    public static readonly Guid Id = Guid.NewGuid();
    private string name;
    private object[] value;

    public PortAttribute(string name)
    {
      this.name = name;
      this.value = (object[]) null;
    }

    public PortAttribute(string name, params object[] value)
    {
      this.name = name;
      this.value = value;
    }

    public override void ComputeMember(Container container, MemberInfo member)
    {
      FieldInfo field = member as FieldInfo;
      if (field != (FieldInfo) null)
      {
        this.ComputeField(container, field);
      }
      else
      {
        MethodInfo method = member as MethodInfo;
        if (method != (MethodInfo) null)
          this.ComputeMethod(container, method);
        else
          Debug.LogError((object) ("Error compute type : " + (object) member.DeclaringType));
      }
    }

    private void ComputeMethod(Container container, MethodInfo method)
    {
      ParameterInfo[] parameters = method.GetParameters();
      if (parameters.Length == 0 && method.ReturnType != typeof (void))
        container.GetHandler(PortAttribute.Id).AddHandle((ComputeHandle) ((target, data) =>
        {
          FlowNode instance = (FlowNode) target;
          System.Type type = typeof (ValueHandler<>);
          System.Type[] typeArray = new System.Type[1]
          {
            method.ReturnType
          };
          Delegate getter = method.RTCreateDelegate(type.MakeGenericType(typeArray), (object) instance);
          instance.AddValueOutputCommon(this.name, method.ReturnType, (object) getter);
        }));
      else if (method.ReturnType == typeof (void) && parameters.Length == 0)
        container.GetHandler(PortAttribute.Id).AddHandle((ComputeHandle) ((target, data) =>
        {
          FlowNode instance = (FlowNode) target;
          FlowHandler pointer = (FlowHandler) method.RTCreateDelegate(typeof (FlowHandler), (object) instance);
          instance.AddFlowInput(this.name, pointer);
        }));
      else
        Debug.LogError((object) ("Error compute method : " + (object) method.MemberType + " , type " + (object) method.DeclaringType));
    }

    private void ComputeField(Container container, FieldInfo field)
    {
      System.Type type = field.FieldType;
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (ValueInput<>))
        container.GetHandler(PortAttribute.Id).AddHandle((ComputeHandle) ((target, data) =>
        {
          FlowNode flowNode = (FlowNode) target;
          ValueInput input = flowNode.AddValueInput(this.name, type.GetGenericArguments()[0]);
          this.SetDefaultValue(input, this.value);
          field.SetValue((object) flowNode, (object) input);
        }));
      else if (type == typeof (FlowOutput))
        container.GetHandler(PortAttribute.Id).AddHandle((ComputeHandle) ((target, data) =>
        {
          FlowNode flowNode = (FlowNode) target;
          FlowOutput flowOutput = flowNode.AddFlowOutput(this.name);
          field.SetValue((object) flowNode, (object) flowOutput);
        }));
      else
        Debug.LogError((object) ("Error compute field : " + (object) field.MemberType + " , type " + (object) field.DeclaringType));
    }

    private void SetDefaultValue(ValueInput input, object[] value)
    {
      if (value == null)
        return;
      System.Type genericArgument = input.GetType().GetGenericArguments()[0];
      if (value.Length == 1 && genericArgument == value[0].GetType())
      {
        input.serializedValue = value[0];
      }
      else
      {
        object instance = Activator.CreateInstance(genericArgument, value);
        input.serializedValue = instance;
      }
    }
  }
}
