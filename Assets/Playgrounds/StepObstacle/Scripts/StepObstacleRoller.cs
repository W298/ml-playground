using TMPro;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class StepObstacleRoller : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private float forceMultiplier = 10;

    [SerializeField] private ArrowGenerator arrow;
    [SerializeField] private ArrowGenerator arrowAlter;
    
    private Rigidbody _rigidbody;
    private RayPerceptionSensorComponent3D _ray;
    
    private Canvas _canvas;
    private TMP_Text _currentRewardText;

    private bool passed01, passed02, passed03 = false;

    private void Start()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _ray = GetComponentInChildren<RayPerceptionSensorComponent3D>();
        _canvas = FindObjectOfType<Canvas>();

        _currentRewardText = _canvas.transform.GetChild(0).GetComponent<TMP_Text>();
    }

    private void Update()
    {
        transform.position = _rigidbody.transform.position;
        _rigidbody.transform.localPosition = Vector3.zero;

        if (!passed03 && transform.position.z < -2.96f)
        {
            AddReward(4f);
            passed03 = true;
        }
        else if (!passed02 && transform.position.z < -0.04f)
        {
            AddReward(4f);
            passed02 = true;
        }
        else if (!passed01 && transform.position.z < 3.04f)
        {
            AddReward(4f);
            passed01 = true;
        }

        var normalVel = Mathf.InverseLerp(0, 3.6f, _rigidbody.velocity.magnitude);
        arrow.stemLength = Mathf.Lerp(0, 2.3f, normalVel);

        if (_rigidbody.velocity.magnitude > 0.0001f)
        {
            var rot = Quaternion.LookRotation(new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z));
            arrow.transform.localRotation = Quaternion.Euler(90, rot.eulerAngles.y - 90, 0);
        }

        _currentRewardText.text = GetCumulativeReward().ToString("F");
    }

    public override void OnEpisodeBegin()
    {
        SetReward(0f);
        
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 0.3f, 7f);

        target.localPosition = new Vector3(Random.value * 6 - 3f, 0.3f, -7f);

        passed01 = false;
        passed02 = false;
        passed03 = false;
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

        if (signal.magnitude > 0.0001f)
        {
            var rot = Quaternion.LookRotation(signal);
            arrowAlter.stemLength = signal.magnitude * 0.8f;
            arrowAlter.transform.localRotation = Quaternion.Euler(90, rot.eulerAngles.y - 90, 0);
        }

        AddReward(-1f / MaxStep);
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
            AddReward(20.0f);
            EndEpisode();
        }
        else if (other.gameObject.CompareTag("LaserCollider"))
        {
            EndEpisode();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            AddReward(20.0f);
            EndEpisode();
        }
        else if (other.gameObject.CompareTag("LaserCollider"))
        {
            EndEpisode();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-10f / MaxStep);
        }
    }
}