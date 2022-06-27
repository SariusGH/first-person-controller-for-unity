using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    public class ExternalInputHandler : MonoBehaviour
    {
        // the external input getting forwarded
        public IObservable<Structs.ExternalInput> ExternalInput;

        // external input
        [SerializeField] private SurroundingInput _surroundingInput;
        [SerializeField] private ThirdPartyInput _thirdPartyInput;

        void Start()
        {
            // setting up combination of external inputs
            ExternalInput = InputObservables.CombineExternalInput(this.UpdateAsObservable(), _surroundingInput, _thirdPartyInput);
        }
    }
}
