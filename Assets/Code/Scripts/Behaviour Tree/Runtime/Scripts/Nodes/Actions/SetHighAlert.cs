using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.BehaviourTree.Runtime
{
    public class SetHighAlert : ActionNode
    {
        [SerializeField] private bool state;
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (_blackboard._npcData == null) return State.Failure;

            _blackboard._npcData.SetHighAlert(state);
            return State.Success;
        }
    }
}
