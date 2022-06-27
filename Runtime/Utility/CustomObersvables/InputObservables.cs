using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace DyrdaDev.FirstPersonController
{
    public static class InputObservables
    {
        // combines input from a first person controller input and grounded input into a state
        public static IObservable<Structs.MovementInput> CombineInput(IObservable<Unit> tick, FirstPersonControllerInput _firstPersonControllerInput, GroundedInput _groundedInput)
        {
            return Observable.Create<Structs.MovementInput>(observer =>
            {
                // the movement input struct to be returned
                Structs.MovementInput _movementInput = new Structs.MovementInput();

                // catch Movement Input and update grounded if needed

                var moveSub = _firstPersonControllerInput.Move.Subscribe(v2 => _movementInput.HorizontalInput = v2);

                var latchSub = LatchObservables.Latch(tick, _firstPersonControllerInput.Jump, false).Subscribe(b => {
                    _groundedInput.GroundedUpdateRequest.OnNext(Unit.Default); // makes sure to have an updated grounded when handling jump inputs
                    _movementInput.JumpInput = b;
                });

                var runSub = _firstPersonControllerInput.Run.Subscribe(b => _movementInput.RunInput = b);

                var crouchSub = _firstPersonControllerInput.Crouch.Subscribe(b => {
                    _groundedInput.GroundedUpdateRequest.OnNext(Unit.Default);
                    _movementInput.CrouchInput = b;
                });

                var groundedSub = _groundedInput.Grounded.Subscribe(b => _movementInput.Grounded = b);

                // Whenever tick fires, emit the current Movment Input
                var tickSub = tick.Subscribe(_ =>
                {
                    _groundedInput.GroundedUpdateRequest.OnNext(Unit.Default);
                    observer.OnNext(_movementInput);
                },
                    observer.OnError,
                    observer.OnCompleted);

                return Disposable.Create(() =>
                {
                    moveSub.Dispose();
                    latchSub.Dispose();
                    runSub.Dispose();
                    crouchSub.Dispose();
                    groundedSub.Dispose();
                    tickSub.Dispose();
                });
            });
        }

        // calculates the movement intention based on input using a switch state machine
        public static IObservable<Structs.MovementIntention> CalcState(this IObservable<Structs.MovementInput> eventObs, LocomotionEnum initialState)
        {
            LocomotionEnum _currentState = initialState;

            // the movement intention to be returned
            Structs.MovementIntention _movementIntention = new Structs.MovementIntention();
            
            return Observable.Create<Structs.MovementIntention>(observer =>
            {
                // Whenever tick = update fires, wecalculate Movement Intention
                var tickSub = eventObs.Subscribe(_movementInput =>
                {
                    _movementIntention.HorizontalInput = _movementInput.HorizontalInput;
                    _movementIntention.JumpInput = _movementInput.JumpInput;
                    _movementIntention.RunInput = _movementInput.RunInput;
                    _movementIntention.CrouchInput = _movementInput.CrouchInput;
                    _movementIntention.Grounded = _movementInput.Grounded;

                    switch(_currentState)
                    {
                        case LocomotionEnum.Idle: // could be falling during this state
                            if (_movementInput.JumpInput && _movementInput.Grounded)
                            {
                                _currentState = LocomotionEnum.Jump;
                            }
                            else if (_movementInput.HorizontalInput != Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Walk;
                            }
                            else if (_movementInput.CrouchInput && _movementInput.Grounded)
                            {
                                _currentState = LocomotionEnum.Crouch;
                            }
                            break;
                        case LocomotionEnum.Walk: // could be falling during this state
                            if (_movementInput.JumpInput && _movementInput.Grounded)
                            {
                                _currentState = LocomotionEnum.Jump;
                            }
                            else if (_movementInput.CrouchInput && _movementInput.Grounded)
                            {
                                _currentState = LocomotionEnum.Crouch;
                            }
                            else if (_movementInput.HorizontalInput == Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Idle;
                            }
                            else if (_movementInput.RunInput && _movementInput.Grounded && _movementInput.HorizontalInput != Vector2.zero) // can only start running when on ground walking
                            {
                                _currentState = LocomotionEnum.Run;
                            }
                            break;
                        case LocomotionEnum.Jump: // could be falling during this state
                            if (!_movementInput.Grounded)
                            {
                                
                            }
                            else if (_movementInput.CrouchInput)
                            {
                                _currentState = LocomotionEnum.Crouch;
                            }
                            else if (_movementInput.HorizontalInput != Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Walk;
                            }
                            else
                            {
                                _currentState = LocomotionEnum.Idle;
                            }
                            break;
                        case LocomotionEnum.Crouch: // can not crouch while falling
                            if (_movementInput.JumpInput && _movementInput.Grounded)
                            {
                                _currentState = LocomotionEnum.Jump;
                            }
                            else if (!_movementInput.CrouchInput && _movementInput.HorizontalInput == Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Idle;
                            }
                            else if (!_movementInput.CrouchInput && _movementInput.HorizontalInput != Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Walk;
                            }
                            else if (!_movementInput.Grounded && _movementInput.HorizontalInput == Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Idle;
                            }
                            else if (!_movementInput.Grounded && _movementInput.HorizontalInput != Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Walk;
                            }
                            break;
                        case LocomotionEnum.Run: // can only run from walking and not when falling
                            if (_movementInput.JumpInput && _movementInput.Grounded)
                            {
                                _currentState = LocomotionEnum.Jump;
                            }
                            else if (_movementInput.RunInput && _movementInput.Grounded && _movementInput.HorizontalInput != Vector2.zero)
                            {
                                
                            }
                            else if (_movementInput.CrouchInput && _movementInput.Grounded)
                            {
                                _currentState = LocomotionEnum.Idle;
                            }
                            else if (_movementInput.HorizontalInput == Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Idle;
                            }
                            else if (!_movementInput.Grounded && _movementInput.HorizontalInput != Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Walk;
                            }
                            else if (!_movementInput.RunInput && _movementInput.HorizontalInput != Vector2.zero)
                            {
                                _currentState = LocomotionEnum.Walk;
                            }
                            break;
                    }

                    // sets the current intention state in the intention struct
                    _movementIntention.LocomotionState = _currentState;

                    observer.OnNext(_movementIntention);
                },
                    observer.OnError,
                    observer.OnCompleted);

                return Disposable.Create(() =>
                {
                    tickSub.Dispose();
                });
            });
        }

        // combines the external input into the external input struct
        public static IObservable<Structs.ExternalInput> CombineExternalInput(IObservable<Unit> tick, SurroundingInput surroundingInput, ThirdPartyInput thirdPartyInput)
        {
            return Observable.Create<Structs.ExternalInput>(observer =>
            {
                // the external input to be returned
                Structs.ExternalInput _externalInput = new Structs.ExternalInput();

                // filling the struct with input info
                var ladderSub = surroundingInput.HasLadder.Subscribe(b => _externalInput.HasLadder = b);
                var doorSub = surroundingInput.HasDoor.Subscribe(b => _externalInput.HasDoor = b);
                var frozenSub = thirdPartyInput.IsFrozen.Subscribe(b => _externalInput.IsFrozen = b);
                var jumpboostSub = thirdPartyInput.HasJumpboost.Subscribe(b => _externalInput.HasJumpboost = b);

                // Whenever tick fires, emit the current external input
                var tickSub = tick.Subscribe(_ =>
                {
                    observer.OnNext(_externalInput);
                },
                    observer.OnError,
                    observer.OnCompleted);

                return Disposable.Create(() =>
                {
                    ladderSub.Dispose();
                    doorSub.Dispose();
                    frozenSub.Dispose();
                    jumpboostSub.Dispose();

                    tickSub.Dispose();
                });
            });
        }
    }
}
