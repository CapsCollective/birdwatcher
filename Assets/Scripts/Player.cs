using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    #region Singleton
    public static Player instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Movement")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxSpeed;
    private float CurrSpeed
    {
        get { return currSpeed; }
        set
        {
            if (value >= maxSpeed) currSpeed = maxSpeed;
            else if (value <= 0) currSpeed = 0;
            else currSpeed = value;
        }
    }
    private float currSpeed;
    private Vector3 currDir;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private Transform camTrans;

    [Header("Jumping")]
    [SerializeField] private float jumpVelo;
    [SerializeField] private float jumpKeyInputAllowance;
    private float jumpKeyPressedTimer;
    private bool grounded;

    void Start()
    {
        // lock & hide the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        grounded = true;
    }

    void Update()
    {
        // wasd & arrows movement

        Vector3 currDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (currDir != Vector3.zero) CurrSpeed += acceleration * Time.deltaTime;
        else CurrSpeed -= decceleration;
        //currDir = (transform.forward * currDir.x) + (transform.right * currDir.z);
        currDir = transform.TransformDirection(currDir);
        currDir *= CurrSpeed;

        rb.velocity = currDir + new Vector3(0,rb.velocity.y,0);


        // mouse movement

        transform.Rotate(0, Input.GetAxis("Mouse X") * 1.5f, 0);
        camTrans.Rotate(-Input.GetAxis("Mouse Y") * 1.5f, 0, 0);


        // jumping

        if (Input.GetAxisRaw("Jump") > 0) jumpKeyPressedTimer = jumpKeyInputAllowance;
        jumpKeyPressedTimer -= Time.deltaTime;
        if (jumpKeyPressedTimer >= 0 && grounded)
        {
            jumpKeyPressedTimer = -1;
            rb.velocity += new Vector3(0, jumpVelo, 0);
            grounded = false;
        }

        // falling
        if (transform.position.y < -20 && fallen == false)
        {
            fallen = true;
            fallPink.SetActive(true);
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            StartCoroutine(WaitForRestart());
        }

    }

    private bool fallen;
    private bool hitten;
    [SerializeField] private GameObject fallPink;
    [SerializeField] private GameObject hitPink;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform")) grounded = true;
        else if (collision.gameObject.CompareTag("EnemyBullet") && hitten == false)
        {
            hitten = true;
            hitPink.SetActive(true);
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            StartCoroutine(WaitForRestart());
        }
    }


    IEnumerator WaitForRestart()
    {
        bool pressed = false;
        while (gameObject)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.R) && !pressed)
            {
                pressed = true;
                SceneManager.LoadScene(0);
            }
        }
    }


}
