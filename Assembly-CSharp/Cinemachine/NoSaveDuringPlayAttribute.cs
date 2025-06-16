using System;
using UnityEngine;

namespace Cinemachine;

[AttributeUsage(AttributeTargets.Field)]
public sealed class NoSaveDuringPlayAttribute : PropertyAttribute { }