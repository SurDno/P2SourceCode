using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class GUIStyle_DirectConverter : fsDirectConverter<GUIStyle>
  {
    protected override fsResult DoSerialize(GUIStyle model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + this.SerializeMember<GUIStyleState>(serialized, (System.Type) null, "active", model.active) + this.SerializeMember<TextAnchor>(serialized, (System.Type) null, "alignment", model.alignment) + this.SerializeMember<RectOffset>(serialized, (System.Type) null, "border", model.border) + this.SerializeMember<TextClipping>(serialized, (System.Type) null, "clipping", model.clipping) + this.SerializeMember<Vector2>(serialized, (System.Type) null, "contentOffset", model.contentOffset) + this.SerializeMember<float>(serialized, (System.Type) null, "fixedHeight", model.fixedHeight) + this.SerializeMember<float>(serialized, (System.Type) null, "fixedWidth", model.fixedWidth) + this.SerializeMember<GUIStyleState>(serialized, (System.Type) null, "focused", model.focused) + this.SerializeMember<Font>(serialized, (System.Type) null, "font", model.font) + this.SerializeMember<int>(serialized, (System.Type) null, "fontSize", model.fontSize) + this.SerializeMember<FontStyle>(serialized, (System.Type) null, "fontStyle", model.fontStyle) + this.SerializeMember<GUIStyleState>(serialized, (System.Type) null, "hover", model.hover) + this.SerializeMember<ImagePosition>(serialized, (System.Type) null, "imagePosition", model.imagePosition) + this.SerializeMember<RectOffset>(serialized, (System.Type) null, "margin", model.margin) + this.SerializeMember<string>(serialized, (System.Type) null, "name", model.name) + this.SerializeMember<GUIStyleState>(serialized, (System.Type) null, "normal", model.normal) + this.SerializeMember<GUIStyleState>(serialized, (System.Type) null, "onActive", model.onActive) + this.SerializeMember<GUIStyleState>(serialized, (System.Type) null, "onFocused", model.onFocused) + this.SerializeMember<GUIStyleState>(serialized, (System.Type) null, "onHover", model.onHover) + this.SerializeMember<GUIStyleState>(serialized, (System.Type) null, "onNormal", model.onNormal) + this.SerializeMember<RectOffset>(serialized, (System.Type) null, "overflow", model.overflow) + this.SerializeMember<RectOffset>(serialized, (System.Type) null, "padding", model.padding) + this.SerializeMember<bool>(serialized, (System.Type) null, "richText", model.richText) + this.SerializeMember<bool>(serialized, (System.Type) null, "stretchHeight", model.stretchHeight) + this.SerializeMember<bool>(serialized, (System.Type) null, "stretchWidth", model.stretchWidth) + this.SerializeMember<bool>(serialized, (System.Type) null, "wordWrap", model.wordWrap);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GUIStyle model)
    {
      fsResult success = fsResult.Success;
      GUIStyleState active = model.active;
      fsResult fsResult1 = success + this.DeserializeMember<GUIStyleState>(data, (System.Type) null, "active", out active);
      model.active = active;
      TextAnchor alignment = model.alignment;
      fsResult fsResult2 = fsResult1 + this.DeserializeMember<TextAnchor>(data, (System.Type) null, "alignment", out alignment);
      model.alignment = alignment;
      RectOffset border = model.border;
      fsResult fsResult3 = fsResult2 + this.DeserializeMember<RectOffset>(data, (System.Type) null, "border", out border);
      model.border = border;
      TextClipping clipping = model.clipping;
      fsResult fsResult4 = fsResult3 + this.DeserializeMember<TextClipping>(data, (System.Type) null, "clipping", out clipping);
      model.clipping = clipping;
      Vector2 contentOffset = model.contentOffset;
      fsResult fsResult5 = fsResult4 + this.DeserializeMember<Vector2>(data, (System.Type) null, "contentOffset", out contentOffset);
      model.contentOffset = contentOffset;
      float fixedHeight = model.fixedHeight;
      fsResult fsResult6 = fsResult5 + this.DeserializeMember<float>(data, (System.Type) null, "fixedHeight", out fixedHeight);
      model.fixedHeight = fixedHeight;
      float fixedWidth = model.fixedWidth;
      fsResult fsResult7 = fsResult6 + this.DeserializeMember<float>(data, (System.Type) null, "fixedWidth", out fixedWidth);
      model.fixedWidth = fixedWidth;
      GUIStyleState focused = model.focused;
      fsResult fsResult8 = fsResult7 + this.DeserializeMember<GUIStyleState>(data, (System.Type) null, "focused", out focused);
      model.focused = focused;
      Font font = model.font;
      fsResult fsResult9 = fsResult8 + this.DeserializeMember<Font>(data, (System.Type) null, "font", out font);
      model.font = font;
      int fontSize = model.fontSize;
      fsResult fsResult10 = fsResult9 + this.DeserializeMember<int>(data, (System.Type) null, "fontSize", out fontSize);
      model.fontSize = fontSize;
      FontStyle fontStyle = model.fontStyle;
      fsResult fsResult11 = fsResult10 + this.DeserializeMember<FontStyle>(data, (System.Type) null, "fontStyle", out fontStyle);
      model.fontStyle = fontStyle;
      GUIStyleState hover = model.hover;
      fsResult fsResult12 = fsResult11 + this.DeserializeMember<GUIStyleState>(data, (System.Type) null, "hover", out hover);
      model.hover = hover;
      ImagePosition imagePosition = model.imagePosition;
      fsResult fsResult13 = fsResult12 + this.DeserializeMember<ImagePosition>(data, (System.Type) null, "imagePosition", out imagePosition);
      model.imagePosition = imagePosition;
      RectOffset margin = model.margin;
      fsResult fsResult14 = fsResult13 + this.DeserializeMember<RectOffset>(data, (System.Type) null, "margin", out margin);
      model.margin = margin;
      string name = model.name;
      fsResult fsResult15 = fsResult14 + this.DeserializeMember<string>(data, (System.Type) null, "name", out name);
      model.name = name;
      GUIStyleState normal = model.normal;
      fsResult fsResult16 = fsResult15 + this.DeserializeMember<GUIStyleState>(data, (System.Type) null, "normal", out normal);
      model.normal = normal;
      GUIStyleState onActive = model.onActive;
      fsResult fsResult17 = fsResult16 + this.DeserializeMember<GUIStyleState>(data, (System.Type) null, "onActive", out onActive);
      model.onActive = onActive;
      GUIStyleState onFocused = model.onFocused;
      fsResult fsResult18 = fsResult17 + this.DeserializeMember<GUIStyleState>(data, (System.Type) null, "onFocused", out onFocused);
      model.onFocused = onFocused;
      GUIStyleState onHover = model.onHover;
      fsResult fsResult19 = fsResult18 + this.DeserializeMember<GUIStyleState>(data, (System.Type) null, "onHover", out onHover);
      model.onHover = onHover;
      GUIStyleState onNormal = model.onNormal;
      fsResult fsResult20 = fsResult19 + this.DeserializeMember<GUIStyleState>(data, (System.Type) null, "onNormal", out onNormal);
      model.onNormal = onNormal;
      RectOffset overflow = model.overflow;
      fsResult fsResult21 = fsResult20 + this.DeserializeMember<RectOffset>(data, (System.Type) null, "overflow", out overflow);
      model.overflow = overflow;
      RectOffset padding = model.padding;
      fsResult fsResult22 = fsResult21 + this.DeserializeMember<RectOffset>(data, (System.Type) null, "padding", out padding);
      model.padding = padding;
      bool richText = model.richText;
      fsResult fsResult23 = fsResult22 + this.DeserializeMember<bool>(data, (System.Type) null, "richText", out richText);
      model.richText = richText;
      bool stretchHeight = model.stretchHeight;
      fsResult fsResult24 = fsResult23 + this.DeserializeMember<bool>(data, (System.Type) null, "stretchHeight", out stretchHeight);
      model.stretchHeight = stretchHeight;
      bool stretchWidth = model.stretchWidth;
      fsResult fsResult25 = fsResult24 + this.DeserializeMember<bool>(data, (System.Type) null, "stretchWidth", out stretchWidth);
      model.stretchWidth = stretchWidth;
      bool wordWrap = model.wordWrap;
      fsResult fsResult26 = fsResult25 + this.DeserializeMember<bool>(data, (System.Type) null, "wordWrap", out wordWrap);
      model.wordWrap = wordWrap;
      return fsResult26;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new GUIStyle();
    }
  }
}
