using System.Collections;
using UnityEngine;

public class LaserObstacle : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private BoxCollider _laserCollider;

    public bool isOn = false;
    public float rate = 1.0f;
    public float stand = 0.5f;
    public float delay = 0.0f;
    
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _laserCollider = transform.GetChild(0).GetComponent<BoxCollider>();

        StartCoroutine(StartRoutine());
    }
    
    void Update()
    {
        if (!isOn)
        {
            _lineRenderer.enabled = false;
            _laserCollider.gameObject.SetActive(false);
            return;
        }
        else
        {
            _lineRenderer.enabled = true;
            _laserCollider.gameObject.SetActive(true);
        }
        
        var hit = Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 1000, 1 << LayerMask.NameToLayer("Obstacle"));
        
        var startPoint = transform.position - new Vector3(0.5f, 0, 0);
        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, hit ? hitInfo.point : transform.position + transform.forward * 1000f);

        var center = (startPoint + hitInfo.point) / 2;
        var zSize = Vector3.Distance(startPoint, hitInfo.point);
        
        _laserCollider.center = transform.InverseTransformPoint(center);
        _laserCollider.size = new Vector3(_laserCollider.size.x, _laserCollider.size.y, zSize);
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        isOn = !isOn;
        yield return new WaitForSeconds(stand);
        isOn = !isOn;
        yield return new WaitForSeconds(rate);
        StartCoroutine(Routine());
    } 
}
