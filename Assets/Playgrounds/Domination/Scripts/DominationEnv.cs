using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Domination
{
    public enum DominatorTeams
    {
        None,
        Red,
        Yellow,
        Purple,
        Green
    }
    
    [Serializable]
    public struct TeamMaterial
    {
        public DominatorTeams Team;
        public Material Material;
        public bool IsEnabled;
    }
    
    public struct FloodFillBuffer
    {
        public int X;
        public int Z;
        public bool IsNotClosed;
    }
    
    public class DominationEnv : MonoBehaviour
    {
        private TMPro.TMP_Text m_remainStepText;
        private List<Dominator> m_dominatorList = new();
        private readonly Dictionary<Tuple<int, int>, Tile> m_tileContainer = new();

        [TitleGroup("Timer")]
        public int MaxEnvStep = 500;
        
        [ReadOnly]
        public int ElapsedStep;
        
        [TitleGroup("Prefabs")]
        public GameObject TilePrefab;
        public GameObject DominatorPrefab;
        
        [TitleGroup("Child")]
        public GameObject Field;
        
        [TitleGroup("Env Configure")]
        public int EnvSize = 30;
        
        [TitleGroup("Env Configure")]
        [ReadOnly]
        public int TotalTileCount = 3721;
        
        [TitleGroup("Env Configure")]
        [Button(ButtonSizes.Medium)]
        private void SpawnTiles()
        {
            foreach (var tile in transform.GetComponentsInChildren<Tile>())
            {
                DestroyImmediate(tile.gameObject);
            }

            for (var i = -EnvSize; i <= EnvSize; i += 1)
            {
                for (var j = -EnvSize; j <= EnvSize; j += 1)
                {
                    var tileObj = Instantiate(TilePrefab, Vector3.zero, Quaternion.identity);
                    tileObj.transform.SetParent(Field.transform);
                    tileObj.transform.localScale = Vector3.one;
                    tileObj.transform.localPosition = new Vector3(i, 0, j);

                    var tile = tileObj.GetComponent<Tile>();
                    tile.X = i;
                    tile.Z = j;
                }
            }

            TotalTileCount = (EnvSize * 2 + 1) * (EnvSize * 2 + 1);
        }
        
        [TitleGroup("Team Configure")]
        public List<TeamMaterial> TeamList;
        
        [TitleGroup("Team Configure")]
        [Button(ButtonSizes.Medium)]
        private void SpawnDominators()
        {
            foreach (var oldDominator in transform.GetComponentsInChildren<Dominator>())
            {
                DestroyImmediate(oldDominator.gameObject);
            }

            for (var i = 0; i < TeamList.Count; i++)
            {
                if (!TeamList[i].IsEnabled) continue;
                
                var d = Instantiate(DominatorPrefab, Vector3.zero, Quaternion.identity);
                d.transform.SetParent(transform);
                d.transform.localPosition = new Vector3(0, 1.2f, i);
                
                var dom = d.GetComponent<Dominator>();
                dom.Init(this, TeamList[i].Material, TeamList[i].Team);
            }
        }

        private void Start()
        {
            m_remainStepText = FindObjectOfType<TMPro.TMP_Text>();
            
            m_dominatorList = GetComponentsInChildren<Dominator>().ToList();

            foreach (var tile in GetComponentsInChildren<Tile>())
                m_tileContainer[new Tuple<int, int>(tile.X, tile.Z)] = tile;
            
            ResetEnv();
        }

        private void FixedUpdate()
        {
            ElapsedStep += 1;
            if (ElapsedStep >= MaxEnvStep && MaxEnvStep > 0)
            {
                EndEpisode();
            }

            m_remainStepText.text = (MaxEnvStep - ElapsedStep).ToString();
        }

        private void ResetEnv()
        {
            ElapsedStep = 0;
            
            foreach (var (_, value) in m_tileContainer)
            {
                value.filledTeam = DominatorTeams.None;
            }

            var innerSize = EnvSize / 2;

            var buffer = new List<Tuple<int, int>>();
            foreach (var dominator in m_dominatorList)
            {
                dominator.SetReward(0f);
                
                int x, z;
                do
                {
                    x = Random.Range(-innerSize, innerSize + 1);
                    z = Random.Range(-innerSize, innerSize + 1);
                } while (buffer.Exists(b => Mathf.Abs(b.Item1 - x) <= 3 || Mathf.Abs(b.Item2 - z) <= 3));
                buffer.Add(new Tuple<int, int>(x, z));
                
                dominator.transform.localPosition = new Vector3(x, 1.2f, z);
                switch (Random.Range(0, 5))
                {
                    case 0:
                        dominator.LookingDirection = new Vector3(0, 0, 1);
                        break;
                    case 1:
                        dominator.LookingDirection = new Vector3(0, 0, -1);
                        break;
                    case 2:
                        dominator.LookingDirection = new Vector3(1, 0, 0);
                        break;
                    case 3:
                        dominator.LookingDirection = new Vector3(-1, 0, 0);
                        break;
                    case 4:
                        break;
                }
                
                FillTile(x, z, dominator.Team);
            }
        }
        
        private void EndEpisode()
        {
            var tempList = m_dominatorList.ToList();
            var rankDict = new Dictionary<Dominator, int>();
            var highestRank = 0;
            
            while (rankDict.Keys.Count < m_dominatorList.Count)
            {
                var max = tempList.Max(d => CountTile(d.Team));
                var maxList = tempList.FindAll(d => CountTile(d.Team) == max);
                
                foreach (var maxDom in maxList)
                {
                    rankDict[maxDom] = highestRank;
                }

                tempList.RemoveAll(d => CountTile(d.Team) == max);
                highestRank++;
            }

            
            var filledTileCount = m_dominatorList.Sum(dominator => CountTile(dominator.Team));
            foreach (var dominator in m_dominatorList)
            {
                var percentOfTotal = (float)(CountTile(dominator.Team)) / filledTileCount;
                if (rankDict[dominator] == 0)
                {
                    dominator.SetReward(percentOfTotal);
                }
                else
                {
                    dominator.SetReward(percentOfTotal - 1);
                }
                
                dominator.EndEpisode();
            }

            ResetEnv();
        }
        
        private bool IsPointValid(int x, int z)
        {
            return Mathf.Abs(x) <= EnvSize && Mathf.Abs(z) <= EnvSize;
        }

        public Dictionary<Tuple<int, int>, Tile> GetTileContainer()
        {
            return m_tileContainer;
        }

        public List<Dominator> GetDominatorList()
        {
            return m_dominatorList;
        }

        public bool FillTile(int x, int z, DominatorTeams team)
        {
            var targetTile = m_tileContainer[new Tuple<int, int>(x, z)];
            var isChanged = targetTile.filledTeam != team;

            targetTile.filledTeam = team;

            return isChanged;
        }

        public bool CheckFill(int x, int z, DominatorTeams team)
        {
            if (!IsPointValid(x, z)) return false;
            return m_tileContainer[new Tuple<int, int>(x, z)].filledTeam == team;
        }

        public void FloodFill(int x, int z, DominatorTeams team, ref List<FloodFillBuffer> buffer)
        {
            if (buffer.Exists(e => e.IsNotClosed)) return;
            
            var right = false;
            for (var i = x + 1; i <= EnvSize; i++)
            {
                if (CheckFill(i, z, team))
                {
                    right = true;
                    break;
                }
            }

            var left = false;
            for (var i = x - 1; i >= -EnvSize; i--)
            {
                if (CheckFill(i, z, team))
                {
                    left = true;
                    break;
                }
            }

            var up = false;
            for (var j = z + 1; j <= EnvSize; j++)
            {
                if (CheckFill(x, j, team))
                {
                    up = true;
                    break;
                }
            }

            var down = false;
            for (var j = z - 1; j >= -EnvSize; j--)
            {
                if (CheckFill(x, j, team))
                {
                    down = true;
                    break;
                }
            }

            if (right && left && up && down)
            {
                buffer.Add(new FloodFillBuffer() { X = x, Z = z, IsNotClosed = false });
            }
            else
            {
                buffer.Add(new FloodFillBuffer() { X = x, Z = z, IsNotClosed = true });
                return;
            }
            
            var searchAry = new List<Tuple<int, int>>
            {
                new(x + 1, z),
                new(x - 1, z),
                new(x, z + 1),
                new(x, z - 1)
            };

            foreach (var (xx, zz) in searchAry.Where(t => IsPointValid(t.Item1, t.Item2)))
            {
                if (CheckFill(xx, zz, team) || buffer.Exists(e => e.X == xx && e.Z == zz)) continue;
                FloodFill(xx, zz, team, ref buffer);
            }
        }

        public int CountTile(DominatorTeams team)
        {
            return m_tileContainer.Values.Count(t => t.filledTeam == team);
        }
    }
}