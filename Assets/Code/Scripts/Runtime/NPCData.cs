using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace femjami.runtime
{
    public class NPCData : MonoBehaviour
    {
        [Header("Patrol data")]
        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private int _currentObjectivePoint = 0;
        private NavMeshAgent _agent;
        private Animator _animator;
        [SerializeField] private float _meleeDistance = 1f;
        [SerializeField] private float _investigateArea = 5f;
        [SerializeField] private float _walkSpeed = 2f, _runSpeed = 7f;
        

        private void Start()
        {
            if (!_agent) _agent = GetComponent<NavMeshAgent>();
            if (!_animator) _animator = GetComponent<Animator>();
        }

        public Vector3 GetInvestigatePosition()
        {
            float x = UnityEngine.Random.Range(-_investigateArea, _investigateArea);
            float z = UnityEngine.Random.Range(-_investigateArea, _investigateArea);
            return new Vector3(x + transform.position.x, transform.position.y, z + transform.position.z);
        }

        public Vector3 GetNextPatrolPoint()
        {
            _currentObjectivePoint += 1;
            return _patrolPoints[_currentObjectivePoint % _patrolPoints.Length].transform.position;
        }

        public Vector3 ChasePlayer()
        {
            Vector3 destination = GameObject.FindGameObjectWithTag("Player").transform.position;
            bool IS_RUNNING = true;
            GoToPosition(destination, IS_RUNNING);

            return destination;
        }

        public void GoToPosition(UnityEngine.Vector3 vector3, bool run = false)
        {
            if (!_agent) return;

            _agent.destination = vector3;
            _agent.speed = _walkSpeed;
            float speedMultiplier = 0.5f;
            if (run)
            {
                speedMultiplier = 1;
                _agent.speed = _runSpeed;
            }            

            var dir = (_agent.steeringTarget - transform.position).normalized;
            var animDir = transform.InverseTransformDirection(dir);

            transform.rotation = UnityEngine.Quaternion.RotateTowards(transform.rotation, UnityEngine.Quaternion.LookRotation(dir), 180 * Time.deltaTime);

            if (!_animator) return;
            _animator.SetFloat("Horizontal", animDir.x * speedMultiplier, .5f, Time.deltaTime);
            _animator.SetFloat("Vertical", animDir.z * speedMultiplier, .5f, Time.deltaTime);
        }

        public bool LookAtPosition(float direction)
        {
            direction = Math.Clamp(direction, 0, 360);
            var dir = (new Vector3(transform.position.x, direction, transform.position.z) - transform.position).normalized;
            var animDir = transform.InverseTransformDirection(dir);

            transform.rotation = UnityEngine.Quaternion.RotateTowards(transform.rotation, UnityEngine.Quaternion.LookRotation(dir), 180 * Time.deltaTime);

            if (_animator)
            {
                _animator.SetFloat("Horizontal", 0, .5f, Time.deltaTime);
                _animator.SetFloat("Vertical", 0, .5f, Time.deltaTime);
            }

            if (dir == Vector3.zero)
                return true;
            return false;
        }

        private void ClearGoToPosition()
        {
            if (!_agent) return;
            _agent.ResetPath();

            if (!_animator) return;
            _animator.SetFloat("Horizontal", 0, .25f, Time.deltaTime);
            _animator.SetFloat("Vertical", 0, .25f, Time.deltaTime);
        }

        public bool IsNPCInLocation(Vector3 location)
        {
            float distance = Vector3.Distance(transform.position, location);
            bool inPosition = distance <= _meleeDistance;

            if (inPosition)
                ClearGoToPosition();

            return inPosition;
        }

        public bool IsPlayerInMelee()
        {
            Vector3 destination = GameObject.FindGameObjectWithTag("Player").transform.position;
            return IsNPCInLocation(destination);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 1f);
            Gizmos.DrawWireSphere(transform.position, _meleeDistance);
            Gizmos.DrawWireSphere(transform.position, _investigateArea);
        }
    }
}