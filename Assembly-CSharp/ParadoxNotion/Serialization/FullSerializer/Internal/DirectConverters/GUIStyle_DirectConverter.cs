using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters;

public class GUIStyle_DirectConverter : fsDirectConverter<GUIStyle> {
	protected override fsResult DoSerialize(GUIStyle model, Dictionary<string, fsData> serialized) {
		return fsResult.Success + SerializeMember(serialized, null, "active", model.active) +
		       SerializeMember(serialized, null, "alignment", model.alignment) +
		       SerializeMember(serialized, null, "border", model.border) +
		       SerializeMember(serialized, null, "clipping", model.clipping) +
		       SerializeMember(serialized, null, "contentOffset", model.contentOffset) +
		       SerializeMember(serialized, null, "fixedHeight", model.fixedHeight) +
		       SerializeMember(serialized, null, "fixedWidth", model.fixedWidth) +
		       SerializeMember(serialized, null, "focused", model.focused) +
		       SerializeMember(serialized, null, "font", model.font) +
		       SerializeMember(serialized, null, "fontSize", model.fontSize) +
		       SerializeMember(serialized, null, "fontStyle", model.fontStyle) +
		       SerializeMember(serialized, null, "hover", model.hover) +
		       SerializeMember(serialized, null, "imagePosition", model.imagePosition) +
		       SerializeMember(serialized, null, "margin", model.margin) +
		       SerializeMember(serialized, null, "name", model.name) +
		       SerializeMember(serialized, null, "normal", model.normal) +
		       SerializeMember(serialized, null, "onActive", model.onActive) +
		       SerializeMember(serialized, null, "onFocused", model.onFocused) +
		       SerializeMember(serialized, null, "onHover", model.onHover) +
		       SerializeMember(serialized, null, "onNormal", model.onNormal) +
		       SerializeMember(serialized, null, "overflow", model.overflow) +
		       SerializeMember(serialized, null, "padding", model.padding) +
		       SerializeMember(serialized, null, "richText", model.richText) +
		       SerializeMember(serialized, null, "stretchHeight", model.stretchHeight) +
		       SerializeMember(serialized, null, "stretchWidth", model.stretchWidth) +
		       SerializeMember(serialized, null, "wordWrap", model.wordWrap);
	}

	protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GUIStyle model) {
		var success = fsResult.Success;
		var active = model.active;
		var fsResult1 = success + DeserializeMember(data, null, "active", out active);
		model.active = active;
		var alignment = model.alignment;
		var fsResult2 = fsResult1 + DeserializeMember(data, null, "alignment", out alignment);
		model.alignment = alignment;
		var border = model.border;
		var fsResult3 = fsResult2 + DeserializeMember(data, null, "border", out border);
		model.border = border;
		var clipping = model.clipping;
		var fsResult4 = fsResult3 + DeserializeMember(data, null, "clipping", out clipping);
		model.clipping = clipping;
		var contentOffset = model.contentOffset;
		var fsResult5 = fsResult4 + DeserializeMember(data, null, "contentOffset", out contentOffset);
		model.contentOffset = contentOffset;
		var fixedHeight = model.fixedHeight;
		var fsResult6 = fsResult5 + DeserializeMember(data, null, "fixedHeight", out fixedHeight);
		model.fixedHeight = fixedHeight;
		var fixedWidth = model.fixedWidth;
		var fsResult7 = fsResult6 + DeserializeMember(data, null, "fixedWidth", out fixedWidth);
		model.fixedWidth = fixedWidth;
		var focused = model.focused;
		var fsResult8 = fsResult7 + DeserializeMember(data, null, "focused", out focused);
		model.focused = focused;
		var font = model.font;
		var fsResult9 = fsResult8 + DeserializeMember(data, null, "font", out font);
		model.font = font;
		var fontSize = model.fontSize;
		var fsResult10 = fsResult9 + DeserializeMember(data, null, "fontSize", out fontSize);
		model.fontSize = fontSize;
		var fontStyle = model.fontStyle;
		var fsResult11 = fsResult10 + DeserializeMember(data, null, "fontStyle", out fontStyle);
		model.fontStyle = fontStyle;
		var hover = model.hover;
		var fsResult12 = fsResult11 + DeserializeMember(data, null, "hover", out hover);
		model.hover = hover;
		var imagePosition = model.imagePosition;
		var fsResult13 = fsResult12 + DeserializeMember(data, null, "imagePosition", out imagePosition);
		model.imagePosition = imagePosition;
		var margin = model.margin;
		var fsResult14 = fsResult13 + DeserializeMember(data, null, "margin", out margin);
		model.margin = margin;
		var name = model.name;
		var fsResult15 = fsResult14 + DeserializeMember(data, null, "name", out name);
		model.name = name;
		var normal = model.normal;
		var fsResult16 = fsResult15 + DeserializeMember(data, null, "normal", out normal);
		model.normal = normal;
		var onActive = model.onActive;
		var fsResult17 = fsResult16 + DeserializeMember(data, null, "onActive", out onActive);
		model.onActive = onActive;
		var onFocused = model.onFocused;
		var fsResult18 = fsResult17 + DeserializeMember(data, null, "onFocused", out onFocused);
		model.onFocused = onFocused;
		var onHover = model.onHover;
		var fsResult19 = fsResult18 + DeserializeMember(data, null, "onHover", out onHover);
		model.onHover = onHover;
		var onNormal = model.onNormal;
		var fsResult20 = fsResult19 + DeserializeMember(data, null, "onNormal", out onNormal);
		model.onNormal = onNormal;
		var overflow = model.overflow;
		var fsResult21 = fsResult20 + DeserializeMember(data, null, "overflow", out overflow);
		model.overflow = overflow;
		var padding = model.padding;
		var fsResult22 = fsResult21 + DeserializeMember(data, null, "padding", out padding);
		model.padding = padding;
		var richText = model.richText;
		var fsResult23 = fsResult22 + DeserializeMember(data, null, "richText", out richText);
		model.richText = richText;
		var stretchHeight = model.stretchHeight;
		var fsResult24 = fsResult23 + DeserializeMember(data, null, "stretchHeight", out stretchHeight);
		model.stretchHeight = stretchHeight;
		var stretchWidth = model.stretchWidth;
		var fsResult25 = fsResult24 + DeserializeMember(data, null, "stretchWidth", out stretchWidth);
		model.stretchWidth = stretchWidth;
		var wordWrap = model.wordWrap;
		var fsResult26 = fsResult25 + DeserializeMember(data, null, "wordWrap", out wordWrap);
		model.wordWrap = wordWrap;
		return fsResult26;
	}

	public override object CreateInstance(fsData data, Type storageType) {
		return new GUIStyle();
	}
}