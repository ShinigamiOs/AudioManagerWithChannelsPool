using UnityEngine;

/// <summary>
/// Entrada de audio, sistema para guardar informacion de un audio
/// </summary>
[System.Serializable]
public class AudioEntry 
{
    [Tooltip("Nombre para identificar el audio.")]
    public string name;

    [Tooltip("Clip de audio. Arrastra un archivo .wav o .mp3 aquí.")]
    public AudioClip clip;

    [Tooltip("Volumen del audio, 0(silencio) - 1(maximo)")]
    [Range(0f, 1f)] public float volume = 1f;

    [Tooltip("Pitch del audio, 1 = normal, <1 más grave, >1 más agudo")]
    [Range(-3f, 3f)] public float pitch = 1f;

    [Tooltip("Si está activado, el audio se repetirá indefinidamente.")]
    public bool loop = false;

    [Tooltip("Si está activado, el audio se reproducirá automáticamente al iniciar la escena.")]
    public bool playOnAwake = false;


    [System.NonSerialized] public AudioSource defaultSource;
    [System.NonSerialized] public GameObject defaultObject;  
}