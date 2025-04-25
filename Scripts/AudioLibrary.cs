using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Libreria de audios disponibles para reproducir
/// </summary>

public class AudioLibrary : MonoBehaviour
{
    [SerializeField] private List<AudioEntry> _entries = new List<AudioEntry>();

    public AudioEntry GetEntry(string audioName)
    {
        return _entries.Find(e => e.name == audioName);
    }

    public AudioEntry GetEntry(int id)
    {
        return (id >= 0 && id < _entries.Count) ? _entries[id] : null;
    }
    public bool IsValidID(int id) => id >= 0 && id < _entries.Count;
}