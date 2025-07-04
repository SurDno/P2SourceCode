﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cofe.Utility;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityHeapCrawler
{
  public class CrawlItem([CanBeNull] CrawlItem parent, [NotNull] object o, [NotNull] string name)
    : IComparable<CrawlItem> {
    private static int depth;
    [CanBeNull]
    public readonly CrawlItem Parent = parent;
    [NotNull]
    public readonly object Object = o;
    [NotNull]
    public string Name = name;
    public int SelfSize;
    public int TotalSize;
    [CanBeNull]
    public List<CrawlItem> Children;
    private bool childrenFiltered;

    public bool SubtreeUpdated { get; private set; }

    public void AddChild([NotNull] CrawlItem child)
    {
      if (Children == null)
        Children = [];
      Children.Add(child);
    }

    public void UpdateSize()
    {
      try
      {
        SelfSize = CalculateSelfSize();
        TotalSize = SelfSize;
        if (Children == null)
          return;
        foreach (CrawlItem child in Children)
        {
          child.UpdateSize();
          TotalSize += child.TotalSize;
        }
        Children.Sort();
      }
      finally
      {
        TypeStats.RegisterItem(this);
      }
    }

    public void Cleanup(CrawlSettings crawlSettings)
    {
      CleanupUnchanged();
      CleanupInternal(crawlSettings);
    }

    private void CleanupUnchanged()
    {
      if (Children != null)
      {
        foreach (CrawlItem child in Children)
          child.CleanupUnchanged();
        Children.RemoveAll(c => !c.SubtreeUpdated);
        SubtreeUpdated = Children.Count > 0;
      }
      SubtreeUpdated = ((SubtreeUpdated ? 1 : 0) | 1) != 0;
    }

    public void CleanupInternal(CrawlSettings crawlSettings)
    {
      if (!crawlSettings.PrintChildren)
        Children = null;
      if (crawlSettings.MaxDepth > 0 && depth >= crawlSettings.MaxDepth)
        Children = null;
      Object @object = Object as Object;
      if ((object) @object != null && !(bool) @object)
      {
        Name = !string.IsNullOrWhiteSpace(Name) ? Name + " (destroyed Unity Object)" : "(destroyed Unity Object)";
        Children = null;
      }
      if (Children == null)
        return;
      if (crawlSettings.PrintOnlyGameObjects)
        Children.RemoveAll(c => !(c.Object is GameObject));
      int count = Children.Count;
      if (crawlSettings.MinItemSize > 0)
        Children.RemoveAll(c => c.TotalSize < crawlSettings.MinItemSize);
      if (crawlSettings.MaxChildren > 0 && Children.Count > crawlSettings.MaxChildren)
        Children.RemoveRange(crawlSettings.MaxChildren, Children.Count - crawlSettings.MaxChildren);
      if (Children.Count < count)
        childrenFiltered = true;
      ++depth;
      foreach (CrawlItem child in Children)
        child.CleanupInternal(crawlSettings);
      --depth;
    }

    public void Print([NotNull] StreamWriter w, SizeFormat sizeFormat)
    {
      for (int index = 0; index < depth; ++index)
        w.Write("  ");
      if (!string.IsNullOrWhiteSpace(Name))
      {
        w.Write(Name);
        w.Write(" ");
      }
      w.Write("[");
      Object @object = Object as Object;
      if (@object != null)
      {
        w.Write(@object.name);
        w.Write(": ");
        w.Write(TypeUtility.GetTypeName(Object.GetType()));
        w.Write(" (");
        w.Write(@object.GetInstanceID());
        w.Write(")");
      }
      else
        w.Write(TypeUtility.GetTypeName(Object.GetType()));
      w.Write("]");
      w.Write(" ");
      w.Write(sizeFormat.Format(TotalSize));
      w.WriteLine();
      if (Children == null)
        return;
      ++depth;
      foreach (CrawlItem child in Children)
        child.Print(w, sizeFormat);
      if (childrenFiltered && Children.Count > 0)
      {
        for (int index = 0; index < depth; ++index)
          w.Write("  ");
        w.WriteLine("...");
      }
      --depth;
    }

    public string GetRootPath()
    {
      List<CrawlItem> source = [];
      CrawlItem crawlItem = this;
      do
      {
        source.Add(crawlItem);
        crawlItem = crawlItem.Parent;
      }
      while (crawlItem != null);
      source.Reverse();
      return string.Join(".", source.Select(i => i.Name).ToArray());
    }

    private int CalculateSelfSize()
    {
      if (Object is string str)
      {
        int selfSize = 3 * IntPtr.Size + 2 + str.Length * 2;
        int num = selfSize % IntPtr.Size;
        if (num != 0)
          selfSize += IntPtr.Size - num;
        return selfSize;
      }
      if (!Object.GetType().IsArray)
        return TypeData.Get(Object.GetType()).Size;
      Type elementType = Object.GetType().GetElementType();
      return elementType != null && (elementType.IsValueType || elementType.IsPrimitive || elementType.IsEnum) ? 0 : IntPtr.Size * GetTotalArrayLength((Array) Object);
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
      return other == null ? 1 : other.TotalSize.CompareTo(TotalSize);
    }

    public override string ToString() => Object.ToString();
  }
}
