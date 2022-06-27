using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;

namespace DyrdaDev.FirstPersonController
{
    public class WalkDefaultBehaviourState : BaseDefaultBehaviourState
    {
        public WalkDefaultBehaviourState(DefaultBehaviourStateMachine context,
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


        }
    }
}