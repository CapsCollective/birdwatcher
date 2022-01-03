using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _player;

    private void Awake()
    {
        _player = Camera.main.transform;
    }

    private void LateUpdate()
    { 
        transform.LookAt(_player.position, Vector3.up);
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);   
    }
}
