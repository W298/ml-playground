using UnityEngine;

public class PredatorPreyEnv : MonoBehaviour
{
    [SerializeField] private WalkAgentPredator predator;
    [SerializeField] private WalkAgentPrey prey;
    
    public int maxEnvStep = 1000;
    private int _resetTimer;
    
    private TMPro.TMP_Text _text;

    private void Start()
    {
        _text = FindObjectOfType<TMPro.TMP_Text>();
        ResetEnv();
    }

    private void FixedUpdate()
    {
        _resetTimer += 1;
        if (_resetTimer >= maxEnvStep && maxEnvStep > 0)
        {
            predator.EndEpisode();
            prey.EndEpisode();
            
            ResetEnv();
        }
        
        _text.text = (maxEnvStep - _resetTimer).ToString();
    }

    private void ResetEnv()
    {
        _resetTimer = 0;
        
        predator.ResetAgent();
        prey.ResetAgent();
    }
    
    public void PredatorCatchPrey()
    {
        predator.AddReward(1f);
        prey.AddReward(-1f);
        
        predator.EndEpisode();
        prey.EndEpisode();
        
        ResetEnv();
    }
}
