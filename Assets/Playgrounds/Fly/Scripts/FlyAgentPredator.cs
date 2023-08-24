using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class FlyAgentPredator : Agent
{
    private Rigidbody _rigidbody;
    
    public float force = 1f;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public void ResetAgent()
    {
        SetReward(0f);
        
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        
        transform.localPosition = new Vector3(Random.value * 14 - 7, 0, Random.value * 14 - 7);
        transform.rotation = Quaternion.identity;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var buffer = new List<Vector3>();
        
        if (actionBuffers.DiscreteActions[0] == 1)
        {
            buffer.Add(transform.position + new Vector3(0, 0, 0.4f));
        }
        
        if (actionBuffers.DiscreteActions[1] == 1)
        {
            buffer.Add(transform.position + new Vector3(0, 0, -0.4f));
        }
        
        if (actionBuffers.DiscreteActions[2] == 1)
        {
            buffer.Add(transform.position + new Vector3(0.4f, 0, 0));
        }
        
        if (actionBuffers.DiscreteActions[3] == 1)
        {
            buffer.Add(transform.position + new Vector3(-0.4f, 0, 0));
        }
        
        foreach (var pos in buffer)
        {
            _rigidbody.AddForceAtPosition(transform.up * force, pos, ForceMode.VelocityChange);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        
        discreteActionsOut[0] = Input.GetKey(KeyCode.W) ? 1 : 0;
        discreteActionsOut[1] = Input.GetKey(KeyCode.S) ? 1 : 0;
        discreteActionsOut[2] = Input.GetKey(KeyCode.D) ? 1 : 0;
        discreteActionsOut[3] = Input.GetKey(KeyCode.A) ? 1 : 0;
    }
}
