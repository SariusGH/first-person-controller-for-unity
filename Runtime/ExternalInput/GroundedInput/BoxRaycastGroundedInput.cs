using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace DyrdaDev.FirstPersonController
{
    // just a mock class, needs actual box raycast to be added
    public class BoxRaycastGroundedInput : GroundedInput
    {
        public override ReactiveProperty<bool> Grounded => _grounded;
        private ReactiveProperty<bool> _grounded;

        public override Subject<Unit> GroundedUpdateRequest => _groundedUpdateRequest;
        private Subject<Unit> _groundedUpdateRequest;

        // add box raycast
    }
}
