using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class GUIStyle_DirectConverter : fsDirectConverter<GUIStyle>
  {
    protected override fsResult DoSerialize(GUIStyle model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember(serialized, null, "active", model.active) + SerializeMember(serialized, null, "alignment", model.alignment) + SerializeMember(serialized, null, "border", model.border) + SerializeMember(serialized, null, "clipping", model.clipping) + SerializeMember(serialized, null, "contentOffset", model.contentOffset) + SerializeMember(serialized, null, "fixedHeight", model.fixedHeight) + SerializeMember(serialized, null, "fixedWidth", model.fixedWidth) + SerializeMember(serialized, null, "focused", model.focused) + SerializeMember(serialized, null, "font", model.font) + SerializeMember(serialized, null, "fontSize", model.fontSize) + SerializeMember(serialized, null, "fontStyle", model.fontStyle) + SerializeMember(serialized, null, "hover", model.hover) + SerializeMember(serialized, null, "imagePosition", model.imagePosition) + SerializeMember(serialized, null, "margin", model.margin) + SerializeMember(serialized, null, "name", model.name) + SerializeMember(serialized, null, "normal", model.normal) + SerializeMember(serialized, null, "onActive", model.onActive) + SerializeMember(serialized, null, "onFocused", model.onFocused) + SerializeMember(serialized, null, "onHover", model.onHover) + SerializeMember(serialized, null, "onNormal", model.onNormal) + SerializeMember(serialized, null, "overflow", model.overflow) + SerializeMember(serialized, null, "padding", model.padding) + SerializeMember(serialized, null, "richText", model.richText) + SerializeMember(serialized, null, "stretchHeight", model.stretchHeight) + SerializeMember(serialized, null, "stretchWidth", model.stretchWidth) + SerializeMember(serialized, null, "wordWrap", model.wordWrap);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GUIStyle model)
    {
      fsResult success = fsResult.Success;
      fsResult fsResult1 = success + DeserializeMember(data, null, "active", out GUIStyleState active);
      model.active = active;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "alignment", out TextAnchor alignment);
      model.alignment = alignment;
      fsResult fsResult3 = fsResult2 + DeserializeMember(data, null, "border", out RectOffset border);
      model.border = border;
      fsResult fsResult4 = fsResult3 + DeserializeMember(data, null, "clipping", out TextClipping clipping);
      model.clipping = clipping;
      fsResult fsResult5 = fsResult4 + DeserializeMember(data, null, "contentOffset", out Vector2 contentOffset);
      model.contentOffset = contentOffset;
      fsResult fsResult6 = fsResult5 + DeserializeMember(data, null, "fixedHeight", out float fixedHeight);
      model.fixedHeight = fixedHeight;
      fsResult fsResult7 = fsResult6 + DeserializeMember(data, null, "fixedWidth", out float fixedWidth);
      model.fixedWidth = fixedWidth;
      fsResult fsResult8 = fsResult7 + DeserializeMember(data, null, "focused", out GUIStyleState focused);
      model.focused = focused;
      fsResult fsResult9 = fsResult8 + DeserializeMember(data, null, "font", out Font font);
      model.font = font;
      fsResult fsResult10 = fsResult9 + DeserializeMember(data, null, "fontSize", out int fontSize);
      model.fontSize = fontSize;
      fsResult fsResult11 = fsResult10 + DeserializeMember(data, null, "fontStyle", out FontStyle fontStyle);
      model.fontStyle = fontStyle;
      fsResult fsResult12 = fsResult11 + DeserializeMember(data, null, "hover", out GUIStyleState hover);
      model.hover = hover;
      fsResult fsResult13 = fsResult12 + DeserializeMember(data, null, "imagePosition", out ImagePosition imagePosition);
      model.imagePosition = imagePosition;
      fsResult fsResult14 = fsResult13 + DeserializeMember(data, null, "margin", out RectOffset margin);
      model.margin = margin;
      fsResult fsResult15 = fsResult14 + DeserializeMember(data, null, "name", out string name);
      model.name = name;
      fsResult fsResult16 = fsResult15 + DeserializeMember(data, null, "normal", out GUIStyleState normal);
      model.normal = normal;
      fsResult fsResult17 = fsResult16 + DeserializeMember(data, null, "onActive", out GUIStyleState onActive);
      model.onActive = onActive;
      fsResult fsResult18 = fsResult17 + DeserializeMember(data, null, "onFocused", out GUIStyleState onFocused);
      model.onFocused = onFocused;
      fsResult fsResult19 = fsResult18 + DeserializeMember(data, null, "onHover", out GUIStyleState onHover);
      model.onHover = onHover;
      fsResult fsResult20 = fsResult19 + DeserializeMember(data, null, "onNormal", out GUIStyleState onNormal);
      model.onNormal = onNormal;
      fsResult fsResult21 = fsResult20 + DeserializeMember(data, null, "overflow", out RectOffset overflow);
      model.overflow = overflow;
      fsResult fsResult22 = fsResult21 + DeserializeMember(data, null, "padding", out RectOffset padding);
      model.padding = padding;
      fsResult fsResult23 = fsResult22 + DeserializeMember(data, null, "richText", out bool richText);
      model.richText = richText;
      fsResult fsResult24 = fsResult23 + DeserializeMember(data, null, "stretchHeight", out bool stretchHeight);
      model.stretchHeight = stretchHeight;
      fsResult fsResult25 = fsResult24 + DeserializeMember(data, null, "stretchWidth", out bool stretchWidth);
      model.stretchWidth = stretchWidth;
      fsResult fsResult26 = fsResult25 + DeserializeMember(data, null, "wordWrap", out bool wordWrap);
      model.wordWrap = wordWrap;
      return fsResult26;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new GUIStyle();
    }
  }
}
