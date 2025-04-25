using System.Collections.Generic;
using UnityEngine;

public class AudioChannelPool
{
    private Transform _parent;
    private int _maxChannels;
    private bool _strictLimit;
    private Queue<AudioChannel> _availableChannels = new Queue<AudioChannel>();
    private List<AudioChannel> _allChannels = new List<AudioChannel>();

    public AudioChannelPool(Transform parent, int initialSize, int maxChannels, bool strictLimit)
    {
        _parent = parent;
        _maxChannels = maxChannels;
        _strictLimit = strictLimit;
        
        initialSize = Mathf.Min(initialSize, maxChannels);
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewChannel(false);
        }
    }

    public AudioChannel GetAvailableChannel()
    {
        // 1. Buscar canales existentes disponibles
        foreach (var channel in _allChannels)
        {
            if (channel.hasFinishedPlaying && !channel.isTemporary)
            {
                return PrepareChannel(channel);
            }
        }

        // 2. Crear nuevo canal si es posible
        if (_allChannels.Count < _maxChannels)
        {
            return CreateNewChannel(false);
        }

        // 3. Modo no estricto: crear temporal
        if (!_strictLimit)
        {
            return CreateNewChannel(true);
        }

        Debug.LogWarning("No hay canales disponibles!");
        return null;
    }

    public void ReleaseChannel(AudioChannel channel)
    {
        if (channel == null) return;

        channel.Release();

        // Destruir temporales que excedan el límite
        if (channel.isTemporary || _allChannels.Count > _maxChannels)
        {
            DestroyChannel(channel);
        }
    }

    private AudioChannel CreateNewChannel(bool isTemporary)
    {
        var channel = new AudioChannel(
            _parent,
            $"Channel_{_allChannels.Count}",
            isTemporary
        );
        
        _allChannels.Add(channel);
        return PrepareChannel(channel);
    }

    private AudioChannel PrepareChannel(AudioChannel channel)
    {
        channel.Release(); // Limpiar estado previo
        return channel;
    }

    private void DestroyChannel(AudioChannel channel)
    {
        if (channel.gameObject != null)
        {
            GameObject.Destroy(channel.gameObject);
        }
        _allChannels.Remove(channel);
    }

    public void CleanupFinishedChannels()
    {
        for (int i = _allChannels.Count - 1; i >= 0; i--)
        {
            var channel = _allChannels[i];
            if (channel.isTemporary && channel.hasFinishedPlaying)
            {
                DestroyChannel(channel);
            }
        }
    }
}