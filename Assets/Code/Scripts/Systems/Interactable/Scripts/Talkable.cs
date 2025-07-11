using TMPro;
using UnityEngine;
using femjami.DialogueTree.Runtime;
using femjami.Managers;

namespace femjami.Systems.Interactable
{
    public class Talkable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject _promptPrefab;
        [SerializeField] private Transform _promtLocation;
        [SerializeField] private bool _isActive = true;

        public GameObject PromptPrefab => _promptPrefab;
        public Transform PromptLocation => _promtLocation;

        [SerializeField] private femjami.DialogueTree.Runtime.DialogueTree _dialogueTree;
        [SerializeField] private femjami.DialogueTree.Runtime.NPCData _npcData;

        public bool IsActive() => _isActive;
        public bool SetActive(bool active) => _isActive = active;
        private bool delay = false;

        private GameObject _promptGo;

        private void Start()
        {
            DialogueEvents.current.onSetDialogueNPC += OnSetDialogueNPC;
        }

        private void OnDestroy()
        {
            DialogueEvents.current.onSetDialogueNPC -= OnSetDialogueNPC;
            if (_promptGo != null)
            {
                Destroy(_promptGo);
            }
        }

        private void OnSetDialogueNPC(femjami.DialogueTree.Runtime.DialogueTree dialogueTree, NPCData npcData)
        {
            if (npcData != _npcData)
                return;

            _dialogueTree = dialogueTree;
        }

        public bool Interact(Interactor interactor)
        {
            if (!delay)
            { 
                GameEvents.current.SetDialogue(true);
                DialogueSystem.Instance.StartDialogue(_dialogueTree, _npcData);
            }
                
            delay = true;
            return true;
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
            if (_promptGo != null)
                return;

            delay = false;
            _promptGo = Instantiate(_promptPrefab, PromptLocation.position, Quaternion.identity);
        }

        private void DeletePrompt()
        {
            if (_promptGo == null)
                return;

            Destroy(_promptGo);
        }
    }
}