using System;
using UnityEngine;

namespace femjami.Managers
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents current;

        private void Awake() => current = this;

        #region DIALOGUE
        public event Action<bool> onSetDialogue;
        public void SetDialogue(bool value) => onSetDialogue?.Invoke(value);
        #endregion

        #region STEALTH
        public event Action<Vector3, float> onMakeSound;
        public void MakeSound(Vector3 soundOrigin, float maxDistance) => onMakeSound?.Invoke(soundOrigin, maxDistance);
        public event Action onStartListeningMode;
        public void StartListeningMode() => onStartListeningMode?.Invoke();
        public event Action onEndListeningMode;
        public void EndListeningMode() => onEndListeningMode?.Invoke();
        #endregion

        #region GAME
        public event Action onLoseGame;
        public void LoseGame() => onLoseGame?.Invoke();
        #endregion
    }
}