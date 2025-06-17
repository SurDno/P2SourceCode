using System;
using System.Reflection;
using Cofe.Meta;
using ParadoxNotion;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, Inherited = false)]
  public class PortAttribute(string name, params object[] value) : MemberAttribute 
  {
    public static readonly Guid Id = Guid.NewGuid();

    public PortAttribute(string name) : this(name, null) { }

    public override void ComputeMember(Container container, MemberInfo member)
    {
      FieldInfo field = member as FieldInfo;
      if (field != null)
      {
        ComputeField(container, field);
      }
      else
      {
        MethodInfo method = member as MethodInfo;
        if (method != null)
          ComputeMethod(container, method);
        else
          Debug.LogError("Error compute type : " + member.DeclaringType);
      }
    }

    private void ComputeMethod(Container container, MethodInfo method)
    {
      ParameterInfo[] parameters = method.GetParameters();
      if (parameters.Length == 0 && method.ReturnType != typeof (void))
        container.GetHandler(Id).AddHandle((target, data) =>
        {
          FlowNode instance = (FlowNode) target;
          Type type = typeof (ValueHandler<>);
          Type[] typeArray = [
            method.ReturnType
          ];
          Delegate getter = method.RTCreateDelegate(type.MakeGenericType(typeArray), instance);
          instance.AddValueOutputCommon(name, method.ReturnType, getter);
        });
      else if (method.ReturnType == typeof (void) && parameters.Length == 0)
        container.GetHandler(Id).AddHandle((target, data) =>
        {
          FlowNode instance = (FlowNode) target;
          FlowHandler pointer = (FlowHandler) method.RTCreateDelegate(typeof (FlowHandler), instance);
          instance.AddFlowInput(name, pointer);
        });
      else
        Debug.LogError("Error compute method : " + method.MemberType + " , type " + method.DeclaringType);
    }

    private void ComputeField(Container container, FieldInfo field)
    {
      Type type = field.FieldType;
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (ValueInput<>))
        container.GetHandler(Id).AddHandle((target, data) =>
        {
          FlowNode flowNode = (FlowNode) target;
          ValueInput input = flowNode.AddValueInput(name, type.GetGenericArguments()[0]);
          SetDefaultValue(input, value);
          field.SetValue(flowNode, input);
        });
      else if (type == typeof (FlowOutput))
        container.GetHandler(Id).AddHandle((target, data) =>
        {
          FlowNode flowNode = (FlowNode) target;
          FlowOutput flowOutput = flowNode.AddFlowOutput(name);
          field.SetValue(flowNode, flowOutput);
        });
      else
        Debug.LogError("Error compute field : " + field.MemberType + " , type " + field.DeclaringType);
    }

    private void SetDefaultValue(ValueInput input, object[] value)
    {
      if (value == null)
        return;
      Type genericArgument = input.GetType().GetGenericArguments()[0];
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
