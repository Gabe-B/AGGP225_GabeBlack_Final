using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Player : MonoBehaviour
{
	#region Player Vars
	public GameObject playerObj;
    public Rigidbody rb;
    public Transform playerCam;
    public float vertSensitivity = 100f;
    public float horizSensitivity = 100f;
    public float moveSpeed = 5.0f;
    public float playerGrav = -9.81f;
    public float minClamp = -89.9f;
    public float maxClamp = 89.9f;
    public float groundCheckDist = 1.0f;
    public float groundCheckDeadZone = 0.05f;
    public float stepHeight = 0.5f;
    public float jumpForce = 25f;

    bool groundCheck;
    float cameraPitch = 0f;
    bool forwards, backwards, left, right = false;
    Vector2 leftStick, rightStick = Vector2.zero;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetPhotonView().IsMine)
        {
            rb = gameObject.GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.useGravity = false;
        leftStick = Vector2.zero;
        rightStick = Vector2.zero;

        if (gameObject.GetPhotonView().IsMine)
        {
            /*
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pauseMenu.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                FPSGameManager.instance.input.interactable = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                canMove = false;
                isChatting = true;
            }

            if (Input.GetKeyDown(KeyCode.Return) && isChatting)
            {
                gameObject.GetPhotonView().RPC("ChatRPC", RpcTarget.AllBuffered, playerNameField.text, FPSGameManager.instance.input.text);
                Debug.Log(FPSGameManager.instance.input.text);
                FPSGameManager.instance.input.text = "";
                FPSGameManager.instance.input.interactable = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                canMove = true;
                isChatting = false;
            }*/

            //if (!pauseMenu.activeSelf && canMove)
            //{
            GetInput();
                MoveStrafe(leftStick);
                RotateRight(rightStick.x);
                CameraPitch(rightStick.y);

            if(Input.GetKeyDown(KeyCode.Space))
			{
                Jump();
			}

                /*if (forwards)
                {
                    playerAnim.SetBool("IsMoving", true);
                }
                else if (backwards)
                {
                    playerAnim.SetBool("IsMoving", true);
                }
                else if (left)
                {
                    playerAnim.SetBool("IsMoving", true);
                }
                else if (right)
                {
                    playerAnim.SetBool("IsMoving", true);
                }
                else
                {

                    playerAnim.SetBool("IsMoving", false);
                }*/
            //}

            CheckForGround();

            if (!groundCheck)
            {
                rb.useGravity = true;
                Physics.gravity = new Vector3(0f, playerGrav, 0f);
            }
        }
    }

    #region Movement Methods
    void CameraPitch(float value)
    {
        if (gameObject.GetPhotonView().IsMine)
        {
            cameraPitch -= (value * vertSensitivity);
            cameraPitch = Mathf.Clamp(cameraPitch, minClamp, maxClamp);
            playerCam.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    void RotateRight(float value)
    {
        if (gameObject.GetPhotonView().IsMine)
        {
            gameObject.transform.Rotate(Vector3.up * value * Time.deltaTime * horizSensitivity);
        }
    }

    void GetInput()
    {
        if (gameObject.GetPhotonView().IsMine)
        {
            leftStick = Vector2.zero;
            rightStick = Vector2.zero;
            forwards = Input.GetKey(KeyCode.W);
            backwards = Input.GetKey(KeyCode.S);
            right = Input.GetKey(KeyCode.D);
            left = Input.GetKey(KeyCode.A);

            rightStick.x = Input.GetAxis("Mouse X");
            rightStick.y = Input.GetAxis("Mouse Y");
            KeyToAxis();
        }
    }

    void KeyToAxis()
    {
        if (forwards)
        {
            leftStick.y = 1;
        }
        if (backwards)
        {
            leftStick.y = -1;
        }
        if (right)
        {
            leftStick.x = 1;
        }
        if (left)
        {
            leftStick.x = -1;
        }

    }

    void Jump()
	{
        if(gameObject.GetPhotonView().IsMine)
		{
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
		}
	}

    void MoveStrafe(Vector2 value)
    {
        if (gameObject.GetPhotonView().IsMine)
        {
            MoveStrafe(value.x, value.y);
        }
    }

    void MoveStrafe(float horizontal, float vertical)
    {
        rb.velocity = Vector3.zero;
        rb.velocity += (gameObject.transform.forward * vertical * moveSpeed);
        rb.velocity += (gameObject.transform.right * horizontal * moveSpeed);
    }

    void CheckForGround()
    {
        RaycastHit hit;
        bool result = Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, groundCheckDist);
        Debug.DrawRay(gameObject.transform.position, Vector3.down, new Color(165, 23, 57), groundCheckDist);
        groundCheck = result;

        if (result)
        {
            if (hit.distance < groundCheckDeadZone)
            {
                return;
            }
            if (hit.distance < stepHeight)
            {
                gameObject.transform.position = hit.point;
            }
        }
    }
    #endregion
}
