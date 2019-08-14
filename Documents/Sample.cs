using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ateam
{
    public class Sample : BaseBattleAISystem
    {

        //---------------------------------------------------
        // InitializeAI
        //---------------------------------------------------
        override public void InitializeAI()
        {
        }

        //---------------------------------------------------
        // UpdateAI
        //---------------------------------------------------
        override public void UpdateAI()
        {
            //ステージデータ取ってくるときのサンプル
            int[,] stageData = GetStageData();

            for(int y = 0; y < stageData.GetLength(0); y++)
            {
                for (int x = 0; x < stageData.GetLength(1); x++)
                {
                    //Debug.Log("stage" + y + " : " + x );

                    //通常ブロック
                    if (stageData[y, x] == (int)Define.Stage.BLOCK_TYPE.NORMAL)
                    {
                        //Debug.Log("通常ブロック");        
                    }
                    else if (stageData[y, x] == (int)Define.Stage.BLOCK_TYPE.OBSTACLE)
                    {
                        //Debug.Log("障害物ブロック");
                    }
                    else if (stageData[y, x] == (int)Define.Stage.BLOCK_TYPE.NONE)
                    {
                        //Debug.Log("ブロックなし");
                    }
                }
            }

            //自チームのデータを取得
            List<CharacterModel.Data> playerList = GetTeamCharacterDataList(TEAM_TYPE.PLAYER);

            //敵チームのデータ取得
            List<CharacterModel.Data> enemyList = GetTeamCharacterDataList(TEAM_TYPE.ENEMY);
           
            //移動のサンプル
            for (int i = 0; i < playerList.Count; i++)
            {
                CharacterModel.Data character = playerList[i];
                int id = character.ActorId;

                //自分がいるステージのブロックタイプを取得
                Define.Stage.BLOCK_TYPE type = GetBlockType(character.BlockPos);

                int move = UnityEngine.Random.Range(0, 4);
                switch (move)
                {
                    case 0:
                        //上移動
                        Move(id, Common.MOVE_TYPE.UP);
                        break;

                    case 1:
                        //下移動
                        Move(id, Common.MOVE_TYPE.DOWN);
                        break;

                    case 2:
                        //左移動
                        Move(id, Common.MOVE_TYPE.LEFT);
                        break;

                    case 3:
                        //右移動
                        Move(id, Common.MOVE_TYPE.RIGHT);
                        break;
                }


                //アクションのサンプル
                switch(UnityEngine.Random.Range(0, 4))
                {
                    case 0:
                        //近距離攻撃
                        Action(id, Define.Battle.ACTION_TYPE.ATTACK_SHORT);
                        break;
                    case 1:
                        //中距離攻撃
                        Action(id, Define.Battle.ACTION_TYPE.ATTACK_MIDDLE);
                        break;
                    case 2:
                        //長距離攻撃
                        Action(id, Define.Battle.ACTION_TYPE.ATTACK_LONG);
                        break;
                    case 3:
                        //無敵アクション
                        Action(id, Define.Battle.ACTION_TYPE.INVINCIBLE);
                        break;
                }
            }
        } 

        //---------------------------------------------------
        // ItemSpawnCallback
        //---------------------------------------------------
        override public void ItemSpawnCallback(ItemSpawnData itemData)
        {
            if (itemData.ItemType == ItemData.ITEM_TYPE.ATTACK_UP)
            {
                //Debug.Log("攻撃力アップアイテム");
            }
            else if (itemData.ItemType == ItemData.ITEM_TYPE.SPEED_UP)
            {
                //Debug.Log("スピードアップアイテム");
            }
            else if (itemData.ItemType == ItemData.ITEM_TYPE.HP_RECOVER)
            {
                //Debug.Log("回復アイテム");
            }

            //生成された位置
            //Debug.Log(itemData.BlockPos);
        }
    }
}