using System;
using UniRx;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    public abstract class ThirdPartyInput : MonoBehaviour
    {
        /// <summary>
        ///     Freeze Input.
        ///     Interaction type: Toggle.
        /// </summary>
        public abstract ReadOnlyReactiveProperty<bool> IsFrozen { get; }

        /// <summary>
        ///     Jumpboost Input.
        ///     Interaction type: Toggle.
        /// </summary>
        public abstract ReadOnlyReactiveProperty<bool> HasJumpboost { get; }
    }
}
