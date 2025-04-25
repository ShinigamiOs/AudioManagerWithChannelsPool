using UnityEngine;
/// <summary>
/// Gestion individual de canales de audio
/// </summary>
[System.Serializable]
public class AudioChannel
{
    public GameObject gameObject;
    public AudioSource source;
    public bool isTemporary;
    public string currentClip;
    public bool hasFinishedPlaying;

    public AudioChannel(Transform parent, string name, bool temporary)
    {
        gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent);
        source = gameObject.AddComponent<AudioSource>();
        isTemporary = temporary;
        hasFinishedPlaying = true; // Inicialmente disponible
    }

    public void AssignClip(AudioEntry entry)
    {
        source.clip = entry.clip;
        source.volume = entry.volume;
        source.pitch = entry.pitch;
        source.loop = entry.loop;
        currentClip = entry.name;
        hasFinishedPlaying = false;
    }

    public void Release()
    {
        source.Stop();
        source.clip = null;
        currentClip = null;
        hasFinishedPlaying = true;
    }

    public void Destroy()
    {
        if (gameObject != null)
            Object.Destroy(gameObject);
    }
}