namespace Project.BehaviourTree.Runtime
{
    public class IsPlayerVisible : ConditionalNode
    {
        public bool test;
        protected override bool Question() => test;
    }
}