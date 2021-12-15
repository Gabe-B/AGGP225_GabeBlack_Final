using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class Player : MonoBehaviour
{
	public GameObject playerObj;
	public Rigidbody rb;
	public Transform playerCam;
	public List<GameObject> spawnPoints = new List<GameObject>();
	public Renderer playerRenderer;
	public TMP_Text nameText;
	public GameObject pauseMenu;
	public float vertSensitivity = 100f;
	public float horizSensitivity = 100f;
	public float moveSpeed = 25.0f;
	public float playerGrav = -9.81f;
	public float minClamp = -89.9f;
	public float maxClamp = 89.9f;
	public float groundCheckDist = 1.0f;
	public float groundCheckDeadZone = 0.05f;
	public float stepHeight = 0.5f;
	public float jumpForce = 25f;
	public float falloutHeight = -5.0f;
	public bool isSpectator = false;
	public bool canMove = true;

	public int points = 0;

	public TMP_Text place;

	public ExitGames.Client.Photon.Hashtable _Placement = new ExitGames.Client.Photon.Hashtable();

	[SerializeField]
	bool groundCheck;

	[SerializeField]
	bool isChatting = false;

	float R, G, B;
	bool canJump = false;
	float cameraPitch = 0f;
	bool forwards, backwards, left, right = false;
	Vector2 leftStick, rightStick = Vector2.zero;

	void Awake()
	{
		if (gameObject.GetPhotonView().IsMine)
		{
			R = PhotonManager.instance.RED;
			G = PhotonManager.instance.GREEN;
			B = PhotonManager.instance.BLUE;
			gameObject.GetPhotonView().RPC("ChangeColor", RpcTarget.AllBuffered, R, G, B);
			gameObject.GetPhotonView().RPC("UsernameRPC", RpcTarget.AllBuffered);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		if (gameObject.GetPhotonView().IsMine)
		{
			rb = gameObject.GetComponent<Rigidbody>();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			isSpectator = false;
		}
	}

	// Update is called once per frame
	void Update()
	{
		rb.useGravity = false;
		leftStick = Vector2.zero;
		rightStick = Vector2.zero;
		canJump = false;

		if (gameObject.GetPhotonView().IsMine)
		{
			if (!isSpectator)
			{
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					pauseMenu.SetActive(true);
					canMove = false;

					rb.velocity = Vector3.zero;
				}

				
                if (Input.GetKeyDown(KeyCode.T))
                {
                    GameManager.instance.input.interactable = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    canMove = false;
                    isChatting = true;
                }
				
                if (Input.GetKeyDown(KeyCode.Return) && isChatting)
                {
                    gameObject.GetPhotonView().RPC("ChatRPC", RpcTarget.AllBuffered, PhotonNetwork.NickName, GameManager.instance.input.text);
                    Debug.Log(GameManager.instance.input.text);
					GameManager.instance.input.text = "";
					GameManager.instance.input.interactable = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;	
                    canMove = true;
                    isChatting = false;
                }

				if (!pauseMenu.activeSelf && canMove)
				{
					CheckForGround();

					if (!groundCheck)
					{
						gameObject.GetPhotonView().RPC("GravRPC", RpcTarget.AllBuffered);
					}
					else
					{
						canJump = true;
					}

					if (Input.GetKeyDown(KeyCode.Space) && canJump)
					{
						Jump();
					}

					GetInput();
					MoveStrafe(leftStick);
					RotateRight(rightStick.x);
					CameraPitch(rightStick.y);

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
				}
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				canMove = false;

				if (Input.GetKeyDown(KeyCode.Escape))
				{
					pauseMenu.SetActive(true);
				}


				if (Input.GetKeyDown(KeyCode.T))
				{
					GameManager.instance.input.interactable = true;
					isChatting = true;
				}

				if (Input.GetKeyDown(KeyCode.Return) && isChatting)
				{
					gameObject.GetPhotonView().RPC("ChatRPC", RpcTarget.AllBuffered, PhotonNetwork.NickName, GameManager.instance.input.text);
					Debug.Log(GameManager.instance.input.text);
					GameManager.instance.input.text = "";
					GameManager.instance.input.interactable = false;
					isChatting = false;
				}
			}

			if (gameObject.transform.position.y <= falloutHeight)
			{
				gameObject.GetPhotonView().RPC("FallRPC", RpcTarget.AllBuffered);
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
		gameObject.GetPhotonView().RPC("JumpRPC", RpcTarget.AllBuffered);
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

	[PunRPC]
	void ChangeColor(float r, float g, float b)
	{
		Color32 newCol = new Color32((byte)r, (byte)g, (byte)b, (byte)255);
		playerRenderer.material.color = newCol;
	}

	[PunRPC]
	void UsernameRPC()
	{
		nameText.text = gameObject.GetPhotonView().Owner.NickName;
	}

	[PunRPC]
	void ChatRPC(string _username, string _chat)
	{
		string message = _username + ": " + _chat + "\n";

		Debug.Log(message);

		ChatHandler.instance.SendMessageToChat(message);
	}

	[PunRPC]
	void FallRPC()
	{
		gameObject.transform.position = new Vector3(0f, 1.5f, 0f);
	}

	[PunRPC]
	void JumpRPC()
	{
		if (gameObject.GetPhotonView().IsMine)
		{
			rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
		}
	}

	[PunRPC]
	void GravRPC()
	{
		if(gameObject.GetPhotonView().IsMine)
		{
			rb.useGravity = true;
			Physics.gravity = new Vector3(0f, playerGrav, 0f);
		}
	}
}
