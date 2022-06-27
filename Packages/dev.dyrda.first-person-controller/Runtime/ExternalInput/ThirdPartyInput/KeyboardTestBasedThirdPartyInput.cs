using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    // mock class for simple testing of external input
    public class KeyboardTestBasedThirdPartyInput : ThirdPartyInput
    {
        #region ThirdParty Input Fields

        public override ReadOnlyReactiveProperty<bool> IsFrozen => _isFrozen;
        private ReadOnlyReactiveProperty<bool> _isFrozen;

        public override ReadOnlyReactiveProperty<bool> HasJumpboost => _hasJumpboost;
        private ReadOnlyReactiveProperty<bool> _hasJumpboost;

        #endregion

        #region Configuration

        #endregion

        protected void Awake()
        {
            // IsFrozen
            _isFrozen = this.UpdateAsObservable()
                .Select(_ => false /*Input.GetKeyDown(KeyCode.F)*/)
                .ToReadOnlyReactiveProperty();

            // HasJumpboost
            _hasJumpboost = this.UpdateAsObservable()
                .Select(_ => false /*Input.GetKeyDown(KeyCode.J)*/)
                .ToReadOnlyReactiveProperty();
        }
    }
}
