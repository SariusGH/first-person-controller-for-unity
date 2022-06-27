using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    public class MovementInputHandler : MonoBehaviour
    {
        // old implementation using motion input state machine

        // State Machine to calculate Movement Intent
        public StateMachineManager StateMachine => _stateMachine; // same as with Movement Input
        private StateMachineManager _stateMachine;

        // Movement Input Struct used by States
        public Structs.MovementInput MovementInput => _movementInput; // not commented out to prevent errors in still existing state machines, would remove state machines and all the old code here on an actual commit, leaving it for comletness
        private Structs.MovementInput _movementInput;

        // old Movement Intention to be returned
        //public IObservable<Structs.MovementIntention> MovementIntention => _movementIntention;
        //private Subject<Structs.MovementIntention> _movementIntention;



        // new custom observable implementation

        // new Movement Intention to be returned
        public IObservable<Structs.MovementIntention> MovementIntention;

        // reverences
        [SerializeField] private FirstPersonControllerInput _firstPersonControllerInput;
        [SerializeField] private GroundedInput _groundedInput;

        void Awake()
        {
            // old implementation using motion input state machine

            // initialize output Stream
            //_movementIntention = new Subject<Structs.MovementIntention>().AddTo(this);

            // create new State Machine and set initial State
            //_stateMachine = gameObject.AddComponent<StateMachineManager>();
            //_stateMachine.SetInitialState(new IdleMovementInputState(this, _movementIntention));

            // initialize Movement Input Struct
            //_movementInput = new Structs.MovementInput();
        }

        void Start()
        {
            // setting up combination of move inputs and calculation of movement intent
            MovementIntention = InputObservables.CombineInput(this.UpdateAsObservable(), _firstPersonControllerInput, _groundedInput).CalcState(LocomotionEnum.Idle);
        }

        // just for testing the types, not used
        void TestNewDeclaretiveStart()
        {
            IObservable<Structs.MovementInput> M1 = InputObservables.CombineInput(this.UpdateAsObservable(), _firstPersonControllerInput, _groundedInput);
            IObservable<Structs.MovementIntention> M2 = M1.CalcState(LocomotionEnum.Idle);

        }

        // moved into custom observable
        // old implementation using motion input state machine
        void OldImperialStart()
        {
            // catch Movement Input and time Grounded Update Check
            //_firstPersonControllerInput.Move.Subscribe(v2 => _movementInput.HorizontalInput = v2).AddTo(this);

            //_firstPersonControllerInput.Jump.Subscribe(_ => _groundedInput.GroundedUpdateRequest.OnNext(Unit.Default)).AddTo(this);

            //LatchObservables.Latch(this.UpdateAsObservable(), _firstPersonControllerInput.Jump, false).Subscribe(b => {
            //    _groundedInput.GroundedUpdateRequest.OnNext(Unit.Default);
            //    _movementInput.JumpInput = b;
            //}).AddTo(this);

            //_firstPersonControllerInput.Run.Subscribe(b => _movementInput.RunInput = b).AddTo(this);

            //_firstPersonControllerInput.Crouch.Subscribe(b => {
            //    _groundedInput.GroundedUpdateRequest.OnNext(Unit.Default);
            //    _movementInput.CrouchInput = b;
            //}).AddTo(this);

            //_groundedInput.Grounded.Subscribe(b => _movementInput.Grounded = b).AddTo(this);
        }
    }
}
