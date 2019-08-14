using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ateam
{
    /// <summary>
    /// AIのベースクラス
    /// </summary>
    /// <remarks>
    /// AIを作る際に必要な公開API群
    /// </remarks>
    public class BaseBattleAISystem : BaseMonoBehaviour
    {
        /// <summary>
        /// チームタイプ
        /// </summary>
        public enum TEAM_TYPE
        {
            /// <summary>
            /// 自分の制御できるチーム
            /// </summary>
            PLAYER = 0, 

            /// <summary>
            /// 敵のチーム
            /// 自分では制御できない
            /// </summary>
            ENEMY,

            MAX,
        }

        Define.Battle.TEAM_TYPE _teamType;
        Define.Battle.TEAM_TYPE _enemyTeam;
        ActorManager _actorManager = null;
        BattleSystem _battleSystem = null;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            _actorManager = null;
        }

        /// <summary>
        /// 初期化
        /// 継承先では呼ばないこと
        /// </summary> 
        /// <remarks>
        /// 外部呼び出しのみ
        /// </remarks>
        /// <param name="teamType">AIが動かすごとができるチームタイプ</param>
        /// <param name="battleSystem">保持するバトルロジッククラス</param>
        public void Initialize(Define.Battle.TEAM_TYPE teamType, BattleSystem battleSystem)
        {
            _teamType       = teamType;
            _enemyTeam         = GetEnemyTeamType (_teamType).Value;
            _actorManager   = ApplicationManager.Instance.ActorManager;
            _battleSystem   = battleSystem;
            _battleSystem.ItemManager.ItenSpawnCallBackDelegate += ItemSpawnCallback; 
        }

        /// <summary>
        /// 敵のチームタイプ取得
        /// </summary> 
        /// <returns>敵のチームタイプ</returns>
        /// <param name="type">自分のチームタイプ</param>
        Define.Battle.TEAM_TYPE? GetEnemyTeamType(Define.Battle.TEAM_TYPE type)
        {
            switch (type) 
            {
            case Define.Battle.TEAM_TYPE.ALPHA:
                return Define.Battle.TEAM_TYPE.BRAVO;

            case Define.Battle.TEAM_TYPE.BRAVO:
                return Define.Battle.TEAM_TYPE.ALPHA;
            }

            return null;
        }
                        

        /// <summary>
        /// 継承先で使用する初期化
        /// </summary>
        /// <remarks>
        /// 継承先でのみ呼ばれる初期化メソッド<br>
        /// かならず最初の一回のみ呼ばれる<br>
        /// </remarks>
        virtual public void InitializeAI()
        {

        }
           
        /// <summary>
        /// 継承先で使用する更新メソッド
        /// </summary>
        /// <remarks>
        /// 継承先でのこのメソッド以外の通常アップデートは禁止<br>
        /// </remarks>
        virtual public void UpdateAI()
        {
            
        }

        /// <summary>
        /// 指定したアクターの移動
        /// </summary> 
        /// <remarks>
        /// 指定したアクターIDのオブジェクトを移動させる<br>
        /// 移動はステージのブロックにそってのみ行われる<br>
        /// 斜め移動はなし、上下左右のみ移動命令が行える<br>
        /// １ブロックの移動スピードはアクターに依存する<br>
        /// 移動中のアクター、もしくは制御下にないアクターは移動命令を受け付けない
        /// </remarks>
        /// <returns> 移動可否 </returns>
        /// <param name="actorId">移動させたいアクターID</param>
        public bool Move(int actorId, Common.MOVE_TYPE type)
        {   
            ActorData? actorData    =_actorManager.GetActor(actorId);
            Character character     = actorData.Value.Actor.gameObject.GetComponent<Character>();

            if (actorData == null)
            {
                return false;
            }

            if (character == null || character.CharacterModel.IsMoveUpdate || character.CharacterModel.TeamId != _teamType)
            {
                return false;
            }
                
            Vector2 blockData                       = _battleSystem.StageManager.getPositionBlock(character.gameObject.transform.position + _battleSystem.StageManager.GetMoveVec(type));
            Define.Stage.BLOCK_TYPE blockType       = _battleSystem.StageManager.GetBlockType(blockData);

            if (blockType != (int)Define.Stage.BLOCK_TYPE.NORMAL)
            {
                return false;
            }

            Vector3 targetPos   = _battleSystem.StageManager.GetBlockPosition((int)blockData.x, (int)blockData.y);
            targetPos.y         = character.transform.position.y;

            if (targetPos == character.transform.position)
            {
                return false;
            }

            character.CharacterModel.TargetPos = targetPos;

            return true;
        }

        /// <summary>
        /// チームに所属するキャラクターデータリスト取得
        /// </summary> 
        /// <returns> キャラクターデータリスト </returns>
        /// <param name="type">取得したいチームタイプ</param>
        public List<CharacterModel.Data> GetTeamCharacterDataList(TEAM_TYPE type)
        {
            if (type == TEAM_TYPE.PLAYER) 
            {
                return _battleSystem.BattleModel.TeamCharacterDataList[_teamType];
            }
            else if(type == TEAM_TYPE.ENEMY) 
            {
                return _battleSystem.BattleModel.TeamCharacterDataList[_enemyTeam];
            }

            return null;
        }

        /// <summary>
        /// 指定したアクターIDのキャラクターデータ取得
        /// </summary> 
        /// <returns> 取得したキャラクターデータ </returns>
        /// <param name="actorId">キャラクターデータを取得したいアクターID</param>
        public CharacterModel.Data GetCharacterData(int actorId)
        {
            ActorData? data = _actorManager.GetActor(actorId);

            if (data == null)
            {
                return null;
            }

            Character character = data.Value.Actor.GetComponent<Character>();

            if (character == null)
            {
                return null;
            }

            return character.CharacterModel.PublicCharacterData;
        }

        /// <summary>
        /// 指定したキャラクターアクターの行動実行
        /// </summary> 
        /// <remarks>
        /// キャラクター行動は下記が行える<br>
        /// 遠距離攻撃<br>
        /// 中距離攻撃<br>
        /// 近距離攻撃<br>
        /// 一定時間無敵<br>
        /// なお、１フレーム以内で実行できる行動に制限はないが、同じ行動は一定時間後にしか実行できない
        /// </remarks>
        /// <returns> 行動が実行できる状態であるかどうかの可否 </returns>
        /// <param name="actorId">行動実行させたいキャラクターアクター</param>
        /// <param name="type">実行させたい行動</param>
        public bool Action(int actorId, Define.Battle.ACTION_TYPE type)
        {
            List<CharacterModel.Data> list = GetTeamCharacterDataList(TEAM_TYPE.PLAYER);
            CharacterModel.Data Data = list.Find(obj => obj.ActorId == actorId);

            if (Data == null)
            {
                Debug.LogError("Action Method : not have the right to operate an actor, or the ID is incorrect");
                return false;
            }

            ActorData? actorData = _actorManager.GetActor(Data.ActorId);

            if (actorData == null)
            {
                return false;
            }

            Character character = actorData.Value.Actor.GetComponent<Character>();

            if (character == null)
            {
                return false;            
            }

            return character.ActionManager.StartAction (type);
        }

        /// <summary>
        /// ステージデータ取得
        /// </summary> 
        /// <remarks>
        /// ブロックで区切られたステージデータの全てを取得する<br>
        /// 0で移動できるブロック<br>
        /// 1が障害物のある移動できないブロック<br>
        /// [縦(y),横(x)]で情報が格納されている
        /// 画面左下が[0][0] 
        /// 画面右上が[14][14]　※15×15のステージの場合
        /// </remarks>
        /// <returns> ステージデータ </returns>
        public int[,] GetStageData()
        {
            return (int [,])_battleSystem.StageManager.StageBlockData.Clone();
        }

        /// <summary>
        /// ステージデータの長さ取得
        /// </summary> 
        /// <remarks>
        /// X : 横のブロック数
        /// Y : 縦のブロック数
        /// </remarks>
        /// <returns> ステージデータの縦横ブロック数 </returns>
        public Vector2 GetStageDataBlockNum()
        {
            return new Vector2(_battleSystem.StageManager.HorizontalBlock, _battleSystem.StageManager.VerticalBlock);
        }

        /// <summary>
        /// 指定したブロックマスのブロックデータを取得
        /// </summary> 
        /// <remarks>
        /// 定義でも判別可能だが、intでも判別可能
        /// 0で移動できるブロック<br>
        /// 1が障害物のある移動できないブロック<br>
        /// </remarks>
        /// <returns> ブロック種類 </returns>
        public Define.Stage.BLOCK_TYPE GetBlockType(Vector2 blockPos)
        {
            return _battleSystem.StageManager.GetBlockType(blockPos);
        }

        /// <summary>
        /// アイテムがステージ上に出現したときに呼ばれるコールバック
        /// </summary> 
        /// <remarks>
        /// スポーン時にしか呼ばれないので注意
        /// </remarks>
        /// <param name="itemData">アイテムデータ</param>
        virtual public void ItemSpawnCallback(ItemSpawnData itemData)
        {

        }
    }
}