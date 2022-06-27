using System;
using UniRx;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    public abstract class SurroundingInput : MonoBehaviour
    {
        /// <summary>
        ///     Ladder Input.
        ///     Interaction type: Toggle.
        /// </summary>
        public abstract ReadOnlyReactiveProperty<bool> HasLadder { get; }

        /// <summary>
        ///     Door Input.
        ///     Interaction type: Toggle.
        /// </summary>
        public abstract ReadOnlyReactiveProperty<bool> HasDoor { get; }
    }
}
