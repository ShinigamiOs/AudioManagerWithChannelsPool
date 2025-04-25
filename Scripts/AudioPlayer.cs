using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public class AudioEvent : UnityEvent<string> { }

    public AudioEvent OnAudioStart;
    public AudioEvent OnAudioComplete;
    public AudioEvent OnAudioStop;

    private AudioLibrary _library;
    private AudioChannelPool _channelPool;
    private Dictionary<string, AudioChannel> _activeNonOverlapChannels = new Dictionary<string, AudioChannel>();

    public void Initialize(AudioLibrary library, AudioChannelPool pool)
    {
        _library = library;
        _channelPool = pool;
    }

    public void Play(string audioName, bool isOverlap)
    {
        AudioEntry entry = _library.GetEntry(audioName);
        if (entry == null) return;

        if (!isOverlap && _activeNonOverlapChannels.TryGetValue(audioName, out AudioChannel existingChannel))
        {
            // Reiniciar audio existente
            existingChannel.source.Stop();
            existingChannel.AssignClip(entry);
            existingChannel.source.Play();
            OnAudioStart?.Invoke(audioName);
            return;
        }

        AudioChannel channel = _channelPool.GetAvailableChannel();
        if (channel == null) return;

        channel.AssignClip(entry);
        channel.source.Play();
        OnAudioStart?.Invoke(audioName);

        if (!isOverlap)
        {
            _activeNonOverlapChannels[audioName] = channel;
        }

        StartCoroutine(TrackAudioCompletion(entry, channel, !isOverlap));
    }

    private IEnumerator TrackAudioCompletion(AudioEntry entry, AudioChannel channel, bool isNonOverlap)
    {
        yield return new WaitForSeconds(entry.clip.length / channel.source.pitch);

        channel.hasFinishedPlaying = true;

        if (isNonOverlap)
        {
            _activeNonOverlapChannels.Remove(entry.name);
        }

        OnAudioComplete?.Invoke(entry.name);
        _channelPool.ReleaseChannel(channel);
        _channelPool.CleanupFinishedChannels();
    }

    public void Stop(string audioName)
    {
        if (_activeNonOverlapChannels.TryGetValue(audioName, out AudioChannel channel))
        {
            channel.source.Stop();
            channel.hasFinishedPlaying = true;
            _channelPool.ReleaseChannel(channel);
            _activeNonOverlapChannels.Remove(audioName);
            OnAudioStop?.Invoke(audioName);
        }
    }
}