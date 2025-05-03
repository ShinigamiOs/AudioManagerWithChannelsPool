// AudioEntry.cs
// 
// Description:
// Defines the data structure for audio assets and their playback configuration.
// Used by the audio system to store and manage audio clip properties.

using UnityEngine;

namespace AudioSystem.Core
{
    /// <summary>
    /// Represents a complete audio asset configuration including clip reference
    /// and playback parameters.
    /// </summary>
    /// <remarks>
    /// This class serves as the primary data container for audio assets in the system,
    /// providing both editor-friendly configuration and runtime playback settings.
    /// </remarks>
    [System.Serializable]
    public class AudioEntry 
    {
        /// <summary>
        /// Unique identifier for this audio entry.
        /// </summary>
        [Tooltip("Unique identifier for this audio entry")]
        public string name;

        /// <summary>
        /// Reference to the AudioClip asset.
        /// </summary>
        [Tooltip("Audio clip asset reference (WAV, MP3, etc.)")]
        public AudioClip clip;

        /// <summary>
        /// Pitch adjustment (-3 to 3).
        /// Values below 1 = slower/lower pitch, above 1 = faster/higher pitch.
        /// </summary>
        [Tooltip("Pitch adjustment (-3 to 3)\n1 = normal, <1 = lower, >1 = higher")]
        [Range(-3f, 3f)] 
        public float pitch = 1f;

        /// <summary>
        /// Whether the audio should loop automatically when finished.
        /// </summary>
        [Tooltip("Enable to loop audio continuously")]
        public bool loop = false;

        /// <summary>
        /// Whether the audio should play automatically when instantiated.
        /// </summary>
        [Tooltip("Enable to play automatically when loaded")]
        public bool playOnAwake = false;

        /// <summary>
        /// [Runtime-only] Default AudioSource reference for this entry.
        /// </summary>
        /// <remarks>
        /// This field is managed by the audio system and should not be set manually.
        /// </remarks>
        [System.NonSerialized] 
        public AudioSource defaultSource;

        /// <summary>
        /// [Runtime-only] Default GameObject containing the AudioSource.
        /// </summary>
        /// <remarks>
        /// This field is managed by the audio system and should not be set manually.
        /// </remarks>
        [System.NonSerialized] 
        public GameObject defaultObject;
    }
}