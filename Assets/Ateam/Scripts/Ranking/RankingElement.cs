using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ateam
{
    public class RankingElement : BaseUi 
    {
        [SerializeField]
        private Text _headerText = null;
        [SerializeField]
        private Text _teamNameText = null;
        [SerializeField]
        private Text _scoreText = null;

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

        private Vector2 _basePosition = Vector2.zero;
        private bool _isAnimation = false;
        private const float ANIMATION_SPEED = 20.0f;

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
            yield return null;

            EndInitilaize();
        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            RunAnimation();
        }

        public void PrepareAnimation()
        {
            _basePosition = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition += Vector2.left * 1000.0f;
        }

        public void StartAnimation()
        {
            _isAnimation = true;
        }

        public void RunAnimation()
        {
            if (! _isAnimation)
            {
                return;
            }

            rectTransform.anchoredPosition += Vector2.right * ANIMATION_SPEED;

            if ((_basePosition - rectTransform.anchoredPosition).magnitude < ANIMATION_SPEED)
            {
                _isAnimation = false;
            }
        }

        //---------------------------------------------------
        // InitializeCoroutine
        //---------------------------------------------------
        public void SetScoreData(Hashtable data)
        {
            int rank = (int) data["rank"];
            string teamName = (string) data["name"];
            int score = (int) data["score"];

            _headerText.text = rank.ToString() + "位";
            _teamNameText.text = teamName;
            _scoreText.text = score.ToString("00000") + "pt";
        }
    }
}