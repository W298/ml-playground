using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class WalkAgentPredator : Agent
{
    private Rigidbody _rigidbody;
    private WalkAgentPrey _prey;
    private TMPro.TMP_Text _text;
    
    public float moveSpeed = 1f;
    public float rotateSpeed = 300f;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _prey = FindObjectOfType<WalkAgentPrey>();
        _text = FindObjectOfType<TMPro.TMP_Text>();
    }

    private void Update()
    {
        _text.text = (MaxStep - StepCount).ToString();
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
        
        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        
        // forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        
        // rotate
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[2] = 2;
        }
        
        // right
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[1] = 2;
        }
    }

    public override void OnEpisodeBegin()
    {
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        
        transform.localPosition = new Vector3(Random.value * 14 - 7, 0, Random.value * 14 - 7);
        transform.rotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.name == "WalkAgentPrey")
        {
            if (_prey)
            {
                _prey.AddReward(-5f);
                _prey.EndEpisode();
            }

            AddReward(5f);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-10f / MaxStep);
        }
    }
}