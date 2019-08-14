using UnityEngine;
using System.Collections;

namespace Ateam
{
    namespace Define
    {
        public static class Battle
        {
            public static readonly int TIME_MAX     = 10800;
            public static readonly int BLOCK_SIZE   = 1;

            public enum TEAM_TYPE
            {
                ALPHA = 0,
                BRAVO,
                MAX,
            }

            public enum ATTACK_TYPE
            {
                LONG,
                MIDDLE,
                SHORT,
            }

            public enum ACTION_TYPE
            {
                ATTACK_LONG,
                ATTACK_MIDDLE, 
                ATTACK_SHORT,
                INVINCIBLE
            }

            public static readonly float CHARACTER_ROT_SPEED = 0.2f;
        }
    }
}