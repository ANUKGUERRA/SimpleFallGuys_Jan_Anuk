using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "powerUp")
        {
            speed += 2.0f;
        }
    }
}
