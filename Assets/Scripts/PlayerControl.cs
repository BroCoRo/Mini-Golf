using UnityEngine;
using TMPro;

//Some of the functionality for shooting the ball was adapted from the following tutorial: https://youtu.be/LjLBHSU_yic

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float shotPower;
    [SerializeField] private float stopVelocity = 5f; //Any velocity below this value will be considered stopped
    [SerializeField] private LineRenderer lineRenderer;
    public GameObject LooseTextObject;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI shotText;
    public GameObject WinTextObject;
    public AudioSource audioPlayer;
    private bool isAiming;
    private bool isIdle;
    private int count;
    private int shots;
    private int score;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        shots = 0;
        SetCountText();
        SetShotsText();
        SetScoreText();
        WinTextObject.SetActive(false);
        LooseTextObject.SetActive(false);
    }

    //This is for starting up the rigid body (aim line)
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        isAiming = false;
        lineRenderer.enabled = false;
    }

    //This will allow the next update to begin
    private void FixedUpdate()
    {
        if (rb.velocity.magnitude < stopVelocity)
        {
            Stop();
        }

        ProcessAim();
    }

    //This is when the user has clicked on the gold ball and is aiming
    private void OnMouseDown()
    {
        if (isIdle)
        {
            isAiming = true;
        }
    }

    //This is the aiming function that will process the aim that the user is inputting 
    private void ProcessAim()
    {
        if (!isAiming || !isIdle)
        {
            return;
        }

        Vector3? worldPoint = CastMouseClickRay();

        if (!worldPoint.HasValue)
        {
            return;
        }

        DrawLine(worldPoint.Value);

        if (Input.GetMouseButtonUp(0))
        {
            Shoot(worldPoint.Value);
        }
    }

    //This is the shoot function that will use the inputted aim information to determine the shot
    private void Shoot(Vector3 worldPoint)
    {
        isAiming = false;
        lineRenderer.enabled = false;

        Vector3 horizontalWorldPoint = new Vector3(worldPoint.x, transform.position.y, worldPoint.z);

        Vector3 direction = (horizontalWorldPoint - transform.position).normalized;
        float strength = Vector3.Distance(transform.position, horizontalWorldPoint);

        rb.AddForce(direction * strength * shotPower);
        isIdle = false;
        shots++;
        SetShotsText();
        SetScoreText();
        audioPlayer.Play();
    }

    //This is the draw line funtion that will display the shoot line off of the ball for the user
    private void DrawLine(Vector3 worldPoint)
    {
        Vector3[] positions = {
            transform.position,
            worldPoint};
        lineRenderer.SetPositions(positions);
        lineRenderer.enabled = true;
    }

    //This is the stop function that occurs when the ball has stopped and this is the function that will start the idle state for the ball
    private void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isIdle = true;
    }

    //This function is used in conjuction with determining the shot of the ball from the user, this funtion helps calcualte things like the distance the line has been dragged for the shot power
    private Vector3? CastMouseClickRay()
    {
        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        if (Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, float.PositiveInfinity))
        {
            return hit.point;
        }
        else
        {
            return null;
        }
    }


    //This funtion handels all of the triggers in the game, loosing, winning, picking up and object
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
            SetScoreText();
        }
        else if (other.gameObject.CompareTag("Loose"))
        {
            LooseTextObject.SetActive(true);
        }
        else if (other.gameObject.CompareTag("Finish"))
        {
            WinTextObject.SetActive(true);
        }
    }

    //This function updates the count of the objects picked up so far in the game
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }

    //This function updates the count of the shots taken so far in the game
    void SetShotsText()
    {
        shotText.text = "Shots: " + shots.ToString();
    }

    //This function updates the current score of the game, this funciton also calcuates the score using the shots taken and the count of objects picked up to calculate the score
    void SetScoreText()
    {
        //Player gets 10x points for each sqaure collected and for every shot taken the score goes down by 1 shot (the higher the points in the game the better!)
        score = (count * 10) - shots;
        scoreText.text = "Score: " + score.ToString();
    }
}
