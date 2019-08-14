using UnityEngine;
using System.Collections;
using System;

namespace Ateam
{
    public class StageManager : BaseMonoBehaviour
    {
        [SerializeField]
        int _verticalBlockNum    = 5;
        public int VerticalBlock
        {
            get { return _verticalBlockNum; }
        }

        [SerializeField]
        int _horizontalBlockNum  = 5; 
        public int HorizontalBlock
        {
            get { return _horizontalBlockNum; }
        }

        [SerializeField]
        int _normalBlockRate = 90;

        [SerializeField]
        int _obstacleBlockRate = 10;

        int [,] _stageBlockData;
        public int [,] StageBlockData
        {
            get { return _stageBlockData; }
        }

        GameObject _stageRoot = null;

        bool _isInitilaize = false;
        public bool IsInitilaize
        {
            get { return _isInitilaize; }
        }

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            _stageBlockData = new int[_verticalBlockNum, _horizontalBlockNum];

            _stageRoot = new GameObject("StageRoot");

            CreateStageData();

            _isInitilaize = true;
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
            _stageBlockData = null;
        }
    
        //---------------------------------------------------
        // CreateStage
        //---------------------------------------------------
        void CreateStageData()
        {
            for (int y = 0; y < _verticalBlockNum; y++)
            {
                for (int x = 0; x < _horizontalBlockNum; x++)
                {
                    _stageBlockData[y, x] = CreateBlockData();
                }
            }

            //プレイヤーの所と外周は障害物なし
            _stageBlockData[0, 0] = (int)Define.Stage.BLOCK_TYPE.NORMAL;
            _stageBlockData[_verticalBlockNum - 1, _horizontalBlockNum - 1] = (int)Define.Stage.BLOCK_TYPE.NORMAL;

            for (int y = 0; y < _verticalBlockNum; y++)
            {
                _stageBlockData[y, 0]                       = (int)Define.Stage.BLOCK_TYPE.NORMAL;
                _stageBlockData[y, _horizontalBlockNum - 1] = (int)Define.Stage.BLOCK_TYPE.NORMAL;
            }

            for (int x = 0; x < _verticalBlockNum; x++)
            {
                _stageBlockData[0, x]                       = (int)Define.Stage.BLOCK_TYPE.NORMAL;
                _stageBlockData[_horizontalBlockNum - 1, x] = (int)Define.Stage.BLOCK_TYPE.NORMAL;
            }

        }

        //---------------------------------------------------
        // CreateBlock
        //---------------------------------------------------
        int CreateBlockData()
        {
            int rate = UnityEngine.Random.Range(0, 100);

            if (rate < _obstacleBlockRate)
            {
                return (int)Define.Stage.BLOCK_TYPE.OBSTACLE;
            }
            else if(rate > _obstacleBlockRate && rate < _normalBlockRate)
            {
                return (int)Define.Stage.BLOCK_TYPE.NORMAL;
            }

            return (int)Define.Stage.BLOCK_TYPE.NORMAL;
        }

        //---------------------------------------------------
        // CreateStage
        //---------------------------------------------------
        public void CreateStage(Action callback)
        {
            StartCoroutine(CreateStageColoutine(callback));
        }

        //---------------------------------------------------
        // CreateStageColoutine
        //---------------------------------------------------
        IEnumerator CreateStageColoutine(Action callback)
        {
            Master master = ApplicationManager.Instance.Master;

            for (int y = 0; y < _verticalBlockNum; y++)
            {
                for (int x = 0; x < _horizontalBlockNum; x++)
                {
                    string path = master.StageData.GetData(_stageBlockData[y, x]).viewPrefabPath;

                    if (master.StageData.GetData(_stageBlockData[y, x]).BlockType == Ateam.Define.Stage.BLOCK_TYPE.NORMAL &&
                        ((x + y) % 2) != 0)
                    {
                        path = master.StageData.GetData(_stageBlockData[y, x]).viewPrefabPathTwo;
                    }

                    yield return Common.LoadAsync(path, (obj)=>{

                        GameObject go = Instantiate(obj as GameObject); 
                        go.transform.SetParent(_stageRoot.transform, false);
                        go.transform.position = GetBlockPosition(x, y);
                    });
                }
            }

            callback();
        }

        //---------------------------------------------------
        // GetBlockPosition
        //---------------------------------------------------
        public Vector3 GetBlockPosition(int x, int y)
        {
            return new Vector3(-((float)(_horizontalBlockNum * Define.Battle.BLOCK_SIZE) / 2) + (x * 1), 0,-((float)(_verticalBlockNum * Define.Battle.BLOCK_SIZE)  / 2) + (y * 1));
        }

        //---------------------------------------------------
        // GetStartPosition
        //---------------------------------------------------
        public Vector3 GetStartPosition(Define.Battle.TEAM_TYPE type)
        {
            if (type == Define.Battle.TEAM_TYPE.ALPHA)
            {
                return GetBlockPosition(0, 0);
            }
            else if (type == Define.Battle.TEAM_TYPE.BRAVO)
            {
                return GetBlockPosition( _horizontalBlockNum - 1,  _verticalBlockNum - 1);
            }

            return GetBlockPosition(0, 0);
        }

        //---------------------------------------------------
        // getPositionBlock
        //---------------------------------------------------
        public Vector2 getPositionBlock(Vector3 pos)
        {
            Vector2 outData = new Vector2();
            float hor = (float)(_horizontalBlockNum * Define.Battle.BLOCK_SIZE) / 2;
            float ver = (float)(_verticalBlockNum * Define.Battle.BLOCK_SIZE)  / 2;

            outData.x = Mathf.RoundToInt(pos.x + hor);
            outData.y = Mathf.RoundToInt(pos.z + ver);

            return outData;
        }

        //---------------------------------------------------
        // GetMoveVec
        //---------------------------------------------------
        public Vector3 GetMoveVec(Common.MOVE_TYPE type)
        {
            Vector3 outVec = Vector3.zero;

            switch (type)
            {
                case Common.MOVE_TYPE.DOWN:
                    outVec = Vector3.back;
                    break;
                
                case Common.MOVE_TYPE.UP:
                    outVec = Vector3.forward;
                    break;

                case Common.MOVE_TYPE.RIGHT:
                    outVec = Vector3.right;
                    break;

                case Common.MOVE_TYPE.LEFT:
                    outVec = Vector3.left;
                    break;
            }

            return outVec * Define.Battle.BLOCK_SIZE;
        }

        //---------------------------------------------------
        // GetBlockType
        //---------------------------------------------------
        public Define.Stage.BLOCK_TYPE GetBlockType(Vector2 data)
        {
            if (data.x < 0 || data.y < 0 || data.x >= _horizontalBlockNum || data.y >= _verticalBlockNum)
            {
                return Define.Stage.BLOCK_TYPE.NONE;
            }

            return (Define.Stage.BLOCK_TYPE)_stageBlockData[(int)data.y, (int)data.x];
        }
            
    }
}