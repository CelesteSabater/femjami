using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.BehaviourTree.Runtime
{
    public class RandomRepeat : DecoratorNode
    {
        [SerializeField] private int _minNumberOfRepeats = 2;
        [SerializeField] private int _maxNumberOfRepeats = 5;
        private int _numberOfRepeats;
        private int _repeatCount;

        protected override void OnStart()
        { 
            _repeatCount = 0;
            _numberOfRepeats = Random.Range(_minNumberOfRepeats, _maxNumberOfRepeats);
        } 

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            SetState(State.Running);
            if (_numberOfRepeats != 0)
            {
                if (_repeatCount < _numberOfRepeats)
                {
                    _child.Update();
                    if (_child.GetState() == State.Success) _repeatCount++;
                    if (_child.GetState() == State.Failure) SetState(State.Failure);
                }
                else SetState(State.Success);
            }
            else _child.Update();

            return GetState();
        }
    }
}