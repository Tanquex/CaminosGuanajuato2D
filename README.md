# 🔔 Caminos de Guanajuato 2D: Las Locas Aventuras en Dolores Hidalgo

[![Unity Version](https://img.shields.io/badge/Unity-6000.0%2B-black?logo=unity)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20Windows%20%7C%20WebGL-brightgreen.svg)]()
[![Branch develop](https://img.shields.io/badge/Branch-develop-orange.svg)]()
[![Branch main](https://img.shields.io/badge/Branch-main-success.svg)]()

> **Caminos de Guanajuato 2D** es un videojuego de acción y defensa táctica (Tower Defense activo con control directo de personaje) ambientado en la cuna de la Independencia de México: la emblemática ciudad colonial de **Dolores Hidalgo, Guanajuato**, en 1810.

---

## 📖 Índice

- [El Juego](#-el-juego)
  - [Sinopsis y Lore](#sinopsis-y-lore)
  - [Objetivo Principal](#objetivo-principal)
  - [Mecánicas Clave](#mecánicas-clave)
- [🎮 Controles del Juego](#-controles-del-juego)
- [🛡️ Elementos y Unidades](#️-elementos-y-unidades)
  - [Protagonista](#protagonista)
  - [Defensas Populares](#defensas-populares)
  - [Enemigos](#enemigos)
- [📂 Estructura del Proyecto](#-estructura-del-proyecto)
- [🚀 Pasos de Instalación y Ejecución](#-pasos-de-instalación-y-ejecución)
  - [Requisitos Previos](#requisitos-previos)
  - [Clonar el Repositorio](#1-clonar-el-repositorio)
  - [Abrir en Unity Hub](#2-abrir-en-unity-hub)
  - [Ejecutar el Juego](#3-ejecutar-el-juego)
- [🛠️ Arquitectura y Tecnologías](#️-arquitectura-y-tecnologías)
- [👥 Créditos y Equipo](#-créditos-y-equipo)

---

## 🎯 El Juego

### Sinopsis y Lore
Es la noche previa al levantamiento en Dolores Hidalgo. Una misteriosa horda de **Invasores Nocturnos** intenta sabotear el movimiento libertario asaltando el atrio de la Parroquia de Nuestra Señora de los Dolores para apagar la **Antorcha Libertaria** antes de que repiquen las campanas de libertad.

Encarnas a **Guanajicho**, un valiente insurgente armado con su tradicional honda de cuero, espada virreinal y escudo, quien junto a los habitantes del pueblo organizará barricadas coloniales y honderos para resistir las oleadas invasoras.

### Objetivo Principal
* **Sobrevivir a las 5 oleadas consecutivas** de invasores que ingresan por las 4 calles cardinales de la plaza (Norte, Sur, Este y Oeste).
* **Impedir a toda costa que la salud de la Antorcha Libertaria llegue a 0**. Si la antorcha se extingue, el juego termina en derrota.
* **Administrar los Recursos Cívicos**: recolectar el botín virreinal dejado por los invasores para colocar barricadas y desplegar honderos aliados en la plaza.

### Mecánicas Clave
* **Control Directo 2D**: Movimiento fluido en 8 direcciones con aceleración de sprint y animación integrada (Idle, Run y Attack).
* **Combate con Honda Activa**: Apunta con el ratón o en dirección de movimiento para disparar proyectiles de piedra contra los invasores.
* **Defensa Táctica en Cuadrícula (Grid Placement)**: Sistema de construcción en tiempo real con vista previa fantasma de color para validar posiciones de barricadas y honderos.
* **Economía de Botín**: Probabilidad de soltar botines cívicos al derrotar enemigos con animación flotante y recolección al contacto.
* **Llama Libertaria Crepitante**: Animación procedural por ruido Perlin que modula la escala y el color cálido de la antorcha en tiempo real.
* **Audio de Época Procedural**: Generador acústico 16-bit integrado en C# que sintetiza campanadas de iglesia colonial, disparos de honda, impactos secos y tintineos de monedas sin necesidad de librerías externas.
* **Cámara Dinámica con Zoom**: Seguimiento suave con anticipación de movimiento, límites automáticos de encuadre en la plaza y zoom ortográfico con la rueda del ratón.

---

## 🎮 Controles del Juego

| Acción | Teclado / Ratón | Descripción |
| :--- | :--- | :--- |
| **Moverse** | `W, A, S, D` o `Flechas` | Desplaza a Guanajicho en 8 direcciones por la plaza colonial. |
| **Correr (Sprint)** | `Shift Izquierdo` / `Shift Derecho` | Aumenta la velocidad de movimiento un 50%. |
| **Disparo de Honda** | `Clic Izquierdo`, `Espacio` o `Tecla J` | Lanza una piedra veloz en dirección al cursor o frente. |
| **Modo Construcción** | `Tecla B` o Clic en botón HUD | Activa/desactiva el modo de colocación de defensas. |
| **Colocar Defensa** | `Clic Izquierdo` (Modo B) o `Tecla E` | Coloca la defensa seleccionada en la cuadrícula o frente a ti. |
| **Cambiar Defensa** | `Tecla 1` (Barricada) / `Tecla 2` (Hondero) | Alterna entre las opciones defensivas del catálogo. |
| **Forzar/Avanzar Oleada** | `Tecla G` | Salta la cuenta regresiva e inicia la siguiente oleada de inmediato. |
| **Zoom de Cámara** | `Rueda del Ratón (Scroll)` | Acerca o aleja la vista panorámica del atrio colonial. |

---

## 🛡️ Elementos y Unidades

### Protagonista
* **Guanajicho**:
  * Salud y resistencia equilibrada.
  * Armado con espada, escudo y honda insurgente.
  * Recoge botines cívicos caídos al aproximarse a ellos.

### Defensas Populares
* **Barricada Colonial** (*Costo: 25 Puntos Cívicos*):
  * Estructura de madera maciza y herrería colonial.
  * Bloquea físicamente el avance de los invasores y absorbe daño.
* **Hondero Insurgente** (*Costo: 50 Puntos Cívicos*):
  * Aliado patriota que vigila un radio de acción.
  * Detecta invasores automáticamente y les dispara proyectiles guiados de piedra.

### Enemigos
* **Invasor Nocturno**:
  * Avanza siguiendo rutas estratégicas hacia la Antorcha Libertaria.
  * Si se topa con una barricada, se detiene a atacarla hasta destruirla.
  * Al ser derrotado otorga puntos cívicos y puede soltar botines en el suelo.

---

## 📂 Estructura del Proyecto

```text
LocasAventurasDH/
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity            # Menú principal con opciones interactivas
│   │   ├── NivelPruebas_Defensa.unity# Nivel 1: Atrio Parroquial de Dolores Hidalgo
│   │   └── SampleScene.unity         # Escena complementaria de desarrollo
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── AntorchaLibertaria.cs   # Lógica y animación de la flama objetivo
│   │   │   ├── InvasorNocturno.cs      # IA de movimiento, ataque y derrota de enemigos
│   │   │   ├── OleadasSpawner.cs       # Gestor multidireccional de oleadas (4 puntos)
│   │   │   ├── RecursosCivicosManager.cs # Sistema de puntos y economía
│   │   │   ├── RecursoCivicoPickup.cs  # Botines flotantes recolectables
│   │   │   └── AudioColonialManager.cs # Sintetizador procedural de audio colonial
│   │   ├── Defense/
│   │   │   ├── ColocadorDefensas.cs    # Sistema de grid, validación y colocación
│   │   │   ├── DefensaElemento.cs      # Salud y comportamiento de barricadas y torres
│   │   │   └── ProyectilDefensa.cs     # Balística y daño de piedras
│   │   ├── Player/
│   │   │   ├── GuanajichoController.cs # Movimiento, límites de mapa y disparo
│   │   │   └── CameraFollow2D.cs       # Cámara cinemática, zoom y bounds
│   │   └── UI/
│   │       ├── MainMenuManager.cs      # Lógica de navegación del menú principal
│   │       └── HUDDefensaUI.cs         # Banners, avisos, temporizador y GameOver
│   ├── Prefabs/                        # Prefabs de Guanajicho, defensas e invasores
│   ├── Sprites/                        # Fondos pixel art del atrio y texturas UI
│   ├── Hero Knight - Pixel Art/        # Animaciones y spritesheets del protagonista
│   └── Audio/                          # Directorios de BGM y SFX de reserva
└── ProjectSettings/                    # Configuración de Quality, Tags, Physics2D e Input
```

---

## 🚀 Pasos de Instalación y Ejecución

### Requisitos Previos
1. **[Unity Hub](https://unity.com/download)** instalado.
2. **Unity Editor versión `6000.0.x` (Unity 6)** o superior instalado mediante Unity Hub.
3. **Git** instalado en el sistema.

### 1. Clonar el Repositorio
Abre tu terminal (PowerShell, Git Bash o Command Prompt) y clona el proyecto:

```bash
git clone https://github.com/Tanquex/CaminosGuanajuato2D.git
```

Entra al directorio del proyecto:
```bash
cd CaminosGuanajuato2D
```

*(Opcional) Si deseas probar la última rama de desarrollo:*
```bash
git checkout develop
```

### 2. Abrir en Unity Hub
1. Abre **Unity Hub**.
2. Dirígete a la pestaña **Projects**.
3. Haz clic en el botón **Add** (o *Add project from disk*).
4. Navega hasta la carpeta donde clonaste el repositorio y selecciónala.
5. Si te pide confirmar la versión de Unity Editor, selecciona la versión **Unity 6 (6000.x)** disponible en tu equipo.
6. Espera a que Unity compile y cargue los paquetes de scripts y assets (la primera vez puede tomar un par de minutos).

### 3. Ejecutar el Juego
1. En la ventana **Project** de Unity, dirígete a:
   `Assets/Scenes/MainMenu.unity`
2. Haz doble clic sobre **MainMenu.unity** para abrir la escena del Menú Principal.
3. En la barra superior de Unity, presiona el botón **Play (▶️)**.
4. Haz clic en **Jugar** para iniciar la defensa del Atrio en el **Nivel 1**.

> [!TIP]
> Para una experiencia visual óptima, en la pestaña **Game** de Unity cambia el selector de resolución de **Free Aspect** a **16:9** o **1920x1080**.

---

## 🛠️ Arquitectura y Tecnologías

* **Motor**: Unity 2026 / Unity 6 (2D Feature Set).
* **Render Pipeline**: Universal Render Pipeline (URP 2D) con iluminación 2D Global y materiales de sprites optimizados.
* **Física**: Unity Physics2D (`Rigidbody2D` cinemático/dinámico, `CircleCollider2D`, `BoxCollider2D`, detección `Continuous`).
* **Entrada**: Soporte dual universal para **Unity Legacy Input Manager** y **New Input System** (WASD, flechas, ratón y teclas de acceso directo).
* **Compilación**: C# .NET Standard 2.1 con arquitectura desacoplada orientada a eventos (`Action<T>`).

---

## 👥 Créditos y Equipo

* **Diego**: Arquitectura del Core, sistema de oleadas multidireccionales, mecánicas de defensa y configuración del repositorio.
* **Sama**: Controlador de Guanajicho, diseño del Menú Principal, balanceo de gameplay e integración artística.

---

*Caminos de Guanajuato 2D © 2026 — Proyecto histórico y lúdico para preservar la memoria de Dolores Hidalgo, Cuna de la Independencia Nacional.*
