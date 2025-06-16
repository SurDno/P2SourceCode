using Cofe.Utility;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityHeapCrawler
{
  public class HeapSnapshotCollector
  {
    [NotNull]
    public readonly CrawlSettings StaticFieldsSettings;
    [NotNull]
    public readonly CrawlSettings HierarchySettings;
    [NotNull]
    public readonly CrawlSettings ScriptableObjectsSettings;
    [NotNull]
    public readonly CrawlSettings PrefabsSettings;
    [NotNull]
    public readonly CrawlSettings UnityObjectsSettings;
    public bool DifferentialMode = true;
    [NotNull]
    private readonly List<CrawlSettings> crawlOrder = new List<CrawlSettings>();
    [NotNull]
    private readonly List<System.Type> rootTypes = new List<System.Type>();
    [NotNull]
    private readonly List<System.Type> forbiddenTypes = new List<System.Type>();
    [NotNull]
    private readonly List<System.Type> staticTypes = new List<System.Type>();
    [NotNull]
    private readonly HashSet<object> unityObjects = new HashSet<object>((IEqualityComparer<object>) ReferenceEqualityComparer.Instance);
    [NotNull]
    private readonly HashSet<object> visitedObjects = new HashSet<object>((IEqualityComparer<object>) ReferenceEqualityComparer.Instance);
    [NotNull]
    private readonly Queue<CrawlItem> rootsQueue = new Queue<CrawlItem>();
    [NotNull]
    private readonly Queue<CrawlItem> localRootsQueue = new Queue<CrawlItem>();
    private int minTypeSize = 1024;
    private SizeFormat sizeFormat = SizeFormat.Short;
    private string output = "";

    public HeapSnapshotCollector()
    {
      this.StaticFieldsSettings = CrawlSettings.CreateStaticFields(new Action(this.CollectStaticFields));
      this.HierarchySettings = CrawlSettings.CreateHierarchy(new Action(this.CollectRootHierarchyGameObjects));
      this.ScriptableObjectsSettings = CrawlSettings.CreateScriptableObjects((Action) (() => this.CollectUnityObjects(typeof (ScriptableObject))));
      this.PrefabsSettings = CrawlSettings.CreatePrefabs(new Action(this.CollectPrefabs));
      this.UnityObjectsSettings = CrawlSettings.CreateUnityObjects((Action) (() => this.CollectUnityObjects(typeof (UnityEngine.Object))));
      this.forbiddenTypes.Add(typeof (TypeData));
      this.forbiddenTypes.Add(typeof (TypeStats));
    }

    public void Start()
    {
      try
      {
        TypeData.Start();
        TypeStats.Init();
        this.output = "snapshot " + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss") + "/";
        Directory.CreateDirectory(this.output);
        using (StreamWriter streamWriter = new StreamWriter(this.output + "profiler.txt"))
        {
          streamWriter.WriteLine("Mono Size Min: " + this.sizeFormat.Format(Profiler.GetMonoUsedSizeLong()));
          streamWriter.WriteLine("Mono Size Max: " + this.sizeFormat.Format(Profiler.GetMonoHeapSizeLong()));
          streamWriter.WriteLine("Total Allocated: " + this.sizeFormat.Format(Profiler.GetTotalAllocatedMemoryLong()));
          streamWriter.WriteLine("Total Reserved: " + this.sizeFormat.Format(Profiler.GetTotalReservedMemoryLong()));
        }
        using (StreamWriter streamWriter = new StreamWriter(this.output + "log.txt"))
        {
          streamWriter.AutoFlush = true;
          GC.Collect();
          GC.Collect();
          this.crawlOrder.Add(this.StaticFieldsSettings);
          if (this.HierarchySettings != null)
            this.crawlOrder.Add(this.HierarchySettings);
          if (this.ScriptableObjectsSettings != null)
            this.crawlOrder.Add(this.ScriptableObjectsSettings);
          if (this.PrefabsSettings != null)
            this.crawlOrder.Add(this.PrefabsSettings);
          if (this.UnityObjectsSettings != null)
            this.crawlOrder.Add(this.UnityObjectsSettings);
          this.crawlOrder.RemoveAll((Predicate<CrawlSettings>) (cs => !cs.Enabled));
          this.crawlOrder.Sort(CrawlSettings.PriorityComparer);
          this.unityObjects.Clear();
          if (this.UnityObjectsSettings != null)
          {
            foreach (object obj in Resources.FindObjectsOfTypeAll<UnityEngine.Object>())
              this.unityObjects.Add(obj);
          }
          int size1 = 0;
          float num = 0.8f / (float) this.crawlOrder.Count;
          for (int index = 0; index < this.crawlOrder.Count; ++index)
          {
            float startProgress = (float) (0.10000000149011612 + (double) num * (double) index);
            float endProgress = (float) (0.10000000149011612 + (double) num * (double) (index + 1));
            CrawlSettings crawlSettings = this.crawlOrder[index];
            crawlSettings.RootsCollector();
            int crawlIndex = index + 1;
            int size2 = this.CrawlRoots(crawlSettings, crawlIndex, startProgress, endProgress);
            streamWriter.WriteLine(crawlSettings.Caption + " size: " + this.sizeFormat.Format((long) size2));
            size1 += size2;
          }
          streamWriter.WriteLine("Total size: " + this.sizeFormat.Format((long) size1));
        }
        this.PrintTypeStats(TypeSizeMode.Self, "types-self.txt");
        this.PrintTypeStats(TypeSizeMode.Total, "types-total.txt");
        this.PrintTypeStats(TypeSizeMode.Native, "types-native.txt");
        Debug.Log((object) ("Heap snapshot created: " + this.output));
      }
      finally
      {
      }
    }

    private void PrintTypeStats(TypeSizeMode mode, string filename)
    {
      using (StreamWriter streamWriter = new StreamWriter(this.output + filename))
      {
        List<TypeStats> list = TypeStats.Data.Values.OrderByDescending<TypeStats, long>((Func<TypeStats, long>) (ts => mode.GetSize(ts))).ToList<TypeStats>();
        streamWriter.Write("Size".PadLeft(14));
        streamWriter.Write("Size".PadLeft(14));
        streamWriter.Write("Count".PadLeft(14));
        streamWriter.Write("    ");
        streamWriter.Write("Type");
        streamWriter.WriteLine();
        foreach (TypeStats typeStats in list)
        {
          if (typeStats.SelfSize >= (long) this.minTypeSize)
          {
            long size = mode.GetSize(typeStats);
            streamWriter.Write(this.sizeFormat.Format(size).PadLeft(14));
            streamWriter.Write(size.ToString().PadLeft(14));
            streamWriter.Write(typeStats.Count.ToString().PadLeft(14));
            streamWriter.Write("    ");
            streamWriter.Write(TypeUtility.GetTypeName(typeStats.Type));
            streamWriter.WriteLine();
          }
        }
      }
    }

    private void CollectStaticFields()
    {
      IEnumerable<System.Type> types = this.staticTypes.Concat<System.Type>(((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>(new Func<Assembly, bool>(HeapSnapshotCollector.IsValidAssembly)).SelectMany<Assembly, System.Type>((Func<Assembly, IEnumerable<System.Type>>) (a => (IEnumerable<System.Type>) a.GetTypes())));
      HashSet<string> stringSet = new HashSet<string>();
      foreach (System.Type type in types)
        this.AddStaticFields(type, stringSet);
      if (stringSet.Count <= 0)
        return;
      List<string> list = stringSet.ToList<string>();
      list.Sort();
      using (StreamWriter streamWriter = new StreamWriter(this.output + "generic-static-fields.txt"))
      {
        foreach (string str in list)
          streamWriter.WriteLine(str);
      }
    }

    private void AddStaticFields([NotNull] System.Type type, [NotNull] HashSet<string> genericStaticFields)
    {
      if (this.IsForbiddenType(type))
        return;
      for (System.Type type1 = type; type1 != (System.Type) null && type1 != typeof (object); type1 = type1.BaseType)
      {
        foreach (FieldInfo field in type1.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
          if (type1.IsGenericTypeDefinition)
          {
            genericStaticFields.Add(type1.FullName + "." + field.Name);
          }
          else
          {
            object root;
            try
            {
              root = field.GetValue((object) null);
            }
            catch (Exception ex)
            {
              Debug.LogError((object) ("Error get value from type : " + (object) type1 + "\r\n" + ex.ToString()));
              continue;
            }
            if (root != null)
            {
              string name = TypeUtility.GetTypeName(type1) + "." + field.Name;
              this.EnqueueRoot(root, name, false);
            }
          }
        }
      }
    }

    private void CollectRootHierarchyGameObjects()
    {
      foreach (object unityObject in this.unityObjects)
      {
        GameObject root = unityObject as GameObject;
        if (!((UnityEngine.Object) root == (UnityEngine.Object) null) && root.scene.IsValid() && !((UnityEngine.Object) root.transform.parent != (UnityEngine.Object) null))
          this.EnqueueRoot((object) root, root.name, false);
      }
    }

    private void CollectPrefabs()
    {
      foreach (object unityObject in this.unityObjects)
      {
        GameObject root = unityObject as GameObject;
        if (!((UnityEngine.Object) root == (UnityEngine.Object) null) && !root.scene.IsValid() && !((UnityEngine.Object) root.transform.parent != (UnityEngine.Object) null))
          this.EnqueueRoot((object) root, root.name, false);
      }
    }

    private void CollectUnityObjects(System.Type type)
    {
      foreach (object unityObject in this.unityObjects)
      {
        if (type.IsInstanceOfType(unityObject))
        {
          UnityEngine.Object root = (UnityEngine.Object) unityObject;
          this.EnqueueRoot((object) root, root.name, false);
        }
      }
    }

    private int CrawlRoots(
      [NotNull] CrawlSettings crawlSettings,
      int crawlIndex,
      float startProgress,
      float endProgress)
    {
      if (this.rootsQueue.Count <= 0)
        return 0;
      int num1 = 0;
      int num2 = 0;
      List<CrawlItem> crawlItemList = new List<CrawlItem>();
      while (this.rootsQueue.Count > 0)
      {
        this.localRootsQueue.Enqueue(this.rootsQueue.Dequeue());
        while (this.localRootsQueue.Count > 0)
        {
          CrawlItem root = this.localRootsQueue.Dequeue();
          this.CrawlRoot(root, crawlSettings);
          num2 += root.TotalSize;
          root.Cleanup(crawlSettings);
          ++num1;
          if (root.SubtreeUpdated && root.TotalSize >= crawlSettings.MinItemSize)
            crawlItemList.Add(root);
        }
      }
      crawlItemList.Sort();
      if (crawlItemList.Count > 0)
      {
        using (StreamWriter w = new StreamWriter(string.Format("{0}{1}-{2}.txt", (object) this.output, (object) crawlIndex, (object) crawlSettings.Filename)))
        {
          foreach (CrawlItem crawlItem in crawlItemList)
            crawlItem.Print(w, this.sizeFormat);
        }
      }
      return num2;
    }

    private void CrawlRoot([NotNull] CrawlItem root, [NotNull] CrawlSettings crawlSettings)
    {
      Queue<CrawlItem> queue = new Queue<CrawlItem>();
      queue.Enqueue(root);
      while (queue.Count > 0)
      {
        CrawlItem parent = queue.Dequeue();
        System.Type type = parent.Object.GetType();
        TypeData typeData = TypeData.Get(type);
        if (type.IsArray)
          this.QueueArrayElements(parent, queue, parent.Object, crawlSettings);
        if (type == typeof (GameObject))
          this.QueueHierarchy(parent, queue, parent.Object, crawlSettings);
        if (typeData.DynamicSizedFields != null)
        {
          foreach (FieldInfo dynamicSizedField in typeData.DynamicSizedFields)
          {
            object v = dynamicSizedField.GetValue(parent.Object);
            this.QueueValue(parent, queue, v, dynamicSizedField.Name, crawlSettings);
          }
        }
      }
      root.UpdateSize();
    }

    private void EnqueueRoot([NotNull] object root, [NotNull] string name, bool local)
    {
      if (this.IsForbidden(root) || this.visitedObjects.Contains(root))
        return;
      this.visitedObjects.Add(root);
      CrawlItem crawlItem = new CrawlItem((CrawlItem) null, root, name);
      if (local)
        this.localRootsQueue.Enqueue(crawlItem);
      else
        this.rootsQueue.Enqueue(crawlItem);
    }

    private void QueueValue(
      [NotNull] CrawlItem parent,
      [NotNull] Queue<CrawlItem> queue,
      [CanBeNull] object v,
      [NotNull] string name,
      [NotNull] CrawlSettings crawlSettings)
    {
      if (v == null || this.IsForbidden(v))
        return;
      TypeStats.RegisterInstance(parent, name, v);
      if (this.visitedObjects.Contains(v) || this.unityObjects.Contains(v) && !crawlSettings.IsUnityTypeAllowed(v.GetType()))
        return;
      if (this.IsRoot(v))
      {
        string name1 = TypeUtility.GetTypeName(parent.Object.GetType()) + "." + name;
        this.EnqueueRoot(v, name1, true);
      }
      else
      {
        this.visitedObjects.Add(v);
        CrawlItem child = new CrawlItem(parent, v, name);
        queue.Enqueue(child);
        parent.AddChild(child);
      }
    }

    private void QueueArrayElements(
      [NotNull] CrawlItem parent,
      [NotNull] Queue<CrawlItem> queue,
      [CanBeNull] object array,
      [NotNull] CrawlSettings crawlSettings)
    {
      if (array == null || !array.GetType().IsArray || array.GetType().GetElementType() == (System.Type) null)
        return;
      int num = 0;
      foreach (object v in (Array) array)
      {
        this.QueueValue(parent, queue, v, string.Format("[{0}]", (object) num), crawlSettings);
        ++num;
      }
    }

    private void QueueHierarchy(
      [NotNull] CrawlItem parent,
      [NotNull] Queue<CrawlItem> queue,
      [CanBeNull] object v,
      [NotNull] CrawlSettings crawlSettings)
    {
      GameObject gameObject1 = v as GameObject;
      if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
        return;
      foreach (Component component in gameObject1.GetComponents<Component>())
      {
        if (component != null)
          this.QueueValue(parent, queue, (object) component, TypeUtility.GetTypeName(((object) component).GetType()), crawlSettings);
      }
      Transform transform = gameObject1.transform;
      for (int index = 0; index < transform.childCount; ++index)
      {
        GameObject gameObject2 = transform.GetChild(index).gameObject;
        this.QueueValue(parent, queue, (object) gameObject2, gameObject2.name, crawlSettings);
      }
    }

    private bool IsRoot([NotNull] object o)
    {
      return this.rootTypes.Any<System.Type>((Func<System.Type, bool>) (t => t.IsInstanceOfType(o)));
    }

    private bool IsForbidden([NotNull] object o)
    {
      return this.forbiddenTypes.Any<System.Type>((Func<System.Type, bool>) (t => t.IsInstanceOfType(o)));
    }

    private bool IsForbiddenType([NotNull] System.Type type)
    {
      return this.forbiddenTypes.Any<System.Type>((Func<System.Type, bool>) (t => t.IsAssignableFrom(type)));
    }

    private static bool IsValidAssembly(Assembly assembly)
    {
      return !assembly.FullName.StartsWith("UnityEditor.") && !assembly.FullName.StartsWith("UnityScript.") && !assembly.FullName.StartsWith("Boo.") && !assembly.FullName.StartsWith("ExCSS.") && !assembly.FullName.StartsWith("I18N") && !assembly.FullName.StartsWith("Microsoft.") && !assembly.FullName.StartsWith("System") && !assembly.FullName.StartsWith("SyntaxTree.") && !assembly.FullName.StartsWith("mscorlib") && !assembly.FullName.StartsWith("Windows.");
    }
  }
}
