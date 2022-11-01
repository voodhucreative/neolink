using System;
namespace neolink.Helpers
{
    public class MovementManager
    {
        public int UserMoveTrigger;
        public enum UserMovementTriggers
        {
            Raise,
            Lower,
            Thrust,
            Swipe,
            Retract,
            Run,
            Walk,
            Shake,
            Tilt
        }

        public int UserMoveStrength;
        public enum UserMovementStrength
        {
            VeryWeak,
            Weak,
            WeakMedium,
            Medium,
            StrongMedium,
            Strong,
            Lunatic
        }

        public MovementManager()
        {

        }
    }
}

