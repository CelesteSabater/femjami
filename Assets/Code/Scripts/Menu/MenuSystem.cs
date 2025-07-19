using UnityEngine;
using femjami.Utils.Singleton;
using StarterAssets;
using UnityEngine.SceneManagement;
using femjami.Managers;
using UnityEngine.UI;
using femjami.Systems.AudioSystem;

public class MenuSystem : Singleton<MenuSystem>
{
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _subMenu;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private bool _pause;
    public bool GetIsPaused() => _pause;
    public bool SetPause(bool b) => _pause = b;
    private bool _gameOver = false;
    public bool _ignoreUpdate = false;
    private bool _isActive = false;

    private void Start()
    {
        GameEvents.current.onLoseGame += GameOver;
        if (_sfxSlider) _sfxSlider.value = AudioData._sfxVolume;
        if (_musicSlider) _musicSlider.value = AudioData._musicVolume;
    }

    private void OnDestroy()
    {
        if (GameEvents.current != null)
        {
            GameEvents.current.onLoseGame -= GameOver;
        }
    }

    private void GameOver() => _gameOver = true;
    public void Update()
    {
        if (_gameOver) return;
        if (_ignoreUpdate) return;
        PauseGame();
    }

    private void PauseGame()
    {
        _pauseMenuUI.SetActive(StarterAssetsInputs.Instance.pause);
        Time.timeScale = StarterAssetsInputs.Instance.pause ? 0 : 1;
        SetPause(StarterAssetsInputs.Instance.pause);
    }

    public void ResumeGame()
    {
        StarterAssetsInputs.Instance.pause = false;
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        SetPause(false);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void SettingsLoop()
    {
        _isActive = !_isActive;
        _pauseMenuUI.SetActive(_isActive);
        if (_subMenu) _subMenu.SetActive(false);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
