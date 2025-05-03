// AudioLibrary.cs
// 
//
// Description:
// Central repository for audio assets and their configurations.
// Provides lookup functionality for audio entries by name or ID.

using System.Collections.Generic;
using UnityEngine;
using AudioSystem.Core;

namespace AudioSystem
{
    /// <summary>
    /// Central registry and access point for all audio assets in the system.
    /// </summary>
    /// <remarks>
    /// Maintains a collection of AudioEntry configurations and provides
    /// efficient lookup methods. Attach to a GameObject in your scene
    /// and populate via the Unity Inspector.
    /// </remarks>
    public class AudioLibrary : MonoBehaviour
    {
        [Header("Audio Entries")]
        [Tooltip("Collection of audio configurations available in this library")]
        [SerializeField] private List<AudioEntry> _entries = new List<AudioEntry>();

        /// <summary>
        /// Gets the number of audio entries in the library.
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        /// Retrieves an audio entry by its exact name match.
        /// </summary>
        /// <param name="audioName">Case-sensitive name of the audio entry</param>
        /// <returns>The matching AudioEntry, or null if not found</returns>
        public AudioEntry GetEntry(string audioName)
        {
            if (string.IsNullOrEmpty(audioName))
            {
                Debug.LogWarning("[AudioSystem] Attempted to find audio entry with empty name");
                return null;
            }

            var entry = _entries.Find(e => e.name == audioName);
            
            if (entry == null)
            {
                Debug.LogWarning($"[AudioSystem] Audio entry not found: {audioName}");
            }

            return entry;
        }

        /// <summary>
        /// Retrieves an audio entry by its index in the library.
        /// </summary>
        /// <param name="id">Zero-based index of the audio entry</param>
        /// <returns>The AudioEntry at the specified index, or null if invalid</returns>
        public AudioEntry GetEntry(int id)
        {
            if (!IsValidID(id))
            {
                Debug.LogWarning($"[AudioSystem] Invalid audio entry ID: {id}");
                return null;
            }
            return _entries[id];
        }

        /// <summary>
        /// Checks if an ID is valid for this library.
        /// </summary>
        /// <param name="id">ID to check</param>
        /// <returns>True if the ID is within valid bounds</returns>
        public bool IsValidID(int id) => id >= 0 && id < _entries.Count;

        /// <summary>
        /// Returns all audio entries in the library.
        /// </summary>
        /// <returns>Read-only collection of audio entries</returns>
        public IReadOnlyList<AudioEntry> GetAllEntries() => _entries.AsReadOnly();
    }
}