﻿using ParadoxNotion.Serialization.FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public class fsAotCompilationManager
  {
    private static Dictionary<Type, string> _computedAotCompilations = new Dictionary<Type, string>();
    private static List<fsAotCompilationManager.AotCompilation> _uncomputedAotCompilations = new List<fsAotCompilationManager.AotCompilation>();

    public static Dictionary<Type, string> AvailableAotCompilations
    {
      get
      {
        for (int index = 0; index < fsAotCompilationManager._uncomputedAotCompilations.Count; ++index)
        {
          fsAotCompilationManager.AotCompilation uncomputedAotCompilation = fsAotCompilationManager._uncomputedAotCompilations[index];
          fsAotCompilationManager._computedAotCompilations[uncomputedAotCompilation.Type] = fsAotCompilationManager.GenerateDirectConverterForTypeInCSharp(uncomputedAotCompilation.Type, uncomputedAotCompilation.Members, uncomputedAotCompilation.IsConstructorPublic);
        }
        fsAotCompilationManager._uncomputedAotCompilations.Clear();
        return fsAotCompilationManager._computedAotCompilations;
      }
    }

    public static bool TryToPerformAotCompilation(
      fsConfig config,
      Type type,
      out string aotCompiledClassInCSharp)
    {
      if (fsMetaType.Get(config, type).EmitAotData())
      {
        aotCompiledClassInCSharp = fsAotCompilationManager.AvailableAotCompilations[type];
        return true;
      }
      aotCompiledClassInCSharp = (string) null;
      return false;
    }

    public static void AddAotCompilation(
      Type type,
      fsMetaProperty[] members,
      bool isConstructorPublic)
    {
      fsAotCompilationManager._uncomputedAotCompilations.Add(new fsAotCompilationManager.AotCompilation()
      {
        Type = type,
        Members = members,
        IsConstructorPublic = isConstructorPublic
      });
    }

    private static string GetConverterString(fsMetaProperty member)
    {
      return member.OverrideConverterType == (Type) null ? "null" : string.Format("typeof({0})", (object) member.OverrideConverterType.CSharpName(true));
    }

    private static string GenerateDirectConverterForTypeInCSharp(
      Type type,
      fsMetaProperty[] members,
      bool isConstructorPublic)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = type.CSharpName(true);
      string str2 = type.CSharpName(true, true);
      stringBuilder.AppendLine("using System;");
      stringBuilder.AppendLine("using System.Collections.Generic;");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("namespace FullSerializer {");
      stringBuilder.AppendLine("    partial class fsConverterRegistrar {");
      stringBuilder.AppendLine("        public static Speedup." + str2 + "_DirectConverter Register_" + str2 + ";");
      stringBuilder.AppendLine("    }");
      stringBuilder.AppendLine("}");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("namespace FullSerializer.Speedup {");
      stringBuilder.AppendLine("    public class " + str2 + "_DirectConverter : fsDirectConverter<" + str1 + "> {");
      stringBuilder.AppendLine("        protected override fsResult DoSerialize(" + str1 + " model, Dictionary<string, fsData> serialized) {");
      stringBuilder.AppendLine("            var result = fsResult.Success;");
      stringBuilder.AppendLine();
      foreach (fsMetaProperty member in members)
        stringBuilder.AppendLine("            result += SerializeMember(serialized, " + fsAotCompilationManager.GetConverterString(member) + ", \"" + member.JsonName + "\", model." + member.MemberName + ");");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("            return result;");
      stringBuilder.AppendLine("        }");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref " + str1 + " model) {");
      stringBuilder.AppendLine("            var result = fsResult.Success;");
      stringBuilder.AppendLine();
      for (int index = 0; index < members.Length; ++index)
      {
        fsMetaProperty member = members[index];
        stringBuilder.AppendLine("            var t" + (object) index + " = model." + member.MemberName + ";");
        stringBuilder.AppendLine("            result += DeserializeMember(data, " + fsAotCompilationManager.GetConverterString(member) + ", \"" + member.JsonName + "\", out t" + (object) index + ");");
        stringBuilder.AppendLine("            model." + member.MemberName + " = t" + (object) index + ";");
        stringBuilder.AppendLine();
      }
      stringBuilder.AppendLine("            return result;");
      stringBuilder.AppendLine("        }");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("        public override object CreateInstance(fsData data, Type storageType) {");
      if (isConstructorPublic)
        stringBuilder.AppendLine("            return new " + str1 + "();");
      else
        stringBuilder.AppendLine("            return Activator.CreateInstance(typeof(" + str1 + "), /*nonPublic:*/true);");
      stringBuilder.AppendLine("        }");
      stringBuilder.AppendLine("    }");
      stringBuilder.AppendLine("}");
      return stringBuilder.ToString();
    }

    private struct AotCompilation
    {
      public Type Type;
      public fsMetaProperty[] Members;
      public bool IsConstructorPublic;
    }
  }
}
