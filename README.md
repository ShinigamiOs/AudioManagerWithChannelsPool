# AudioManager With Channels Pool 🎧

Gestor de audio modular y eficiente para Unity, con sistema de canales reutilizables, control de superposición y soporte para reproducción concurrente o controlada.

Realizado en Unity 6.0, verificacion de compatibilidad con otras versiones pendiente.

---

## 🚀 Características principales

- Reproducción de sonidos por nombre o ID
- Reproducción con superposición (`PlayOverlapping`) o única (`Play`)
-  Pool de canales 
-  Plug & Play: solo necesitas un `GameObject` con dos componentes

---

## 🛠️ Instalación

1. Clona o descarga este repositorio en tu proyecto de Unity.


---

## 🧩 Configuración rápida
1. Crea un nuevo `GameObject` en la escena. Nómbralo por ejemplo: `SFXManager`.
2. Agrega los siguientes **componentes** al GameObject:
   - `AudioLibrary` → contiene la lista de audios y sus configuraciones.
   - `AudioManager` → gestiona la reproducción usando un sistema de canales.

3. En el componente `AudioLibrary`:
   - Agrega tantos `AudioEntry` como necesites.
   - Asigna:
     - `Nombre`
     - `AudioClip`
     -  pitch, loop, etc.
  

4. En el componente `AudioManager`:
   - Ajusta la configuración:
     - `ManagerName`: Nombre independiente del manager (si hay varios managers nombra cada uno diferente)
     Configuracion de canal:
     - `ChannelCount`: numero de canales de la pool.
     - `MasterVolume`: volumen de los clips de audio.
     - `StopOnMute`: Cuando se silencian los audios deberian detenrse o pausarse? (los SFX se deberian detener, la musica pausar)
     Configuracion de UI (no es necesario pero recomendable si tienes un menu de opciones):
     - `SlideUI`: Slider que controla el volumen de este manager.
     - `SliderImageFill`: La imagen de relleno del SLider.
     - `SliderHanddlerImage`: La imagen del handdler del slider.
     - `ToggleUI`: El Toggle de la ui que representa el Mute.
     - `SliderMutedColor`: El color del que sera el slider si esta Muted.
Recomendaciones:

- Divide los audios por area y usa un manager para cada area, para que sea mas simple utilizarlos, ejemplo:
- SFXmanager(maneja clips de efectos de sonido)
- UIsoundManager(Maneja clips de la ui)
- MusicManager(Maneja clips de la musica)


---

## 🎮 Uso en código

### Reproducir un sonido por nombre:
```csharp
audioManager.Play("Explosion");
```

### Reproducir un sonido con superposición:
```csharp
audioManager.PlayOverlapping("Footstep");
```

### Detener un sonido:
```csharp
audioManager.Stop("Explosion");
```

---

## 📌 Notas importantes

- `Play`: Si el audio ya se está reproduciendo, lo reinicia en el mismo canal.
- `PlayOverlapping`: Reproduce múltiples instancias del mismo sonido si hay canales disponibles.

---


## 🪦 Licencia

Este proyecto es de código abierto bajo la licencia GNU.
