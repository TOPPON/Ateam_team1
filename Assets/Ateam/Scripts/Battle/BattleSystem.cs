
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NCMB;

namespace Ateam
{
    public class BattleSystem : BaseCoroutineMonobehaviour, IObserver
    {
        public static readonly string IGNORE_SAVE_SCORE_TEAM_NAME = "エイチームAI";
        public static readonly string RESULT_RECORD_OTHER_TEAME_NAME = "インターン生AI";

        BattleModel _battleModel = null;
        public BattleModel BattleModel
        {
            get { return _battleModel; }
        }

        BattleStateMachine _stateMachine = new BattleStateMachine();

        [SerializeField]
        StageManager _stageManager = null;
        public StageManager StageManager
        {
            get { return _stageManager; }
        }

        [SerializeField]
        ItemManager _itemManager = null;
        public ItemManager ItemManager
        {
            get { return _itemManager; }
        }

        [SerializeField]
        List<GameObject> _AISystemList = new List<GameObject>();
        public List<GameObject> AISystemList
        {
            get { return _AISystemList; }
        }

        [SerializeField]
        protected List<string> _teamNameList = new List<string>();
        public List<string> TeamNameList
        {
            get { return _teamNameList; }
        }

        [SerializeField]
        GameObject _damageObjectPrefab = null;

        Dictionary<string, Action<Hashtable>> _notifyList   = new Dictionary<string, Action<Hashtable>>();

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            base.Initialize();
        }

        //---------------------------------------------------
        // InitializeCoroutine
        //---------------------------------------------------
        override protected IEnumerator InitializeCoroutine()
        {
            InitRandomGameData();

            while (ApplicationManager.Instance.IsInitialize == false)
            {
                yield return null;
            }

            _battleModel = new BattleModel();
            _battleModel.AddListerner(this);

            while (_stageManager.IsInitilaize == false)
            {
                yield return null;
            }

            _itemManager.Initialize(_battleModel);

            while (_itemManager.IsInitilaize == false)
            {
                yield return null;
            }

            _stateMachine.Initialize(this, _battleModel);
            _stateMachine.SetNextState(BATTLE_STATE.INIT);

            CreateAISystem();

            ApplicationManager.Instance.Battlesystem = this;

            InitializeObserver();

            EndInitilaize();
        }

        //---------------------------------------------------
        // InitializeObserver
        //---------------------------------------------------
        void InitializeObserver()
        {
            _notifyList.Add("EVENT_Hp", EVENT_Hp);
        }

        //---------------------------------------------------
        // InitRandomSeed
        //---------------------------------------------------
        private void InitRandomGameData()
        {
            int seedIndex = PlayerPrefs.GetInt(Define.Config.PREFS_SEED_INDEX_KEY, -1);

            if (seedIndex == -1)
            {
                seedIndex = 0;
            }

            int seed = ApplicationManager.Instance.Master.SeedData.GetData(seedIndex++);
            UnityEngine.Random.InitState(seed);

            if (seedIndex >= ApplicationManager.Instance.Master.SeedData.GetLength())
            {
                seedIndex = 0;
            }

            PlayerPrefs.SetInt(Define.Config.PREFS_SEED_INDEX_KEY, seedIndex);

            if (!ApplicationManager.Instance.Master.IsMasterMode)
            {
                if (seedIndex % 2 == 0)
                {
                    _AISystemList.Reverse();
                    _teamNameList.Reverse();
                }
            }
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            if (_battleModel != null)
            {
                _battleModel.RemoveListerner(this);
                _battleModel.Release();
                _battleModel = null;
            }

            if (_stateMachine != null)
            {
                _stateMachine.Release();
                _stateMachine = null;
            }
        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            if (_stateMachine != null)
            {
                _stateMachine.Update(Time.deltaTime);
            }
        }

        //---------------------------------------------------
        // OnNotify
        //---------------------------------------------------
        virtual public void OnNotify(string eventName, Hashtable hashTable)
        {
            if (_notifyList.ContainsKey(eventName))
            {
                _notifyList[eventName](hashTable);
            }
        }

        //---------------------------------------------------
        // CreateAISystem
        //---------------------------------------------------
        void CreateAISystem()
        {
            for (int i = 0; i < _AISystemList.Count; i++)
            {
                Define.Battle.TEAM_TYPE teamType = (Define.Battle.TEAM_TYPE)i;

                BaseBattleAISystem system = Instantiate(_AISystemList[i]).GetComponent<BaseBattleAISystem>();
                system.Initialize(teamType, this);
                _battleModel.AddAISystem(teamType, system, TeamNameList[i]);
            }
        }

        //---------------------------------------------------
        // CreateCharacter
        //---------------------------------------------------
        public void CreateCharacter(Define.Battle.TEAM_TYPE type, int id)
        {
            Character character = (GameObject.Instantiate(Resources.Load("Prefabs/Character/Character")) as GameObject).GetComponent<Character>();
            character.Initialize(id, type, this.StageManager.GetStartPosition(type));
            _battleModel.AddCharacter(type, character);
        }

        //---------------------------------------------------
        // CreateDamageObject
        //---------------------------------------------------
        public void CreateDamageObject(Define.Battle.TEAM_TYPE type, Vector3 pos , Vector3 range,int attackFrame, float attackPower)
        {
            DamageObject damageObj = Instantiate(_damageObjectPrefab).GetComponent<DamageObject>();
            damageObj.Initialize(type, pos, range, attackFrame, attackPower);
            damageObj.AttackHitDelegate += AttackHitCallBack;
        }

