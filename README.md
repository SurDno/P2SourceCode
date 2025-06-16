# Pathologic 2 - Decompiled Source Code

This repository contains the unmodded decompiled source code of the core gameplay assemblies of the latest version of **Pathologic 2**, compatible with latest .NET Framework 4.8.1 and C# 13. 

The following core game assemblies have been decompiled (in the order of .csproj compilation):

- **Engine.Common** 
- **VirtualMachine.Common** 
- **VirtualMachine** 
- **Assembly-CSharp**

UnityEngine assemblies and third-party plugins that are not a part of the game's own codebase remain as compiled `.dll` files in the `Managed/` folder and may be referenced as dependencies in new code.

The decompilation process was initially done with DotPeek, with occasional snippets being taken from dnSpy implementations where DotPeek failed, with manual fixes of the remaining compilation issues.
To make the code shorter and more concise, redundant qualifiers have been automatically removed with Rider, but other than that, the code mostly stayed exactly as it is. 

The provided codebase can be compiled into .dll-s to be used with in-game assembly.

## Potential Uses
- Analysing and referencing how the game works internally.
- Providing access to original game functions for mod developers, eliminating the need to decompile assemblies manually.
- Feeding parts of game logic and implementation details into LLMs (such as Claude) for further analysis & mod development assistance.