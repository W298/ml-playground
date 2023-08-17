using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class RollerAgent : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private float forceMultiplier = 10;

    [SerializeField] private ArrowGenerator arrow;
    [SerializeField] private ArrowGenerator arrowAlter;
    
    private Rigidbody _rigidbody;
    private RayPerceptionSensorComponent3D _ray;

    private void Start()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _ray = GetComponentInChildren<RayPerceptionSensorComponent3D>();
    }

    private void Update()
    {
        transform.position = _rigidbody.transform.position;
        _rigidbody.transform.localPosition = Vector3.zero;

        var normalVel = Mathf.InverseLerp(0, 3.6f, _rigidbody.velocity.magnitude);
        arrow.stemLength = Mathf.Lerp(0.3f, 2.3f, normalVel);

        var rot = Quaternion.LookRotation(new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z));
        arrow.transform.localRotation = Quaternion.Euler(90, rot.eulerAngles.y - 90, 0);
    }

    public override void OnEpisodeBegin()
    {
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0.5f, 5f);

        Vector3 desireTargetPosition;
        do
        {
            desireTargetPosition = new Vector3(Random.value * 1.56f + 3.65f, 0.5f, Random.value * 7.59f - 3.08f);
            // desireTargetPosition = new Vector3(Random.value * 12 - 6, 0.5f, Random.value * 12 - 6);
        } while (Physics.OverlapBox(desireTargetPosition, Vector3.one * 0.5f, Quaternion.identity,
                     LayerMask.GetMask("Obstacle")).Length > 0);

        target.localPosition = desireTargetPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_rigidbody.velocity.x);
        sensor.AddObservation(_rigidbody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var signal = new Vector3(actionBuffers.ContinuousActions[0], 0, actionBuffers.ContinuousActions[1]);
        _rigidbody.AddForce(signal * forceMultiplier);

        var rot = Quaternion.LookRotation(signal);
        arrowAlter.stemLength = signal.magnitude;
        arrowAlter.transform.localRotation = Quaternion.Euler(90, rot.eulerAngles.y - 90, 0);
        
        if (transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        SetReward(-2f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            SetReward(5.0f);
            EndEpisode();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            SetReward(-10f / MaxStep);
        } 
        else if (other.gameObject.CompareTag("Target"))
        {
            SetReward(5.0f);
            EndEpisode();
        }
    }
}