using UnityEngine;
using femjami.Utils.Singleton;
using StarterAssets;
using UnityEngine.SceneManagement;
using femjami.Managers;

public class MenuSystem : Singleton<MenuSystem>
{
    [SerializeField] private GameObject _pauseMenuUI;
    private bool _pause;
    public bool GetIsPaused() => _pause;
    public bool SetPause(bool b) => _pause = b;
    private bool _gameOver = false;

    private void Start()
    {
        GameEvents.current.onLoseGame += GameOver;
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

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
