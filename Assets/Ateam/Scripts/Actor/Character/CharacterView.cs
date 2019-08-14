using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ateam
{
    public class CharacterView : ActorView
    {
        Dictionary<string, Action<Hashtable>> _notifyList   = new Dictionary<string, Action<Hashtable>>();

        Master _master                  = null;
        EffectManager _effectManager    = null;
        GameObject _powerUpEffect       = null;
        GameObject _speedUpEffect       = null;
        GameObject _teamMaker = null;

        //---------------------------------------------------
        // Initialize
        //---------------------------------------------------
        override protected void Initialize()
        {
            _master         = ApplicationManager.Instance.Master;
            _effectManager  = ApplicationManager.Instance.EffectManager;

            InitializeObderver();
        }
    
        //---------------------------------------------------
        // Release
        //---------------------------------------------------
        override protected void Release()
        {
    
        }

        //---------------------------------------------------
        // InitializeObderver
        //---------------------------------------------------
        void InitializeObderver()
        {
            _notifyList.Add("EVENT_Hp", EVENT_Hp);
            _notifyList.Add("EVENT_AttackPowerBias", EVENT_AttackPowerBias);
            _notifyList.Add("EVENT_Speed", EVENT_Speed);
        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------
        void Update()
        {
            if (_powerUpEffect != null)
            {
                _powerUpEffect.transform.position = transform.position;
            }

            if (_speedUpEffect != null)
            {
                _speedUpEffect.transform.position = transform.position;
            }
        }

        //---------------------------------------------------
        // OnNotify
        //---------------------------------------------------
        override public void OnNotify(string eventName, Hashtable hashTable)
        {
            if (_notifyList.ContainsKey(eventName))
            {
                _notifyList[eventName](hashTable);
            }
        }

        //---------------------------------------------------
        // EVENT_TeamId
        //---------------------------------------------------
        public void SetTeamId(Define.Battle.TEAM_TYPE teamId)
        {
            _teamMaker = Instantiate(Resources.Load("Prefabs/Character/Hud") as GameObject);
            _teamMaker.transform.position = Vector3.up;
            _teamMaker.transform.SetParent(this.transform, false);
            _teamMaker.GetComponent<HudView>().SetTeamMarker(teamId);
        }

        //---------------------------------------------------
        // EVENT_Hp
        //---------------------------------------------------
        void EVENT_Hp(Hashtable table)
        {
            if (table.ContainsKey("hp"))
            {
                bool enable = false;

                if ((float)table["hp"] <= 0)
                {
                    string path = _master.EffectData.GetPath(Define.EffectType.EXPLOSION);
                    _effectManager.Play(path, transform.position, Vector3.zero, null);

                    enable = false;
                }
                else
                {
                    enable = true;
                }

                gameObject.SetActive(enable);
            }

            if(table.ContainsKey("diff"))
            {
                if ((float)table["diff"] < 0)
                {
                    string path = _master.EffectData.GetPath(Define.EffectType.HIT);
                    _effectManager.Play(path, transform.position, Vector3.zero, null);
                }
                else
                {
                    string path = _master.EffectData.GetPath(Define.EffectType.HP_RECOVER);
                    _effectManager.Play(path, transform.position, Vector3.zero, null);
                }
            }
        }

        //---------------------------------------------------
        // EVENT_AttackPowerBias
        //---------------------------------------------------
        void EVENT_AttackPowerBias(Hashtable table)
        {
            if (table.ContainsKey("diff"))
            {
                if ((float)table["diff"] > 0)
                {
                    string path = _master.EffectData.GetPath(Define.EffectType.ATTACK_POWERUP);

                    if (_powerUpEffect == null)
                    {
                        _effectManager.Play(path, transform.position, Vector3.zero, (obj) =>
                            {
                                _powerUpEffect = obj;
                            });
                    }
                }
                else if((float)table["diff"] < 0)
                {
                    if (_powerUpEffect != null)
                    {
                        _powerUpEffect.SetActive(false);
                        _powerUpEffect = null;                    
                    }
                }
            }
        }

        //---------------------------------------------------
        // EVENT_Speed
        //---------------------------------------------------
        void EVENT_Speed(Hashtable table)
        {
            if (table.Contains("diff"))
            {
                if ((float)table["diff"] > 0)
                {
                    string path = _master.EffectData.GetPath(Define.EffectType.SPEED_UP);

                    if (_speedUpEffect == null)
                    {
                        _effectManager.Play(path, transform.position, Vector3.zero, (obj) =>
                            {
                                _speedUpEffect = obj;
                            });
                    }
                }
                else if ((float)table["diff"] < 0)
                {
                    if (_speedUpEffect != null)
                    {
                        _speedUpEffect.SetActive(false);
                        _speedUpEffect = null;                    
                    }
                }
            }
        }
    }
}