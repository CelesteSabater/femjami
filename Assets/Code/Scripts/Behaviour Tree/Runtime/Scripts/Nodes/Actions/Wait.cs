using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.BehaviourTree.Runtime
{
    public class Wait : ActionNode
    {
        [SerializeField] private float duration = 3;
        float startTime;

        protected override void OnStart()
        {
            startTime = 0;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            startTime += Time.deltaTime;
            if (startTime > duration) return State.Success;
            return State.Running;
        }
    }
}
