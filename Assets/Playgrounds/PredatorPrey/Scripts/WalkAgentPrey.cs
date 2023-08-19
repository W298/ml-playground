using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class WalkAgentPrey : Agent
{
    private PredatorPreyEnv _env;
    private Rigidbody _rigidbody;
    
    public float moveSpeed = 1.7f;
    public float rotateSpeed = 300f;

    private void Start()
    {
        _env = FindObjectOfType<PredatorPreyEnv>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void ResetAgent()
    {
        SetReward(0f);
        
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        
        transform.localPosition = new Vector3(Random.value * 14 - 7, 0, Random.value * 14 - 7);
        transform.rotation = Quaternion.identity;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var forwardAxis = actionBuffers.DiscreteActions[0];
        var rightAxis = actionBuffers.DiscreteActions[1];
        var rotateAxis = actionBuffers.DiscreteActions[2];

        var moveDir = Vector3.zero;
        var rotateDir = Vector3.zero;
        
        switch (forwardAxis)
        {
            case 1:
                moveDir = transform.forward * moveSpeed;
                break;
            case 2:
                moveDir = transform.forward * -moveSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                moveDir = transform.right * moveSpeed;
                break;
            case 2:
                moveDir = transform.right * -moveSpeed;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }
        
        transform.Rotate(rotateDir, Time.deltaTime * rotateSpeed);
        _rigidbody.AddForce(moveDir, ForceMode.VelocityChange);
        
        AddReward(1f / _env.maxEnvStep);
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        
        // forward
        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActionsOut[0] = 2;
        }
        
        // rotate
        if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[2] = 2;
        }
        
        // right
        if (Input.GetKey(KeyCode.L))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.J))
        {
            discreteActionsOut[1] = 2;
        }
    }
    
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-10f / _env.maxEnvStep);
        }
    }
}