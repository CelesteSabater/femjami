using femjami.DialogueTree.Runtime;
using femjami.UI;
using UnityEngine.SceneManagement;

namespace DialogueTree.Runtime
{
    public class ChangeScene : ActionNode
    {
        protected override void StartAction() 
        { 
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        protected override void EndAction() { }
    }
}