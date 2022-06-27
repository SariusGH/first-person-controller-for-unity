using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    // base state for all default behaviour states
    public class BaseDefaultBehaviourState : BaseState
    {
        // The behaviour context the State is in
        protected DefaultBehaviourStateMachine _context;

        // The return Stream for calculated Motion
        protected Subject<Vector3> _motionInput;

        // The Input to work on
        protected FirstPersonControllerInput _firstPersonControllerInput;

        // true, if i am a active state
        public override ReactiveProperty<bool> ActiveRP { get => _activeRP; set => _activeRP = value; }
        protected ReactiveProperty<bool> _activeRP;

        public BaseDefaultBehaviourState(DefaultBehaviourStateMachine context, Subject<Vector3> motionInput, FirstPersonControllerInput firstPersonControllerInput)
        {
            _context = context;
            _motionInput = motionInput;
            _firstPersonControllerInput = firstPersonControllerInput;
        }

        public override void AwakeState()
        {
            // initialising itself to not ba a active state on creation
            _activeRP = new ReactiveProperty<bool>(false);
        }

        public override void StartState() { }

        public override void EnterState()
        {
            // Active = true;
            _activeRP.Value = true;
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
