using UnityEngine;

namespace DyrdaDev.FirstPersonController
{
    public class Structs
    {
        public struct Loc
        {
            public LocomotionEnum LocomotionState;

            public Loc(LocomotionEnum locomotionState)
            {
                LocomotionState = locomotionState;
            }
        }

        // combined player input and grounded
        public struct MovementInput
        {
            public Vector2 HorizontalInput;
            public bool JumpInput;
            public bool RunInput;
            public bool CrouchInput;

            public bool Grounded;

            public MovementInput(Vector2 horizontalInput, bool jumpInput, bool runInput, bool crouchInput, bool grounded)
            {
                HorizontalInput = horizontalInput;
                JumpInput = jumpInput;
                RunInput = runInput;
                CrouchInput = crouchInput;

                Grounded = grounded;
            }
        }

        // combined movement input and ressult state into movement intent
        public struct MovementIntention
        {
            public Vector2 HorizontalInput;
            public bool JumpInput;
            public bool RunInput;
            public bool CrouchInput;

            public bool Grounded;

            public LocomotionEnum LocomotionState;

            public MovementIntention(Vector2 horizontalInput, bool jumpInput, bool runInput, bool crouchInput, bool grounded, LocomotionEnum locomotionState)
            {
                HorizontalInput = horizontalInput;
                JumpInput = jumpInput;
                RunInput = runInput;
                CrouchInput = crouchInput;

                Grounded = grounded;

                LocomotionState = locomotionState;
            }
        }

        // external input combined
        public struct ExternalInput
        {
            public bool HasLadder;
            public bool HasDoor;
            public bool IsFrozen;
            public bool HasJumpboost;

            public bool Grounded;

            public ExternalInput(bool hasLadder, bool hasDoor, bool isFrozen, bool hasJumpboost, bool grounded)
            {
                HasLadder = hasLadder;
                HasDoor = hasDoor;
                IsFrozen = isFrozen;
                HasJumpboost = hasJumpboost;

                Grounded = grounded;
            }
        }
    }
}
