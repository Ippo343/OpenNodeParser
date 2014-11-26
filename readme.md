	Open Node Parser
    
This is a small utility class that can parse the ConfigNode format used by KSP and produce ConfigNode objects.

It is the analogous of ConfigNode.Load, except that method cannot be called outside an assembly running in KSP, while this method can be used in any application (e.g, to write unit tests that load .cfg files).

Distributed under a CC-0 license.