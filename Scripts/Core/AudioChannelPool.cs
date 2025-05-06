// AudioChannelPool.cs
// 
//
// Description:
// Implementation of a fixed-size pool of audio channels for managing audio playback.
// Uses a strict limit of channels with FIFO replacement when all channels are busy.

using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem.Core
{
    /// <summary>
    /// Manages a fixed pool of audio channels for controlled audio playback.
    /// </summary>
    /// <remarks>
    /// When all channels are occupied, the system will either:
    /// 1. Return null (if configured to do so)
    /// 2. Or stop the oldest playing channel and reuse it (default behavior)
    /// </remarks>
    public class AudioChannelPool
    {
        /// <summary> Transform for the new Pool. </summary>
        private Transform _parent;
        /// <summary> Number of channels of the pool</summary>
        private int _maxChannels;
        /// <summary> free channels to play audio </summary>
        private Queue<AudioChannel> _availableChannels = new Queue<AudioChannel>();
        /// <summary> all channels of the pool </summary>
        private List<AudioChannel> _allChannels = new List<AudioChannel>();
        /// <summary> Busy channels that are playing audio </summary>
        private Queue<AudioChannel> _busyChannels = new Queue<AudioChannel>();

        /// <summary>
        /// Initializes a new instance of the AudioChannelPool with a fixed number of channels.
        /// </summary>
        /// <param name="parent">Parent transform for channel GameObjects</param>
        /// <param name="channelCount">Fixed number of audio channels to create</param>
        public AudioChannelPool(Transform parent, int channelCount)
        {
            _parent = parent;
            _maxChannels = channelCount;
            
            for (int i = 0; i < channelCount; i++)
            {
                CreateNewChannel();
            }
        }

        /// <summary>
        /// Gets an available audio channel from the pool.
        /// </summary>
        /// <returns>
        /// An available AudioChannel, or null if all channels are busy and 
        /// the pool is configured to not replace playing channels.
        /// </returns>
        /// <remarks>
        /// If no channels are available, this will stop the oldest playing channel
        /// and return it for reuse.
        /// </remarks>
        public AudioChannel GetAvailableChannel()
        {
            // 1. Check for available channels first
            if (_availableChannels.Count > 0)
            {
                var channel = _availableChannels.Dequeue();
                _busyChannels.Enqueue(channel);
                return channel;
            }

            // 2. If no available channels, reclaim the oldest busy channel
            if (_busyChannels.Count > 0)
            {
                var oldestChannel = _busyChannels.Dequeue();
                oldestChannel.Release();
                _busyChannels.Enqueue(oldestChannel);
                return oldestChannel;
            }

            // This should never happen since we pre-create channels but just in case
            Debug.LogError("[AudioSystem] No channels available in pool!");
            return null;
        }

        /// <summary>
        /// Releases a channel back to the available pool.
        /// </summary>
        /// <param name="channel">The channel to release</param>
        public void ReleaseChannel(AudioChannel channel)
        {
            if (channel == null) return;

            channel.Release();
            
            // Remove from busy channels if present
            if (_busyChannels.Contains(channel))
            {
                var tempQueue = new Queue<AudioChannel>();
                while (_busyChannels.Count > 0)
                {
                    var ch = _busyChannels.Dequeue();
                    if (ch != channel) tempQueue.Enqueue(ch);
                }
                _busyChannels = tempQueue;
            }

            _availableChannels.Enqueue(channel);
        }

        /// <summary>
        /// Creates a new audio channel and adds it to the available pool.
        /// </summary>
        /// <returns>The newly created AudioChannel</returns>
        private AudioChannel CreateNewChannel()
        {
            var channel = new AudioChannel(
                _parent,
                $"Channel_{_allChannels.Count}"
            );
            
            _allChannels.Add(channel);
            _availableChannels.Enqueue(channel);
            return channel;
        }

        /// <summary>
        /// Cleans up all channel resources.
        /// </summary>
        public void Cleanup()
        {
            foreach (var channel in _allChannels)
            {
                if (channel.gameObject != null)
                {
                    GameObject.Destroy(channel.gameObject);
                }
            }
            
            _allChannels.Clear();
            _availableChannels.Clear();
            _busyChannels.Clear();
        }
        /// <summary>
        /// Set Volume to All channels
        /// </summary>
        public void SetMasterVolume(float volume){
            foreach (var channel in _allChannels){
                channel.SetVolume(volume);
            }
        }
    }
}
