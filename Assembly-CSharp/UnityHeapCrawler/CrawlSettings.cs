// Decompiled with JetBrains decompiler
// Type: UnityHeapCrawler.CrawlSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace UnityHeapCrawler
{
  public class CrawlSettings
  {
    [NotNull]
    private static readonly System.Type[] hierarchyTypes = new System.Type[6]
    {
      typeof (GameObject),
      typeof (Component),
      typeof (Material),
      typeof (Texture),
      typeof (Sprite),
      typeof (Mesh)
    };
    public bool Enabled = true;
    internal readonly CrawlOrder Order;
    [NotNull]
    internal readonly Action RootsCollector;
    internal readonly string Caption;
    [NotNull]
    public string Filename;
    public bool PrintChildren = true;
    public bool PrintOnlyGameObjects = false;
    public bool IncludeAllUnityTypes = false;
    [NotNull]
    public List<System.Type> IncludedUnityTypes = new List<System.Type>();
    public int MaxDepth = 0;
    public int MaxChildren = 10;
    public int MinItemSize = 1024;

    [NotNull]
    public static IComparer<CrawlSettings> PriorityComparer { get; } = (IComparer<CrawlSettings>) new CrawlSettings.PriorityRelationalComparer();

    public CrawlSettings([NotNull] string filename, [NotNull] string caption, [NotNull] Action rootsCollector, CrawlOrder order)
    {
      this.Filename = filename;
      this.Caption = caption;
      this.RootsCollector = rootsCollector;
      this.Order = order;
    }

    internal bool IsUnityTypeAllowed(System.Type type)
    {
      if (this.IncludeAllUnityTypes)
        return true;
      foreach (System.Type includedUnityType in this.IncludedUnityTypes)
      {
        if (includedUnityType.IsAssignableFrom(type))
          return true;
      }
      return false;
    }

    [NotNull]
    public static CrawlSettings CreateUserRoots([NotNull] Action objectsProvider)
    {
      return new CrawlSettings("user-roots", "User Roots", objectsProvider, CrawlOrder.UserRoots)
      {
        MaxChildren = 0
      };
    }

    [NotNull]
    public static CrawlSettings CreateStaticFields([NotNull] Action objectsProvider)
    {
      return new CrawlSettings("static-fields", "Static Roots", objectsProvider, CrawlOrder.StaticFields)
      {
        MaxDepth = 1
      };
    }

    [NotNull]
    public static CrawlSettings CreateHierarchy([NotNull] Action objectsProvider)
    {
      return new CrawlSettings("hierarchy", "Hierarchy", objectsProvider, CrawlOrder.Hierarchy)
      {
        PrintOnlyGameObjects = true,
        MaxChildren = 0,
        IncludedUnityTypes = new List<System.Type>((IEnumerable<System.Type>) CrawlSettings.hierarchyTypes)
      };
    }

    [NotNull]
    public static CrawlSettings CreateScriptableObjects([NotNull] Action objectsProvider)
    {
      return new CrawlSettings("scriptable_objects", "Scriptable Objects", objectsProvider, CrawlOrder.UnityObjects)
      {
        IncludeAllUnityTypes = true
      };
    }

    [NotNull]
    public static CrawlSettings CreatePrefabs([NotNull] Action objectsProvider)
    {
      return new CrawlSettings("prefabs", "Prefabs", objectsProvider, CrawlOrder.Prefabs)
      {
        PrintOnlyGameObjects = true,
        MaxChildren = 0,
        IncludedUnityTypes = new List<System.Type>((IEnumerable<System.Type>) CrawlSettings.hierarchyTypes)
      };
    }

    [NotNull]
    public static CrawlSettings CreateUnityObjects([NotNull] Action objectsProvider)
    {
      return new CrawlSettings("unity_objects", "Unity Objects", objectsProvider, CrawlOrder.UnityObjects);
    }

    public override string ToString()
    {
      return "Crawl Settings [" + this.Caption + ", " + this.Filename + "]";
    }

    private sealed class PriorityRelationalComparer : IComparer<CrawlSettings>
    {
      public int Compare(CrawlSettings x, CrawlSettings y)
      {
        if (x == y)
          return 0;
        if (y == null)
          return 1;
        return x == null ? -1 : x.Order.CompareTo((object) y.Order);
      }
    }
  }
}
