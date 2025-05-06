// AudioManager.cs
// 
//
// Description:
// Main entry point for the audio system that manages playback operations
// and coordinates between the audio library and channel pool.

using UnityEngine;
using AudioSystem.Core;
using UnityEngine.UI;

namespace AudioSystem
{
    /// <summary>
    /// Central controller for the audio system that provides a simplified interface
    /// for audio playback and management.
    /// </summary>
    /// <remarks>
    /// Handles:
    /// - System initialization
    /// - Playback requests (both single and overlapping)
    /// - Audio stopping
    /// - Resource management
    /// </remarks>
     public class AudioManager : MonoBehaviour
    {
        /// <summary> Used for prefab save, need to be diferente for each audiomanager </summary>
        [Header("Identification")]
        [Tooltip("Used for store in PlayerPrefs (Try not to Repite in others AudioManagers)")]
        [SerializeField] private string _managerName = "SFX";

        
        [Header("Channel Configuration")]
        [Tooltip("Number of audio channels")]
        [SerializeField, Min(1)] private int _channelCount = 10;

        /// <summary> General volumen for all the clips </summary>
        [Tooltip("Audios Volume")]
        [Range(0, 1f)] 
        [SerializeField] private float _masterVolume = 1;

        /// <summary> 
        /// The music should just pause so it can continue where it left or if changed in another 
        /// scene, SFX should stop since they are just in the moment, no need to keep track.
        ///</summary>
        [Tooltip("Should the audios stop when mute or pause(is sfx or music)")]
        [SerializeField] private bool _stopOnMute = false;


        /// <summary> 
        /// COnfiguration to show in a UI to volume with a slider and a mute toggle.
        ///</summary>
        [Header("UI Configuration (Not necesary but recomended)")]
        [Tooltip("Slider to initialize Volumen Setting")]
        [SerializeField] private Slider _sliderUI;

        [Tooltip("Fill image of the slider")]
        [SerializeField] private Image _sliderImageFill;

        [Tooltip("image of the alider handdle")]
        [SerializeField] private Image _sliderHanddleImage;

        [Tooltip("Toggle to initialize Mute Setting")]
        [SerializeField] private Toggle _toggleUI;

        [Tooltip("Color of slider when Muted")]
        [SerializeField] private Color _sliderMutedColor;

        ///<summary> If the manager is muted</summary>
        private bool _isMuted;
         ///<summary> Reference to the image fill original color</summary>
        private Color _sliderImageFillColor;
        ///<summary> Reference to the handle original color</summary>
        private Color _sliderHanddleImageColor;

        ///<summary> Reference to the library of the clip</summary>
        private AudioLibrary _library;
        ///<summary> Reference to the channel pool</summary>
        private AudioChannelPool _channelPool;
        ///<summary> Reference to the AudioPlayer to manage the clips</summary>
        private AudioPlayer _player;
        ///<summary> Scripts for save in prefab</summary>
        private const string VOLUME_PREF_KEY = "AudioManager_Volume";
        private const string MUTE_PREF_KEY = "AudioManager_IsMuted";

        private void Awake()
        {
            InitializeSystem();
            if (_sliderHanddleImage != null) 
                _sliderHanddleImageColor = _sliderHanddleImage.color;
            if (_sliderImageFill != null) 
                _sliderImageFillColor = _sliderImageFill.color;

        }

        void Start()
        {
            LoadVolumeSetting();
            SetVolume(_masterVolume);
            LoadMuteSetting();

            if(_isMuted){
                Mute();
            }
            else{
                UnMute();
            }
            UpdateUIElements();
            
        }
        private void InitializeSystem()
        {
            _library = GetComponent<AudioLibrary>();
            if (_library == null)
            {
                Debug.LogError("[AudioSystem] Missing AudioLibrary component!");
                enabled = false;
                return;
            }

            // Create pool with all channels upfront
            _channelPool = new AudioChannelPool(transform, _channelCount);
            _player = gameObject.AddComponent<AudioPlayer>();
            _player.Initialize(_library, _channelPool);

        }

        /// <summary>
        /// Plays audio, replacing any existing instance
        /// </summary>
        public void Play(string audioName){
            if (!ValidateAudioName(audioName, out string validName))
                return; 

            if (_isMuted && _stopOnMute){
                return;
            }
            if (_isMuted && !_stopOnMute){
                _player.PlayMuted(validName, false);
                return;
            }
            _player.Play(validName, false);
        }

        /// <summary>
        /// Plays audio by ID, replacing any existing instance
        /// </summary>
        public void Play(int audioID) => Play(GetAudioNameByID(audioID));

