using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    BaseState currentState;
    public StateMachine _stateMachine;
    public int score;
    [SerializeField] Text scoreText;
    Animator animator;
    Vector3 startGamePosition;
    Quaternion startGameRotation;
    float laneOffset;
    public float laneChangeSpeed = 15;
    Rigidbody rb;
    float pointStart;
    float pointFinish;
    bool isMoving = false;
    Coroutine movingCoroutine;
    float lastVectorX;
    bool isJumping = false;
    public float jumpPower = 15;
    public float jumpGravity = -49;
    public float realGravity = -9.8f;
    public List<BaseState> states;

    void Start()
    {
        laneOffset = MapGenerator.Instance.laneOffset;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        startGamePosition = transform.position;
        startGameRotation = transform.rotation;
        //SwipeManager.Instance.MoveEvent += MovePlayer;
        _stateMachine = new StateMachine();
        _stateMachine.Initialize(new JumpState(this));
    }

    // Update is called once per frame
    void Update()
    {

        scoreText.text = score.ToString();
        if (Input.GetKeyDown(KeyCode.A) && pointFinish > -laneOffset)
        {
            _stateMachine.ChangeState(new LeftRoadState(this));
        }
        if (Input.GetKeyDown(KeyCode.D) && pointFinish < laneOffset)
        {
            _stateMachine.ChangeState(new RightRoadState(this));
        }
        if (Input.GetKeyDown(KeyCode.W) && isJumping == false)
        {
            _stateMachine.ChangeState(new JumpState(this));
        }

        //scoreText.text = score.ToString();
        //if (Input.GetKeyDown(KeyCode.A) && pointFinish > -laneOffset)
        //{
        //    MoveHorizontal(-laneChangeSpeed);
        //}

        //if (Input.GetKeyDown(KeyCode.D) && pointFinish < laneOffset)
        //{
        //    MoveHorizontal(laneChangeSpeed);
        //}
        //if(Input.GetKeyDown(KeyCode.W) && isJumping == false)
        //{
        //    Jump();
        //}
    }

    public void Jump()
    {
        isJumping = true;
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, jumpGravity, 0);
        StartCoroutine(StopJumpCoroutine());
    }

    IEnumerator StopJumpCoroutine()
    {
        do
        {
            yield return new WaitForSeconds(0.02f);

        }
        while (rb.velocity.y != 0);
        isJumping = false;
        Physics.gravity = new Vector3(0, realGravity, 0);
    }

    public void MoveHorizontal(float speed)
    {
        animator.applyRootMotion = false;
        pointStart = pointFinish;
        pointFinish += Mathf.Sign(speed) * laneOffset;

        if(isMoving) { StopCoroutine(movingCoroutine); isMoving = false; }
        movingCoroutine = StartCoroutine(MoveCoroutine(speed));
    }

    IEnumerator MoveCoroutine(float vectorX)
    {
        isMoving = true;
        while(Mathf.Abs(pointStart - transform.position.x)< laneOffset)
        {
            yield return new WaitForFixedUpdate();
            rb.velocity = new Vector3(vectorX, rb.velocity.y, 0);
            lastVectorX = vectorX;
            float x = Mathf.Clamp(transform.position.x, Mathf.Min(pointStart, pointFinish), Mathf.Max(pointStart, pointFinish));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(pointFinish, transform.position.y, transform.position.z);
        if(transform.position.y > 0.9)
        {
            rb.velocity = new Vector3(rb.velocity.x, -10, rb.velocity.z);
        }
        isMoving = false;
    }

    public void StartGame()
    {
        animator.SetTrigger("Turn Head Trigger");
    }

    public void StartLevel()
    {
        animator.applyRootMotion = false;
        RoadGenerator.Instance.StartLevel();
    }

    public void ResetGame()
    {
        rb.velocity = Vector3.zero;
        pointStart = 0;
        pointFinish = 0;
        animator.applyRootMotion = true;
        animator.SetTrigger("Idle");
        transform.position = startGamePosition;
        transform.rotation = startGameRotation;
        RoadGenerator.Instance.ResetLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Coin")
        {
            score++;
        }
        if(other.gameObject.tag == "Ramp")
        {
            rb.constraints |= RigidbodyConstraints.FreezePositionZ;
        }
        if(other.gameObject.tag == "Lose")
        {
            ResetGame();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ramp")
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        if(collision.gameObject.tag == "NotLose")
        {
            MoveHorizontal(-lastVectorX);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "RampPlane")
        {
            if(rb.velocity.x == 0 && isJumping == false)
            {
                rb.velocity = new Vector3(rb.velocity.x, -10, rb.velocity.z);
            }
        }
    }
}
