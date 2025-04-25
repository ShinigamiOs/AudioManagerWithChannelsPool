# AudioManager With Channels Pool 🎧

Gestor de audio modular y eficiente para Unity, con sistema de canales reutilizables, control de superposición y soporte para reproducción concurrente o controlada.

Realizado en Unity 6.0, verificacion de compatibilidad con otras versiones pendiente.

---

## 🚀 Características principales

- 🎛️ Reproducción de sonidos por nombre o ID
- 🔁 Reproducción con superposición (`PlayOverlapping`) o única (`Play`)
- 🧠 Pool de canales con control de límite dinámico
- 🔥 Precarga inicial de canales configurables
- 🚫 Soporte para modo estricto (no se crean canales si se supera el límite)
- 📡 Eventos de inicio, fin y parada de audio
- ✅ Plug & Play: solo necesitas un `GameObject` con dos componentes

---

## 🛠️ Instalación

1. Clona o descarga este repositorio en tu proyecto de Unity.
2. Crea un nuevo `GameObject` en la escena. Nómbralo por ejemplo: `AudioManager`.

---

## 🧩 Configuración rápida

1. Agrega los siguientes **componentes** al GameObject:
   - `AudioLibrary` → contiene la lista de audios y sus configuraciones.
   - `AudioManager` → gestiona la reproducción usando un sistema de canales.

2. En el componente `AudioLibrary`:
   - Agrega tantos `AudioEntry` como necesites.
   - Asigna:
     - `Nombre`
     - `AudioClip`
     - Volumen, pitch, loop, etc.
  

3. En el componente `AudioManager`:
   - Ajusta la configuración de canales:
     - `MaxChannels`: límite total de canales simultáneos.
     - `PrewarmChannels`: cuántos canales se crean desde el inicio.
     - `StrictLimit`: si está activo, no se permiten canales adicionales al límite.
     - 
Recomendaciones:

- Divide los audios por area y usa un manager para cada area, para que sea mas simple utilizarlos.
- SFX: pitch=1±0.1, Volumen:1

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
- Si se reproducen muchos sonidos rápidamente, el sistema crea canales adicionales si `StrictLimit` está desactivado, y luego los destruye automáticamente.

---


## 🪦 Licencia

Este proyecto es de código abierto bajo la licencia MIT.
