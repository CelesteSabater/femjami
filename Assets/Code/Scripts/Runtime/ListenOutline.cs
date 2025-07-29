using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using femjami.Managers;
using System.Linq;

public class ListenOutline : MonoBehaviour
{
    private Material[] _normalMaterial;
    [SerializeField] private Material _listeningModeMaterial;
    private SkinnedMeshRenderer[] _skinnedMeshRenderer;

    void Start()
    {
        _skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        _normalMaterial = new Material[_skinnedMeshRenderer.Count()];
        GameEvents.current.onStartListeningMode += OnStartListeningMode;
        GameEvents.current.onEndListeningMode += OnEndListeningMode;
    }

    void OnDestroy() {
        GameEvents.current.onStartListeningMode -= OnStartListeningMode;
        GameEvents.current.onEndListeningMode -= OnEndListeningMode;       
    }

    private void OnStartListeningMode()
    {
        for (int i = 0; i < _skinnedMeshRenderer.Count(); i++)
        {
            Material[] mats = _skinnedMeshRenderer[i].materials;
            _normalMaterial[i] = mats[0];
            mats[0] = _listeningModeMaterial;
            _skinnedMeshRenderer[i].materials = mats;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.layer = 9;
        }
    }

    private void OnEndListeningMode()
    {
        for (int i = 0; i < _skinnedMeshRenderer.Count(); i++)
        {
            Material[] mats = _skinnedMeshRenderer[i].materials;
            mats[0] = _normalMaterial[i];
            _skinnedMeshRenderer[i].materials = mats;
        }
        
        foreach (Transform child in transform)
        {
            child.gameObject.layer = 0;
        }
    }
}
