using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cofe.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

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
    private readonly List<Type> rootTypes = new List<Type>();
    [NotNull]
    private readonly List<Type> forbiddenTypes = new List<Type>();
    [NotNull]
    private readonly List<Type> staticTypes = new List<Type>();
    [NotNull]
    private readonly HashSet<object> unityObjects = new HashSet<object>(ReferenceEqualityComparer.Instance);
    [NotNull]
    private readonly HashSet<object> visitedObjects = new HashSet<object>(ReferenceEqualityComparer.Instance);
    [NotNull]
    private readonly Queue<CrawlItem> rootsQueue = new Queue<CrawlItem>();
    [NotNull]
    private readonly Queue<CrawlItem> localRootsQueue = new Queue<CrawlItem>();
    private int minTypeSize = 1024;
    private SizeFormat sizeFormat = SizeFormat.Short;
    private string output = "";

    public HeapSnapshotCollector()
    {
      StaticFieldsSettings = CrawlSettings.CreateStaticFields(CollectStaticFields);
      HierarchySettings = CrawlSettings.CreateHierarchy(CollectRootHierarchyGameObjects);
      ScriptableObjectsSettings = CrawlSettings.CreateScriptableObjects((Action) (() => CollectUnityObjects(typeof (ScriptableObject))));
      PrefabsSettings = CrawlSettings.CreatePrefabs(CollectPrefabs);
      UnityObjectsSettings = CrawlSettings.CreateUnityObjects((Action) (() => CollectUnityObjects(typeof (Object))));
      forbiddenTypes.Add(typeof (TypeData));
      forbiddenTypes.Add(typeof (TypeStats));
    }

    public void Start() {
      TypeData.Start();
      TypeStats.Init();
      output = "snapshot " + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss") + "/";
      Directory.CreateDirectory(output);
      using (StreamWriter streamWriter = new StreamWriter(output + "profiler.txt"))
      {
        streamWriter.WriteLine("Mono Size Min: " + sizeFormat.Format(Profiler.GetMonoUsedSizeLong()));
        streamWriter.WriteLine("Mono Size Max: " + sizeFormat.Format(Profiler.GetMonoHeapSizeLong()));
        streamWriter.WriteLine("Total Allocated: " + sizeFormat.Format(Profiler.GetTotalAllocatedMemoryLong()));
        streamWriter.WriteLine("Total Reserved: " + sizeFormat.Format(Profiler.GetTotalReservedMemoryLong()));
      }
      using (StreamWriter streamWriter = new StreamWriter(output + "log.txt"))
      {
        streamWriter.AutoFlush = true;
        GC.Collect();
        GC.Collect();
        crawlOrder.Add(StaticFieldsSettings);
        if (HierarchySettings != null)
          crawlOrder.Add(HierarchySettings);
        if (ScriptableObjectsSettings != null)
          crawlOrder.Add(ScriptableObjectsSettings);
        if (PrefabsSettings != null)
          crawlOrder.Add(PrefabsSettings);
        if (UnityObjectsSettings != null)
          crawlOrder.Add(UnityObjectsSettings);
        crawlOrder.RemoveAll(cs => !cs.Enabled);
        crawlOrder.Sort(CrawlSettings.PriorityComparer);
        unityObjects.Clear();
        if (UnityObjectsSettings != null)
        {
          foreach (object obj in Resources.FindObjectsOfTypeAll<Object>())
            unityObjects.Add(obj);
        }
        int size1 = 0;
        float num = 0.8f / crawlOrder.Count;
        for (int index = 0; index < crawlOrder.Count; ++index)
        {
          float startProgress = (float) (0.10000000149011612 + num * (double) index);
          float endProgress = (float) (0.10000000149011612 + num * (double) (index + 1));
          CrawlSettings crawlSettings = crawlOrder[index];
          crawlSettings.RootsCollector();
          int crawlIndex = index + 1;
          int size2 = CrawlRoots(crawlSettings, crawlIndex, startProgress, endProgress);
          streamWriter.WriteLine(crawlSettings.Caption + " size: " + sizeFormat.Format(size2));
          size1 += size2;
        }
        streamWriter.WriteLine("Total size: " + sizeFormat.Format(size1));
      }
      PrintTypeStats(TypeSizeMode.Self, "types-self.txt");
      PrintTypeStats(TypeSizeMode.Total, "types-total.txt");
      PrintTypeStats(TypeSizeMode.Native, "types-native.txt");
      Debug.Log("Heap snapshot created: " + output);
    }

    private void PrintTypeStats(TypeSizeMode mode, string filename)
    {
      using (StreamWriter streamWriter = new StreamWriter(output + filename))
      {
        List<TypeStats> list = TypeStats.Data.Values.OrderByDescending(ts => mode.GetSize(ts)).ToList();
        streamWriter.Write("Size".PadLeft(14));
        streamWriter.Write("Size".PadLeft(14));
        streamWriter.Write("Count".PadLeft(14));
        streamWriter.Write("    ");
        streamWriter.Write("Type");
        streamWriter.WriteLine();
        foreach (TypeStats typeStats in list)
        {
          if (typeStats.SelfSize >= minTypeSize)
          {
            long size = mode.GetSize(typeStats);
            streamWriter.Write(sizeFormat.Format(size).PadLeft(14));
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
      IEnumerable<Type> types = staticTypes.Concat(AppDomain.CurrentDomain.GetAssemblies().Where(IsValidAssembly).SelectMany(a => a.GetTypes()));
      HashSet<string> stringSet = new HashSet<string>();
      foreach (Type type in types)
        AddStaticFields(type, stringSet);
      if (stringSet.Count <= 0)
        return;
      List<string> list = stringSet.ToList();
      list.Sort();
      using (StreamWriter streamWriter = new StreamWriter(output + "generic-static-fields.txt"))
      {
        foreach (string str in list)
          streamWriter.WriteLine(str);
      }
    }

    private void AddStaticFields([NotNull] Type type, [NotNull] HashSet<string> genericStaticFields)
    {
      if (IsForbiddenType(type))
        return;
      for (Type type1 = type; type1 != null && type1 != typeof (object); type1 = type1.BaseType)
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
              root = field.GetValue(null);
            }
            catch (Exception ex)
            {
              Debug.LogError("Error get value from type : " + type1 + "\r\n" + ex);
              continue;
            }
            if (root != null)
            {
              string name = TypeUtility.GetTypeName(type1) + "." + field.Name;
              EnqueueRoot(root, name, false);
            }
          }
        }
      }
    }

    private void CollectRootHierarchyGameObjects()
    {
      foreach (object unityObject in unityObjects)
      {
        GameObject root = unityObject as GameObject;
        if (!(root == null) && root.scene.IsValid() && !(root.transform.parent != null))
          EnqueueRoot(root, root.name, false);
      }
    }

    private void CollectPrefabs()
    {
      foreach (object unityObject in unityObjects)
      {
        GameObject root = unityObject as GameObject;
        if (!(root == null) && !root.scene.IsValid() && !(root.transform.parent != null))
          EnqueueRoot(root, root.name, false);
      }
    }

    private void CollectUnityObjects(Type type)
    {
      foreach (object unityObject in unityObjects)
      {
        if (type.IsInstanceOfType(unityObject))
        {
          Object root = (Object) unityObject;
          EnqueueRoot(root, root.name, false);
        }
      }
    }

    private int CrawlRoots(
      [NotNull] CrawlSettings crawlSettings,
      int crawlIndex,
      float startProgress,
      float endProgress)
    {
      if (rootsQueue.Count <= 0)
        return 0;
      int num1 = 0;
      int num2 = 0;
      List<CrawlItem> crawlItemList = new List<CrawlItem>();
      while (rootsQueue.Count > 0)
      {
        localRootsQueue.Enqueue(rootsQueue.Dequeue());
        while (localRootsQueue.Count > 0)
        {
          CrawlItem root = localRootsQueue.Dequeue();
          CrawlRoot(root, crawlSettings);
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
        using (StreamWriter w = new StreamWriter(string.Format("{0}{1}-{2}.txt", output, crawlIndex, crawlSettings.Filename)))
        {
          foreach (CrawlItem crawlItem in crawlItemList)
            crawlItem.Print(w, sizeFormat);
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
        Type type = parent.Object.GetType();
        TypeData typeData = TypeData.Get(type);
        if (type.IsArray)
          QueueArrayElements(parent, queue, parent.Object, crawlSettings);
        if (type == typeof (GameObject))
          QueueHierarchy(parent, queue, parent.Object, crawlSettings);
        if (typeData.DynamicSizedFields != null)
        {
          foreach (FieldInfo dynamicSizedField in typeData.DynamicSizedFields)
          {
            object v = dynamicSizedField.GetValue(parent.Object);
            QueueValue(parent, queue, v, dynamicSizedField.Name, crawlSettings);
          }
        }
      }
      root.UpdateSize();
    }

    private void EnqueueRoot([NotNull] object root, [NotNull] string name, bool local)
    {
      if (IsForbidden(root) || visitedObjects.Contains(root))
        return;
      visitedObjects.Add(root);
      CrawlItem crawlItem = new CrawlItem(null, root, name);
      if (local)
        localRootsQueue.Enqueue(crawlItem);
      else
        rootsQueue.Enqueue(crawlItem);
    }

    private void QueueValue(
      [NotNull] CrawlItem parent,
      [NotNull] Queue<CrawlItem> queue,
      [CanBeNull] object v,
      [NotNull] string name,
      [NotNull] CrawlSettings crawlSettings)
    {
      if (v == null || IsForbidden(v))
        return;
      TypeStats.RegisterInstance(parent, name, v);
      if (visitedObjects.Contains(v) || unityObjects.Contains(v) && !crawlSettings.IsUnityTypeAllowed(v.GetType()))
        return;
      if (IsRoot(v))
      {
        string name1 = TypeUtility.GetTypeName(parent.Object.GetType()) + "." + name;
        EnqueueRoot(v, name1, true);
      }
      else
      {
        visitedObjects.Add(v);
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
      if (array == null || !array.GetType().IsArray || array.GetType().GetElementType() == null)
        return;
      int num = 0;
      foreach (object v in (Array) array)
      {
        QueueValue(parent, queue, v, string.Format("[{0}]", num), crawlSettings);
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
      if (gameObject1 == null)
        return;
      foreach (Component component in gameObject1.GetComponents<Component>())
      {
        if (component != null)
          QueueValue(parent, queue, component, TypeUtility.GetTypeName(component.GetType()), crawlSettings);
      }
      Transform transform = gameObject1.transform;
      for (int index = 0; index < transform.childCount; ++index)
      {
        GameObject gameObject2 = transform.GetChild(index).gameObject;
        QueueValue(parent, queue, gameObject2, gameObject2.name, crawlSettings);
      }
    }

    private bool IsRoot([NotNull] object o)
    {
      return rootTypes.Any(t => t.IsInstanceOfType(o));
    }

    private bool IsForbidden([NotNull] object o)
    {
      return forbiddenTypes.Any(t => t.IsInstanceOfType(o));
    }

    private bool IsForbiddenType([NotNull] Type type)
    {
      return forbiddenTypes.Any(t => t.IsAssignableFrom(type));
    }

    private static bool IsValidAssembly(Assembly assembly)
    {
      return !assembly.FullName.StartsWith("UnityEditor.") && !assembly.FullName.StartsWith("UnityScript.") && !assembly.FullName.StartsWith("Boo.") && !assembly.FullName.StartsWith("ExCSS.") && !assembly.FullName.StartsWith("I18N") && !assembly.FullName.StartsWith("Microsoft.") && !assembly.FullName.StartsWith("System") && !assembly.FullName.StartsWith("SyntaxTree.") && !assembly.FullName.StartsWith("mscorlib") && !assembly.FullName.StartsWith("Windows.");
    }
  }
}
