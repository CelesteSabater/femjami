using System.Collections.Generic;
using UnityEngine;
using femjami.DialogueTree.Runtime;

namespace DialogueTree.Runtime
{
    public class Test : ActionNode
    {
        protected override void StartAction() { }

        protected override void EndAction()
        {
            Debug.Log("Test");
        }
    }
}