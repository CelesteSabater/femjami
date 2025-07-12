using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class ListenMode : MonoBehaviour
{
    [SerializeField] private Camera auxiliarCamera;
    private bool _currentlyListening = false;
    void Update()
    {
        Listen();
    }

    void Listen()
    {
        if (!StarterAssetsInputs.Instance.listen)
        {
            StopListenMode();
            return;
        }

        StartListenMode();
    }

    void StartListenMode()
    {
        if (_currentlyListening) return;
        _currentlyListening = true;
        auxiliarCamera.gameObject.SetActive(true);
    }

    void StopListenMode()
    {
        if (!_currentlyListening) return;
        _currentlyListening = false;
        auxiliarCamera.gameObject.SetActive(false);
    }
}
