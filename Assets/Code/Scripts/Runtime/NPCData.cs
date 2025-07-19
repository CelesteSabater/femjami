using System;
using System.Collections;
using System.Collections.Generic;
using femjami.Managers;
using femjami.Systems.AudioSystem;
using Project.BehaviourTree.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace femjami.runtime
{
    public class NPCData : MonoBehaviour
    {
        [Header("Patrol data")]
        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private int _currentObjectivePoint = 0;
        [SerializeField] private float _meleeDistance = 1f;
        [SerializeField] private float _investigateArea = 5f;
        [SerializeField] private float _walkSpeed = 2f, _runSpeed = 7f;
        private NavMeshAgent _agent;
        private Animator _animator;
        private bool _highAlert = false;

        public bool GetHighAlert() => _highAlert;
        public void SetHighAlert(bool b)
        {
            if (_highAlert == b) return;
            
            _highAlert = b;

            if (_highAlert) SpawnAlertEffects();
            else DeSpawnAlertEfffects();
        }

        [Header("Visual Effects")]
        [SerializeField] private GameObject _alertPrefab;
        private GameObject _alertGO;
        [SerializeField] private Transform _alertLocation;
        [SerializeField] private string _alertSound;
        public AudioClip[] FootstepAudioClips;
        [SerializeField] private  GameObject FootstepRippleEffect;
        [SerializeField] private AnimationCurve SoundEffectsBySpeed;

        private void Start()
        {
            if (!_agent) _agent = GetComponent<NavMeshAgent>();
            if (!_animator) _animator = GetComponent<Animator>();
            GameEvents.current.onMakeSound += ReciveSound;
        }

        private void OnDestroy()
        {
            GameEvents.current.onMakeSound -= ReciveSound;
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
            GoToPosition(destination);

            return destination;
        }

        public bool GoToPosition(UnityEngine.Vector3 vector3)
        {
            if (!_agent) return false;

            NavMeshPath navMeshPath = new NavMeshPath();

            if (!_agent.CalculatePath(vector3, navMeshPath) || navMeshPath.status != NavMeshPathStatus.PathComplete)
                return false;

            _agent.destination = vector3;
            _agent.speed = _walkSpeed;

            float speedMultiplier = 0.5f;
            if (_highAlert)
            {
                speedMultiplier = 1;
                _agent.speed = _runSpeed;
            }

            var dir = (_agent.steeringTarget - transform.position).normalized;
            var animDir = transform.InverseTransformDirection(dir);

            transform.rotation = UnityEngine.Quaternion.RotateTowards(transform.rotation, UnityEngine.Quaternion.LookRotation(dir), 180 * Time.deltaTime);

            if (!_animator) return true;
            _animator.SetFloat("Horizontal", animDir.x * speedMultiplier, .5f, Time.deltaTime);
            _animator.SetFloat("Vertical", animDir.z * speedMultiplier, .5f, Time.deltaTime);
            
            return true;
        }

        public bool LookAtPosition(float direction)
        {
            direction = Mathf.Repeat(direction, 360f);

            Vector3 targetDirection = Quaternion.Euler(0, direction, 0) * Vector3.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                180 * Time.deltaTime
            );

            if (_animator)
            {
                _animator.SetFloat("Horizontal", 0.5f, 0.25f, Time.deltaTime);
                _animator.SetFloat("Vertical", 0, 0.25f, Time.deltaTime);
            }   
            
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            if (angleDifference < 1f)
                _animator.SetFloat("Horizontal", 0, 0.5f, Time.deltaTime);

            return angleDifference < 1f;
        }

        public void ClearGoToPosition()
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

        public void SpawnAlertEffects()
        {
            if (!_highAlert) return;

            _alertGO = Instantiate(_alertPrefab, _alertLocation);
            _alertGO.transform.parent = _alertLocation;
            AudioSystem.Instance.PlaySFX(_alertSound, _alertLocation.position, false);
        }

        public void DeSpawnAlertEfffects()
        {
            if (_highAlert) return;

            Animator animator = _alertGO.GetComponent<Animator>();
            if (animator) animator.Play("UnPop");
        }

        public void ReciveSound(Vector3 soundOrigin, float maxDistance)
        {
            float distance = Vector3.Distance(transform.position, soundOrigin);
            if (distance > maxDistance) return;

            bool SOUND_DETECTED = true;
            SetHighAlert(SOUND_DETECTED);

            BehaviourTree behaviourTree = GetComponent<BehaviourTreeRunner>().GetTree();
            behaviourTree._blackboard._lastPlayerPosition = soundOrigin;
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, _agent.speed / _runSpeed * AudioData._sfxVolume);
                    GameObject ripple = Instantiate(FootstepRippleEffect, transform.position, transform.rotation);
                    float scale = SoundEffectsBySpeed.Evaluate(_agent.speed);
                    ripple.transform.localScale = ripple.transform.localScale * scale;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 1f);
            Gizmos.DrawWireSphere(transform.position, _meleeDistance);
            Gizmos.DrawWireSphere(transform.position, _investigateArea);
        }
    }
}