﻿using UnityEngine;
using femjami.Managers;
using System.Linq;
using System.Collections;
using UnityEngine.AI;
using TMPro;
using femjami.Systems.AudioSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; 
using Cinemachine.PostFX;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]

        [Tooltip("Sneak speed of the character in m/s")]
        public float SneakSpeed = 1.0f;

        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        public CinemachineVolumeSettings cinemachineVolumeSettings;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Header("Smell Tracking")]
        [SerializeField] private Transform[] targets;
        [SerializeField] private GameObject smellParticles;
        [SerializeField] private float distanceBetweenParticles = 1.0f;
        [SerializeField] private float timeBetweenInstantiates = 1.0f;
        [SerializeField] private float multiplierToDestroy = 10;
        private float timeCounter = 0;

        [Header("Listening Mode")]
        [SerializeField] private Camera auxiliarCamera;
        [SerializeField] private  GameObject FootstepRippleEffect;
        [SerializeField] private AnimationCurve SoundEffectsBySpeed;
        private bool _currentlyListening = false;

        [Header("Ability Cooldowns")]
        [Tooltip("Cooldown time for smell ability in seconds")]
        public float smellCooldown = 10f;
        [Tooltip("Duration for smell ability in seconds")]
        public float smellDuration = 5f;
        [Tooltip("Cooldown time for listen ability in seconds")]
        public float listenCooldown = 8f;
        [Tooltip("Duration for listen ability in seconds")]
        public float listenDuration = 4f;

        [Header("UI References")]
        [Tooltip("Slider for smell cooldown")]
        public UnityEngine.UI.Slider smellCooldownSlider;
        [Tooltip("Slider for listen cooldown")]
        public UnityEngine.UI.Slider listenCooldownSlider;
        [Tooltip("Slider for sneak state")]
        public UnityEngine.UI.Slider sneakSlider;
        [Tooltip("Text for smell cooldown")]
        public TextMeshProUGUI smellCooldownText;
        [Tooltip("Text for listen cooldown")]
        public TextMeshProUGUI listenCooldownText;

        // Variables para cooldowns
        private float currentSmellCooldown = 0f;
        private float currentListenCooldown = 0f;
        private float smellActiveTime = 0f;
        private float listenActiveTime = 0f;
        private bool isSmellActive = false;
        private bool isListenActive = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDSneak;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        private bool _inDialogue = false;
        private void SetInDialogue(bool value)
        {
            _inDialogue = value;
        }

        private bool _gameOver = false;

        private void GameOver() => _gameOver = true;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            GameEvents.current.onSetDialogue += SetInDialogue;
            GameEvents.current.onLoseGame += GameOver;
        }

        private void OnDestroy()
        {
            GameEvents.current.onSetDialogue -= SetInDialogue;
            GameEvents.current.onLoseGame -= GameOver;
        }

        private void Update()
        {
            if (_inDialogue || _gameOver || MenuSystem.Instance.GetIsPaused())
                return;

            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();

            UpdateCooldowns();
            UpdateUI();

            UpdateSmell();
            UpdateListen();
        }

        private void LateUpdate()
        {
            if (_inDialogue || _gameOver || MenuSystem.Instance.GetIsPaused())
            {
                Cursor.lockState = CursorLockMode.None;
                _input.cursorLocked = false;
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            _input.cursorLocked = true;

            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDSneak = Animator.StringToHash("Sneak");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.sneak) targetSpeed = SneakSpeed;
            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                _animator.SetBool(_animIDSneak, _input.sneak);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), _speed / SprintSpeed * AudioData._sfxVolume);
                    GameObject ripple = Instantiate(FootstepRippleEffect, transform.position, transform.rotation);
                    float scale = SoundEffectsBySpeed.Evaluate(_speed);
                    ripple.transform.localScale = ripple.transform.localScale * scale;
                    GameEvents.current.MakeSound(transform.position, scale);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), _speed / SprintSpeed * AudioData._sfxVolume);
                GameObject ripple = Instantiate(FootstepRippleEffect, transform.position, transform.rotation);
                ripple.transform.localScale = ripple.transform.localScale * 30;
                GameEvents.current.MakeSound(transform.position, 30);
            }
        }

        private void UpdateCooldowns()
        {
            if (currentSmellCooldown > 0)
            {
                currentSmellCooldown -= Time.deltaTime;
            }

            if (isSmellActive)
            {
                smellActiveTime -= Time.deltaTime;
                if (smellActiveTime <= 0)
                {
                    isSmellActive = false;
                    currentSmellCooldown = smellCooldown;
                }
            }

            if (currentListenCooldown > 0)
            {
                currentListenCooldown -= Time.deltaTime;
            }

            if (isListenActive)
            {
                listenActiveTime -= Time.deltaTime;
                if (listenActiveTime <= 0)
                {
                    StopListenMode();
                    isListenActive = false;
                    currentListenCooldown = listenCooldown;
                }
            }
        }

        private void UpdateSmell()
        {
            if (StarterAssetsInputs.Instance.smell && currentSmellCooldown <= 0 && !isSmellActive)
            {
                isSmellActive = true;
                smellActiveTime = smellDuration;
            }

            if (!isSmellActive)
                return;
                
            if (timeCounter > 0)
            {
                timeCounter -= Time.deltaTime;
                return;
            }

            for (int i = 0; i < targets.Count(); i++)
            {
                if (targets[i] != null)
                    GenerateSmeellPath(targets[i]);
            }

            timeCounter = timeBetweenInstantiates;
        }

        private void UpdateListen()
        {
            if (StarterAssetsInputs.Instance.listen && currentListenCooldown <= 0 && !isListenActive)
            {
                isListenActive = true;
                listenActiveTime = listenDuration;   
            }
            
            if (isListenActive)
            {
                StartListenMode();
            }
            else if (!isListenActive)
            {
                StopListenMode();
            }
        }

        private void UpdateUI()
        {
            if (isSmellActive)
            {
                smellCooldownSlider.value = 1 - (smellActiveTime / smellDuration);
                smellCooldownText.text = Mathf.Ceil(smellActiveTime).ToString();
            }
            else if (currentSmellCooldown > 0)
            {
                smellCooldownSlider.value = currentSmellCooldown / smellCooldown;
                smellCooldownText.text = Mathf.Ceil(currentSmellCooldown).ToString();
            }
            else
            {
                smellCooldownSlider.value = 0;
                smellCooldownText.text = "";
            }

            if (isListenActive)
            {
                listenCooldownSlider.value = 1 - (listenActiveTime / listenDuration);
                listenCooldownText.text = Mathf.Ceil(listenActiveTime).ToString();
            }
            else if (currentListenCooldown > 0)
            {
                listenCooldownSlider.value = currentListenCooldown / listenCooldown;
                listenCooldownText.text = Mathf.Ceil(currentListenCooldown).ToString();
            }
            else
            {
                listenCooldownSlider.value = 0;
                listenCooldownText.text = "";
            }

            sneakSlider.value = _input.sneak ? 0 : 1;
        }

        private void GenerateSmeellPath(Transform target)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
            {
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    Vector3 start = path.corners[i];
                    Vector3 end = path.corners[i + 1];
                    float segmentDistance = Vector3.Distance(start, end);
                    int particlesInSegment = (int)(segmentDistance / distanceBetweenParticles);

                    for (int j = 0; j < particlesInSegment; j++)
                    {
                        Vector3 point = Vector3.Lerp(start, end, j / (float)particlesInSegment);
                        GameObject go = Instantiate(smellParticles, point, Quaternion.identity);
                        go.transform.LookAt(target);
                        StartCoroutine(Destroy(go, timeBetweenInstantiates * multiplierToDestroy));
                    }
                }
            }
        }

        IEnumerator Destroy(GameObject go, float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(go);
        }

        void StartListenMode()
        {
            if (_currentlyListening) return;
            _currentlyListening = true;
            auxiliarCamera.gameObject.SetActive(true);
            if (cinemachineVolumeSettings.m_Profile.TryGet(out ColorAdjustments colorAdjustments))
            {
                colorAdjustments.saturation.value = Mathf.Lerp(
                    colorAdjustments.saturation.value,
                    -100,
                    Time.deltaTime * 5f
                );
            }
        }

        void StopListenMode()
        {
            if (!_currentlyListening) return;
            _currentlyListening = false;
            auxiliarCamera.gameObject.SetActive(false);
            if (cinemachineVolumeSettings.m_Profile.TryGet(out ColorAdjustments colorAdjustments))
            {
                colorAdjustments.saturation.value = Mathf.Lerp(
                    colorAdjustments.saturation.value,
                    -8,
                    Time.deltaTime * 5f
                );
            }
        }
    }
}