using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    // concrete implementation of the character behaviour as state machine using soe default behaviour
    public class DefaultBehaviourStateMachine : CharacterBehaviour
    {
        // used for returning the motion after calculating everything using the State Machine
        public override IObservable<Vector3> MotionInput => _motionInput;
        private Subject<Vector3> _motionInput;

        // the State Machine
        public StateMachineManager StateMachine => _stateMachine;
        private StateMachineManager _stateMachine;

        // States used for this State Machine
        public BaseState IdleState => _idleState;
        public BaseState WalkState => _walkState;
        public BaseState SprintState => _sprintState;
        public BaseState CrouchState => _crouchState;
        public BaseState CrouchWalkState => _crouchWalkState;
        public BaseState JumpState => _jumpState;
        public BaseState JumpWalkState => _jumpWalkState;
        public BaseState JumpSprintState => _jumpSprintState;
        private BaseState _idleState, _walkState, _sprintState, _crouchState, _crouchWalkState, _jumpState, _jumpWalkState, _jumpSprintState;

        // Input to work on
        public override FirstPersonControllerInput FirstPersonControllerInput { set => _firstPersonControllerInput = value; }
        private FirstPersonControllerInput _firstPersonControllerInput;

        // reference to character controller, has set as it the reference was provided via the first person controller
        public override CharacterController CharacterController { get => _characterController; set => _characterController = value; }
        private CharacterController _characterController;

        // duplications as copies from first person controller have been made while testing
        [Header("Locomotion Properties")]
        [SerializeField] public float walkSpeed = 5f;
        [SerializeField] public float runSpeed = 10f;
        [SerializeField] public float crouchSpeed = 2f;
        [SerializeField] public float jumpForceMagnitude = 10f;
        [SerializeField] public float strideLength = 4f;
        public float StrideLength => strideLength;
        [SerializeField] public float stickToGroundForceMagnitude = 5f;

        [Header("Look Properties")]
        [Range(-90, 0)] [SerializeField] public float minViewAngle = -60f;
        [Range(0, 90)] [SerializeField] public float maxViewAngle = 60f;

        void Awake()
        {
            // initialize states
            _idleState = new IdleDefaultBehaviourState(this, _motionInput, _firstPersonControllerInput);
            _walkState = new WalkDefaultBehaviourState(this, _motionInput, _firstPersonControllerInput);
            _sprintState = new SprintDefaultBehaviourState(this, _motionInput, _firstPersonControllerInput);
            _crouchState = new CrouchDefaultBehaviourState(this, _motionInput, _firstPersonControllerInput);
            _crouchWalkState = new CrouchWalkDefaultBehaviourState(this, _motionInput, _firstPersonControllerInput);
            _jumpState = new JumpDefaultBehaviourState(this, _motionInput, _firstPersonControllerInput);
            _jumpWalkState = new JumpWalkDefaultBehaviourState(this, _motionInput, _firstPersonControllerInput);
            _jumpSprintState = new JumpSprintDefaultBehaviourState(this, _motionInput, _firstPersonControllerInput);

            // call awake methodes of states
            _idleState.AwakeState();
            _walkState.AwakeState();
            _sprintState.AwakeState();
            _crouchState.AwakeState();
            _crouchWalkState.AwakeState();
            _jumpState.AwakeState();
            _jumpWalkState.AwakeState();
            _jumpSprintState.AwakeState();

            // create new State Machine and set initial State
            _stateMachine = gameObject.AddComponent<StateMachineManager>();
            _stateMachine.SetInitialState(_idleState);

            // initialize return Stream
            _motionInput = new Subject<Vector3>().AddTo(this);
        }

        void Start()
        {
            // call the start methods of the states
            _idleState.StartState();
            _walkState.StartState();
            _sprintState.StartState();
            _crouchState.StartState();
            _crouchWalkState.StartState();
            _jumpState.StartState();
            _jumpWalkState.StartState();
            _jumpSprintState.StartState();
        }

        void Update()
        {

        }
    }
}