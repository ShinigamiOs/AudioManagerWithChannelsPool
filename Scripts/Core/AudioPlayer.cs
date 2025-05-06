// AudioPlayer.cs
// 
//
// Description:
// Core audio playback controller that manages audio clip playback,
// channel allocation, and event notifications.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AudioSystem.Core
{
    /// <summary>
    /// Main controller for audio playback operations within the audio system.
    /// </summary>
    /// <remarks>
    /// Handles:
    /// - Playback initiation/termination
    /// - Channel management via AudioChannelPool
    /// - Audio event notifications
    /// - Overlap and non-overlap playback modes
    /// </remarks>
    public class AudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// Custom UnityEvent for audio-related callbacks with audio name parameter.
        /// </summary>
        [System.Serializable]
        public class AudioEvent : UnityEvent<string> { }

        /// <summary>
        /// Events if the are needed
        /// </summary>
        [Header("Event Callbacks")]
        [Tooltip("Invoked when audio playback begins")]
        public AudioEvent OnAudioStart;
        
        [Tooltip("Invoked when audio completes natural playback")]
        public AudioEvent OnAudioComplete;
        
        [Tooltip("Invoked when audio is manually stopped")]
        public AudioEvent OnAudioStop;

        private AudioLibrary _library;
        private AudioChannelPool _channelPool;
        private Dictionary<string, AudioChannel> _activeNonOverlapChannels = new Dictionary<string, AudioChannel>();

        /// <summary>
        /// Initializes the AudioPlayer with required dependencies.
        /// </summary>
        /// <param name="library">Reference to the AudioLibrary</param>
        /// <param name="pool">Configured AudioChannelPool instance</param>
        public void Initialize(AudioLibrary library, AudioChannelPool pool)
        {
            _library = library;
            _channelPool = pool;
        }

        /// <summary>
        /// Plays the specified audio clip.
        /// </summary>
        /// <param name="audioName">Name of the audio entry to play</param>
        /// <param name="isOverlap">Whether to allow multiple instances of the same clip</param>
        /// <returns>True if playback was initiated successfully</returns>
        public bool Play(string audioName, bool isOverlap = false)
        {
            AudioEntry entry = _library.GetEntry(audioName);
            if (entry == null)
            {
                Debug.LogWarning($"[AudioSystem] Audio entry not found: {audioName}");
                return false;
            }

            // Handle non-overlap case
            if (!isOverlap && _activeNonOverlapChannels.TryGetValue(audioName, out AudioChannel existingChannel))
            {
                RestartChannel(existingChannel, entry);
                OnAudioStart?.Invoke(audioName);
                return true;
            }

            // Acquire and configure new channel
            AudioChannel channel = _channelPool.GetAvailableChannel();
            if (channel == null)
            {
                Debug.LogWarning($"[AudioSystem] No available channels for: {audioName}");
                return false;
            }

            ConfigureNewPlayback(channel, entry, audioName, isOverlap);
            return true;
        }
        /// <summary>
        /// Prepares the audio clip for playback but doesn't play it (ideal for muted music that needs channel assignment)
        /// </summary>
        public bool PlayMuted(string audioName, bool isOverlap = false)
        {
            AudioEntry entry = _library.GetEntry(audioName);
            if (entry == null)
            {
                Debug.LogWarning($"[AudioSystem] Audio entry not found: {audioName}");
                return false;
            }

            // Handle non-overlap case
            if (!isOverlap && _activeNonOverlapChannels.TryGetValue(audioName, out AudioChannel existingChannel))
            {
                existingChannel.AssignClip(entry); // Reasigna el clip pero no reproduce
                return true;
            }

            // Adquirir canal sin reproducir
            AudioChannel channel = _channelPool.GetAvailableChannel();
            if (channel == null)
            {
                Debug.LogWarning($"[AudioSystem] No available channels for: {audioName}");
                return false;
            }

            channel.AssignClip(entry); // Asigna el clip
            channel.source.Stop(); // Asegura que no se reproduzca

            if (!isOverlap)
            {
                _activeNonOverlapChannels[audioName] = channel;
            }

            return true;
        }

        /// <summary>
        /// Stops playback of the specified audio clip.
        /// </summary>
        /// <param name="audioName">Name of the audio entry to stop</param>
        /// <returns>True if audio was found and stopped</returns>
        public bool Stop(string audioName)
        {
            if (_activeNonOverlapChannels.TryGetValue(audioName, out AudioChannel channel))
            {
                CleanupChannel(channel, audioName);
                OnAudioStop?.Invoke(audioName);
                return true;
            }
            return false;
        }

        private void RestartChannel(AudioChannel channel, AudioEntry entry)
        {
            channel.source.Stop();
            channel.AssignClip(entry);
            channel.source.Play();
        }

        private void ConfigureNewPlayback(AudioChannel channel, AudioEntry entry, string audioName, bool isOverlap)
        {
            channel.AssignClip(entry);
            channel.source.Play();
            OnAudioStart?.Invoke(audioName);

            if (!isOverlap)
            {
                _activeNonOverlapChannels[audioName] = channel;
            }

            StartCoroutine(TrackAudioCompletion(entry, channel, !isOverlap));
        }

        private void CleanupChannel(AudioChannel channel, string audioName)
        {
            channel.source.Stop();
            channel.hasFinishedPlaying = true;
            _channelPool.ReleaseChannel(channel);
            _activeNonOverlapChannels.Remove(audioName);
        }

        /// <summary>
        /// Coroutine that tracks audio completion and triggers cleanup.
        /// </summary>
        private IEnumerator TrackAudioCompletion(AudioEntry entry, AudioChannel channel, bool isNonOverlap)
        {
            // Calculate actual duration considering pitch
            float duration = entry.clip.length / Mathf.Abs(channel.source.pitch);
            yield return new WaitForSeconds(duration);

            channel.hasFinishedPlaying = true;

            if (isNonOverlap)
            {
                _activeNonOverlapChannels.Remove(entry.name);
            }

            OnAudioComplete?.Invoke(entry.name);
            _channelPool.ReleaseChannel(channel);
        }
        /// <summary>
        /// Stops all currently playing audio channels
        /// </summary>
        public void StopAll()
        {
            foreach (var channelPair in _activeNonOverlapChannels)
            {
                CleanupChannel(channelPair.Value, channelPair.Key);
                OnAudioStop?.Invoke(channelPair.Key);
            }
            _activeNonOverlapChannels.Clear();

            // Opcional: También limpiar canales overlap si los estás trackeando
        }

        /// <summary>
        /// Pauses all currently playing audio channels preserving playback position
        /// </summary>
        public void PauseAll()
        {
            foreach (var channelPair in _activeNonOverlapChannels)
            {
                channelPair.Value.source.Pause();
            }
            
        }
        /// <summary>
        /// Resumes all paused audio channels
        /// </summary>
        public void ResumeAll(){
            foreach (var channelPair in _activeNonOverlapChannels){
                AudioChannel channel = channelPair.Value;
                AudioSource source = channel.source;
                
                if (source.clip == null){
                    Debug.LogWarning($"[Audio] Channel {channelPair.Key} has no clip assigned");
                    continue;
                }

                if (source.time > 0 && !source.isPlaying){
                    source.UnPause();
                }
                else if (!source.isPlaying){
                    source.Play();
                }
            }
        }
    }
}
