using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;

namespace DyrdaDev.FirstPersonController
{
    public class JumpSprintDefaultBehaviourState : BaseDefaultBehaviourState
    {
        public JumpSprintDefaultBehaviourState(DefaultBehaviourStateMachine context,
            Subject<Vector3> motionInput,
            FirstPersonControllerInput firstPersonControllerInput)
            : base(context, motionInput, firstPersonControllerInput)
        {

        }

        public override void EnterState() { }

        public override void ExitState() { }

        public override void FixedUpdateState() { }

        public override void LateUpdateState() { }

        public override void UpdateState() { }
    }
}