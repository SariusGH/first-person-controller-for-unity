using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    /// <summary>
    ///     Controller that handles the character controls and camera controls of the first person player.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    //[RequireComponent(typeof(CharacterBehaviour))] // old approach
    public class FirstPersonController : MonoBehaviour, ICharacterSignals
    {
        #region Character Signals

        public IObservable<Vector3> Moved => _moved;
        private Subject<Vector3> _moved;

        public ReactiveProperty<bool> IsRunning => _isRunning;
        private ReactiveProperty<bool> _isRunning;

        public IObservable<Unit> Landed => _landed;
        private Subject<Unit> _landed;

        public IObservable<Unit> Jumped => _jumped;
        private Subject<Unit> _jumped;

        public IObservable<Unit> Stepped => _stepped;
        private Subject<Unit> _stepped;

        public ReactiveProperty<bool> IsCrouching => _isCrouching;
        private ReactiveProperty<bool> _isCrouching;

        public ReactiveProperty<Vector3> LocalCameraPos => _localCameraPos;
        private ReactiveProperty<Vector3> _localCameraPos;

        public ReactiveProperty<AnimationEnum> AnimationState => _animationState;
        private ReactiveProperty<AnimationEnum> _animationState;

        #endregion

        #region Configuration

        [Header("References")]
        [SerializeField] private FirstPersonControllerInput firstPersonControllerInput;
        private CharacterController _characterController;
        private Camera _camera;
        //[SerializeField] private CharacterBehaviour _characterBehaviour;
        [SerializeField] private MovementInputHandler _movementInputHandler;
        [SerializeField] private ExternalInputHandler _externalInputHandler;

        [Header("Locomotion Properties")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float crouchSpeed = 2f;
        [SerializeField] private float jumpForceMagnitude = 10f;
        [SerializeField] private float strideLength = 4f;
        public float StrideLength => strideLength; // here
        [SerializeField] private float stickToGroundForceMagnitude = 5f;

        [Header("Look Properties")]
        [Range(-90, 0)] [SerializeField] private float minViewAngle = -60f;
        [Range(0, 90)] [SerializeField] private float maxViewAngle = 60f;

        #endregion

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _camera = GetComponentInChildren<Camera>();

            _isRunning = new ReactiveProperty<bool>(false);
            _moved = new Subject<Vector3>().AddTo(this);
            _jumped = new Subject<Unit>().AddTo(this);
            _landed = new Subject<Unit>().AddTo(this);
            _stepped = new Subject<Unit>().AddTo(this);
            _isCrouching = new ReactiveProperty<bool>(false); // here
            _localCameraPos = new ReactiveProperty<Vector3>(_camera.transform.localPosition); // here
        }

        private void Start()
        {
            // intent input
            HandleInput();

            // approach from base project
            // HandleLocomotion();

            // approach from base project
            HandleLook();

            // behaviour and state machine approach
            // HandleCharacterBehaviour();

            HandleSteppedCharacterSignal();
        }

        // input is handled by using input intents
        private void HandleInput()
        {
            // Ensures the first frame counts as "grounded".
            _characterController.Move(-stickToGroundForceMagnitude * transform.up);

            var inputStream = _movementInputHandler.MovementIntention
                .Zip(_externalInputHandler.ExternalInput, (m, e) => new Tuple<Structs.MovementIntention, Structs.ExternalInput>(m, e));

            inputStream.Subscribe(inputT =>
            {
                // no movment if frozen
                if (inputT.Item2.IsFrozen)
                {
                    _animationState.Value = AnimationEnum.Frozen;
                    return;
                }

                // base values
                var verticalVelocity = 0f;
                var currentSpeed = 0f;

                // combine movment intent with external input
                switch (inputT.Item1.LocomotionState)
                {
                    case LocomotionEnum.Idle: // could be falling here
                        if (inputT.Item2.HasLadder /*&& Input for interacting*/) // can go on ladder while falling
                        {
                            _animationState.Value = AnimationEnum.Ladder;
                        }
                        else if (!inputT.Item1.Grounded)
                        {
                            _animationState.Value = AnimationEnum.Fall;
                        }
                        else if (inputT.Item2.HasDoor /*&& *Input for interacting*/) // can not go into door while falling
                        {
                            _animationState.Value = AnimationEnum.Door;
                        }
                        else
                        {
                            _animationState.Value = AnimationEnum.Idle;
                        }
                        break;

                    case LocomotionEnum.Walk: // could be falling here
                        if (inputT.Item2.HasLadder /*&& Input for interacting*/) // can go on ladder while falling
                        {
                            _animationState.Value = AnimationEnum.Ladder;
                        }
                        else if (!inputT.Item1.Grounded) // falling
                        {
                            verticalVelocity = _characterController.velocity.y + Physics.gravity.y * Time.deltaTime * 3.0f;
                            _animationState.Value = AnimationEnum.Fall;
                        }
                        else if (inputT.Item2.HasDoor /*&& *Input for interacting*/) // can not go into door while falling
                        {
                            _animationState.Value = AnimationEnum.Door;
                            verticalVelocity = -Mathf.Abs(stickToGroundForceMagnitude);
                        }
                        else
                        {
                            verticalVelocity = -Mathf.Abs(stickToGroundForceMagnitude);
                        }
                        currentSpeed = walkSpeed;
                        break;

                    case LocomotionEnum.Jump: // could be falling here
                        if (inputT.Item1.Grounded) // first jump frame
                        {
                            if (inputT.Item2.HasJumpboost)
                            {
                                verticalVelocity = jumpForceMagnitude * 2;
                            } else
                            {
                                verticalVelocity = jumpForceMagnitude;
                            }
                            _jumped.OnNext(Unit.Default);
                            _animationState.Value = AnimationEnum.Jump;
                        }
                        else // not first jump frame
                        {
                            if (inputT.Item2.HasLadder /*&& Input for interacting*/) // can go on ladder mid jump or while falling
                            {
                                _animationState.Value = AnimationEnum.Ladder;
                            } else
                            {
                                _animationState.Value = AnimationEnum.Jump;
                            }
                        }
                        currentSpeed = walkSpeed;
                        break;

                    case LocomotionEnum.Crouch:  // could not be falling here
                        currentSpeed = crouchSpeed;
                        break;

                    case LocomotionEnum.Run:  // could not be falling here
                        currentSpeed = runSpeed;
                        break;
                }

                var horizontalVelocity = inputT.Item1.HorizontalInput * currentSpeed; //Calculate velocity (direction * speed).

                // Combine horizontal and vertical movement.
                var characterVelocity = transform.TransformVector(new Vector3(
                    horizontalVelocity.x,
                    verticalVelocity,
                    horizontalVelocity.y));

                // Apply movement.
                var motion = characterVelocity * Time.deltaTime;
                _characterController.Move(motion);

                // rest of signals here

                // landed?
                if (!inputT.Item1.Grounded && _characterController.isGrounded)
                {
                    _landed.OnNext(Unit.Default);
                }
                
                // normal step on ground?
                if (inputT.Item1.Grounded && _characterController.isGrounded)
                {
                    _moved.OnNext(_characterController.velocity * Time.deltaTime);
                }

                // crouching?
                if (inputT.Item1.LocomotionState == LocomotionEnum.Crouch)
                {
                    _isCrouching.Value = true;
                }
                else
                {
                    _isCrouching.Value = false;
                }

                // running?
                if (inputT.Item1.LocomotionState == LocomotionEnum.Run)
                {
                    _isRunning.Value = true;
                }
                else
                {
                    _isRunning.Value = false;
                }

            }).AddTo(this);
        }

        // old character behaviour approach
        private void HandleCharacterBehaviour()
        {
            // forwarding the Input to the Character Behaviour to handle it
            //_characterBehaviour.FirstPersonControllerInput = firstPersonControllerInput;

            // pass unity character controller for now
            //_characterBehaviour.CharacterController = _characterController;

            // takine the resulting motion and apply it to the Unity Character Controller --------------- change later to decide Modular
            //_characterBehaviour.MotionInput.Subscribe(motionInput => _characterController.Move(motionInput)).AddTo(this);
        }

        // approach from the base project
        private void HandleLocomotion()
        {
            // Ensures the first frame counts as "grounded".
            _characterController.Move(-stickToGroundForceMagnitude * transform.up);

            // Create a jump latch for sync + map from events to true/false values.
            var jumpLatch = LatchObservables.Latch(this.UpdateAsObservable(), firstPersonControllerInput.Jump, false);

            // Handle move:
            firstPersonControllerInput.Move
                .Zip(jumpLatch, (m, j) => new MoveInputData(m, j))
                .Where(moveInputData => moveInputData.Jump ||
                                        moveInputData.Move != Vector2.zero ||
                                        _characterController.isGrounded == false)
                .Subscribe(i =>
                {
                    var wasGrounded = _characterController.isGrounded;
                    
                    // Vertical movement:
                    var verticalVelocity = 0f;
                    // The character is ...
                    if (i.Jump && wasGrounded)
                    {
                        // ... grounded and wants to jump.
                        
                        verticalVelocity = jumpForceMagnitude;
                        _jumped.OnNext(Unit.Default);
                    }
                    else if (!wasGrounded)
                    {
                        // ... in the air.
                        
                        verticalVelocity = _characterController.velocity.y + Physics.gravity.y * Time.deltaTime * 3.0f;
                    }
                    else
                    {
                        // ... otherwise grounded.
                        
                        verticalVelocity = -Mathf.Abs(stickToGroundForceMagnitude);
                    }

                    // Horizontal movement:
                    // var currentSpeed = firstPersonControllerInput.Crouch.Value ? crouchSpeed : walkSpeed; // here
                    var currentSpeed = IsCrouching.Value ? crouchSpeed : walkSpeed; // here
                    currentSpeed = firstPersonControllerInput.Run.Value ? runSpeed : currentSpeed; // here, was normally walkspeed as default
                    var horizontalVelocity = i.Move * currentSpeed; //Calculate velocity (direction * speed).

                    // Combine horizontal and vertical movement.
                    var characterVelocity = transform.TransformVector(new Vector3(
                        horizontalVelocity.x,
                        verticalVelocity,
                        horizontalVelocity.y));

                    // Apply movement.
                    var motion = characterVelocity * Time.deltaTime;
                    _characterController.Move(motion);

                    // Set ICharacterSignals output signals related to the movement.
                    HandleLocomotionCharacterSignalsIteration(wasGrounded, _characterController.isGrounded);
                }).AddTo(this);
        }

        // side methode of base project approach
        private void HandleLocomotionCharacterSignalsIteration(bool wasGrounded, bool isGrounded)
        {
            var tempIsRunning = false;
            // var tempIsCrouching = false; // here

            if (wasGrounded && isGrounded)
            {
                // The character was grounded at the beginning and end of this frame.

                _moved.OnNext(_characterController.velocity * Time.deltaTime); // does this imply we only update Moved when on ground??? -> yes and therefore also only have headbob on ground, but bad backwards connection, right?

                if (_characterController.velocity.magnitude > 0)
                {
                    // The character is running if the input is active and
                    // the character is actually moving on the ground
                    tempIsRunning = firstPersonControllerInput.Run.Value;
                    
                    // here
                    if (!tempIsRunning)
                    {
                        // The character is actually moving on the ground
                        // but he is not running
                        // tempIsCrouching = firstPersonControllerInput.Crouch.Value; // maybe rework as it only effects when crouching but not while jumping, but maybe does when back on ground which would make sence -> just test
                    }
                } else // here
                {
                    // The character is actually on the ground
                    // even if run is pressed, he does not move or run but can crouch
                    // tempIsCrouching = firstPersonControllerInput.Crouch.Value;
                }
            }

            if (!wasGrounded && isGrounded)
            {
                // The character was airborne at the beginning, but grounded at the end of this frame.
                
                _landed.OnNext(Unit.Default);
            }

            _isRunning.Value = tempIsRunning;
            // _isCrouching.Value = tempIsCrouching;
            // here
            if (tempIsRunning)
            {
                _isCrouching.Value = false;
            }
        }

        private void HandleSteppedCharacterSignal()
        {
            // Emit stepped events:
            
            var stepDistance = 0f;
            Moved.Subscribe(w =>
            {
                stepDistance += w.magnitude;

                // changed here to have different stride lenghts
                var strideLength2 = strideLength;
                if (IsRunning.Value == true)
                {
                    strideLength2 *= 2;
                }
                else
                if (IsCrouching.Value == true)
                {
                    strideLength2 /= 2;
                }

                if (stepDistance > strideLength2)
                {
                    _stepped.OnNext(Unit.Default);
                }

                stepDistance %= strideLength2;
            }).AddTo(this);
        }

        private void HandleLook()
        {
            firstPersonControllerInput.Look
                .Where(v => v != Vector2.zero)
                .Subscribe(inputLook =>
                {
                    // Translate 2D mouse input into euler angle rotations.

                    // Horizontal look with rotation around the vertical axis, where + means clockwise.
                    var horizontalLook = inputLook.x * Vector3.up * Time.deltaTime;
                    transform.localRotation *= Quaternion.Euler(horizontalLook);

                    // Vertical look with rotation around the horizontal axis, where + means upwards.
                    var verticalLook = inputLook.y * Vector3.left * Time.deltaTime;
                    var newQ = _camera.transform.localRotation * Quaternion.Euler(verticalLook);
                    
                    _camera.transform.localRotation =
                        RotationTools.ClampRotationAroundXAxis(newQ, -maxViewAngle, -minViewAngle);
                }).AddTo(this);

            // here
            firstPersonControllerInput.Crouch.Subscribe(v => _isCrouching.Value = v);
            IsCrouching // a lot to change here, maybe even move to different scrip as effect, just a test
                .Subscribe(v =>
                {
                    if (v)
                    {
                        _camera.transform.localPosition -= Vector3.up * _characterController.height / 2.0f;
                        jumpForceMagnitude /= 2.0f;
                    } else
                    {
                        _camera.transform.localPosition += Vector3.up * _characterController.height / 2.0f;
                        jumpForceMagnitude *= 2.0f;
                    }
                    _localCameraPos.Value = _camera.transform.localPosition;
                }).AddTo(this);
            _camera.transform.localPosition -= Vector3.up * _characterController.height / 2.0f; // needed to everride the initial set of IsCrouching to false
            jumpForceMagnitude /= 2.0f; // change later, as causes inti thing to flicker at first move, cause fix is to early
        }

        public struct MoveInputData
        {
            public readonly Vector2 Move;
            public readonly bool Jump;

            public MoveInputData(Vector2 move, bool jump)
            {
                Move = move;
                Jump = jump;
            }
        }
    }
}