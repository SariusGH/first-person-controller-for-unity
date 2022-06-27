using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    // a base state used by any state machine
    public abstract class BaseState
    {
        // public bool Active = false;
        public abstract ReactiveProperty<bool> ActiveRP { get; set; } // give inpormation about bing an active state

        public abstract void AwakeState();

        public abstract void StartState();

        public abstract void EnterState();

        public abstract void UpdateState();

        public abstract void FixedUpdateState();

        public abstract void LateUpdateState();

        public abstract void ExitState();
    }
}
