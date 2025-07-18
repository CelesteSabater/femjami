using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using femjami.Systems.AudioSystem;
using UnityEngine.SceneManagement;
using femjami.Managers;

public class SceneTransition : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float _fadeDuration = 2f;
    [SerializeField] private float _soundDelay = 1.1f;
    [SerializeField] private string _targetScene = "Menu";

    private CanvasGroup _canvasGroup;
    private float _currentFadeTime;
    private float _soundTimer;
    private bool _isTransitioning;

    private void Start()
    {
        GameEvents.current.onLoseGame += StartTransition;
    }

    private void OnDestroy()
    {
        if (GameEvents.current != null)
        {
            GameEvents.current.onLoseGame -= StartTransition;
        }
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    public void StartTransition()
    {
        if (_isTransitioning) return;

        _isTransitioning = true;
        Time.timeScale = 0f;
        _currentFadeTime = 0f;
        _soundTimer = 0f;
        _canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (!_isTransitioning) return;

        HandleFade();
        HandleSceneChange();
    }

    private void HandleFade()
    {
        if (_currentFadeTime >= _fadeDuration) return;
        
        _currentFadeTime += Time.unscaledDeltaTime;
        float progress = Mathf.Clamp01(_currentFadeTime / _fadeDuration);
        _canvasGroup.alpha = progress;

        if (progress >= 1f)
        {
            AudioSystem.Instance.PlaySFX("Slash");
        }
    }

    private void HandleSceneChange()
    {
        if (_currentFadeTime < _fadeDuration) return;

        _soundTimer += Time.unscaledDeltaTime;

        if (_soundTimer >= _soundDelay)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_targetScene);
        }
    }
}
