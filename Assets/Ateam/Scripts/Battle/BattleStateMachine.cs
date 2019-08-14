using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ateam
{
    public enum BATTLE_STATE:int
    {
        NONE = 0, //特殊ステート必須
        INIT,
        STAGE_INIT,
        CHARACTER_INIT,
        BATTLE,
        RESULT,
    }

    public class BattleStateMachine : StateMachine<BATTLE_STATE>
    {
        class JobtypeCompare : IEqualityComparer<BATTLE_STATE>
        {
            public bool Equals(BATTLE_STATE x, BATTLE_STATE y)
            {
                return x == y;
            }

            public int GetHashCode(BATTLE_STATE obj)
            {
                return (int)obj;
            }
        }

        BattleModel _battleModel    = null;
        BattleSystem _battleSystem  = null;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        public void Initialize(BattleSystem system, BattleModel battleModel)
        {
            base.Initialize(new JobtypeCompare());

            _battleModel    = battleModel;
            _battleSystem   = system;

            InitilazeState();
        }

        //---------------------------------------------------
        // InitilazeState
        //---------------------------------------------------
        void InitilazeState()
        {
            Add(BATTLE_STATE.INIT, BattleInitEnter, null, null);
            Add(BATTLE_STATE.STAGE_INIT, StageInitEnter, null, null);
            Add(BATTLE_STATE.CHARACTER_INIT, CharacterInitEnter, null, null);
            Add(BATTLE_STATE.BATTLE, null, BattleAction, null);
            Add(BATTLE_STATE.RESULT, ResultEnter, ResultUpdate, null);
        }

        //---------------------------------------------------
        // BattleInitEnter
        //---------------------------------------------------
        void BattleInitEnter(StateData data)
        {
            _battleModel.FrameTime = Define.Battle.TIME_MAX;

            SetNextState(BATTLE_STATE.STAGE_INIT);
        }

        //---------------------------------------------------
        // StageInitEnter
        //---------------------------------------------------
        void StageInitEnter(StateData data)
        {
            _battleSystem.StageManager.CreateStage(()=>{

                SetNextState(BATTLE_STATE.CHARACTER_INIT);
            });
        }

        //---------------------------------------------------
        // CharacterInitEnter
        //---------------------------------------------------
        void CharacterInitEnter(StateData data)
        {
            //キャラクター作成
            _battleSystem.CreateCharacter(Define.Battle.TEAM_TYPE.ALPHA, 0);
            _battleSystem.CreateCharacter(Define.Battle.TEAM_TYPE.ALPHA, 1);
            _battleSystem.CreateCharacter(Define.Battle.TEAM_TYPE.ALPHA, 1);

            _battleSystem.CreateCharacter(Define.Battle.TEAM_TYPE.BRAVO, 0);
            _battleSystem.CreateCharacter(Define.Battle.TEAM_TYPE.BRAVO, 1);
            _battleSystem.CreateCharacter(Define.Battle.TEAM_TYPE.BRAVO, 1);

            //最大HP計算
            for(int y = 0; y < _battleModel.TeamCharacterList.Count; y++)
            {
                for (int x = 0; x < _battleModel.TeamCharacterList[(Define.Battle.TEAM_TYPE)y].Count; x++)
                {
                    Character character = _battleModel.TeamCharacterList[(Define.Battle.TEAM_TYPE)y][x];
                    _battleModel.SetTotalHp((Define.Battle.TEAM_TYPE)y , character.CharacterModel.Hp);
                    character.CharacterModel.AddListerner(_battleSystem);
                }
            }

            for (int i = 0; i < _battleModel.TotalHp.Count; i++)
            {
                _battleModel.SetMaxHp((Define.Battle.TEAM_TYPE)i, _battleModel.TotalHp[(Define.Battle.TEAM_TYPE)i]);
            }

            for (int i = 0; i < _battleModel.AISystemList.Count; i++)
            {
                _battleModel.AISystemList[(Define.Battle.TEAM_TYPE)i].InitializeAI();
            }

            _battleSystem.ItemManager.CreateStart();

            SetNextState(BATTLE_STATE.BATTLE);
        }

        //---------------------------------------------------
        // BattleAction
        //---------------------------------------------------
        void BattleAction(StateData data)
        {
            for (int i = 0; i < _battleModel.AISystemList.Count; i++)
            {
                _battleModel.AISystemList[(Define.Battle.TEAM_TYPE)i].UpdateAI();
            }

            _battleModel.FrameTime--;

            if (_battleModel.FrameTime <= 0 || _battleSystem.CheckAllDeath())
            {
                SetNextState(BATTLE_STATE.RESULT);
            }
        }

        //---------------------------------------------------
        // ResultEnter
        //---------------------------------------------------
        void ResultEnter(StateData data)
        {
            if (ApplicationManager.Instance.Master.CheckDataAll() == false)
            {
                ApplicationManager.Instance.GameSceneManager.ChangeScene(Define.Scenes.DANGER);
                return;
            }

            int scoreAlpha = _battleSystem.CreateResultScore(Define.Battle.TEAM_TYPE.ALPHA, Define.Battle.TEAM_TYPE.BRAVO);
            int scoreBravo = _battleSystem.CreateResultScore(Define.Battle.TEAM_TYPE.BRAVO, Define.Battle.TEAM_TYPE.ALPHA);

            string teamNameAlpha = _battleModel.TeamNameList[Define.Battle.TEAM_TYPE.ALPHA];
            string teamNameBravo = _battleModel.TeamNameList[Define.Battle.TEAM_TYPE.BRAVO];

            Debug.Log("ALPHA : " + scoreAlpha.ToString());
            Debug.Log("BRAVO : " + scoreBravo.ToString());

            _battleModel.ShowResultWindow(Common.CreateHashTable(
                "scoreAlpha", scoreAlpha, "scoreBravo", scoreBravo,
                "teamNameAlpha", teamNameAlpha, "teamNameBravo", teamNameBravo
            ));

            ApplicationManager.Instance.Battlesystem.SaveScore(teamNameAlpha, scoreAlpha);
            ApplicationManager.Instance.Battlesystem.SaveScore(teamNameBravo, scoreBravo);

            ApplicationManager.Instance.Battlesystem.SaveResultVictoryRecord( (scoreAlpha > scoreBravo) ? teamNameAlpha : teamNameBravo );
        }

        //---------------------------------------------------
        // ResultUpdate
        //---------------------------------------------------
        void ResultUpdate(StateData data)
        {

        }
    }
}
