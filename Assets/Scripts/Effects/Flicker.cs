using UnityEngine;

public class Flicker : MonoBehaviour
{
    public float
        MaxRange = 30.0f,
        MinRange = 10.0f,
        FlickerModifier = 1.0f;

    private Light flame;

    void Start()
    {
        flame = GetComponent<Light>();
    }

    void Update()
    {
        doFlicker();
    }

    private void doFlicker()
    {
        float rangeIncrement = Random.value * FlickerModifier;
        if (Random.value > 0.5f) // 50% chance.
        {
            if (flame.range >= MaxRange)
            {
                flame.range -= rangeIncrement;
            }
            else
            {
                flame.range += rangeIncrement;
            }
        }
        else
        {
            if (flame.range <= MinRange)
            {
                flame.range += rangeIncrement;
            }
            else
            {
                flame.range -= rangeIncrement;
            }
        }
    }
}
