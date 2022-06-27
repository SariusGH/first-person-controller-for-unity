using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System;

namespace DyrdaDev.FirstPersonController
{
    public class CrouchMovementInputState : BaseMovementInputState
    {
        public CrouchMovementInputState(MovementInputHandler context,
            Subject<Structs.MovementIntention> movementIntention)
            : base(context, movementIntention)
        {

        }
    }
}
