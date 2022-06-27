using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    public class RaycastBasedSurroundingInput : SurroundingInput
    {
        #region Surrounding Input Fields

        public override ReadOnlyReactiveProperty<bool> HasLadder => _hasLadder;
        private ReadOnlyReactiveProperty<bool> _hasLadder;

        public override ReadOnlyReactiveProperty<bool> HasDoor => _hasDoor;
        private ReadOnlyReactiveProperty<bool> _hasDoor;

        #endregion

        #region Configuration

        #endregion

        protected void Awake()
        {
            // these are just move observables for the structure, no checks for doors or ladders are made

            // Ladder
            // Raycast to see if there is a ladder in front
            _hasLadder = this.UpdateAsObservable()
                .Select(_ => false)
                .ToReadOnlyReactiveProperty();

            // Door
            // Raycast to see if there is a door in front
            _hasDoor = this.UpdateAsObservable()
                .Select(_ => false)
                .ToReadOnlyReactiveProperty();
        }
    }
}
