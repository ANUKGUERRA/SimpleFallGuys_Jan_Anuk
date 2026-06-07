using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private int points = 0;
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
           Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("finishLine"))
        {
            return;
        }
        else if( other.gameObject.tag == "deathZone")
        {
            this.gameObject.SetActive(false);
        }
    }
}