        //---------------------------------------------------
        // AttackHitCallBack
        //---------------------------------------------------
        public void AttackHitCallBack(Actor hitActor, float attackPower)
        {
            ActorManager actorManager   =  ApplicationManager.Instance.ActorManager;
            Character hit               = hitActor.GetComponent<Character>();

            if(hit.CharacterModel.Invincible == false
                && hit.CharacterModel.Hp > 0)
            {
                if (hit.CharacterModel.Hp - attackPower < 0)
                {
                    attackPower = hit.CharacterModel.Hp;
                }

                hit.CharacterModel.Hp -= attackPower;
            }
        }

        //---------------------------------------------------
        // CheckAllDeath
        //---------------------------------------------------
        public bool CheckAllDeath()
        {
            for (int i = 0; i < _battleModel.TotalHp.Count; i++)
            {
                if (_battleModel.TotalHp[(Define.Battle.TEAM_TYPE)i] <= 0)
                {
                    return true;
                }
            }

            return false;
        }

        //---------------------------------------------------
        // CreateResultScore
        //---------------------------------------------------
        public int CreateResultScore(Define.Battle.TEAM_TYPE playerTeamType, Define.Battle.TEAM_TYPE enemyTeamType)
        {
            int playerScoreHp       = (int)_battleModel.TotalHp[playerTeamType];
            int playerScoreAlive    = 0;
            int enemyScoreHp        = (int)_battleModel.TotalHp[enemyTeamType];
            int playerScoreKill     = 0;

            int playerAliveCount = 0;
            for (int i = 0; i < _battleModel.TeamCharacterList[playerTeamType].Count; i++)
            {
                if (_battleModel.TeamCharacterList[playerTeamType][i].CharacterModel.Hp > 0)
                {
                    playerAliveCount++;
                }
            }

            int enemyKillCount = 0;
            for (int i = 0; i < _battleModel.TeamCharacterList[enemyTeamType].Count; i++)
            {
                if (_battleModel.TeamCharacterList[enemyTeamType][i].CharacterModel.Hp <= 0)
                {
                    enemyKillCount++;
                }
            }

            //係数計算
            playerScoreHp       = playerScoreHp * 1;
            playerScoreAlive    = playerAliveCount * 500;
            playerScoreKill     = enemyKillCount * 500;
            enemyScoreHp        = enemyScoreHp * 1;

            return (playerScoreHp + playerScoreAlive + playerScoreKill - enemyScoreHp);
        }

        //---------------------------------------------------
        // EVENT_Hp
        //---------------------------------------------------
        public void EVENT_Hp(Hashtable table)
        {
            if (table.ContainsKey("diff") && table.ContainsKey("teamId"))
            {
                float diff                      = (float)table["diff"];
                Define.Battle.TEAM_TYPE type = (Define.Battle.TEAM_TYPE)table["teamId"];

                _battleModel.SetTotalHp(type, diff);
            }
        }

        //---------------------------------------------------
        // SaveScore
        //---------------------------------------------------
        public void SaveScore(string name, int score)
        {
            if (name == IGNORE_SAVE_SCORE_TEAM_NAME)
            {
                return;
            }

            //データストア検索
            // データストアの「HighScore」クラスから、Nameをキーにして検索
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("HighScore");
            query.WhereEqualTo ("Name", name);
            query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

                //検索成功したら
                if (e == null) {
                    if(objList.Count > 0)
                    {
                        if(System.Convert.ToInt32(objList[0]["Score"]) < score)
                        {
                            objList[0]["Score"] = score;
                            objList[0].SaveAsync();
                        }
                    }
                    //新規登録
                    else
                    {
                        //スコアデータ送信
                        NCMBObject obj = new NCMBObject("HighScore");
                        obj["Name"]  = name;
                        obj["Score"] = score;
                        obj.SaveAsync();
                    }

                }
            });
        }

        //---------------------------------------------------
        // SaveResultVictoryRecord
        //---------------------------------------------------
        public void SaveResultVictoryRecord(string name)
        {
            if (name != IGNORE_SAVE_SCORE_TEAM_NAME)
            {
                name = RESULT_RECORD_OTHER_TEAME_NAME;
            }

            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("ResultRecord");
            query.WhereEqualTo ("Name", name);
            query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

                if (e == null) {
                    if(objList.Count > 0)
                    {
                        int temp = Convert.ToInt32(objList[0]["VictoryNum"]);
                        objList[0]["VictoryNum"] = temp + 1;
                        objList[0].SaveAsync();
                    }
                    else
                    {
                        NCMBObject obj = new NCMBObject("ResultRecord");
                        obj["Name"]  = name;
                        obj["VictoryNum"] = 1;
                        obj.SaveAsync();
                    }

                }
            });
        }

        //---------------------------------------------------
        // GetHighScoreRanking
        //---------------------------------------------------
        public void GetHighScoreRanking()
        {
            // データストアの「HighScore」クラスから検索
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("HighScore");
            query.OrderByDescending ("Score");
            query.Limit = 5;
            query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

                if (e != null) {
                    //検索失敗時の処理
                } else {
                    //検索成功時の処理
                    // 取得したレコードをHighScoreクラスとして保存
                    int i = 1;
                    foreach (NCMBObject obj in objList) {
                        int    s = System.Convert.ToInt32(obj["Score"]);
                        string n = System.Convert.ToString(obj["Name"]);

                        Debug.Log("RANK : " + i + " | NAME : " + n + " | SCORE : " + s.ToString());
                        i++;
                    }
                }
            });
        }


    }
}
