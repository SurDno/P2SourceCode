using System;
using System.Collections.Generic;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public class fsSerializer
  {
    private static HashSet<string> _reservedKeywords = new HashSet<string> {
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
    private Dictionary<Type, fsBaseConverter> _cachedConverterTypeInstances;
    private Dictionary<Type, fsBaseConverter> _cachedConverters;
    private Dictionary<Type, List<fsObjectProcessor>> _cachedProcessors;
    private readonly List<fsConverter> _availableConverters;
    private readonly Dictionary<Type, fsDirectConverter> _availableDirectConverters;
    private readonly List<fsObjectProcessor> _processors;
    private readonly fsCyclicReferenceManager _references;
    private readonly fsLazyCycleDefinitionWriter _lazyReferenceWriter;
    public fsContext Context;
    public fsConfig Config;

    public static bool IsReservedKeyword(string key)
    {
      return _reservedKeywords.Contains(key);
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
      Type storageType,
      object instance)
    {
      for (int index = 0; index < processors.Count; ++index)
        processors[index].OnBeforeSerialize(storageType, instance);
      if (!(instance is ISerializationCallbackReceiver) || instance is Object)
        return;
      ((ISerializationCallbackReceiver) instance).OnBeforeSerialize();
    }

    private static void Invoke_OnAfterSerialize(
      List<fsObjectProcessor> processors,
      Type storageType,
      object instance,
      ref fsData data)
    {
      for (int index = processors.Count - 1; index >= 0; --index)
        processors[index].OnAfterSerialize(storageType, instance, ref data);
    }

    private static void Invoke_OnBeforeDeserialize(
      List<fsObjectProcessor> processors,
      Type storageType,
      ref fsData data)
    {
      for (int index = 0; index < processors.Count; ++index)
        processors[index].OnBeforeDeserialize(storageType, ref data);
    }

    private static void Invoke_OnBeforeDeserializeAfterInstanceCreation(
      List<fsObjectProcessor> processors,
      Type storageType,
      object instance,
      ref fsData data)
    {
      for (int index = 0; index < processors.Count; ++index)
        processors[index].OnBeforeDeserializeAfterInstanceCreation(storageType, instance, ref data);
    }

    private static void Invoke_OnAfterDeserialize(
      List<fsObjectProcessor> processors,
      Type storageType,
      object instance)
    {
      for (int index = processors.Count - 1; index >= 0; --index)
        processors[index].OnAfterDeserialize(storageType, instance);
      if (!(instance is ISerializationCallbackReceiver) || instance is Object)
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
      _cachedConverterTypeInstances = new Dictionary<Type, fsBaseConverter>();
      _cachedConverters = new Dictionary<Type, fsBaseConverter>();
      _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
      _references = new fsCyclicReferenceManager();
      _lazyReferenceWriter = new fsLazyCycleDefinitionWriter();
      List<fsConverter> fsConverterList = new List<fsConverter>();
      fsNullableConverter nullableConverter = new fsNullableConverter();
      nullableConverter.Serializer = this;
      fsConverterList.Add(nullableConverter);
      fsGuidConverter fsGuidConverter = new fsGuidConverter();
      fsGuidConverter.Serializer = this;
      fsConverterList.Add(fsGuidConverter);
      fsTypeConverter fsTypeConverter = new fsTypeConverter();
      fsTypeConverter.Serializer = this;
      fsConverterList.Add(fsTypeConverter);
      fsDateConverter fsDateConverter = new fsDateConverter();
      fsDateConverter.Serializer = this;
      fsConverterList.Add(fsDateConverter);
      fsEnumConverter fsEnumConverter = new fsEnumConverter();
      fsEnumConverter.Serializer = this;
      fsConverterList.Add(fsEnumConverter);
      fsPrimitiveConverter primitiveConverter = new fsPrimitiveConverter();
      primitiveConverter.Serializer = this;
      fsConverterList.Add(primitiveConverter);
      fsArrayConverter fsArrayConverter = new fsArrayConverter();
      fsArrayConverter.Serializer = this;
      fsConverterList.Add(fsArrayConverter);
      fsDictionaryConverter dictionaryConverter = new fsDictionaryConverter();
      dictionaryConverter.Serializer = this;
      fsConverterList.Add(dictionaryConverter);
      fsIEnumerableConverter ienumerableConverter = new fsIEnumerableConverter();
      ienumerableConverter.Serializer = this;
      fsConverterList.Add(ienumerableConverter);
      fsKeyValuePairConverter valuePairConverter = new fsKeyValuePairConverter();
      valuePairConverter.Serializer = this;
      fsConverterList.Add(valuePairConverter);
      fsWeakReferenceConverter referenceConverter = new fsWeakReferenceConverter();
      referenceConverter.Serializer = this;
      fsConverterList.Add(referenceConverter);
      fsReflectedConverter reflectedConverter = new fsReflectedConverter();
      reflectedConverter.Serializer = this;
      fsConverterList.Add(reflectedConverter);
      _availableConverters = fsConverterList;
      _availableDirectConverters = new Dictionary<Type, fsDirectConverter>();
      _processors = new List<fsObjectProcessor>();
      Context = new fsContext();
      Config = new fsConfig();
      foreach (Type converter in fsConverterRegistrar.Converters)
        AddConverter((fsBaseConverter) Activator.CreateInstance(converter));
    }

    public void AddProcessor(fsObjectProcessor processor)
    {
      _processors.Add(processor);
      _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
    }

    public void RemoveProcessor<TProcessor>()
    {
      int index = 0;
      while (index < _processors.Count)
      {
        if (_processors[index] is TProcessor)
          _processors.RemoveAt(index);
        else
          ++index;
      }
      _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
    }

    private List<fsObjectProcessor> GetProcessors(Type type)
    {
      List<fsObjectProcessor> processors;
      if (_cachedProcessors.TryGetValue(type, out processors))
        return processors;
      fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
      if (attribute != null && attribute.Processor != null)
      {
        fsObjectProcessor instance = (fsObjectProcessor) Activator.CreateInstance(attribute.Processor);
        processors = new List<fsObjectProcessor>();
        processors.Add(instance);
        _cachedProcessors[type] = processors;
      }
      else if (!_cachedProcessors.TryGetValue(type, out processors))
      {
        processors = new List<fsObjectProcessor>();
        for (int index = 0; index < _processors.Count; ++index)
        {
          fsObjectProcessor processor = _processors[index];
          if (processor.CanProcess(type))
            processors.Add(processor);
        }
        _cachedProcessors[type] = processors;
      }
      return processors;
    }

    public void AddConverter(fsBaseConverter converter)
    {
      if (converter.Serializer != null)
        throw new InvalidOperationException("Cannot add a single converter instance to multiple fsConverters -- please construct a new instance for " + converter);
      switch (converter)
      {
        case fsDirectConverter _:
          fsDirectConverter fsDirectConverter = (fsDirectConverter) converter;
          _availableDirectConverters[fsDirectConverter.ModelType] = fsDirectConverter;
          break;
        case fsConverter _:
          _availableConverters.Insert(0, (fsConverter) converter);
          break;
        default:
          throw new InvalidOperationException("Unable to add converter " + converter + "; the type association strategy is unknown. Please use either fsDirectConverter or fsConverter as your base type.");
      }
      converter.Serializer = this;
      _cachedConverters = new Dictionary<Type, fsBaseConverter>();
    }

    private fsBaseConverter GetConverter(Type type, Type overrideConverterType)
    {
      if (overrideConverterType != null)
      {
        fsBaseConverter instance;
        if (!_cachedConverterTypeInstances.TryGetValue(overrideConverterType, out instance))
        {
          instance = (fsBaseConverter) Activator.CreateInstance(overrideConverterType);
          instance.Serializer = this;
          _cachedConverterTypeInstances[overrideConverterType] = instance;
        }
        return instance;
      }
      fsBaseConverter converter;
      if (_cachedConverters.TryGetValue(type, out converter))
        return converter;
      fsObjectAttribute attribute1 = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
      if (attribute1 != null && attribute1.Converter != null)
      {
        fsBaseConverter instance = (fsBaseConverter) Activator.CreateInstance(attribute1.Converter);
        instance.Serializer = this;
        return _cachedConverters[type] = instance;
      }
      fsForwardAttribute attribute2 = fsPortableReflection.GetAttribute<fsForwardAttribute>(type);
      if (attribute2 != null)
      {
        fsBaseConverter fsBaseConverter = new fsForwardConverter(attribute2);
        fsBaseConverter.Serializer = this;
        return _cachedConverters[type] = fsBaseConverter;
      }
      if (!_cachedConverters.TryGetValue(type, out converter))
      {
        if (_availableDirectConverters.ContainsKey(type))
        {
          converter = _availableDirectConverters[type];
          return _cachedConverters[type] = converter;
        }
        for (int index = 0; index < _availableConverters.Count; ++index)
        {
          if (_availableConverters[index].CanProcess(type))
          {
            converter = _availableConverters[index];
            return _cachedConverters[type] = converter;
          }
        }
      }
      throw new InvalidOperationException("Internal error -- could not find a converter for " + type);
    }

    public fsResult TrySerialize<T>(T instance, out fsData data)
    {
      return TrySerialize(typeof (T), instance, out data);
    }

    public fsResult TryDeserialize<T>(fsData data, ref T instance)
    {
      object result = instance;
      fsResult fsResult = TryDeserialize(data, typeof (T), ref result);
      if (fsResult.Succeeded)
        instance = (T) result;
      return fsResult;
    }

    public fsResult TrySerialize(Type storageType, object instance, out fsData data)
    {
      return TrySerialize(storageType, null, instance, out data);
    }

    public fsResult TrySerialize(
      Type storageType,
      Type overrideConverterType,
      object instance,
      out fsData data)
    {
      List<fsObjectProcessor> processors = GetProcessors(instance == null ? storageType : instance.GetType());
      Invoke_OnBeforeSerialize(processors, storageType, instance);
      if (instance == null)
      {
        data = new fsData();
        Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
        return fsResult.Success;
      }
      fsResult fsResult2 = InternalSerialize_1_ProcessCycles(storageType, overrideConverterType, instance, out data);
      Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
      return fsResult2;
    }

    private fsResult InternalSerialize_1_ProcessCycles(
      Type storageType,
      Type overrideConverterType,
      object instance,
      out fsData data)
    {
      try
      {
        _references.Enter();
        if (!GetConverter(instance.GetType(), overrideConverterType).RequestCycleSupport(instance.GetType()))
          return InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
        if (_references.IsReference(instance))
        {
          data = fsData.CreateDictionary();
          _lazyReferenceWriter.WriteReference(_references.GetReferenceId(instance), data.AsDictionary);
          return fsResult.Success;
        }
        _references.MarkSerialized(instance);
        fsResult fsResult2 = InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
        if (fsResult2.Failed)
          return fsResult2;
        _lazyReferenceWriter.WriteDefinition(_references.GetReferenceId(instance), data);
        return fsResult2;
      }
      finally
      {
        if (_references.Exit())
          _lazyReferenceWriter.Clear();
      }
    }

    private fsResult InternalSerialize_2_Inheritance(
      Type storageType,
      Type overrideConverterType,
      object instance,
      out fsData data)
    {
      fsResult fsResult = InternalSerialize_4_Converter(overrideConverterType, instance, out data);
      if (fsResult.Failed || !(storageType != instance.GetType()) || !GetConverter(storageType, overrideConverterType).RequestInheritanceSupport(storageType))
        return fsResult;
      EnsureDictionary(data);
      Type safeType = SafeTypeUtility.GetSafeType(instance);
      data.AsDictionary["$type"] = new fsData(safeType.FullName);
      return fsResult;
    }

    private fsResult InternalSerialize_4_Converter(
      Type overrideConverterType,
      object instance,
      out fsData data)
    {
      Type type = instance.GetType();
      return GetConverter(type, overrideConverterType).TrySerialize(instance, out data, type);
    }

    public fsResult TryDeserialize(fsData data, Type storageType, ref object result)
    {
      return TryDeserialize(data, storageType, null, ref result);
    }

    public fsResult TryDeserialize(
      fsData data,
      Type storageType,
      Type overrideConverterType,
      ref object result)
    {
      if (data.IsNull)
      {
        result = null;
        List<fsObjectProcessor> processors = GetProcessors(storageType);
        Invoke_OnBeforeDeserialize(processors, storageType, ref data);
        Invoke_OnAfterDeserialize(processors, storageType, null);
        return fsResult.Success;
      }
      try
      {
        _references.Enter();
        List<fsObjectProcessor> processors;
        fsResult fsResult = InternalDeserialize_1_CycleReference(overrideConverterType, data, storageType, ref result, out processors);
        if (fsResult.Succeeded)
          Invoke_OnAfterDeserialize(processors, storageType, result);
        return fsResult;
      }
      catch (Exception ex)
      {
        string str = string.Format("<b>(Deserialization Error)</b>: {0}\n{1}", ex.Message, ex.StackTrace);
        Debug.LogError(str);
        return fsResult.Fail(str);
      }
      finally
      {
        _references.Exit();
      }
    }

    private fsResult InternalDeserialize_1_CycleReference(
      Type overrideConverterType,
      fsData data,
      Type storageType,
      ref object result,
      out List<fsObjectProcessor> processors)
    {
      if (!IsObjectReference(data))
        return InternalDeserialize_3_Inheritance(overrideConverterType, data, storageType, ref result, out processors);
      int id = int.Parse(data.AsDictionary["$ref"].AsString);
      result = _references.GetReferenceObject(id);
      processors = GetProcessors(result.GetType());
      return fsResult.Success;
    }

    private fsResult InternalDeserialize_3_Inheritance(
      Type overrideConverterType,
      fsData data,
      Type storageType,
      ref object result,
      out List<fsObjectProcessor> processors)
    {
      fsResult success = fsResult.Success;
      processors = GetProcessors(storageType);
      Invoke_OnBeforeDeserialize(processors, storageType, ref data);
      Type type1 = storageType;
      if (IsTypeSpecified(data))
      {
        fsData fsData = data.AsDictionary["$type"];
        if (!fsData.IsString)
        {
          string message = "$type value must be a string (in " + data + ")";
          success.AddMessage(message);
        }
        else
        {
          string asString = fsData.AsString;
          Type type2 = fsTypeCache.GetType(asString, storageType);
          if (type2 == null)
          {
            string message = "Unable to locate specified type \"" + asString + "\" , " + ReflectionTools.GetContext();
            success.AddMessage(message);
            Debug.LogWarning(message);
          }
          else if (!storageType.IsAssignableFrom(type2))
          {
            string message = "Ignoring type specifier; a field/property of type " + storageType + " cannot hold an instance of " + type2;
            success.AddMessage(message);
            Debug.LogWarning(message);
          }
          else
            type1 = type2;
        }
      }
      if (result == null || result.GetType() != type1)
      {
        fsBaseConverter converter = GetConverter(type1, overrideConverterType);
        result = converter.CreateInstance(data, type1);
      }
      Invoke_OnBeforeDeserializeAfterInstanceCreation(processors, storageType, result, ref data);
      return success += InternalDeserialize_4_Cycles(overrideConverterType, data, type1, ref result);
    }

    private fsResult InternalDeserialize_4_Cycles(
      Type overrideConverterType,
      fsData data,
      Type resultType,
      ref object result)
    {
      if (IsObjectDefinition(data))
        _references.AddReferenceWithId(int.Parse(data.AsDictionary["$id"].AsString), result);
      return InternalDeserialize_5_Converter(overrideConverterType, data, resultType, ref result);
    }

    private fsResult InternalDeserialize_5_Converter(
      Type overrideConverterType,
      fsData data,
      Type resultType,
      ref object result)
    {
      if (IsWrappedData(data))
        data = data.AsDictionary["$content"];
      return GetConverter(resultType, overrideConverterType).TryDeserialize(data, ref result, resultType);
    }

    internal class fsLazyCycleDefinitionWriter
    {
      private Dictionary<int, fsData> _pendingDefinitions = new Dictionary<int, fsData>();
      private HashSet<int> _references = new HashSet<int>();

      public void WriteDefinition(int id, fsData data)
      {
        if (_references.Contains(id))
        {
          EnsureDictionary(data);
          data.AsDictionary["$id"] = new fsData(id.ToString());
        }
        else
          _pendingDefinitions[id] = data;
      }

      public void WriteReference(int id, Dictionary<string, fsData> dict)
      {
        if (_pendingDefinitions.ContainsKey(id))
        {
          fsData pendingDefinition = _pendingDefinitions[id];
          EnsureDictionary(pendingDefinition);
          pendingDefinition.AsDictionary["$id"] = new fsData(id.ToString());
          _pendingDefinitions.Remove(id);
        }
        else
          _references.Add(id);
        dict["$ref"] = new fsData(id.ToString());
      }

      public void Clear()
      {
        _pendingDefinitions.Clear();
        _references.Clear();
      }
    }
  }
}
