using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace DyrdaDev.FirstPersonController
{
    [RequireComponent(typeof(CharacterController))]
    public class UnityCharacterControllerGroundedInput : GroundedInput
    {
        public override ReactiveProperty<bool> Grounded => _grounded;
        private ReactiveProperty<bool> _grounded;

        public override Subject<Unit> GroundedUpdateRequest => _groundedUpdateRequest;
        private Subject<Unit> _groundedUpdateRequest;

        private CharacterController _characterController;

        void Awake()
        {
            // finds the required Unity Character Controller needed when using this Module
            _characterController = GetComponent<CharacterController>();

            // initialize streams 
            _grounded = new ReactiveProperty<bool>(false);
            _groundedUpdateRequest = new Subject<Unit>().AddTo(this);
        }

        void Start()
        {
            // Adds the IsGrounded check to the Update Notifier
            _groundedUpdateRequest.Subscribe(_ =>
            {
                _grounded.Value = _characterController.isGrounded;
            }).AddTo(this);

            // performs initial IsGrounded Update
            _groundedUpdateRequest.OnNext(Unit.Default);
        }
    }
}
