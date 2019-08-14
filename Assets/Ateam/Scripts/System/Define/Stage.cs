using UnityEngine;
using System.Collections;

namespace Ateam
{
    namespace Define
    {
        /// <summary>
        /// ステージ関連の定義
        /// </summary>
        public class Stage 
        {
            /// <summary>
            /// ブロックタイプ
            /// </summary>
            public enum BLOCK_TYPE
            {
                /// <summary>
                /// 地位上ブロック
                /// </summary>
                NORMAL = 0,
                /// <summary>
                /// 障害物ブロック
                /// </summary>
                OBSTACLE,
                /// <summary>
                /// 特殊定義
                /// ブロックなし
                /// </summary>
                NONE,
            }
        }
    }
}