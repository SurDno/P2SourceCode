using ParadoxNotion.Serialization.FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public class fsSerializer
  {
    private static HashSet<string> _reservedKeywords = new HashSet<string>()
    {
      "$ref",
      "$id",
      "$type",
      "$version",
      "$content"
    };
    private const string Key_ObjectReference = "$ref";
    private const string Key_ObjectDefinition = "$id";
    private const string Key_InstanceType = "$type";
    private const string Key_Version = "$version";
    private const string Key_Content = "$content";
    private Dictionary<System.Type, fsBaseConverter> _cachedConverterTypeInstances;
    private Dictionary<System.Type, fsBaseConverter> _cachedConverters;
    private Dictionary<System.Type, List<fsObjectProcessor>> _cachedProcessors;
    private readonly List<fsConverter> _availableConverters;
    private readonly Dictionary<System.Type, fsDirectConverter> _availableDirectConverters;
    private readonly List<fsObjectProcessor> _processors;
    private readonly fsCyclicReferenceManager _references;
    private readonly fsSerializer.fsLazyCycleDefinitionWriter _lazyReferenceWriter;
    public fsContext Context;
    public fsConfig Config;

    public static bool IsReservedKeyword(string key)
    {
      return fsSerializer._reservedKeywords.Contains(key);
    }

    private static bool IsObjectReference(fsData data)
    {
      return data.IsDictionary && data.AsDictionary.ContainsKey("$ref");
    }

    private static bool IsObjectDefinition(fsData data)
    {
      return data.IsDictionary && data.AsDictionary.ContainsKey("$id");
    }

    private static bool IsVersioned(fsData data)
    {
      return data.IsDictionary && data.AsDictionary.ContainsKey("$version");
    }

    private static bool IsTypeSpecified(fsData data)
    {
      return data.IsDictionary && data.AsDictionary.ContainsKey("$type");
    }

    private static bool IsWrappedData(fsData data)
    {
      return data.IsDictionary && data.AsDictionary.ContainsKey("$content");
    }

    public static void StripDeserializationMetadata(ref fsData data)
    {
      if (data.IsDictionary && data.AsDictionary.ContainsKey("$content"))
        data = data.AsDictionary["$content"];
      if (!data.IsDictionary)
        return;
      Dictionary<string, fsData> asDictionary = data.AsDictionary;
      asDictionary.Remove("$ref");
      asDictionary.Remove("$id");
      asDictionary.Remove("$type");
      asDictionary.Remove("$version");
    }

    private static void Invoke_OnBeforeSerialize(
      List<fsObjectProcessor> processors,
      System.Type storageType,
      object instance)
    {
      for (int index = 0; index < processors.Count; ++index)
        processors[index].OnBeforeSerialize(storageType, instance);
      if (!(instance is ISerializationCallbackReceiver) || instance is UnityEngine.Object)
        return;
      ((ISerializationCallbackReceiver) instance).OnBeforeSerialize();
    }

    private static void Invoke_OnAfterSerialize(
      List<fsObjectProcessor> processors,
      System.Type storageType,
      object instance,
      ref fsData data)
    {
      for (int index = processors.Count - 1; index >= 0; --index)
        processors[index].OnAfterSerialize(storageType, instance, ref data);
    }

    private static void Invoke_OnBeforeDeserialize(
      List<fsObjectProcessor> processors,
      System.Type storageType,
      ref fsData data)
    {
      for (int index = 0; index < processors.Count; ++index)
        processors[index].OnBeforeDeserialize(storageType, ref data);
    }

    private static void Invoke_OnBeforeDeserializeAfterInstanceCreation(
      List<fsObjectProcessor> processors,
      System.Type storageType,
      object instance,
      ref fsData data)
    {
      for (int index = 0; index < processors.Count; ++index)
        processors[index].OnBeforeDeserializeAfterInstanceCreation(storageType, instance, ref data);
    }

    private static void Invoke_OnAfterDeserialize(
      List<fsObjectProcessor> processors,
      System.Type storageType,
      object instance)
    {
      for (int index = processors.Count - 1; index >= 0; --index)
        processors[index].OnAfterDeserialize(storageType, instance);
      if (!(instance is ISerializationCallbackReceiver) || instance is UnityEngine.Object)
        return;
      ((ISerializationCallbackReceiver) instance).OnAfterDeserialize();
    }

    private static void EnsureDictionary(fsData data)
    {
      if (data.IsDictionary)
        return;
      fsData fsData = data.Clone();
      data.BecomeDictionary();
      data.AsDictionary["$content"] = fsData;
    }

    public fsSerializer()
    {
      this._cachedConverterTypeInstances = new Dictionary<System.Type, fsBaseConverter>();
      this._cachedConverters = new Dictionary<System.Type, fsBaseConverter>();
      this._cachedProcessors = new Dictionary<System.Type, List<fsObjectProcessor>>();
      this._references = new fsCyclicReferenceManager();
      this._lazyReferenceWriter = new fsSerializer.fsLazyCycleDefinitionWriter();
      List<fsConverter> fsConverterList = new List<fsConverter>();
      fsNullableConverter nullableConverter = new fsNullableConverter();
      nullableConverter.Serializer = this;
      fsConverterList.Add((fsConverter) nullableConverter);
      fsGuidConverter fsGuidConverter = new fsGuidConverter();
      fsGuidConverter.Serializer = this;
      fsConverterList.Add((fsConverter) fsGuidConverter);
      fsTypeConverter fsTypeConverter = new fsTypeConverter();
      fsTypeConverter.Serializer = this;
      fsConverterList.Add((fsConverter) fsTypeConverter);
      fsDateConverter fsDateConverter = new fsDateConverter();
      fsDateConverter.Serializer = this;
      fsConverterList.Add((fsConverter) fsDateConverter);
      fsEnumConverter fsEnumConverter = new fsEnumConverter();
      fsEnumConverter.Serializer = this;
      fsConverterList.Add((fsConverter) fsEnumConverter);
      fsPrimitiveConverter primitiveConverter = new fsPrimitiveConverter();
      primitiveConverter.Serializer = this;
      fsConverterList.Add((fsConverter) primitiveConverter);
      fsArrayConverter fsArrayConverter = new fsArrayConverter();
      fsArrayConverter.Serializer = this;
      fsConverterList.Add((fsConverter) fsArrayConverter);
      fsDictionaryConverter dictionaryConverter = new fsDictionaryConverter();
      dictionaryConverter.Serializer = this;
      fsConverterList.Add((fsConverter) dictionaryConverter);
      fsIEnumerableConverter ienumerableConverter = new fsIEnumerableConverter();
      ienumerableConverter.Serializer = this;
      fsConverterList.Add((fsConverter) ienumerableConverter);
      fsKeyValuePairConverter valuePairConverter = new fsKeyValuePairConverter();
      valuePairConverter.Serializer = this;
      fsConverterList.Add((fsConverter) valuePairConverter);
      fsWeakReferenceConverter referenceConverter = new fsWeakReferenceConverter();
      referenceConverter.Serializer = this;
      fsConverterList.Add((fsConverter) referenceConverter);
      fsReflectedConverter reflectedConverter = new fsReflectedConverter();
      reflectedConverter.Serializer = this;
      fsConverterList.Add((fsConverter) reflectedConverter);
      this._availableConverters = fsConverterList;
      this._availableDirectConverters = new Dictionary<System.Type, fsDirectConverter>();
      this._processors = new List<fsObjectProcessor>();
      this.Context = new fsContext();
      this.Config = new fsConfig();
      foreach (System.Type converter in fsConverterRegistrar.Converters)
        this.AddConverter((fsBaseConverter) Activator.CreateInstance(converter));
    }

    public void AddProcessor(fsObjectProcessor processor)
    {
      this._processors.Add(processor);
      this._cachedProcessors = new Dictionary<System.Type, List<fsObjectProcessor>>();
    }

    public void RemoveProcessor<TProcessor>()
    {
      int index = 0;
      while (index < this._processors.Count)
      {
        if (this._processors[index] is TProcessor)
          this._processors.RemoveAt(index);
        else
          ++index;
      }
      this._cachedProcessors = new Dictionary<System.Type, List<fsObjectProcessor>>();
    }

    private List<fsObjectProcessor> GetProcessors(System.Type type)
    {
      List<fsObjectProcessor> processors;
      if (this._cachedProcessors.TryGetValue(type, out processors))
        return processors;
      fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>((MemberInfo) type);
      if (attribute != null && attribute.Processor != (System.Type) null)
      {
        fsObjectProcessor instance = (fsObjectProcessor) Activator.CreateInstance(attribute.Processor);
        processors = new List<fsObjectProcessor>();
        processors.Add(instance);
        this._cachedProcessors[type] = processors;
      }
      else if (!this._cachedProcessors.TryGetValue(type, out processors))
      {
        processors = new List<fsObjectProcessor>();
        for (int index = 0; index < this._processors.Count; ++index)
        {
          fsObjectProcessor processor = this._processors[index];
          if (processor.CanProcess(type))
            processors.Add(processor);
        }
        this._cachedProcessors[type] = processors;
      }
      return processors;
    }

    public void AddConverter(fsBaseConverter converter)
    {
      if (converter.Serializer != null)
        throw new InvalidOperationException("Cannot add a single converter instance to multiple fsConverters -- please construct a new instance for " + (object) converter);
      switch (converter)
      {
        case fsDirectConverter _:
          fsDirectConverter fsDirectConverter = (fsDirectConverter) converter;
          this._availableDirectConverters[fsDirectConverter.ModelType] = fsDirectConverter;
          break;
        case fsConverter _:
          this._availableConverters.Insert(0, (fsConverter) converter);
          break;
        default:
          throw new InvalidOperationException("Unable to add converter " + (object) converter + "; the type association strategy is unknown. Please use either fsDirectConverter or fsConverter as your base type.");
      }
      converter.Serializer = this;
      this._cachedConverters = new Dictionary<System.Type, fsBaseConverter>();
    }

    private fsBaseConverter GetConverter(System.Type type, System.Type overrideConverterType)
    {
      if (overrideConverterType != (System.Type) null)
      {
        fsBaseConverter instance;
        if (!this._cachedConverterTypeInstances.TryGetValue(overrideConverterType, out instance))
        {
          instance = (fsBaseConverter) Activator.CreateInstance(overrideConverterType);
          instance.Serializer = this;
          this._cachedConverterTypeInstances[overrideConverterType] = instance;
        }
        return instance;
      }
      fsBaseConverter converter;
      if (this._cachedConverters.TryGetValue(type, out converter))
        return converter;
      fsObjectAttribute attribute1 = fsPortableReflection.GetAttribute<fsObjectAttribute>((MemberInfo) type);
      if (attribute1 != null && attribute1.Converter != (System.Type) null)
      {
        fsBaseConverter instance = (fsBaseConverter) Activator.CreateInstance(attribute1.Converter);
        instance.Serializer = this;
        return this._cachedConverters[type] = instance;
      }
      fsForwardAttribute attribute2 = fsPortableReflection.GetAttribute<fsForwardAttribute>((MemberInfo) type);
      if (attribute2 != null)
      {
        fsBaseConverter fsBaseConverter = (fsBaseConverter) new fsForwardConverter(attribute2);
        fsBaseConverter.Serializer = this;
        return this._cachedConverters[type] = fsBaseConverter;
      }
      if (!this._cachedConverters.TryGetValue(type, out converter))
      {
        if (this._availableDirectConverters.ContainsKey(type))
        {
          converter = (fsBaseConverter) this._availableDirectConverters[type];
          return this._cachedConverters[type] = converter;
        }
        for (int index = 0; index < this._availableConverters.Count; ++index)
        {
          if (this._availableConverters[index].CanProcess(type))
          {
            converter = (fsBaseConverter) this._availableConverters[index];
            return this._cachedConverters[type] = converter;
          }
        }
      }
      throw new InvalidOperationException("Internal error -- could not find a converter for " + (object) type);
    }

    public fsResult TrySerialize<T>(T instance, out fsData data)
    {
      return this.TrySerialize(typeof (T), (object) instance, out data);
    }

    public fsResult TryDeserialize<T>(fsData data, ref T instance)
    {
      object result = (object) instance;
      fsResult fsResult = this.TryDeserialize(data, typeof (T), ref result);
      if (fsResult.Succeeded)
        instance = (T) result;
      return fsResult;
    }

    public fsResult TrySerialize(System.Type storageType, object instance, out fsData data)
    {
      return this.TrySerialize(storageType, (System.Type) null, instance, out data);
    }

    public fsResult TrySerialize(
      System.Type storageType,
      System.Type overrideConverterType,
      object instance,
      out fsData data)
    {
      List<fsObjectProcessor> processors = this.GetProcessors(instance == null ? storageType : instance.GetType());
      fsSerializer.Invoke_OnBeforeSerialize(processors, storageType, instance);
      if (instance == null)
      {
        data = new fsData();
        fsSerializer.Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
        return fsResult.Success;
      }
      fsResult fsResult = this.InternalSerialize_1_ProcessCycles(storageType, overrideConverterType, instance, out data);
      fsSerializer.Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
      return fsResult;
    }

    private fsResult InternalSerialize_1_ProcessCycles(
      System.Type storageType,
      System.Type overrideConverterType,
      object instance,
      out fsData data)
    {
      try
      {
        this._references.Enter();
        if (!this.GetConverter(instance.GetType(), overrideConverterType).RequestCycleSupport(instance.GetType()))
          return this.InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
        if (this._references.IsReference(instance))
        {
          data = fsData.CreateDictionary();
          this._lazyReferenceWriter.WriteReference(this._references.GetReferenceId(instance), data.AsDictionary);
          return fsResult.Success;
        }
        this._references.MarkSerialized(instance);
        fsResult fsResult = this.InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
        if (fsResult.Failed)
          return fsResult;
        this._lazyReferenceWriter.WriteDefinition(this._references.GetReferenceId(instance), data);
        return fsResult;
      }
      finally
      {
        if (this._references.Exit())
          this._lazyReferenceWriter.Clear();
      }
    }

    private fsResult InternalSerialize_2_Inheritance(
      System.Type storageType,
      System.Type overrideConverterType,
      object instance,
      out fsData data)
    {
      fsResult fsResult = this.InternalSerialize_4_Converter(overrideConverterType, instance, out data);
      if (fsResult.Failed || !(storageType != instance.GetType()) || !this.GetConverter(storageType, overrideConverterType).RequestInheritanceSupport(storageType))
        return fsResult;
      fsSerializer.EnsureDictionary(data);
      System.Type safeType = SafeTypeUtility.GetSafeType(instance);
      data.AsDictionary["$type"] = new fsData(safeType.FullName);
      return fsResult;
    }

    private fsResult InternalSerialize_4_Converter(
      System.Type overrideConverterType,
      object instance,
      out fsData data)
    {
      System.Type type = instance.GetType();
      return this.GetConverter(type, overrideConverterType).TrySerialize(instance, out data, type);
    }

    public fsResult TryDeserialize(fsData data, System.Type storageType, ref object result)
    {
      return this.TryDeserialize(data, storageType, (System.Type) null, ref result);
    }

    public fsResult TryDeserialize(
      fsData data,
      System.Type storageType,
      System.Type overrideConverterType,
      ref object result)
    {
      if (data.IsNull)
      {
        result = (object) null;
        List<fsObjectProcessor> processors = this.GetProcessors(storageType);
        fsSerializer.Invoke_OnBeforeDeserialize(processors, storageType, ref data);
        fsSerializer.Invoke_OnAfterDeserialize(processors, storageType, (object) null);
        return fsResult.Success;
      }
      try
      {
        this._references.Enter();
        List<fsObjectProcessor> processors;
        fsResult fsResult = this.InternalDeserialize_1_CycleReference(overrideConverterType, data, storageType, ref result, out processors);
        if (fsResult.Succeeded)
          fsSerializer.Invoke_OnAfterDeserialize(processors, storageType, result);
        return fsResult;
      }
      catch (Exception ex)
      {
        string str = string.Format("<b>(Deserialization Error)</b>: {0}\n{1}", (object) ex.Message, (object) ex.StackTrace);
        Debug.LogError((object) str);
        return fsResult.Fail(str);
      }
      finally
      {
        this._references.Exit();
      }
    }

    private fsResult InternalDeserialize_1_CycleReference(
      System.Type overrideConverterType,
      fsData data,
      System.Type storageType,
      ref object result,
      out List<fsObjectProcessor> processors)
    {
      if (!fsSerializer.IsObjectReference(data))
        return this.InternalDeserialize_3_Inheritance(overrideConverterType, data, storageType, ref result, out processors);
      int id = int.Parse(data.AsDictionary["$ref"].AsString);
      result = this._references.GetReferenceObject(id);
      processors = this.GetProcessors(result.GetType());
      return fsResult.Success;
    }

    private fsResult InternalDeserialize_3_Inheritance(
      System.Type overrideConverterType,
      fsData data,
      System.Type storageType,
      ref object result,
      out List<fsObjectProcessor> processors)
    {
      fsResult success = fsResult.Success;
      processors = this.GetProcessors(storageType);
      fsSerializer.Invoke_OnBeforeDeserialize(processors, storageType, ref data);
      System.Type type1 = storageType;
      if (fsSerializer.IsTypeSpecified(data))
      {
        fsData fsData = data.AsDictionary["$type"];
        if (!fsData.IsString)
        {
          string message = "$type value must be a string (in " + (object) data + ")";
          success.AddMessage(message);
        }
        else
        {
          string asString = fsData.AsString;
          System.Type type2 = fsTypeCache.GetType(asString, storageType);
          if (type2 == (System.Type) null)
          {
            string message = "Unable to locate specified type \"" + asString + "\" , " + ReflectionTools.GetContext();
            success.AddMessage(message);
            Debug.LogWarning((object) message);
          }
          else if (!storageType.IsAssignableFrom(type2))
          {
            string message = "Ignoring type specifier; a field/property of type " + (object) storageType + " cannot hold an instance of " + (object) type2;
            success.AddMessage(message);
            Debug.LogWarning((object) message);
          }
          else
            type1 = type2;
        }
      }
      if (result == null || result.GetType() != type1)
      {
        fsBaseConverter converter = this.GetConverter(type1, overrideConverterType);
        result = converter.CreateInstance(data, type1);
      }
      fsSerializer.Invoke_OnBeforeDeserializeAfterInstanceCreation(processors, storageType, result, ref data);
      return success += this.InternalDeserialize_4_Cycles(overrideConverterType, data, type1, ref result);
    }

    private fsResult InternalDeserialize_4_Cycles(
      System.Type overrideConverterType,
      fsData data,
      System.Type resultType,
      ref object result)
    {
      if (fsSerializer.IsObjectDefinition(data))
        this._references.AddReferenceWithId(int.Parse(data.AsDictionary["$id"].AsString), result);
      return this.InternalDeserialize_5_Converter(overrideConverterType, data, resultType, ref result);
    }

    private fsResult InternalDeserialize_5_Converter(
      System.Type overrideConverterType,
      fsData data,
      System.Type resultType,
      ref object result)
    {
      if (fsSerializer.IsWrappedData(data))
        data = data.AsDictionary["$content"];
      return this.GetConverter(resultType, overrideConverterType).TryDeserialize(data, ref result, resultType);
    }

    internal class fsLazyCycleDefinitionWriter
    {
      private Dictionary<int, fsData> _pendingDefinitions = new Dictionary<int, fsData>();
      private HashSet<int> _references = new HashSet<int>();

      public void WriteDefinition(int id, fsData data)
      {
        if (this._references.Contains(id))
        {
          fsSerializer.EnsureDictionary(data);
          data.AsDictionary["$id"] = new fsData(id.ToString());
        }
        else
          this._pendingDefinitions[id] = data;
      }

      public void WriteReference(int id, Dictionary<string, fsData> dict)
      {
        if (this._pendingDefinitions.ContainsKey(id))
        {
          fsData pendingDefinition = this._pendingDefinitions[id];
          fsSerializer.EnsureDictionary(pendingDefinition);
          pendingDefinition.AsDictionary["$id"] = new fsData(id.ToString());
          this._pendingDefinitions.Remove(id);
        }
        else
          this._references.Add(id);
        dict["$ref"] = new fsData(id.ToString());
      }

      public void Clear()
      {
        this._pendingDefinitions.Clear();
        this._references.Clear();
      }
    }
  }
}