        /// <summary>
        /// Plays audio allowing multiple instances
        /// </summary>
        public void PlayOverlapping(string audioName)
        {
             if (!ValidateAudioName(audioName, out string validName))
                return; 

            if (_isMuted && _stopOnMute){
                return;
            }
            if (_isMuted && !_stopOnMute){
                _player.PlayMuted(validName, true);
                return;
            }
            _player.Play(validName, true);
        }
        /// <summary>
        /// Plays audio by ID allowing multiple instances
        /// </summary>
        public void PlayOverlapping(int audioID) => PlayOverlapping(GetAudioNameByID(audioID));

        /// <summary>
        /// Stops playback of specified audio
        /// </summary>
        public void Stop(string audioName)
        {
            if (ValidateAudioName(audioName, out string validName))
                _player.Stop(validName);
        }

        /// <summary>
        /// Stops playback of specified audio by ID
        /// </summary>
        public void Stop(int audioID) => Stop(GetAudioNameByID(audioID));

        /// <summary>
        /// Set Volume
        /// </summary>
        public void SetVolume(float volume){
            _masterVolume = volume;
            _channelPool.SetMasterVolume(_masterVolume);
            if(_isMuted){
                UnMute();
                UpdateUIElements();
            }
        }
        /// <summary>
        /// Get Volume
        /// </summary>
        public float GetVolume(){
            return _masterVolume;
        }
        /// <summary>
        /// Mute Audio
        /// </summary>
        private void Mute(){
            if(_stopOnMute){
                _player.StopAll();
            }
            else{
                _player.PauseAll();
            }
            _isMuted = true;
            UpdateUIElements();
        }
        /// <summary>
        /// UnMute Audio
        /// </summary>
        private void UnMute(){
            if(!_stopOnMute){
                _player.ResumeAll();
            }
            _isMuted = false;
            UpdateUIElements();
        }

        /// <summary>
        /// Toggle to Mute-Unmute Audio
        /// </summary>
        public void ToggleMute(){
            if(_isMuted){
                UnMute();
            }
            else{
                Mute();
            }
        }
        /// <summary>
        /// Save the mastervolume in PlayerPrefs
        /// </summary>
        public void SaveVolumeSetting()
        {
            float volume = Mathf.Clamp01(_masterVolume); 
            PlayerPrefs.SetFloat(VOLUME_PREF_KEY + _managerName, volume);
            PlayerPrefs.Save(); 
            
            Debug.Log($"[Audio] Volume setting saved: {volume}");
        }

        /// <summary>
        /// Save the Mute State in PlayerPrefss
        /// </summary>
        public void SaveMuteSetting()
        {
            PlayerPrefs.SetInt(MUTE_PREF_KEY + _managerName, _isMuted ? 1 : 0);
            PlayerPrefs.Save();
            
            Debug.Log($"[Audio] Mute setting saved: {_isMuted}");
        }

        /// <summary>
        /// Load the Master volume saved (1 by defect)
        /// </summary>
        public void LoadVolumeSetting()
        {
            _masterVolume =  PlayerPrefs.GetFloat(VOLUME_PREF_KEY + _managerName, 1f); 
        }

        /// <summary>
        /// Load the Mute state saved (false by default)
        /// </summary>
        public void LoadMuteSetting()
        {
            _isMuted = PlayerPrefs.GetInt(MUTE_PREF_KEY + _managerName, 0) == 1; 
            Debug.Log("Muted" + _isMuted);
        }
        /// <summary>
        /// Update the Slider an the toggle to initialize the value of volume and mute
        /// </summary>
        private void UpdateUIElements(){
            if(_sliderUI != null){
                _sliderUI.SetValueWithoutNotify(_masterVolume);
                if(_isMuted){
                    _sliderHanddleImage.color = _sliderMutedColor;
                    _sliderImageFill.color = _sliderMutedColor;
                }
                else{
                    _sliderHanddleImage.color = _sliderHanddleImageColor;
                    _sliderImageFill.color = _sliderImageFillColor;
                }
            }
            if(_toggleUI != null){
                _toggleUI.SetIsOnWithoutNotify(_isMuted);
            }
        }


        #region Helpers

        /// <summary>
        /// Get the name of the audio by his id, id = index in array
        /// </summary>
        private string GetAudioNameByID(int audioID)
        {
            AudioEntry entry = _library?.GetEntry(audioID);
            if (entry == null)
            {
                Debug.LogError($"[AudioSystem] Invalid audio ID: {audioID}");
                return null;
            }
            return entry.name;
        }
        
        /// <summary>
        /// Check if the name exist in the library
        /// </summary>
        /// <param name="audioName">Name of the file to check.</param>
        /// <param name="validName">Output parameter that will contain the validated name.</param>
        /// <returns>
        private bool ValidateAudioName(string audioName, out string validName)
        {
            validName = null;
            
            if (_library == null)
            {
                Debug.LogError("[AudioSystem] System not initialized");
                return false;
            }

            if (string.IsNullOrEmpty(audioName))
            {
                Debug.LogError("[AudioSystem] Invalid audio name");
                return false;
            }

            validName = audioName;
            return true;
        }

        #endregion
    }
}
