using System;
using System.Collections.Generic;
using UnityEngine;
using femjami.runtime;

namespace Project.BehaviourTree.Runtime
{
    [System.Serializable]
    public class Blackboard
    {
        public GameObject _gameObject;
        public Vector3 _nextPatrolPoint, _lastPlayerPosition;
        public Transform _playerTransform;
        public bool _highAlert, _investigateArea;
        public NPCData _npcData;
    }
}
