using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    // states that a animator could subscribe on and use in his animator sate machine mirror the states as possible approach
    public enum AnimationEnum
    {
        Idle,
        Walk,
        Run,
        Jump,
        Crouch,
        Fall,
        Ladder,
        Door,
        Frozen
    }
}