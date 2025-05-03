// AudioChannel.cs
//
// Description:
// Implementation of the AudioChannel class which represents an individual audio playback channel
// in the audio system. Each channel manages its own AudioSource and playback state.

using UnityEngine;

namespace AudioSystem.Core
{
    /// <summary>
    /// Represents an individual audio channel for playing sounds in the audio system.
    /// Each channel manages its own GameObject with an AudioSource component and maintains
    /// playback state information.
    /// </summary>
    [System.Serializable]
    public class AudioChannel
    {
        /// <summary>The GameObject containing the AudioSource for this channel</summary>
        public GameObject gameObject;
        
        /// <summary>The AudioSource component for this channel</summary>
        public AudioSource source;
        
        /// <summary>Name of the currently assigned audio clip</summary>
        public string currentClip;
        
        /// <summary>Flag indicating whether playback has completed</summary>
        public bool hasFinishedPlaying;

        /// <summary>
        /// Constructs a new AudioChannel instance.
        /// </summary>
        /// <param name="parent">Transform to parent this channel's GameObject to</param>
        /// <param name="name">Name to assign to this channel's GameObject</param>
        public AudioChannel(Transform parent, string name)
        {
            gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent);
            source = gameObject.AddComponent<AudioSource>();
            hasFinishedPlaying = true;
        }

        /// <summary>
        /// Assigns an audio clip and configuration from an AudioEntry to this channel.
        /// </summary>
        /// <param name="entry">The AudioEntry containing clip and playback settings</param>
        public void AssignClip(AudioEntry entry)
        {
            source.Stop();
            source.clip = entry.clip;
            source.pitch = entry.pitch;
            source.loop = entry.loop;
            currentClip = entry.name;
            hasFinishedPlaying = false;
        }
        /// <summary>
        /// Releases the currently assigned audio clip and resets playback state.
        /// </summary>
        public void Release()
        {
            source.Stop();
            source.clip = null;
            currentClip = null;
            hasFinishedPlaying = true;
        }
        public void SetVolume(float volume){
            source.volume = volume;
        }

        /// <summary>
        /// Destroys the GameObject associated with this channel.
        /// </summary>
        /// <remarks>
        /// This should only be called when the channel is no longer needed.
        /// </remarks>
        public void Destroy()
        {
            if (gameObject != null)
                Object.Destroy(gameObject);
        }
    }
}