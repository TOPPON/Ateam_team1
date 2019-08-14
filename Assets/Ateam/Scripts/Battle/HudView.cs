using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ateam
{
    public class HudView : BillBoard
    {
        [SerializeField]
        private MeshRenderer _markMeshRenderer = null;
        [SerializeField]
        private List<Material> _markMaterial = new List<Material>();

        public void SetTeamMarker(Define.Battle.TEAM_TYPE teamId)
        {
            if (teamId == Define.Battle.TEAM_TYPE.ALPHA)
            {
                _markMeshRenderer.material = _markMaterial[0];
            }
            else if (teamId == Define.Battle.TEAM_TYPE.BRAVO)
            {
                _markMeshRenderer.material = _markMaterial[1];
            }
        }
    }
}