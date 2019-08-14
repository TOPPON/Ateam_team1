using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using NCMB;

namespace Ateam
{
    public class RankingUiMain : BaseUi
    {
        [SerializeField]
        private GameObject _rankingPrefab = null;

        [SerializeField]
        protected VerticalLayoutGroup _verticalLayoutGroup;

        [SerializeField]
        private const int MAX_RANKING_NUM = 5;

        [SerializeField]
        public Text _ateamAIText;

        [SerializeField]
        public Text _otherAIText;

        [SerializeField]
        public List<Hashtable> _currentRankingData = new List<Hashtable>();
        private List<RankingElement> _rankingList = new List<RankingElement>();

        // 指定秒数ごとに更新
        private const float UPDATE_INTERVAL = 5.0f;
        private float _updateTimer = UPDATE_INTERVAL;

        private bool _isConnect = false;
        private bool _isRankingUpdate = false;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            base.Initialize();
        }

        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            base.Release(); 
        }

        //---------------------------------------------------
        // InitializeCoroutine
        //---------------------------------------------------
        override protected IEnumerator InitializeCoroutine()
        {
            GameObject rankingObj = null;
            for (int i = 0; i < MAX_RANKING_NUM; ++i)
            {
                rankingObj = Instantiate(_rankingPrefab);
                rankingObj.transform.SetParent(this._verticalLayoutGroup.transform, false);
                _rankingList.Add(rankingObj.GetComponent<RankingElement>());
            }

            _ateamAIText.text = this.MakeResultRecordTextFormat(BattleSystem.IGNORE_SAVE_SCORE_TEAM_NAME, 0);
            _otherAIText.text = this.MakeResultRecordTextFormat(BattleSystem.RESULT_RECORD_OTHER_TEAME_NAME, 0);

            yield return new WaitForSeconds(0.1f);

            EndInitilaize();

            yield return null;
        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            if (! IsInitialize)
            {
                return;
            }

            if (! _isConnect)
            {
                _updateTimer -= Time.deltaTime;
            }

            if (0.0f < _updateTimer || _isConnect)
            {
                return;
            }

            _isConnect = true;
            _updateTimer = UPDATE_INTERVAL;
            GetHighScoreRanking();
            GetResultRecord();
            StartCoroutine(RankingAnimation());
        }

        //---------------------------------------------------
        // PrepareRankingAnimation
        //---------------------------------------------------
        protected IEnumerator RankingAnimation()
        {
            while (_isConnect)
            {
                yield return null;
            }

            if (! _isRankingUpdate)
            {
                yield break;
            }

            _verticalLayoutGroup.enabled = false;

            for (int i = 0, length = _rankingList.Count; i < length; ++i)
            {
                _rankingList[i].PrepareAnimation();
            }

            yield return null;

            for (int i = _rankingList.Count - 1; i >= 0; --i)
            {
                _rankingList[i].StartAnimation();
                yield return new WaitForSeconds(0.2f);
            }
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
                    _isRankingUpdate = false;

                    int i = 0;
                    foreach (NCMBObject obj in objList) {
                        int    s = System.Convert.ToInt32(obj["Score"]);
                        string n = System.Convert.ToString(obj["Name"]);

                        Hashtable data = Common.CreateHashTable(
                            "rank", i + 1,
                            "name", n,
                            "score", s
                        );

                        _rankingList[i].SetScoreData(data);

                        if (_currentRankingData.Count <= i
                            || (string)_currentRankingData[i]["name"] != n 
                            || (int)_currentRankingData[i]["score"] != s) {
                            _isRankingUpdate = true;
                        }

                        if (_currentRankingData.Count <= i) {
                            _currentRankingData.Add(data);
                        } else {
                            _currentRankingData[i] = data;
                        }

                        Debug.Log("RANK : " + i + " | NAME : " + n + " | SCORE : " + s.ToString());
                        i++;
                    }

                    _isConnect = false;
                }
            });
        }

        //---------------------------------------------------
        // GetResultRecord
        //---------------------------------------------------
        public void GetResultRecord()
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("ResultRecord");
            query.OrderByDescending ("VictoryNum");
            query.Limit = 5;
            query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

                if (e != null) {
                } else {

                    int i = 0;
                    foreach (NCMBObject obj in objList) {
                        int    s = System.Convert.ToInt32(obj["VictoryNum"]);
                        string n = System.Convert.ToString(obj["Name"]);

                        if(n == BattleSystem.IGNORE_SAVE_SCORE_TEAM_NAME)
                        {
                            _ateamAIText.text = this.MakeResultRecordTextFormat(n, s);
                        }
                        else
                        {
                            _otherAIText.text = this.MakeResultRecordTextFormat(n, s);
                        }

                        i++;
                    }
                }
            });
        }

        //---------------------------------------------------
        // 勝利数のテキストフォーマットを作成
        //---------------------------------------------------
        private string MakeResultRecordTextFormat(string i_name, int i_victoryNum)
        {
            return string.Format("{0} : {1}", i_name, i_victoryNum);
        }
    }
}