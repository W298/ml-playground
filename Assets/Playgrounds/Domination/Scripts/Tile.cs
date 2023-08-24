using UnityEngine;

namespace Domination
{
    public class Tile : MonoBehaviour
    {
        public Material[] MaterialAry;
        public int X;
        public int Z;
        
        private DominatorTeams m_filledTeam = DominatorTeams.None;
        public DominatorTeams filledTeam
        {
            get => m_filledTeam;
            set
            {
                m_filledTeam = value;
                Fill(value);
            }
        }

        private void Fill(DominatorTeams newTeam)
        {
            GetComponent<MeshRenderer>().material = MaterialAry[(int)newTeam];
        }
    }
}
