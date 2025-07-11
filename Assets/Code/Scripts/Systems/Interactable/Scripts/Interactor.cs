using System.Linq;
using StarterAssets;
using UnityEngine;

namespace femjami.Systems.Interactable
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private Transform _interactionPoint;
        [SerializeField] private float _interactionPointRadius = 0.5f;
        [SerializeField] private LayerMask _interactableMask;

        private Collider[] _collider;
        private IInteractable _interactable;

        private void Update()
        {
            CheckInteractable();
            Interact();
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
            } else
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
        }
    }
}
