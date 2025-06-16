using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class GUIStyle_DirectConverter : fsDirectConverter<GUIStyle>
  {
    protected override fsResult DoSerialize(GUIStyle model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember<GUIStyleState>(serialized, null, "active", model.active) + SerializeMember<TextAnchor>(serialized, null, "alignment", model.alignment) + SerializeMember<RectOffset>(serialized, null, "border", model.border) + SerializeMember<TextClipping>(serialized, null, "clipping", model.clipping) + SerializeMember<Vector2>(serialized, null, "contentOffset", model.contentOffset) + SerializeMember<float>(serialized, null, "fixedHeight", model.fixedHeight) + SerializeMember<float>(serialized, null, "fixedWidth", model.fixedWidth) + SerializeMember<GUIStyleState>(serialized, null, "focused", model.focused) + SerializeMember<Font>(serialized, null, "font", model.font) + SerializeMember<int>(serialized, null, "fontSize", model.fontSize) + SerializeMember<FontStyle>(serialized, null, "fontStyle", model.fontStyle) + SerializeMember<GUIStyleState>(serialized, null, "hover", model.hover) + SerializeMember<ImagePosition>(serialized, null, "imagePosition", model.imagePosition) + SerializeMember<RectOffset>(serialized, null, "margin", model.margin) + SerializeMember<string>(serialized, null, "name", model.name) + SerializeMember<GUIStyleState>(serialized, null, "normal", model.normal) + SerializeMember<GUIStyleState>(serialized, null, "onActive", model.onActive) + SerializeMember<GUIStyleState>(serialized, null, "onFocused", model.onFocused) + SerializeMember<GUIStyleState>(serialized, null, "onHover", model.onHover) + SerializeMember<GUIStyleState>(serialized, null, "onNormal", model.onNormal) + SerializeMember<RectOffset>(serialized, null, "overflow", model.overflow) + SerializeMember<RectOffset>(serialized, null, "padding", model.padding) + SerializeMember<bool>(serialized, null, "richText", model.richText) + SerializeMember<bool>(serialized, null, "stretchHeight", model.stretchHeight) + SerializeMember<bool>(serialized, null, "stretchWidth", model.stretchWidth) + SerializeMember<bool>(serialized, null, "wordWrap", model.wordWrap);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GUIStyle model)
    {
      fsResult success = fsResult.Success;
      GUIStyleState active = model.active;
      fsResult fsResult1 = success + DeserializeMember(data, null, "active", out active);
      model.active = active;
      TextAnchor alignment = model.alignment;
      fsResult fsResult2 = fsResult1 + DeserializeMember(data, null, "alignment", out alignment);
      model.alignment = alignment;
      RectOffset border = model.border;
      fsResult fsResult3 = fsResult2 + DeserializeMember(data, null, "border", out border);
      model.border = border;
      TextClipping clipping = model.clipping;
      fsResult fsResult4 = fsResult3 + DeserializeMember(data, null, "clipping", out clipping);
      model.clipping = clipping;
      Vector2 contentOffset = model.contentOffset;
      fsResult fsResult5 = fsResult4 + DeserializeMember(data, null, "contentOffset", out contentOffset);
      model.contentOffset = contentOffset;
      float fixedHeight = model.fixedHeight;
      fsResult fsResult6 = fsResult5 + DeserializeMember(data, null, "fixedHeight", out fixedHeight);
      model.fixedHeight = fixedHeight;
      float fixedWidth = model.fixedWidth;
      fsResult fsResult7 = fsResult6 + DeserializeMember(data, null, "fixedWidth", out fixedWidth);
      model.fixedWidth = fixedWidth;
      GUIStyleState focused = model.focused;
      fsResult fsResult8 = fsResult7 + DeserializeMember(data, null, "focused", out focused);
      model.focused = focused;
      Font font = model.font;
      fsResult fsResult9 = fsResult8 + DeserializeMember(data, null, "font", out font);
      model.font = font;
      int fontSize = model.fontSize;
      fsResult fsResult10 = fsResult9 + DeserializeMember(data, null, "fontSize", out fontSize);
      model.fontSize = fontSize;
      FontStyle fontStyle = model.fontStyle;
      fsResult fsResult11 = fsResult10 + DeserializeMember(data, null, "fontStyle", out fontStyle);
      model.fontStyle = fontStyle;
      GUIStyleState hover = model.hover;
      fsResult fsResult12 = fsResult11 + DeserializeMember(data, null, "hover", out hover);
      model.hover = hover;
      ImagePosition imagePosition = model.imagePosition;
      fsResult fsResult13 = fsResult12 + DeserializeMember(data, null, "imagePosition", out imagePosition);
      model.imagePosition = imagePosition;
      RectOffset margin = model.margin;
      fsResult fsResult14 = fsResult13 + DeserializeMember(data, null, "margin", out margin);
      model.margin = margin;
      string name = model.name;
      fsResult fsResult15 = fsResult14 + DeserializeMember(data, null, "name", out name);
      model.name = name;
      GUIStyleState normal = model.normal;
      fsResult fsResult16 = fsResult15 + DeserializeMember(data, null, "normal", out normal);
      model.normal = normal;
      GUIStyleState onActive = model.onActive;
      fsResult fsResult17 = fsResult16 + DeserializeMember(data, null, "onActive", out onActive);
      model.onActive = onActive;
      GUIStyleState onFocused = model.onFocused;
      fsResult fsResult18 = fsResult17 + DeserializeMember(data, null, "onFocused", out onFocused);
      model.onFocused = onFocused;
      GUIStyleState onHover = model.onHover;
      fsResult fsResult19 = fsResult18 + DeserializeMember(data, null, "onHover", out onHover);
      model.onHover = onHover;
      GUIStyleState onNormal = model.onNormal;
      fsResult fsResult20 = fsResult19 + DeserializeMember(data, null, "onNormal", out onNormal);
      model.onNormal = onNormal;
      RectOffset overflow = model.overflow;
      fsResult fsResult21 = fsResult20 + DeserializeMember(data, null, "overflow", out overflow);
      model.overflow = overflow;
      RectOffset padding = model.padding;
      fsResult fsResult22 = fsResult21 + DeserializeMember(data, null, "padding", out padding);
      model.padding = padding;
      bool richText = model.richText;
      fsResult fsResult23 = fsResult22 + DeserializeMember(data, null, "richText", out richText);
      model.richText = richText;
      bool stretchHeight = model.stretchHeight;
      fsResult fsResult24 = fsResult23 + DeserializeMember(data, null, "stretchHeight", out stretchHeight);
      model.stretchHeight = stretchHeight;
      bool stretchWidth = model.stretchWidth;
      fsResult fsResult25 = fsResult24 + DeserializeMember(data, null, "stretchWidth", out stretchWidth);
      model.stretchWidth = stretchWidth;
      bool wordWrap = model.wordWrap;
      fsResult fsResult26 = fsResult25 + DeserializeMember(data, null, "wordWrap", out wordWrap);
      model.wordWrap = wordWrap;
      return fsResult26;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return (object) new GUIStyle();
    }
  }
}
