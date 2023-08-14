using UnityEngine;

public class HorizontalAreaReplicator : MonoBehaviour
{
    [SerializeField] private GameObject baseArea;
    [SerializeField] private int rowCount;
    [SerializeField] private int colCount;
    [SerializeField] private int separation;

    private void OnEnable()
    {
        for (var i = 0; i < rowCount; i++)
        {
            for (var j = 0; j < colCount; j++)
            {
                if (i == 0 && j == 0) continue;
                var replicatedArea = Instantiate(baseArea, new Vector3(separation * i, 0, separation * j), Quaternion.identity);
                replicatedArea.name = baseArea.name;
            }
        }
    }
}
