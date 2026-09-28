# STAY SEEN

**STAY SEEN** is a small stealth prototype developed in Unity 6.

The player takes control of a warrior who has been captured and imprisoned in a dungeon. Upon awakening, he discovers that he is unarmed and must find a way to escape while avoiding detection by the creatures patrolling the area.

The game is centered around **enemy AI**, with a particular focus on their **perception and behavior**. For more information about the AI systems and their architecture, see [`AIArchitecture.md`](AIArchitecture.md).

### Game Loop

```mermaid
flowchart TD
    subgraph S1 ["1. Gameplay State: Unaware (Explore) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"]
        direction LR
        START([START]) --> Explore[Explore] --> Avoid[Avoid Enemies]
    end

    subgraph S2 ["2. Encounter & Detection Phase &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"]
        Detected{Detected?}
    end

    subgraph S3 ["3. Progress & Success Path &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"]
        Progress[Progress] --> ExitFound{Exit Found?}
    end

    subgraph S4 ["4. Engagement & Escape Path &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"]
        RunHide[Run or Hide] --> LostPlayer{Lost the Player?}
        LostPlayer -- NO --> RunHide
    end

    EXIT([EXIT])

    %% Conexiones
    Avoid --> Detected
    Detected -- NO --> Progress
    ExitFound -- YES --> EXIT
    ExitFound -- NO --> Explore
    Detected -- YES --> RunHide
    LostPlayer -- YES --> Explore
```
