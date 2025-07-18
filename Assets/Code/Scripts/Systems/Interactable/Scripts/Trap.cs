using UnityEngine;
using StarterAssets;
using UnityEngine.UI;
using femjami.Systems.AudioSystem;

namespace femjami.Systems.Interactable
{
    public class Trap : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject _promptPrefab;
        [SerializeField] private Transform _promptLocation;
        [SerializeField] private float _interactionTime = 1f; 
        [SerializeField] private bool _isActive = true;

        public GameObject PromptPrefab => _promptPrefab;
        public Transform PromptLocation => _promptLocation;
        public bool IsActive() => _isActive; 

        private GameObject _promptGo;
        private float _interactionTimer = 0f;
        private bool _interacting = false;
        public bool SetActive(bool active) => _isActive = active;
        private int _animIDInteracting;
        private Animator _animator;

        void Start()
        {
            GameObject[] go = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < go.Length; i++)
            {
                Animator anim = go[i].GetComponent<Animator>();
                if (anim) _animator = anim;
            }
            _animIDInteracting = Animator.StringToHash("Interacting");
        }

        private void Update()
        {
            UpdateSlider();

            if (_interacting && !StarterAssetsInputs.Instance.interact)
            {
                if (_animator) _animator.SetBool(_animIDInteracting, false);
                _interacting = false;
                _interactionTimer = 0f;
            }
        }

        public bool Interact(Interactor interactor)
        {
            if (!_interacting)
            {
                _interacting = true;
                _interactionTimer = 0f;
                if (_animator) _animator.SetBool(_animIDInteracting, false);
            }

            if (_animator) _animator.SetBool(_animIDInteracting, true);
            _interactionTimer += Time.deltaTime;

            if (_interactionTimer >= _interactionTime)
            {
                AudioSystem.AudioSystem.Instance.PlaySFX("ClickClack");
                if (_animator) _animator.SetBool(_animIDInteracting, false);
                Destroy(gameObject);
                return true;
            }

            return false;
        }

        public void SetupPrompt(bool show)
        {
            if (show)
                GeneratePrompt();
            else
                DeletePrompt();
        }

        private void GeneratePrompt()
        {
            if (_promptGo != null) return;
            _promptGo = Instantiate(_promptPrefab, PromptLocation.position, Quaternion.identity);
        }

        private void DeletePrompt()
        {
            if (_promptGo != null)
            {
                Destroy(_promptGo);
                _promptGo = null;
            }

            _interacting = false;
            _interactionTimer = 0f;
        }

        private void UpdateSlider()
        {
            if (_promptGo == null) return;
            Slider slider = _promptGo.GetComponentInChildren<Slider>();
            slider.value = _interactionTimer / _interactionTime;
        }

        private void OnDestroy()
        {
            if (_promptGo != null)
            {
                Destroy(_promptGo);
            }
        }
    }
}