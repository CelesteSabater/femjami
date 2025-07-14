namespace Project.BehaviourTree.Runtime
{
    public class IsInHighAlert : ConditionalNode
    {
        protected override bool Question() => _blackboard._highAlert;
    }
}