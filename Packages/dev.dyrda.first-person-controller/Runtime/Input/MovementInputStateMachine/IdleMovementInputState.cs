using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System;

namespace DyrdaDev.FirstPersonController
{
    public class IdleMovementInputState : BaseMovementInputState
    {
        public IdleMovementInputState(MovementInputHandler context,
            Subject<Structs.MovementIntention> movementIntention)
            : base(context, movementIntention)
        {
            
        }

        public override void EnterState()
        {
            base.EnterState();

            // Setting up new current Movement Input after State change
            _currentMovementIntention.LocomotionState = LocomotionEnum.Idle;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            // tests to state machines to handle movement intent
            if (_context.MovementInput.HorizontalInput != Vector2.zero) // moving?
            {
                _currentMovementIntention.HorizontalInput = _context.MovementInput.HorizontalInput; // update intent
                _movementIntention.OnNext(_currentMovementIntention); // keep state as the one we are and send intent
                _context.StateMachine.ChangeState(new WalkMovementInputState(_context, _movementIntention)); // switch state over
            } else if (_context.MovementInput.JumpInput && _context.MovementInput.Grounded) // jumping?
            {
                _currentMovementIntention.JumpInput = true; // things that are not allowed wont be updated, like when player pressen jump but not on ground -> no jump intent and ofc no jump state
                // so intent is correct, also on the transition intent
                _currentMovementIntention.Grounded = true;
                _movementIntention.OnNext(_currentMovementIntention);
                _context.StateMachine.ChangeState(new JumpMovementInputState(_context, _movementIntention));
            } else if (_context.MovementInput.CrouchInput && _context.MovementInput.Grounded) // crouching?
            {
                _currentMovementIntention.CrouchInput = true;
                _currentMovementIntention.Grounded = true;
                _movementIntention.OnNext(_currentMovementIntention);
                _context.StateMachine.ChangeState(new CrouchMovementInputState(_context, _movementIntention));
            } else
            {
                _movementIntention.OnNext(_currentMovementIntention); // nothing relevant changed, intent is the same as last frame
            }
        }
    }
}
