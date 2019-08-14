using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ateam
{
    public class ResultUi : BaseUi
    {
        [SerializeField]
        private Text _alphaWinLoseText = null;
        [SerializeField]
        private Text _bravoWinLoseText = null;

        [SerializeField]
        private Text _alphaTeamNameText = null;
        [SerializeField]
        private Text _bravoTeamNameText = null;

        [SerializeField]
        private Text _alphaScoreText = null;
        [SerializeField]
        private Text _bravoScoreText = null;

        [SerializeField]
        private Button _titleButton = null;

        private RectTransform _rectTransform = null;
        public RectTransform rectTransform
        {
            get {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }

        readonly static Color WIN_COLOR = new Color(1.0f, 1.0f, 0.0f);
        readonly static Color LOSE_COLOR = new Color(0.8f, 0.25f, 1.0f);

        //---------------------------------------------------
        // InitializeCoroutine
        //---------------------------------------------------
        override protected IEnumerator InitializeCoroutine()
        {
            _titleButton.onClick.AddListener(() => {
                    ApplicationManager.Instance.GameSceneManager.ChangeScene(Define.Scenes.Title);
            });

            yield return null;
        }

        //---------------------------------------------------
        // SetResultData
        //---------------------------------------------------
        public void SetResultData(Hashtable table)
        {
            string teamNameAlpha = (string)table["teamNameAlpha"];
            string teamNameBravo = (string)table["teamNameBravo"];
            int scoreAlpha = (int)table["scoreAlpha"];
            int scoreBravo = (int)table["scoreBravo"];

            _alphaWinLoseText.text = (scoreBravo < scoreAlpha) ? "Win" : "Lose";
            _alphaWinLoseText.color =  (scoreBravo < scoreAlpha) ? WIN_COLOR : LOSE_COLOR;
            _bravoWinLoseText.text = (scoreAlpha < scoreBravo) ? "Win" : "Lose";
            _bravoWinLoseText.color =  (scoreAlpha < scoreBravo) ? WIN_COLOR : LOSE_COLOR;

            _alphaTeamNameText.text = teamNameAlpha;
            _bravoTeamNameText.text = teamNameBravo;

            _alphaScoreText.text = scoreAlpha.ToString("00000");
            _bravoScoreText.text = scoreBravo.ToString("00000");
        }
    }
}
