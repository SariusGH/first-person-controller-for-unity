using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System;

namespace DyrdaDev.FirstPersonController
{
    // a concrete default behaviour state
    public class IdleDefaultBehaviourState : BaseDefaultBehaviourState
    {
        public IdleDefaultBehaviourState(DefaultBehaviourStateMachine context,
            Subject<Vector3> motionInput,
            FirstPersonControllerInput firstPersonControllerInput)
            : base(context, motionInput, firstPersonControllerInput)
        {

        }

        public override void AwakeState()
        {
            base.AwakeState();
        }

        public override void StartState()
        {
            base.StartState();

            // tests about aproaches to handling the input streams inside of a state machine
            _firstPersonControllerInput.Move
                .Where(_ => _activeRP.Value)
                // also defeats purpose of states to not have code executed when state is inactive, but this checks every frame if it is active on every state
                .Where(i => i != Vector2.zero) // checks for movement input
                .Subscribe(_ =>
                {
                    // moving, so change state to walk
                    _context.StateMachine.ChangeState(_context.WalkState);
                })/*.AddTo(this)*/; // maybe needs something similar, as AddTo(this) is not possible cause this is no Monobehaviour

                //_firstPersonControllerInput.Jump
                //  .Where(_ => _activeRP.Value)
        }

        public override void UpdateState()
        {
            base.UpdateState();

            //_motionInput.OnNext(Vector3.zero);
        }
    }
}