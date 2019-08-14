using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Ateam
{
    public class Alfa : BaseBattleAISystem
    {
        public CharacterModel.Data[] datas = new CharacterModel.Data[6];
        List <HpData>OldHPList = new List<HpData>();
        struct HpData
        {
            public float Hp;
            public int id;
            public HpData(int Hp,int id)
            {
                this.Hp = Hp;
                this.id = id;
            }
        }
        List <ItemSpawnData>  FieldItem=new List<ItemSpawnData>();
        enum ITEM_TYPE
        {
            ATKUP,
            SPEEDUP,
            RECOVER
        }
        //---------------------------------------------------
        // InitializeAI
        //---------------------------------------------------
        override public void InitializeAI()
        {
            for (int i = 0; i < 6; i++)
            {
                datas[i] = GetCharacterData(i);
            }

            print(datas[0].TeamType);
            print(datas[5].TeamType);
            print(datas.Length);
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
        bool DataUpdate()
        { 
            List<CharacterModel.Data> player = GetTeamCharacterDataList(TEAM_TYPE.PLAYER);
            for (int i=0;i<OldHPList.Count;i++)
            { 
                HpData thisHP = OldHPList[i];
                //ダメージ検出
                foreach (CharacterModel.Data character in player)
                {
                    if (character.ActorId == thisHP.id)
                    {
                        if (character.Hp != thisHP.Hp)//ダメージを食らっている
                        {
                            if (character.Hp <= 0)
                            {
                                DieCallBack(character.ActorId);
                            }
                            DamageCallBack(character.ActorId);
                        }
                        thisHP.Hp = character.Hp;
                        OldHPList[i]=thisHP;
                        break;
                    }
                }
            }
            //プレイヤーの情報の更新
            for (int i = 0; i < 6; i++)
            {
                datas[i] = GetCharacterData(i);
            }
            return true;
        }
        //---------------------------------------------------
        // ItemDataUpdate
        //---------------------------------------------------
        void ItemDataUpdate()
        {
            foreach(ItemSpawnData tempItem in FieldItem)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (tempItem.BlockPos ==datas[i].BlockPos&&datas[i].Hp>0)
                    {
                        FieldItem.Remove(tempItem);
                    }
                }
            }
            //FieldItem.Clear();
            
        }
        //---------------------------------------------------
        // MoveUpdate
        //---------------------------------------------------
        void MoveUpdate()
        {
        }  //---------------------------------------------------
        // ActionUpdate
        //---------------------------------------------------
        void ActionUpdate()
        {
        }
        //---------------------------------------------------
        // ItemSpawnCallback
        //---------------------------------------------------
        override public void ItemSpawnCallback(ItemSpawnData itemData)
        {
            FieldItem.Add(itemData);
            //print(itemData.GetType().ToString()+itemData.BlockPos.ToString());
        }

        void DamageCallBack(int actorid)
        {

        }
        void DieCallBack(int actorid)
        {
        }

        List<ItemSpawnData> GetItemNearDistance(Vector2 Playerpos ,ITEM_TYPE targetItem)
        {
            List<ItemSpawnData> ITEMS = new List<ItemSpawnData>(); 
            return ITEMS;
        }
        /// <summary>
        /// 味方を取得
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CharacterModel.Data> GetPlayers()
        {
            if (GetCharacterData(0).TeamType.Equals(TEAM_TYPE.PLAYER))
            {
                yield return datas[0];
                yield return datas[1];
                yield return datas[2];
            }
            else
            {
                yield return datas[3];
                yield return datas[4];
                yield return datas[5];
            }
        }
        /// <summary>
        /// 敵を取得
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CharacterModel.Data> GetEnemies()
        {
            if (GetCharacterData(0).TeamType.Equals(TEAM_TYPE.PLAYER))
            {
                yield return datas[3];
                yield return datas[4];
                yield return datas[5];
            }
            else
            {
                yield return datas[0];
                yield return datas[1];
                yield return datas[2];
            }
        }
        //メイン機を取得
        public CharacterModel.Data GetMainMachine(TEAM_TYPE teamType)
        {
            if (teamType.Equals((GetCharacterData(0).TeamType)))
            {
                return datas[0];
            }
            else return datas[3];
        }
        //サブ機を取得
        public IEnumerator<CharacterModel.Data> GetSubMachines(TEAM_TYPE teamType)
        {
            if (teamType.Equals((GetCharacterData(0).TeamType)))
            {
                yield return datas[1];
                yield return datas[2];
            }
            else
            {
                yield return datas[4];
                yield return datas[5];
            }
        }

        public LinkedList<PathNode> GetPath(Vector2Int start, Vector2Int goal)
        {
            var startNode = new PathNode(start);
            var goalNode = new PathNode(goal);

            var hCost = startNode.GetHeuristicCost(goalNode);
            startNode.costFromStart = 0;
            startNode.heuristicCostToGoal = hCost;
            startNode.parent = null;

            var openList = new PriorityList();
            var closedList = new LinkedList<PathNode>();

            openList.Add(startNode);

            while (openList.Count != 0)
            {
                var curNode = openList[0];
            }

            return null;
        }
    }

    public class PathNode : IComparable
    {
        public Vector2Int pos;

        public int costFromStart;
        public int heuristicCostToGoal;

        public int cost = 1;

        public PathNode parent;

        public PathNode(Vector2Int pos)
        {
            this.pos = pos;
        }

        public bool Equals(Object node)
        {
            if (node is PathNode && ((PathNode) node).Equals(pos))
            {
                return true;
            }

            return false;
        }

        public int GetHeuristicCost(PathNode node)
        {
            return (int) Vector2Int.Distance(pos, node.pos);
        }

        public int CompareTo(object node)
        {
            int c1 = this.costFromStart + this.heuristicCostToGoal;
            int c2 = ((PathNode) node).costFromStart
                     + ((PathNode) node).heuristicCostToGoal;

            if (c1 < c2)
                return -1;
            else if (c1 == c2)
                return 0;
            else
                return 1;
        }

        public LinkedList<PathNode> getNeighbors()
        {
            LinkedList<PathNode> neighbors = new LinkedList<PathNode>();

            neighbors.AddLast(new PathNode(pos + Vector2Int.up));
            neighbors.AddLast(new PathNode(pos + Vector2Int.down));
            neighbors.AddLast(new PathNode(pos + Vector2Int.right));
            neighbors.AddLast(new PathNode(pos + Vector2Int.left));

            return neighbors;
        }
    }

    public class PriorityList : List<PathNode>
    {
        public void Add(PathNode node)
        {
            for (int i = 0; i < Count; i++)
            {
                if (node.CompareTo(this[i]) <= 0)
                {
                    Insert(i, node);
                    return;
                }
            }

            Add(node);
        }
    }
}