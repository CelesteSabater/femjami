using System.Linq;
using StarterAssets;
using femjami.Managers;
using femjami.DialogueTree.Runtime;
using UnityEngine;
using femjami.Utils.Singleton;
using femjami.Systems.Interactable;
using femjami.Systems.AudioSystem;

namespace femjami.Systems.Interactable
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private Transform _interactionPoint;
        [SerializeField] private float _interactionPointRadius = 0.5f;
        [SerializeField] private LayerMask _interactableMask;
        [SerializeField] private LayerMask _soundMakerMask;
        [SerializeField] private  GameObject FootstepRippleEffect;

        private Collider[] _collider;
        private IInteractable _interactable;

        private void Update()
        {
            if (MenuSystem.Instance.GetIsPaused())
                return;
            if (DialogueSystem.Instance.GetInDialogue()) return;

            CheckSoundMakers();
            CheckInteractable();
            Interact();
        }

        private void CheckSoundMakers()
        {
            Collider[] _soundColliders = Physics.OverlapSphere(transform.position, _interactionPointRadius, _soundMakerMask);
            if (_soundColliders.Count() == 0)
                return;

            for (int i = 0; i < _soundColliders.Count(); i++)
            {
                SoundMaker soundMaker = _soundColliders[i].GetComponent<SoundMaker>();
                if (soundMaker)
                    InteractSoundMaker(soundMaker);
            }
        }

        private void CheckInteractable()
        {
            _collider = Physics.OverlapSphere(_interactionPoint.position, _interactionPointRadius, _interactableMask);
            if (_collider.Count() != 0)
            {
                if (_interactable != null && _interactable != _collider[0].GetComponent<IInteractable>())
                    _interactable.SetupPrompt(false);

                if (_collider[0].GetComponent<IInteractable>().IsActive())
                {
                    _interactable = _collider[0].GetComponent<IInteractable>();
                    _interactable.SetupPrompt(true);
                }
            }
            else
            {
                if (_interactable != null)
                {
                    _interactable.SetupPrompt(false);
                    _interactable = null;
                }
            }
        }

        private void Interact()
        {
            if (!StarterAssetsInputs.Instance.interact)
                return;

            if (_interactable != null)
                _interactable.Interact(this);
        }

        private void InteractSoundMaker(SoundMaker soundMaker)
        {
            GameEvents.current.MakeSound(transform.position, soundMaker._soundDistance);    
            AudioSystem.AudioSystem.Instance.PlaySFX(soundMaker._soundName, transform.position);        

            GameObject ripple = Instantiate(FootstepRippleEffect, transform.position, transform.rotation);
            ripple.transform.localScale = ripple.transform.localScale * soundMaker._soundDistance;

            Destroy(soundMaker.gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
        }
    }
}