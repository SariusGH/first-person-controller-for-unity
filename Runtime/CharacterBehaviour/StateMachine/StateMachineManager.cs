using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    // managing any state machine used
    public class StateMachineManager : MonoBehaviour
    {
        // the current state
        public BaseState CurrentState => _currentState;
        private BaseState _currentState;

        // if the state machine is running
        public bool Enabled { get => _enabled; set => _enabled = value; }
        private bool _enabled = true;

        public void SetInitialState(BaseState initialState)
        {
            if (_currentState != null) // backup, if methode gets called a second time to prevent two states with active = true
            {
                //_currentState.Active = false;
                _currentState.ActiveRP.Value = false;
            }
            _currentState = initialState;
            //_currentState.Active = true;
            _currentState.ActiveRP.Value = true;
        }

        // swap state
        public void ChangeState(BaseState newState)
        {
            _currentState.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }

        // update, fixtupdate and lateupdate for the currently active state

        void Update()
        {
            if (_enabled) {
                _currentState.UpdateState();
            }
        }

        void FixedUpdate()
        {
            if (_enabled)
            {
                _currentState.FixedUpdateState();
            }
        }

        void LateUpdate()
        {
            if (_enabled)
            {
                _currentState.LateUpdateState();
            }
        }
    }
}
