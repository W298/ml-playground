using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

namespace Domination
{
    [RequireComponent(typeof(DecisionRequester))]
    public class Dominator : Agent
    {
        private Vector3 m_lookingDirection = new(0, 0, 1);
        public Vector3 LookingDirection
        {
            get => m_lookingDirection;
            set
            {
                m_lookingDirection = value;
                transform.localRotation = Quaternion.LookRotation(value);
            }
        }
        
        public DominationEnv Env;
        public DominatorTeams Team;

        private void Respawn()
        {
            // Reset Filled Tile
            Env.GetTileContainer().ForEach(pair =>
            {
                if (pair.Value.filledTeam == Team)
                {
                    pair.Value.filledTeam = DominatorTeams.None;
                }
            });
            
            // Set New Location
            var innerSize = Env.EnvSize / 2;
            int x, z;
            
            do
            {
                x = Random.Range(-innerSize, innerSize + 1);
                z = Random.Range(-innerSize, innerSize + 1);
            } while (Env.GetDominatorList().Exists(d => Mathf.Abs(d.transform.localPosition.x - x) <= 3 || Mathf.Abs(d.transform.localPosition.z - z) <= 3));

            transform.localPosition = new Vector3(x, 1.2f, z);
            switch (Random.Range(0, 5))
            {
                case 0:
                    LookingDirection = new Vector3(0, 0, 1);
                    break;
                case 1:
                    LookingDirection = new Vector3(0, 0, -1);
                    break;
                case 2:
                    LookingDirection = new Vector3(1, 0, 0);
                    break;
                case 3:
                    LookingDirection = new Vector3(-1, 0, 0);
                    break;
                case 4:
                    break;
            }
            
            Env.FillTile(x, z, Team);
        }
        
        private bool IsPointValid(int x, int z)
        {
            return Mathf.Abs(x) <= Env.EnvSize && Mathf.Abs(z) <= Env.EnvSize;
        }

        private void CheckNeedFloodFill(int x, int z, int lastX, int lastZ)
        {
            var isChanged = Env.FillTile(x, z, Team);
            if (isChanged)
            {
                var searchAry = new List<Tuple<int, int>>
                {
                    new(x + 1, z),
                    new(x - 1, z),
                    new(x, z + 1),
                    new(x, z - 1)
                };

                var conFound = false;
                var conX = -1;
                var conZ = -1;
                
                foreach (var (xx, zz) in searchAry.Where(t => IsPointValid(t.Item1, t.Item2)))
                {
                    if (lastX == xx && lastZ == zz) continue;
                    if (Env.CheckFill(xx, zz, Team))
                    {
                        conFound = true;
                        conX = xx;
                        conZ = zz;
                        break;
                    }
                }

                if (conFound)
                {
                    var extendedSearchAry = new List<Tuple<int, int>>
                    {
                        new(x + 1, z),
                        new(x + 1, z + 1),
                        new(x, z + 1),
                        new(x - 1, z + 1),
                        new(x - 1, z),
                        new(x - 1, z - 1),
                        new(x, z - 1),
                        new(x + 1, z - 1),
                    };

                    foreach (var (ex, ez) in extendedSearchAry.Where(t => IsPointValid(t.Item1, t.Item2)))
                    {
                        if (ex == conX && ez == conZ) continue;
                        if (Env.CheckFill(ex, ez, Team)) continue;

                        var right = false;
                        for (var i = ex + 1; i <= Env.EnvSize; i++)
                        {
                            if (Env.CheckFill(i, ez, Team))
                            {
                                right = true;
                                break;
                            }
                        }

                        var left = false;
                        for (var i = ex - 1; i >= -Env.EnvSize; i--)
                        {
                            if (Env.CheckFill(i, ez, Team))
                            {
                                left = true;
                                break;
                            }
                        }

                        var up = false;
                        for (var j = ez + 1; j <= Env.EnvSize; j++)
                        {
                            if (Env.CheckFill(ex, j, Team))
                            {
                                up = true;
                                break;
                            }
                        }

                        var down = false;
                        for (var j = ez - 1; j >= -Env.EnvSize; j--)
                        {
                            if (Env.CheckFill(ex, j, Team))
                            {
                                down = true;
                                break;
                            }
                        }

                        if (right && left && up && down)
                        {
                            var buffer = new List<FloodFillBuffer>();
                            Env.FloodFill(ex, ez, Team, ref buffer);

                            if (!buffer.Exists(e => e.IsNotClosed))
                            {
                                foreach (var container in buffer)
                                {
                                    Env.FillTile(container.X, container.Z, Team);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public void Init(DominationEnv env, Material material, DominatorTeams team)
        {
            Env = env;
            Team = team;
            
            transform.GetChild(0).GetComponent<MeshRenderer>().material = material;
            transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>().material = material;
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            var lastX = Mathf.FloorToInt(transform.localPosition.x);
            var lastZ = Mathf.FloorToInt(transform.localPosition.z);

            var direction = Vector3.zero;
            switch (actionBuffers.DiscreteActions[0])
            {
                case 0:
                    direction = new Vector3(0, 0, 1);
                    break;
                case 1:
                    direction = new Vector3(0, 0, -1);
                    break;
                case 2:
                    direction = new Vector3(1, 0, 0);
                    break;
                case 3:
                    direction = new Vector3(-1, 0, 0);
                    break;
                case 4:
                    break;
            }

            if (actionBuffers.DiscreteActions[0] != 4)
            {
                transform.localPosition += direction;
                LookingDirection = direction;
            }
            else
            {
                transform.localPosition += LookingDirection;
            }

            var x = Mathf.FloorToInt(transform.localPosition.x);
            var z = Mathf.FloorToInt(transform.localPosition.z);

            var oldCount = Env.CountTile(Team);

            if (!IsPointValid(x, z))
            {
                Respawn();
            }
            else
            {
                CheckNeedFloodFill(x, z, lastX, lastZ);
            }
            
            // Add Reward Depending on Delta Tile Count.
            AddReward((float)(Env.CountTile(Team) - oldCount) / Env.TotalTileCount);
            // SetReward((float)(Env.CountTile(Team)) / Env.TotalTileCount);
        }
        
        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(Mathf.FloorToInt(transform.localPosition.x));
            sensor.AddObservation(Mathf.FloorToInt(transform.localPosition.z));

            var dir =
                transform.forward == new Vector3(0, 0, 1) ? 0 :
                transform.forward == new Vector3(0, 0, -1) ? 1 :
                transform.forward == new Vector3(1, 0, 0) ? 2 :
                transform.forward == new Vector3(-1, 0, 0) ? 3 : 0;
            
            sensor.AddObservation(dir);
            
            // Add for Multi-Agent
            // foreach (var otherDominator in Env.GetDominatorList())
            // {
            //     if (otherDominator.Team == Team) continue;
            //     
            //     var ox = Mathf.FloorToInt(otherDominator.transform.localPosition.x);
            //     var oz = Mathf.FloorToInt(otherDominator.transform.localPosition.z);
            //     
            //     sensor.AddObservation(ox);
            //     sensor.AddObservation(oz);
            // }
            
            foreach (var (_, value) in Env.GetTileContainer())
            {
                sensor.AddObservation((int)value.filledTeam);
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut[0] =
                Input.GetKey(KeyCode.W) ? 0 :
                Input.GetKey(KeyCode.S) ? 1 :
                Input.GetKey(KeyCode.D) ? 2 :
                Input.GetKey(KeyCode.A) ? 3 : 4;
        }
    }
}