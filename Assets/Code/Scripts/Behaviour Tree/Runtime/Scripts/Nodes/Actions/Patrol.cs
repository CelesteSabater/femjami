using femjami.runtime;
using UnityEngine;

namespace Project.BehaviourTree.Runtime
{
    public class Patrol : ActionNode
    {
        protected override void OnStart()
        {
            if (_blackboard._nextPatrolPoint == Vector3.zero)
                _blackboard._nextPatrolPoint = _blackboard._npcData.GetNextPatrolPoint(); 
        }

        protected override void OnStop()
        {
            if (_blackboard._npcData) 
            _blackboard._npcData.ClearGoToPosition();
        }

        protected override State OnUpdate()
        {

            if (_blackboard._npcData == null) return State.Failure;
            
            if (!_blackboard._npcData.GoToPosition(_blackboard._nextPatrolPoint))
            {
                _blackboard._npcData.SetHighAlert(false);
                return State.Failure;
            }

            if (_blackboard._npcData.IsNPCInLocation(_blackboard._nextPatrolPoint))
            {
                _blackboard._nextPatrolPoint = _blackboard._npcData.GetNextPatrolPoint();
                return State.Success;
            }
            else
                return State.Running;
        }
    }
}
