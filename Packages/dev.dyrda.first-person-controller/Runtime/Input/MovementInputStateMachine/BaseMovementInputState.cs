using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    // the specific base state for the current implementation of the state machine
    public class BaseMovementInputState : BaseState
    {
        // The behaviour context the State is in
        protected MovementInputHandler _context;

        // The return Stream for Movement Intention
        protected Subject<Structs.MovementIntention> _movementIntention;

        // the current Movement Intention managed by this State
        protected Structs.MovementIntention _currentMovementIntention;

        // true of I, as a state am currently active
        public override ReactiveProperty<bool> ActiveRP { get => _activeRP; set => _activeRP = value; }
        protected ReactiveProperty<bool> _activeRP;

        public BaseMovementInputState(MovementInputHandler context, Subject<Structs.MovementIntention> movementIntention)
        {
            _context = context;
            _movementIntention = movementIntention;

            _currentMovementIntention = new Structs.MovementIntention();
        }

        public override void AwakeState()
        {
            _activeRP = new ReactiveProperty<bool>(false);
        }

        public override void StartState() { }

        public override void EnterState()
        {
            // Active = true;
            _activeRP.Value = true;

            // Setting up new current Movement Input after State change
            _currentMovementIntention.HorizontalInput = _context.MovementInput.HorizontalInput;
            _currentMovementIntention.JumpInput = _context.MovementInput.JumpInput;
            _currentMovementIntention.RunInput = _context.MovementInput.RunInput;
            _currentMovementIntention.CrouchInput = _context.MovementInput.CrouchInput;
            _currentMovementIntention.Grounded = _context.MovementInput.Grounded;
        }

        public override void ExitState()
        {
            // Active = false;
            _activeRP.Value = false;
        }

        public override void FixedUpdateState() { }

        public override void LateUpdateState() { }

        public override void UpdateState() { }
    }
}
