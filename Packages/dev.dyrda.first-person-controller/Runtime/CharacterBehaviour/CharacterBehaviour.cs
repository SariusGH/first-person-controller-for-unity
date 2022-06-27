using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    // a character that can have different implementations handling input and outputing 3D motion, a state machine approach was tested here
    public abstract class CharacterBehaviour : MonoBehaviour
    {
        /// <summary>
        ///     Motion input for Character Controller
        /// </summary>
        public abstract IObservable<Vector3> MotionInput { get; }

        /// <summary>
        ///     Input to work on
        /// </summary>
        public abstract FirstPersonControllerInput FirstPersonControllerInput { set; }

        /// <summary>
        ///     Unity Char Controller to work with
        /// </summary>
        public abstract CharacterController CharacterController { get; set; }
    }
}