using femjami.runtime;
using UnityEngine;

namespace Project.BehaviourTree.Runtime
{
    public class LookAtRandomPosition : ActionNode
    {
        private float direction;
        protected override void OnStart()
        {
            direction = UnityEngine.Random.Range(0, 360);
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {

            if (_blackboard._npcData == null) return State.Failure;

            if (_blackboard._npcData.LookAtPosition(direction))
                return State.Success;
            else
                return State.Running;
        }
    }
}
