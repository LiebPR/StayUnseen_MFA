# Game Systems

Los **Gameplay Systems** gestionan las acciones del jugador, las interacciones con el escenario y la progresión de la misión.

Estos sistemas se mantienen estrictamente separados de la lógica de IA. La IA consume información generada por el gameplay (por ejemplo, eventos de sonido), pero no controla ni modifica estos sistemas directamente.

## 1. Player Architecture
### `PlayerController.cs`
Orquestador principal del jugador. Se encarga de gestionar el ciclo de vida y la comunicación entre los diferentes subsistemas del jugador.

### `PlayerInstaller.cs`
Encargado de la inyección de dependencias e inicialización ordenada de los sistemas del jugador antes de ceder el control al orquestador.

## 2. Core Game Logics
### `StateMachine.cs`
Sistema central de máquina de estados agnóstico a Unity. Su única responsabilidad es gestionar el cambio determinista de estados bajo petición.

### `MovementSystem.cs`
Sistema de movimiento escrito en C# puro. Aplica las matemáticas de simulación y traslación de forma homogénea tanto para el jugador como para las entidades enemigas.

### `ITickable.cs`
Interfaz que expone el método `Tick(float deltaTime)` para actualizar la lógica interna de los sistemas que requieren un refresco por frame.

### `IFixedTickable.cs`
Interfaz que expone el método `FixedTick(float fixedDeltaTime)` para actualizar sistemas basados en física o simulación determinista (como `MovementSystem`) de forma segura.

### `InteractionSystem.cs`
Sistema central que implementa la interfaz IInteractable para definir el comportamiento de un objeto interactuable ante la interacción del jugador. 
Es un sistema que puede ser usado por cualquier entidad en caso de ser necesario. 

### `ThrowSystem.cs`
Sistema encargado de aplicar una fuerza x a un objeto que tengas en el `HandInventory.cs`

### `HandInventory.cs`
Sistema encargado de definir un espacio para el jugador, que envia señales en caso de estar vacío o lleno. 
- **Lleno**: Interactuas con un objeto recogible, informa de que esta lleno.
- **Vacío**: Interactuas con un objeto recogible, informa de que esta vacío.

Esto nos permite centrar un comportamiento en una clase para que lance los eventos tras una interactuación con un objeto recogible, entonces el juego se pregunta como esta el inventario y el resto de sistemas reaccionan a esto, haciendo que suelten el objeto específico o lo equipen directamente. 
---

## 3. Decisiones Arquitectónicas (FAQ)

### Sistema de Movimiento (`MovementSystem.cs`)
* **¿Por qué usar un sistema de movimiento centralizado y compartido?**  
  Centraliza el manejo matemático del desplazamiento. Si se modifica o ajusta la física/lógica de movimiento del juego, los cambios se aplican automáticamente a todas las entidades (jugador y enemigos) sin duplicar código.

* **¿Por qué el sistema de movimiento no depende del ciclo de vida de Unity (`MonoBehaviour`)?**  
  Mantiene el comportamiento agnóstico, previene *race conditions* garantizando un orden de actualización explícito y optimiza el rendimiento al evitar llamadas constantes a través del puente interop C++/C# de Unity.

* **¿Qué ocurre si un enemigo requiere un comportamiento de movimiento diferente?**  
  Se extiende la funcionalidad mediante herencia o composición sobre el sistema base, reutilizando los cálculos comunes e implementando únicamente la variación específica.

### Interfaces del Ciclo de Vida (`ITickable`, `IFixedTickable`)
* **¿Por qué implementar un ciclo de vida propio en lugar de usar `Update`/`FixedUpdate`?**  
  Evita la proliferación de componentes `MonoBehaviour` y permite mantener la lógica central en C# puro. Esto habilita el uso de **Unit Testing** aislado y delega el control del orden de ejecución a un orquestador centralizado.

### Instalador del Jugador (`PlayerInstaller.cs`)
* **¿Por qué usar un Instalador en lugar de inicializar todo dentro de `PlayerController.cs`?**  
  Encapsula la construcción y configuración de dependencias. Si en el futuro se requieren variantes del jugador, basta con crear diferentes configuraciones de instaladores (por ejemplo, mediante `ScriptableObjects`) e inyectarlas al `PlayerController` sin modificar su lógica interna.
