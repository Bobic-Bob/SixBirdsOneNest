using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UIButtonsHolder : MonoBehaviour
{

    private SaveLoadSettings _saveLoadSettings;

    [SerializeField]
    private GameObject _tutorialPanel;
    [SerializeField]
    private GameObject _legendPanel;

    [SerializeField]
    private AudioMixerGroup _master;
    [SerializeField]
    private AudioMixerGroup _game;
    [SerializeField]
    private AudioMixerGroup _music;

    private static bool _masterVolumeEnabled;

    public static bool MasterVolumeEnabled
    {
        get { return _masterVolumeEnabled; }
        private set { _masterVolumeEnabled = value; }
    }

    private static bool _gameVolumeEnabled = true;

    public static bool GameVolumeEnabled
    {
        get { return _gameVolumeEnabled; }
        private set { _gameVolumeEnabled = value; }
    }

    private static bool _musicEnabled = true;

    public static bool MusicEnabled
    {
        get { return _musicEnabled; }
        private set { _musicEnabled = value; }
    }

    private bool _gamePaused = true;

    private static bool _showLegend = true;

    public static bool ShowLegend
    {
        get { return _showLegend; }
        private set { _showLegend = value; }
    }

    private static bool _showTutorial = true;

    public static bool ShowTutorial
    {
        get { return _showTutorial; }
        private set { _showTutorial = value; }
    }

    private void Start()
    {
        _saveLoadSettings = FindObjectOfType<SaveLoadSettings>();
        if (_saveLoadSettings)
        {
            bool[] settings = _saveLoadSettings.LoadSettings();
            if (settings != null)
            {
                MusicEnabled = settings[0];
                ToggleMusicVolume();
                GameVolumeEnabled = settings[1];
                ToggleGameVolume();
                ShowLegend = settings[2];
                ShowTutorial = settings[3];
            }
            if (_tutorialPanel)
            {
                _tutorialPanel.SetActive(ShowTutorial);
            }
            if (_legendPanel)
            {
                _legendPanel.SetActive(ShowLegend);
            }
        }
        else
        {
            throw new System.NullReferenceException(nameof(_saveLoadSettings));
        }
    }

    // œ≈–≈’Œƒ€ œŒ —÷≈Õ¿Ã
    public void GoToGame()
    {
        Time.timeScale = 1f;
        MasterVolumeEnabled = true;
        ToggleMasterVolume();
        SceneManager.LoadScene(1);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        MasterVolumeEnabled = true;
        ToggleMasterVolume();
        SceneManager.LoadScene(0);
    }

    // œŒ ¿«¿“‹/— –€“‹ œ¿Õ≈À»
    public void ToggleThis(GameObject toggleObject)
    {
        if (toggleObject != null && toggleObject.activeSelf)
        {
            toggleObject.SetActive(false);
        }
        else
        {
            toggleObject.SetActive(true);
        }
    }

    // ƒ–”√Œ≈
    public void Exit()
    {
        Application.Quit();
    }

    public void TogglePause()
    {
        if (_gamePaused)
        {
            Time.timeScale = 1f;
            _gamePaused = false;
        }
        else
        {
            Time.timeScale = 0f;
            _gamePaused = true;
        }
    }

    public void ToggleMasterVolume()
    {
        if (MasterVolumeEnabled)
        {
            _master.audioMixer.SetFloat("MasterVolume", 0);
        }
        else
        {
            _master.audioMixer.SetFloat("MasterVolume", -80);
        }
        MasterVolumeEnabled = !MasterVolumeEnabled;
    }

    public void ToggleMusicVolume(bool toggleMusicEnabled = false)
    {
        if (toggleMusicEnabled)
        {
            MusicEnabled = !MusicEnabled;
            _saveLoadSettings.SaveSettings();
        }
        if (MusicEnabled)
        {
            _music.audioMixer.SetFloat("MusicVolume", 0);
        }
        else
        {
            _music.audioMixer.SetFloat("MusicVolume", -80);
        }
    }

    public void ToggleGameVolume(bool toggleGameVolumeEnabled = false)
    {
        if (toggleGameVolumeEnabled)
        {
            GameVolumeEnabled = !GameVolumeEnabled;
            _saveLoadSettings.SaveSettings();
        }
        if (GameVolumeEnabled)
        {
            _game.audioMixer.SetFloat("GameVolume", 0);
        }
        else
        {
            _game.audioMixer.SetFloat("GameVolume", -80);
        }
    }

    public void DontShowAgainLegend()
    {
        ShowLegend = false;
        _saveLoadSettings.SaveSettings();
    }

    public void DontShowAgainTutorial()
    {
        ShowTutorial = false;
        _saveLoadSettings.SaveSettings();
    }
}
