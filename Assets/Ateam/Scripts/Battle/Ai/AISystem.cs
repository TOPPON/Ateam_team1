using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ateam
{
    //そこそこなAIに進化
    //まだ弱い場合は個別管理のAIと群衆を管理するAIを別個クラス分けして対応するとより良くなる
    //やはり1クラスだと限界あるよねー
    //HPが低くなったら逃げるやつとかほしいよねー
    public class AISystem : BaseBattleAISystem
    {
        class JobtypeCompare : IEqualityComparer<CHARACTER_MOVE_STATE>
        {
            public bool Equals(CHARACTER_MOVE_STATE x, CHARACTER_MOVE_STATE y)
            {
                return x == y;
            }

            public int GetHashCode(CHARACTER_MOVE_STATE obj)
            {
                return (int)obj;
            }
        }

        class MoveNodeData
        {
            public Vector2 Pos { get; set; }
            public int Score { get; set; }
            public int Cost { get; set; }
            public MoveNodeData ParentNode { get; set; }

            public MoveNodeData(Vector2 pos, int cost, int score)
            {
                Pos     = pos;
                Cost    = cost;
                Score   = score;
            }
        };

        struct HpData
        {
            public HpData(float oldHp, int actorId)
            {
                this.OldHp      = oldHp;
                this.ActorId    = actorId;
            }

            public float OldHp;
            public int ActorId;
        }

        enum CHARACTER_MOVE_STATE
        {
            NONE,
            ATTACK,
            GET_ITEM,
        }

        readonly static float ATTACK_MIDDLE_THRESHOLD           = 8;
        readonly static float ATTACK_SHORT_THRESHOLD            = 2;
        readonly static float INVINCIBLE_THRESHOLD              = 3;
        readonly static int UPDATE_SEARCH_ROUTE_FRAME_THRESHOLD = 60;
        readonly static float MAIN_MOVE_UPDATE_THRESHOLD        = 2;
        readonly static int RANDOM_POS_TRIALS                   = 5;

        List<HpData> _oldHpList                             = new List<HpData>();
        List<ItemSpawnData> _stageItemDataList              = new List<ItemSpawnData>();
        Dictionary<int, int> _getReservItemList             = new Dictionary<int, int>();

        Dictionary<int, StateMachine<CHARACTER_MOVE_STATE>> _characterStateList = new Dictionary<int, StateMachine<CHARACTER_MOVE_STATE>>();
        int _currentActorId = 0;
        bool _isEscape = false;

        //---------------------------------------------------
        // InitializeAI
        //---------------------------------------------------
        override public void InitializeAI()
        {
            foreach (CharacterModel.Data data in GetTeamCharacterDataList(TEAM_TYPE.PLAYER))
            {
                _oldHpList.Add(new HpData(data.Hp, data.ActorId));
                StateMachine<CHARACTER_MOVE_STATE> stateMachine = new StateMachine<CHARACTER_MOVE_STATE>();
                stateMachine.Initialize(new JobtypeCompare());
                stateMachine.Add(CHARACTER_MOVE_STATE.ATTACK, CharacterAttackMoveStateEnter, CharacterAttackMoveStateUpdate, null);
                stateMachine.Add(CHARACTER_MOVE_STATE.GET_ITEM, CharacterGetItemMoveStateEnter, CharacterGetItemMoveStateUpdate, null);

                stateMachine.SetNextState(CHARACTER_MOVE_STATE.ATTACK);

                _characterStateList.Add(data.ActorId, stateMachine);
            }

            DataUpdate();
        }
    
        //---------------------------------------------------
        // UpdateAI
        //---------------------------------------------------
        override public void UpdateAI()
        {
            //アイテムのステージ情報更新
            ItemDataUpdate();

            //戦略データの更新
            if (DataUpdate() == false)
            {
                return;
            }

            //移動更新
            MoveUpdate();

            //攻撃更新
            ActionUpdate();
        }

        //---------------------------------------------------
        // MoveTargetBlock
        //---------------------------------------------------
        List<Vector2> MoveTargetBlock(int actorId, Vector2 targetBlockPos)
        {
            Vector2 currentBlockPos     = GetCharacterData(actorId).BlockPos;
            List<Vector2> commandList   = new List<Vector2>();

            //死んでいた場合は空のリストを返却
            if (GetCharacterData(actorId).Hp <= 0)
            {
                return commandList;
            }

            Dictionary<Vector2, MoveNodeData> openDic   = new Dictionary<Vector2, MoveNodeData>();
            Dictionary<Vector2, MoveNodeData> closeDic  = new Dictionary<Vector2, MoveNodeData>();
            MoveNodeData currentNode                    = new MoveNodeData(currentBlockPos, 0, GetMoveEvaluationScore(currentBlockPos, targetBlockPos));

            //ゴールにたどり着くまで探索
            while (currentNode.Pos != targetBlockPos)
            {
                List<Vector2> nextNodeKey = new List<Vector2>();

                //4方向のブロックのコスト算出
                for (int i = 0; i < (int)Common.MOVE_TYPE.NONE_MAX; i++)
                {
                    Vector2 nextVec     = GetMoveTypeToVec((Common.MOVE_TYPE)i);
                    Vector2 nextPos     = new Vector2 ();
                    nextPos.x           = currentNode.Pos.x + nextVec.x;
                    nextPos.y           = currentNode.Pos.y + nextVec.y;

                    if (closeDic.ContainsKey(nextPos)
                        || nextPos.x < 0
                        || nextPos.y < 0
                        || GetBlockType(nextPos) != Define.Stage.BLOCK_TYPE.NORMAL)
                    {
                        continue;
                    }

                    if (!openDic.ContainsKey(nextPos))
                    {
                        int cost = currentNode.Cost + 1;
                        MoveNodeData nodeData = new MoveNodeData (nextPos, cost, GetMoveEvaluationScore(nextPos, targetBlockPos) + cost);
                        nodeData.ParentNode = currentNode;
                        openDic.Add(nextPos, nodeData);
                    }

                    if (!nextNodeKey.Contains(nextPos))
                    {
                        nextNodeKey.Add(nextPos);
                    }
                }

                //基準ノードを探索しないリストへ追加
                if(!closeDic.ContainsKey(currentNode.Pos))
                {
                   closeDic.Add(currentNode.Pos, currentNode);
                }

                //どの方向にもない場合は完全に詰まっているので親ノードへ戻る
                if (nextNodeKey.Count == 0)
                {
                    //完全に死んでいるので抜ける
                    if (currentNode.ParentNode == null)
                    {
                        break;
                    }

                    currentNode = currentNode.ParentNode;

                    continue;
                }

                //4方向のノードで次のノードを探索
                currentNode = openDic[nextNodeKey[0]];

                foreach (Vector2 key in nextNodeKey)
                {
                    MoveNodeData node = openDic[key];

                    //ゴールが含まれていたら終了
                    if (node.Pos == targetBlockPos)
                    {
                        currentNode = node;
                        break;
                    }

                    //スコアが一緒の場合はコストで比較
                    if (node.Score == currentNode.Score)
                    {
                        if (node.Cost > currentNode.Cost)
                        {
                            currentNode = node;
                        }
                    }
                    else if(node.Score < currentNode.Score)
                    {
                        currentNode = node;
                    }
                }
            }

            //ゴールまでのコマンドリストを作成
            commandList.Add(currentNode.Pos);

            while (currentNode.ParentNode != null)
            {
                commandList.Add(currentNode.ParentNode.Pos);
                currentNode = currentNode.ParentNode;
            }

            commandList.Reverse();

            //最初の一個は自分なので無視
            if (commandList.Count >= 1)
            {
                commandList.RemoveAt(0);
            }

            return commandList;
        }

        //---------------------------------------------------
        // GetMoveEvaluationScore
        //---------------------------------------------------
        private int GetMoveEvaluationScore(Vector2 pos, Vector2 targetPos)
        {
            int x = Mathf.Abs((int)(targetPos.x - pos.x));
            int y = Mathf.Abs((int)(targetPos.y - pos.y));

            return x + y;
        }

        //---------------------------------------------------
        // GetVecToMoveType
        //---------------------------------------------------
        public Common.MOVE_TYPE GetVecToMoveType(Vector2 vec)
        {
            if (vec == Vector2.down)
            {
                return Common.MOVE_TYPE.DOWN;
            }
            else if(vec == Vector2.up)
            {
                return Common.MOVE_TYPE.UP;
            }
            else if(vec == Vector2.right)
            {
                return Common.MOVE_TYPE.RIGHT;
            }
            else if(vec == Vector2.left)
            {
                return Common.MOVE_TYPE.LEFT;
            }

            return Common.MOVE_TYPE.NONE_MAX;
        }

        //---------------------------------------------------
        // GetVecToMoveType
        //---------------------------------------------------
        public Vector2 GetMoveTypeToVec(Common.MOVE_TYPE type)
        {
            Vector2 outVec = Vector2.zero;

            switch (type)
            {
                case Common.MOVE_TYPE.DOWN:
                    outVec = Vector2.down;
                    break;

                case Common.MOVE_TYPE.UP:
                    outVec = Vector2.up;
                    break;

                case Common.MOVE_TYPE.RIGHT:
                    outVec = Vector2.right;
                    break;

                case Common.MOVE_TYPE.LEFT:
                    outVec = Vector2.left;
                    break;
            }

            return outVec;
        }

        //---------------------------------------------------
        // MoveVec
        //---------------------------------------------------
        bool MoveVec(int actorId, Vector2 vec)
        {
            if (vec.x < 0)
            {
                return Move(actorId, Common.MOVE_TYPE.LEFT);
            }
            else if (vec.x > 0)
            {
                return Move(actorId, Common.MOVE_TYPE.RIGHT);
            }
            else if (vec.y < 0)
            {
                return Move(actorId, Common.MOVE_TYPE.DOWN);
            }
            else
            {
                return Move(actorId, Common.MOVE_TYPE.UP);
            }
        }

        //---------------------------------------------------
        // GetLowHpCharacterData
        //---------------------------------------------------
        CharacterModel.Data GetLowHpCharacterData(TEAM_TYPE type)
        {
            List<CharacterModel.Data> list  = GetTeamCharacterDataList(type);
            CharacterModel.Data retData     = null;
            float hp = 0;

            foreach (CharacterModel.Data data in list)
            {
                if (hp < data.Hp)
                {
                    retData = data;
                }
            }

            return retData;
        }

        //---------------------------------------------------
        // searchAliveTeam
        //---------------------------------------------------
        CharacterModel.Data searchAliveTeam(TEAM_TYPE type)
        {
            List<CharacterModel.Data> list  = GetTeamCharacterDataList(type);
            CharacterModel.Data retData     = null;

            foreach (CharacterModel.Data data in list)
            {
                if (data.Hp > 0)
                {
                    retData = data;
                    break;
                }
            }

            return retData;
        }

        //---------------------------------------------------
        // ActionUpdate
        //---------------------------------------------------
        void ActionUpdate()
        {
            List<CharacterModel.Data> playerList = GetTeamCharacterDataList(TEAM_TYPE.PLAYER);
            List<CharacterModel.Data> enemyList = GetTeamCharacterDataList(TEAM_TYPE.ENEMY);
            
            //遠距離は常に出し続ける
            foreach(CharacterModel.Data character in playerList)
            {
                Action(character.ActorId, Define.Battle.ACTION_TYPE.ATTACK_LONG);
            }

            //中距離と近距離は敵が一定範囲内にいたら発射
            //一定距離の場合バリア展開
            foreach(CharacterModel.Data playerData in playerList)
            {
                foreach(CharacterModel.Data enemyData in enemyList)
                {
                    if (enemyData.Hp <= 0)
                    {
                        continue;
                    }

                    float len =  (enemyData.BlockPos - playerData.BlockPos).magnitude;

                    if (len < ATTACK_MIDDLE_THRESHOLD)
                    {
                        Action(playerData.ActorId, Define.Battle.ACTION_TYPE.ATTACK_MIDDLE);
                    }

                    if (len < ATTACK_SHORT_THRESHOLD)
                    {
                        Action(playerData.ActorId, Define.Battle.ACTION_TYPE.ATTACK_SHORT);
                    }

                    if (len < INVINCIBLE_THRESHOLD)
                    {
                        Action(playerData.ActorId, Define.Battle.ACTION_TYPE.INVINCIBLE);
                    }
                }
            }
        }

        //---------------------------------------------------
        // MoveUpdate
        //---------------------------------------------------
        void MoveUpdate()
        {
            StateUpdate();

            foreach (CharacterModel.Data character in GetTeamCharacterDataList(TEAM_TYPE.PLAYER))
            {
                if (!_characterStateList.ContainsKey(character.ActorId))
                {
                    continue;;
                }

                StateMachine<CHARACTER_MOVE_STATE> stateMachine  = _characterStateList[character.ActorId];
                _currentActorId                             = character.ActorId;

                stateMachine.Update(Time.deltaTime);
            }
        }

        //---------------------------------------------------
        // StateUpdate
        //---------------------------------------------------
        void StateUpdate()
        {
            ExecuteSetItemState(ItemData.ITEM_TYPE.HP_RECOVER, ItemRecoverDecisionFunc);
            ExecuteSetItemState(ItemData.ITEM_TYPE.ATTACK_UP, ItemAttackDecisionFunc);
            ExecuteSetItemState(ItemData.ITEM_TYPE.SPEED_UP, ItemSpeedDecisionFunc);
        }

        //---------------------------------------------------
        // ItemRecoverDecisionFunc
        //---------------------------------------------------
        int ItemRecoverDecisionFunc(ItemSpawnData item)
        {
            List<CharacterModel.Data> playerList    = GetTeamCharacterDataList(TEAM_TYPE.PLAYER);
            int executeActorId                      = -1;
            float hpRate                            = 1;
            StateMachine<CHARACTER_MOVE_STATE> stateMachine;

            foreach (CharacterModel.Data character in playerList)
            {
                if (_getReservItemList.ContainsValue(character.ActorId)
                    || character.Hp <= 0)
                {
                    continue;
                }

                stateMachine            = _characterStateList [character.ActorId];
                CHARACTER_MOVE_STATE state   = stateMachine.CurrentStateKey;
                float dstHpRate         = character.Hp / character.MaxHp;

                if (hpRate > dstHpRate
                    && state == CHARACTER_MOVE_STATE.ATTACK)
                {
                    hpRate          = dstHpRate;
                    executeActorId  = character.ActorId;
                }
            }

            return executeActorId;
        }

        //---------------------------------------------------
        // ItemAttackDecisionFunc
        //---------------------------------------------------
        int ItemAttackDecisionFunc(ItemSpawnData item)
        {
            List<CharacterModel.Data> playerList    = GetTeamCharacterDataList(TEAM_TYPE.PLAYER);
            float length                            = 1000;
            int executeActorId                      = -1;
            StateMachine<CHARACTER_MOVE_STATE> stateMachine;

            foreach (CharacterModel.Data character in playerList)
            {
                if (_getReservItemList.ContainsValue(character.ActorId)
                    || character.Hp <= 0)
                {
                    continue;
                }

                stateMachine            = _characterStateList [character.ActorId];
                CHARACTER_MOVE_STATE state   = stateMachine.CurrentStateKey;
                float dstLength         = (character.BlockPos - item.BlockPos).magnitude;

                if (length > dstLength
                    && state == CHARACTER_MOVE_STATE.ATTACK)
                {
                    length = dstLength;
                    executeActorId = character.ActorId;
                }
            }

            return executeActorId;
        }

        //---------------------------------------------------
        // ItemSpeedDecisionFunc
        //---------------------------------------------------
        int ItemSpeedDecisionFunc(ItemSpawnData item)
        {
            List<CharacterModel.Data> playerList    = GetTeamCharacterDataList(TEAM_TYPE.PLAYER);
            float length                            = 1000;
            int executeActorId                      = -1;
            StateMachine<CHARACTER_MOVE_STATE> stateMachine;

            foreach (CharacterModel.Data character in playerList)
            {
                if (_getReservItemList.ContainsValue(character.ActorId)
                    || character.Hp <= 0)
                {
                    continue;
                }

                stateMachine            = _characterStateList [character.ActorId];
                CHARACTER_MOVE_STATE state   = stateMachine.CurrentStateKey;

                if (length > (character.BlockPos - item.BlockPos).magnitude
                    && state == CHARACTER_MOVE_STATE.ATTACK)
                {
                    
                    executeActorId = character.ActorId;
                }
            }

            return executeActorId;
        }

        //---------------------------------------------------
        // ExecuteSetItemState
        //---------------------------------------------------
        void ExecuteSetItemState(ItemData.ITEM_TYPE itemType, Func<ItemSpawnData,int> DecisionFunc)
        {
            List<ItemSpawnData> itemList = GetSpawnStageItemList(itemType);

            if (itemList.Count <= 0)
            {
                return;
            }

            foreach (ItemSpawnData item in itemList)
            {
                int executeActorId                      = -1;
                StateMachine<CHARACTER_MOVE_STATE> stateMachine;

                if (_getReservItemList.ContainsKey(item.ActorId))
                {
                    continue;
                }


                executeActorId = DecisionFunc(item);

                if (executeActorId >= 0)
                {
                    stateMachine = _characterStateList [executeActorId];
                    stateMachine.SetNextState(CHARACTER_MOVE_STATE.GET_ITEM, Common.CreateHashTable("itemActorId", item.ActorId));
                    _getReservItemList.Add(item.ActorId, executeActorId);
                }
           }
        }

        //---------------------------------------------------
        // CharacterAttackMoveStateEnter
        //---------------------------------------------------
        void CharacterAttackMoveStateEnter(StateData data)
        {
            data.table = Common.CreateHashTable("frameCount", 0, "moveList", new List<Vector2>());
            _isEscape = false;
        }

        //---------------------------------------------------
        // CharacterAttackMoveStateUpdate
        //---------------------------------------------------
        void CharacterAttackMoveStateUpdate(StateData data)
        {
            CharacterModel.Data character = GetAliveActor(_currentActorId, TEAM_TYPE.PLAYER);

            if (character == null)
            {
                return;
            }

            CharacterModel.Data targetCharacterData = GetLowHpCharacterData(TEAM_TYPE.ENEMY);
            List<Vector2> moveList                  = (List<Vector2>)data.table ["moveList"];
            int frameCount                          = (int)data.table ["frameCount"];
            List<CharacterModel.Data> enemyList     = GetTeamCharacterDataList(TEAM_TYPE.ENEMY);

            //ターゲットの近くになったら更新
            //逃げる
            if(targetCharacterData != null 
                && !_isEscape
                &&(targetCharacterData.BlockPos - character.BlockPos).magnitude  < MAIN_MOVE_UPDATE_THRESHOLD)
            {
                moveList    = MoveTargetBlock(character.ActorId, GetRandomPos());
                _isEscape   = true;
            }
            //通常思考
            else if (targetCharacterData != null
                && (moveList.Count <= 0 || frameCount >= UPDATE_SEARCH_ROUTE_FRAME_THRESHOLD ))
            {
                moveList    = MoveTargetBlock(character.ActorId, targetCharacterData.BlockPos);
                frameCount  = 0;
                _isEscape   = false;
            }

            //自分の正面に敵がいたら停止
            if (!_isEscape)
            {
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (enemyList[i].Hp <= 0)
                    {
                        continue;
                    }

                    if (CheckFrontEnemy(character, enemyList[i]))
                    {
                        return;
                    }
                }
            }

            if(character.IsMoveEnable 
                &&moveList.Count > 0)
            {
                //経路探索されたもので移動
                Move(character.ActorId, GetVecToMoveType(moveList[0] - character.BlockPos));
                moveList.RemoveAt(0);
            }

            frameCount++;

            data.table["frameCount"]    = frameCount;
            data.table["moveList"]      = moveList;
        }

        //---------------------------------------------------
        // MoveUpdateGetItemAttackState
        //---------------------------------------------------
        void CharacterGetItemMoveStateEnter(StateData data)
        {
            CharacterModel.Data character = GetAliveActor(_currentActorId, TEAM_TYPE.PLAYER);

            if (character == null)
            {
                return;
            }

            int itemActorId = (int)data.table["itemActorId"];
            int index = _stageItemDataList.FindIndex(obj => obj.ActorId == itemActorId );

            if (index < 0)
            {
                _characterStateList[_currentActorId].SetNextState(CHARACTER_MOVE_STATE.ATTACK);
            }
            else
            {
                ItemSpawnData item      = _stageItemDataList[index];
                List<Vector2> moveList  = MoveTargetBlock(character.ActorId, item.BlockPos);
                data.table              = Common.CreateHashTable("itemActorId", item.ActorId, "moveList", moveList);
            }
        }

        //---------------------------------------------------
        // CharacterGetItemMoveStateUpdate
        //---------------------------------------------------
        void CharacterGetItemMoveStateUpdate(StateData data)
        {
            CharacterModel.Data character = GetAliveActor(_currentActorId, TEAM_TYPE.PLAYER);

            if (character == null)
            {
                return;
            }

            int itemActorId = (int)data.table["itemActorId"];
            int index       = _stageItemDataList.FindIndex(obj => obj.ActorId == itemActorId );
            List<Vector2> moveList  = (List<Vector2>)data.table["moveList"]; 

            if (index < 0
                || moveList.Count <= 0)
            {
                if(_getReservItemList.ContainsKey(itemActorId))
                {
                    _getReservItemList.Remove(itemActorId);
                }

                _characterStateList[_currentActorId].SetNextState(CHARACTER_MOVE_STATE.ATTACK);
            }
            else if(character.IsMoveEnable)
            {
                Move(character.ActorId, GetVecToMoveType(moveList[0] - character.BlockPos));
                moveList.RemoveAt(0);
            }

            data.table["moveList"] = moveList;
        }

        //---------------------------------------------------
        // DataUpdate
        //---------------------------------------------------
        bool DataUpdate()
        {
            List<CharacterModel.Data> playerList = GetTeamCharacterDataList(TEAM_TYPE.PLAYER);

            for(int i = 0; i < _oldHpList.Count; i++)
            {
                HpData hpData = _oldHpList[i];

                foreach (CharacterModel.Data playerData in playerList)
                {
                    if (hpData.ActorId == playerData.ActorId)
                    {
                        if (hpData.OldHp != playerData.Hp
                           && playerData.Hp <= 0)
                        {
                            DieCallBack(playerData.ActorId);
                        }
                        else if (hpData.OldHp != playerData.Hp)
                        {
                            DamageCallBack(playerData.ActorId);
                        }

                        hpData.OldHp    = playerData.Hp;
                        _oldHpList[i]   = hpData;
                        break;
                    }
                }
            }

            return true;
        }

        //---------------------------------------------------
        // ItemDataUpdate
        //---------------------------------------------------
        void ItemDataUpdate()
        {
            CheckGetItem(TEAM_TYPE.PLAYER);
            CheckGetItem(TEAM_TYPE.ENEMY);
        }

        //---------------------------------------------------
        // CheckGetItem
        //---------------------------------------------------
        void CheckGetItem(TEAM_TYPE teamType)
        {
            List<ItemSpawnData> removeList          = new List<ItemSpawnData>();
            List<CharacterModel.Data> characterList = GetTeamCharacterDataList(teamType); 

            foreach (ItemSpawnData item in _stageItemDataList)
            {
                foreach (CharacterModel.Data character in characterList)
                {
                    if (character.Hp <= 0)
                    {
                        continue;
                    }

                    if (item.BlockPos == character.BlockPos)
                    {
                        removeList.Add(item);
                        break;
                    }
                }
            }

            foreach(ItemSpawnData item in removeList)
            {
                _stageItemDataList.Remove(item);

                if (_getReservItemList.ContainsKey(item.ActorId))
                {
                    _getReservItemList.Remove(item.ActorId);
                }
            }
        }

        //---------------------------------------------------
        // DamageCallBack
        //---------------------------------------------------
        void DamageCallBack(int actorId)
        {
            if(GetCharacterData(actorId).Hp < 100)
            {
                Action(actorId, Define.Battle.ACTION_TYPE.INVINCIBLE);
            }
        }

        //---------------------------------------------------
        // DieCallBack
        //---------------------------------------------------
        void DieCallBack(int actorId)
        {
            _characterStateList.Remove(actorId);

            if (_getReservItemList.ContainsValue(actorId))
            {
                foreach (var pair in _getReservItemList)
                {
                    if (pair.Value == actorId)
                    {
                        _getReservItemList.Remove(pair.Key);
                        break;
                    }
                }
            }
        }

        //---------------------------------------------------
        // ItemSpawnCallback
        //---------------------------------------------------
        override public void ItemSpawnCallback(ItemSpawnData itemData)
        {
            _stageItemDataList.Add(itemData);
        }

        //---------------------------------------------------
        // GetAliveActor
        //---------------------------------------------------
        CharacterModel.Data GetAliveActor(int acctorId, TEAM_TYPE teamType)
        {
            CharacterModel.Data character = GetTeamCharacterDataList(teamType).Find(obj => obj.ActorId == acctorId);

            if (character == null
                || character.Hp <= 0)
            {
                return null;
            }

            return character;
        }

        //---------------------------------------------------
        // GetSpawnStageItemList
        //---------------------------------------------------
        List<ItemSpawnData> GetSpawnStageItemList(ItemData.ITEM_TYPE itemType)
        {
            List<ItemSpawnData> list = new List<ItemSpawnData>();

            foreach (ItemSpawnData item in _stageItemDataList)
            {
                if (item.ItemType == itemType)
                {
                    list.Add(item);
                }
            }

            return list;
        }

        //---------------------------------------------------
        // CheckFrontEnemy
        //---------------------------------------------------
        bool CheckFrontEnemy(CharacterModel.Data character, CharacterModel.Data enemy)
        {
            bool flag = ((enemy.BlockPos - character.BlockPos).normalized != character.Forward)? true : false;

            if (character.BlockPos == enemy.BlockPos
                || flag )
            {
                return false;
            }

            if (character.BlockPos.x == enemy.BlockPos.x)
            {
                float startBlockPosY = (character.BlockPos.y < enemy.BlockPos.y) ? character.BlockPos.y : enemy.BlockPos.y;
                float endBlockPosY = (character.BlockPos.y > enemy.BlockPos.y) ? character.BlockPos.y : enemy.BlockPos.y;

                for (int blockPosY = (int)startBlockPosY; blockPosY < endBlockPosY; blockPosY++)
                {
                    if (GetBlockType(new Vector2(character.BlockPos.x, blockPosY)) == Ateam.Define.Stage.BLOCK_TYPE.OBSTACLE)
                    {
                        return false;
                    }
                }
            }
            else if (character.BlockPos.y == enemy.BlockPos.y)
            {
                float startBlockPosX = (character.BlockPos.x < enemy.BlockPos.x) ? character.BlockPos.x : enemy.BlockPos.x;
                float endBlockPosX = (character.BlockPos.x > enemy.BlockPos.x) ? character.BlockPos.x : enemy.BlockPos.x;

                for (int blockPosX = (int)startBlockPosX; blockPosX < endBlockPosX; blockPosX++)
                {
                    if (GetBlockType(new Vector2(blockPosX, character.BlockPos.y)) == Ateam.Define.Stage.BLOCK_TYPE.OBSTACLE)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //---------------------------------------------------
        // GetRandomPos
        //---------------------------------------------------
        Vector2 GetRandomPos()
        {
            Vector2 blockNum = GetStageDataBlockNum();

            for (int i = 0; i < RANDOM_POS_TRIALS; i++)
            {
                int x = UnityEngine.Random.Range(0, (int)blockNum.x - 1);
                int y = UnityEngine.Random.Range(0, (int)blockNum.y - 1);

                Vector2 pos = new Vector2(x, y);

                if (GetBlockType(pos) == Ateam.Define.Stage.BLOCK_TYPE.NORMAL)
                {
                    return pos;
                }
            }

            return Vector2.zero;
        }
    }
}