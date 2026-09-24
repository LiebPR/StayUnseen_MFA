# STAY SEEN

**STAY SEEN** is a small stealth prototype developed in Unity 6.

The player takes control of a warrior who has been captured and imprisoned in a dungeon. Upon awakening, he discovers that he is unarmed and must find a way to escape while avoiding detection by the creatures patrolling the area.

The game is centered around **enemy AI**, with a particular focus on their **perception and behavior**. For more information about the AI systems and their architecture, see [`AIArchitecture.md`](AIArchitecture.md).

### Game Loop

```mermaid
flowchart BT
    n1([START]) --> n2[Explore]
    n2 --> n3[Avoid Enemies]
    n3 --> n4{Detected?}
    n4 --> n5[/NO/]
    n4 --> n6[\YES\]
    n5 --> n7[Progress]
    n6 --> n8[Run or Hide]
    n7 --> n9{Exit Found?}
    n8 --> n10{Lost the Player?}
    n9 --> n11[/NO/]
    n9 --> n12[\YES\]
    n11 --> n2
    n10 --> n13[/NO/]
    n10 --> n14[\YES\]
    n12 --> n15([EXIT])
```
