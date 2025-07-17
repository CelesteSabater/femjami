namespace Project.BehaviourTree.Runtime
{
    public class IsPlayerVisible : ConditionalNode
    {
        protected override bool Question()
        {
            if (!_blackboard._fieldOfView) return false;
            return _blackboard._fieldOfView.canSeePlayer;
        }
    }
}