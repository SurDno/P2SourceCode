// Decompiled with JetBrains decompiler
// Type: Inspectors.InspectedDrawerUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Inspectors
{
  public static class InspectedDrawerUtility
  {
    public static InspectedDrawerUtility.Foldout BeginComplex(
      string name,
      string displayName,
      IExpandedProvider context,
      IInspectedDrawer drawer,
      Action contextMenu = null)
    {
      InspectedDrawerUtility.Foldout foldout = new InspectedDrawerUtility.Foldout()
      {
        ExpandInternal = context.GetExpanded(context.DeepName + name)
      };
      foldout.Expand = drawer.Foldout(displayName, foldout.ExpandInternal, contextMenu);
      if (foldout.Expand)
      {
        foldout.IndentLevel = drawer.IndentLevel;
        ++drawer.IndentLevel;
        foldout.Name = context.DeepName;
        context.DeepName += name;
      }
      return foldout;
    }

    public static void EndComplex(
      InspectedDrawerUtility.Foldout fold,
      string name,
      IExpandedProvider context,
      IInspectedDrawer drawer)
    {
      if (fold.Expand)
      {
        drawer.IndentLevel = fold.IndentLevel;
        context.DeepName = fold.Name;
      }
      if (fold.ExpandInternal && !fold.Expand)
      {
        context.SetExpanded(context.DeepName + name, false);
      }
      else
      {
        if (fold.ExpandInternal || !fold.Expand)
          return;
        context.SetExpanded(context.DeepName + name, true);
      }
    }

    public struct Foldout
    {
      public bool ExpandInternal;
      public bool Expand;
      public int IndentLevel;
      public string Name;
    }
  }
}
