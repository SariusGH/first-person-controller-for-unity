using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    public class RandomBasedFirstPersonControllerInput : FirstPersonControllerInput
    {
        #region Controller Input Fields

        public override IObservable<Vector2> Move => _move;
        private IObservable<Vector2> _move;

        public override IObservable<Unit> Jump => _jump;
        private Subject<Unit> _jump;

        public override ReadOnlyReactiveProperty<bool> Run => _run;
        private ReadOnlyReactiveProperty<bool> _run;

        public override IObservable<Vector2> Look => _look;
        private IObservable<Vector2> _look;

        // here
        public override ReadOnlyReactiveProperty<bool> Crouch => _crouch;
        private ReadOnlyReactiveProperty<bool> _crouch;

        #endregion

        #region Configuration

        [Header("Look Properties")]
        [SerializeField] private float lookSmoothingFactor = 14.0f;

        #endregion

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        protected void Awake()
        {

            // Hide the mouse cursor and lock it in the game window.
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;

            // Move:
            _move = this.UpdateAsObservable()
                .Select(_ => new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)));

            // Jump:
            _jump = new Subject<Unit>().AddTo(this);
            //_controls.Character.Jump.performed += context => _jump.OnNext(Unit.Default);


            // Run:
            _run = this.UpdateAsObservable()
                .Select(_ => UnityEngine.Random.Range(-1.0f, 1.0f) > 0.0f ? true : false)
                .ToReadOnlyReactiveProperty();

            // Look:
            var smoothLookValue = new Vector2(0, 0);
            _look = this.UpdateAsObservable()
                .Select(_ =>
                {
                    var rawLookValue = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));

                    smoothLookValue = new Vector2(
                        Mathf.Lerp(smoothLookValue.x, rawLookValue.x, lookSmoothingFactor * Time.deltaTime),
                        Mathf.Lerp(smoothLookValue.y, rawLookValue.y, lookSmoothingFactor * Time.deltaTime)
                    );

                    return smoothLookValue;
                });

            // here
            // Crouch
            _crouch = this.UpdateAsObservable()
                .Select(_ => UnityEngine.Random.Range(-1.0f, 1.0f) > 0.0f ? true : false)
                .ToReadOnlyReactiveProperty();
        }
    }
}