using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int _maxChannels = 10;
    [SerializeField] private bool _strictLimit = false;
    [SerializeField] private int _prewarmChannels = 5;

    private AudioLibrary _library;
    private AudioChannelPool _channelPool;
    private AudioPlayer _player;

    private void Awake()
    {
        _library = GetComponent<AudioLibrary>();
        _channelPool = new AudioChannelPool(
            transform, 
            _prewarmChannels,
            _maxChannels,
            _strictLimit
        );
        
        _player = gameObject.AddComponent<AudioPlayer>();
        _player.Initialize(_library, _channelPool);
    }

// ==================== INTERFAZ PÚBLICA ====================

    // ----- Reproducir -----
    public void Play(string audioName) => _player.Play(GetValidAudioName(audioName), false);
    public void Play(int audioID) => Play(GetAudioNameByID(audioID));

    public void PlayOverlapping(string audioName) => _player.Play(GetValidAudioName(audioName), true);
    public void PlayOverlapping(int audioID) => PlayOverlapping(GetAudioNameByID(audioID));

    // ----- Detener -----
    public void Stop(string audioName) => _player.Stop(GetValidAudioName(audioName));
    public void Stop(int audioID) => Stop(GetAudioNameByID(audioID));

    // ----- Helpers -----
    private string GetAudioNameByID(int audioID)
    {
        if (_library == null) return null;
        
        AudioEntry entry = _library.GetEntry(audioID);
        if (entry == null)
        {
            Debug.LogError($"Audio ID {audioID} no encontrado!");
            return null;
        }
        return entry.name;
    }

    private string GetValidAudioName(string audioName)
    {
        if (_library == null || string.IsNullOrEmpty(audioName)) 
        {
            Debug.LogError("Nombre de audio inválido o librería no inicializada");
            return null;
        }
        return audioName;
    }
}