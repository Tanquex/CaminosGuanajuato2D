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
  - [Desglose de Misiones y Niveles](#desglose-de-misiones-y-niveles)
  - [Mecánicas Clave](#mecánicas-clave)
- [🎮 Controles del Juego](#-controles-del-juego)
  - [Controles Nivel 1: Defensa del Atrio](#controles-nivel-1-defensa-del-atrio)
  - [Controles Nivel 2: Desafío de Nieves](#controles-nivel-2-desafío-de-nieves)
- [🛡️ Elementos, Unidades y Personajes](#️-elementos-unidades-y-personajes)
  - [Elenco Principal](#elenco-principal)
  - [Defensas Populares (Nivel 1)](#defensas-populares-nivel-1)
  - [Enemigos Invasores (Nivel 1)](#enemigos-invasores-nivel-1)
  - [Sabores Exóticos de Nieve (Nivel 2)](#sabores-exóticos-de-nieve-nivel-2)
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
El jugador asume el rol de **Guanajicho**, un joven cronista y aprendiz de alfarero en Dolores Hidalgo C.I.N. en septiembre de 1810. Para apoyar la causa independentista y preservar las memorias vivas de Guanajuato frente al olvido, debe resolver las misiones locales del pueblo, ganar el respaldo popular de Don Chon y defender el atrio parroquial para obtener el **Salvoconducto Insurgente** que habilitará la ruta norte hacia **San Felipe**.

### Desglose de Misiones y Niveles
* **Nivel 1: La Defensa del Atrio Parroquial** (*Tower Defense 2D Activo*):
  * Sobrevivir a 5 oleadas de invasores nocturnos que atacan desde los 4 puntos cardinales de la plaza.
  * Proteger la Antorcha Libertaria y colocar barricadas y honderos acumulando recursos cívicos.
  * Recompensa: **Emblema de la Libertad (Medalla 1)**.
* **Nivel 2: El Desafío de las Nieves Tradicionales** (*Gestión Culinaria y QTE Contra Reloj*):
  * Asistir a **Don Chon (El Nevero Mayor)** en el quiosco del jardín principal.
  * Atender a 8 comensales preparando nieves exóticas (*Garambullo, Borrachita, Queso con Fresa, Aguacate y Limón virreinal*) en barquillos de trigo o vasos de barro.
  * Raspar hielo en la barrica tradicional y servir los pedidos antes de agotar la paciencia de los clientes.
  * Recompensa: **Cuchara Dorada Artesanal (Medalla 2)**.
* **Overworld Interactivo de Guanajuato**:
  * Cartografía estilizada que conecta los municipios del estado.
  * Dolores Hidalgo desbloqueado desde el inicio; San Felipe custodiado hasta completar 3/3 misiones.

---

## 🎮 Controles del Juego

### Controles Nivel 1: Defensa del Atrio
| Acción | Teclado / Ratón | Descripción |
| :--- | :--- | :--- |
| **Moverse** | `W, A, S, D` o `Flechas` | Desplaza a Guanajicho en 8 direcciones por la plaza colonial. |
| **Correr (Sprint)** | `Shift Izquierdo` / `Shift Derecho` | Aumenta la velocidad de movimiento un 50%. |
| **Disparo de Honda** | `Clic Izquierdo`, `Espacio` o `Tecla J` | Lanza una piedra veloz en dirección al cursor o frente. |
| **Modo Construcción** | `Tecla B` o Clic en botón HUD | Activa/desactiva el modo de colocación de defensas. |
| **Colocar Defensa** | `Clic Izquierdo` (Modo B) o `Tecla E` | Coloca la defensa seleccionada en la cuadrícula o frente a ti. |
| **Cambiar Defensa** | `Tecla 1` (Barricada) / `Tecla 2` (Hondero) | Alterna entre las opciones defensivas del catálogo. |
| **Avivar Antorcha** | `Tecla E` (Cerca de la antorcha) | Gasta 10 puntos cívicos para restaurar +25 HP a la flama. |
| **Forzar Oleada** | `Tecla G` | Salta la cuenta regresiva e inicia la siguiente oleada de inmediato. |
| **Zoom de Cámara** | `Rueda del Ratón (Scroll)` | Acerca o aleja la vista panorámica del atrio colonial. |

### Controles Nivel 2: Desafío de Nieves
| Acción | Teclado / Ratón | Descripción |
| :--- | :--- | :--- |
| **Elegir Base** | `Tecla 1` (Barquillo) / `Tecla 2` (Vaso) | Selecciona el recipiente de la nieve en el mostrador. |
| **Raspar Hielo** | `Barra Espaciadora` (Pulsar/Mantener) | Bate y raspa el hielo artesanal en la garrafa de madera. |
| **Sabor Garambullo** | `Tecla Q` o Clic en botón | Vierte pulpa silvestre de garambullo (púrpura cactáceo). |
| **Sabor Borrachita**| `Tecla W` o Clic en botón | Vierte miel tradicional de agave y biznaga (ámbar dorado). |
| **Sabor Queso/Fresa**| `Tecla E` o Clic en botón | Combina cubos de queso fresco y fresa del Bajío (rosa crema). |
| **Sabor Aguacate**  | `Tecla R` o Clic en botón | Agrega pulpa exótica de aguacate (verde olivo). |
| **Sabor Limón**     | `Tecla T` o Clic en botón | Exprime jugo de limón virreinal fresco (verde lima). |
| **Servir Pedido**   | `Tecla Enter` / `Keypad Enter` | Entrega la nieve terminada al comensal en espera. |
| **Desechar / Tirar**| `Tecla Backspace` o `Delete` | Descarta la preparación actual si te equivocaste de receta. |

---

## 🛡️ Elementos, Unidades y Personajes

### Elenco Principal
* **Guanajicho**: Joven explorador y protagonista jugable; defiende el atrio con su honda y atiende la nevería con destreza.
* **Don Chon (El Nevero Mayor)**: PNJ y mentor culinario en el quiosco; enseña las recetas y califica la rapidez del jugador.
* **El Insurgente Guardián**: PNJ táctico en el atrio que guía durante la defensa nocturna.

### Sabores Exóticos de Nieve (Nivel 2)
1. **Garambullo**: Cactácea silvestre de la región semidesértica dolorense (#7E22CE).
2. **Borrachita**: Receta festiva con toque de miel y biznaga (#D97706).
3. **Queso con Fresa**: Clásica delicia del Bajío que mezcla queso artesanal y fresa dulce (#F43F5E).
4. **Aguacate**: Nieve exótica emblemática con pulpa cremosa (#65A30D).
5. **Limón Virreinal**: La receta clásica refrescante de los portales coloniales (#84CC16).

---

## 📂 Estructura del Proyecto

```text
CaminosGuanajuato2D/
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity                 # Menú principal interactivo
│   │   ├── Overworld_MapaGuanajuato.unity # Selector de municipios y misiones del estado
│   │   ├── NivelPruebas_Defensa.unity     # Nivel 1: Atrio Parroquial (Tower Defense)
│   │   ├── Nivel2_DesafioNieves.unity     # Nivel 2: Quiosco de Nieves (Gestión y QTE)
│   │   └── SampleScene.unity              # Escena complementaria
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── ProgresoJuegoManager.cs    # Persistencia PlayerPrefs, medallas y desbloqueos
│   │   │   ├── AntorchaLibertaria.cs      # Lógica y animación de la flama objetivo
│   │   │   ├── InvasorNocturno.cs         # IA de movimiento, ataque y botines de enemigos
│   │   │   ├── OleadasSpawner.cs          # Gestor multidireccional de oleadas (4 puntos)
│   │   │   ├── RecursosCivicosManager.cs  # Sistema de puntos y economía
│   │   │   ├── RecursoCivicoPickup.cs     # Botines flotantes recolectables
│   │   │   └── AudioColonialManager.cs    # Sintetizador procedural 16-bit de audio colonial
│   │   ├── Overworld/
│   │   │   ├── OverworldManager.cs        # Cartografía estatal y candado de San Felipe
│   │   │   └── SelectorMisionesUI.cs      # Selector de las 3 misiones de Dolores Hidalgo
│   │   ├── Nieves/
│   │   │   ├── NeveriaGameManager.cs      # Administrador de la jornada, cuota de clientes y medallas
│   │   │   ├── PedidoNieve.cs             # Modelo de recetas, recipientes y sabores exóticos
│   │   │   ├── MesaPreparacion.cs         # Mostrador de trabajo (base, hielo y sabores)
│   │   │   ├── ClienteTurista.cs          # IA de comensales y barra de paciencia
│   │   │   ├── DonChonConsejero.cs        # Mentoría, diálogos y reacciones de Don Chon
│   │   │   └── HUDNeveriaUI.cs            # Interfaz gráfica colonial de la nevería
│   │   ├── Defense/
│   │   │   ├── ColocadorDefensas.cs       # Sistema de cuadrícula y previsualización fantasma
│   │   │   ├── DefensaElemento.cs         # Salud y comportamiento de barricadas y honderos
│   │   │   └── ProyectilDefensa.cs        # Balística y daño de piedras
│   │   ├── Player/
│   │   │   ├── GuanajichoController.cs    # Movimiento 8 direcciones, sprint, límites y honda
│   │   │   └── CameraFollow2D.cs          # Cámara cinematográfica, zoom y encuadre
│   │   └── UI/
│   │       ├── MainMenuManager.cs         # Navegación del menú hacia el Overworld
│   │       └── HUDDefensaUI.cs            # Banners, telemetría y modales de Nivel 1
│   ├── Prefabs/                           # Prefabs de Guanajicho, defensas, comensales e invasores
│   ├── Sprites/                           # Fondos coloniales y texturas UI
│   ├── Hero Knight - Pixel Art/           # Spritesheets del protagonista
│   └── Audio/                             # BGM y SFX de reserva
└── ProjectSettings/                       # EditorBuildSettings, Input, Tags y Physics2D
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
