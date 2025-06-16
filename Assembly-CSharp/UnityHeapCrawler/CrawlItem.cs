using Cofe.Utility;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityHeapCrawler
{
  public class CrawlItem : IComparable<CrawlItem>
  {
    private static int depth;
    [CanBeNull]
    public readonly CrawlItem Parent;
    [NotNull]
    public readonly object Object;
    [NotNull]
    public string Name;
    public int SelfSize;
    public int TotalSize;
    [CanBeNull]
    public List<CrawlItem> Children;
    private bool childrenFiltered;

    public bool SubtreeUpdated { get; private set; }

    public CrawlItem([CanBeNull] CrawlItem parent, [NotNull] object o, [NotNull] string name)
    {
      this.Parent = parent;
      this.Object = o;
      this.Name = name;
    }

    public void AddChild([NotNull] CrawlItem child)
    {
      if (this.Children == null)
        this.Children = new List<CrawlItem>();
      this.Children.Add(child);
    }

    public void UpdateSize()
    {
      try
      {
        this.SelfSize = this.CalculateSelfSize();
        this.TotalSize = this.SelfSize;
        if (this.Children == null)
          return;
        foreach (CrawlItem child in this.Children)
        {
          child.UpdateSize();
          this.TotalSize += child.TotalSize;
        }
        this.Children.Sort();
      }
      finally
      {
        TypeStats.RegisterItem(this);
      }
    }

    public void Cleanup(CrawlSettings crawlSettings)
    {
      this.CleanupUnchanged();
      this.CleanupInternal(crawlSettings);
    }

    private void CleanupUnchanged()
    {
      if (this.Children != null)
      {
        foreach (CrawlItem child in this.Children)
          child.CleanupUnchanged();
        this.Children.RemoveAll((Predicate<CrawlItem>) (c => !c.SubtreeUpdated));
        this.SubtreeUpdated = this.Children.Count > 0;
      }
      this.SubtreeUpdated = ((this.SubtreeUpdated ? 1 : 0) | 1) != 0;
    }

    public void CleanupInternal(CrawlSettings crawlSettings)
    {
      if (!crawlSettings.PrintChildren)
        this.Children = (List<CrawlItem>) null;
      if (crawlSettings.MaxDepth > 0 && CrawlItem.depth >= crawlSettings.MaxDepth)
        this.Children = (List<CrawlItem>) null;
      UnityEngine.Object @object = this.Object as UnityEngine.Object;
      if ((object) @object != null && !(bool) @object)
      {
        this.Name = !string.IsNullOrWhiteSpace(this.Name) ? this.Name + " (destroyed Unity Object)" : "(destroyed Unity Object)";
        this.Children = (List<CrawlItem>) null;
      }
      if (this.Children == null)
        return;
      if (crawlSettings.PrintOnlyGameObjects)
        this.Children.RemoveAll((Predicate<CrawlItem>) (c => !(c.Object is GameObject)));
      int count = this.Children.Count;
      if (crawlSettings.MinItemSize > 0)
        this.Children.RemoveAll((Predicate<CrawlItem>) (c => c.TotalSize < crawlSettings.MinItemSize));
      if (crawlSettings.MaxChildren > 0 && this.Children.Count > crawlSettings.MaxChildren)
        this.Children.RemoveRange(crawlSettings.MaxChildren, this.Children.Count - crawlSettings.MaxChildren);
      if (this.Children.Count < count)
        this.childrenFiltered = true;
      ++CrawlItem.depth;
      foreach (CrawlItem child in this.Children)
        child.CleanupInternal(crawlSettings);
      --CrawlItem.depth;
    }

    public void Print([NotNull] StreamWriter w, SizeFormat sizeFormat)
    {
      for (int index = 0; index < CrawlItem.depth; ++index)
        w.Write("  ");
      if (!string.IsNullOrWhiteSpace(this.Name))
      {
        w.Write(this.Name);
        w.Write(" ");
      }
      w.Write("[");
      UnityEngine.Object @object = this.Object as UnityEngine.Object;
      if (@object != (UnityEngine.Object) null)
      {
        w.Write(@object.name);
        w.Write(": ");
        w.Write(TypeUtility.GetTypeName(this.Object.GetType()));
        w.Write(" (");
        w.Write(@object.GetInstanceID());
        w.Write(")");
      }
      else
        w.Write(TypeUtility.GetTypeName(this.Object.GetType()));
      w.Write("]");
      w.Write(" ");
      w.Write(sizeFormat.Format((long) this.TotalSize));
      w.WriteLine();
      if (this.Children == null)
        return;
      ++CrawlItem.depth;
      foreach (CrawlItem child in this.Children)
        child.Print(w, sizeFormat);
      if (this.childrenFiltered && this.Children.Count > 0)
      {
        for (int index = 0; index < CrawlItem.depth; ++index)
          w.Write("  ");
        w.WriteLine("...");
      }
      --CrawlItem.depth;
    }

    public string GetRootPath()
    {
      List<CrawlItem> source = new List<CrawlItem>();
      CrawlItem crawlItem = this;
      do
      {
        source.Add(crawlItem);
        crawlItem = crawlItem.Parent;
      }
      while (crawlItem != null);
      source.Reverse();
      return string.Join(".", source.Select<CrawlItem, string>((Func<CrawlItem, string>) (i => i.Name)).ToArray<string>());
    }

    private int CalculateSelfSize()
    {
      if (this.Object is string str)
      {
        int selfSize = 3 * IntPtr.Size + 2 + str.Length * 2;
        int num = selfSize % IntPtr.Size;
        if (num != 0)
          selfSize += IntPtr.Size - num;
        return selfSize;
      }
      if (!this.Object.GetType().IsArray)
        return TypeData.Get(this.Object.GetType()).Size;
      System.Type elementType = this.Object.GetType().GetElementType();
      return elementType != (System.Type) null && (elementType.IsValueType || elementType.IsPrimitive || elementType.IsEnum) ? 0 : IntPtr.Size * CrawlItem.GetTotalArrayLength((Array) this.Object);
    }

    private static int GetTotalArrayLength(Array val)
    {
      int length = val.GetLength(0);
      for (int dimension = 1; dimension < val.Rank; ++dimension)
        length *= val.GetLength(dimension);
      return length;
    }

    public int CompareTo(CrawlItem other)
    {
      if (this == other)
        return 0;
      return other == null ? 1 : other.TotalSize.CompareTo(this.TotalSize);
    }

    public override string ToString() => this.Object.ToString();
  }
}
