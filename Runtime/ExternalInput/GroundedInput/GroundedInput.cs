using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    public abstract class GroundedInput : MonoBehaviour
    {
        /// <summary>
        ///     IsGrounded enviroment check.
        /// </summary>
        public abstract ReactiveProperty<bool> Grounded { get; }

        /// <summary>
        ///     Request IsGrounded to Update.
        /// </summary>
        public abstract Subject<Unit> GroundedUpdateRequest { get; }
    }
}
