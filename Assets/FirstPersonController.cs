using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class FirstPersonController : MonoBehaviour {
	
	// public vars
	public float mouseSensitivity = 1f;
	public float smoothRotation = 12f;
	public float walkSpeed = 6f;
	public float flightForce = 200f;
	private float flightInputStr = 0f;
	public float gravity = 10f;

	private float timeSinceLastAction = 0f;
	private readonly float timeToSelfCorrectRotation = 2f;
	bool inOrbit = false;
	
	// System vars
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	float pitch;
	float yaw;
	Transform cameraTransform;
	Rigidbody rb;
	[SerializeField] private Transform planet;
	public bool rotaterb = false;
	
	
	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		cameraTransform = Camera.main.transform;
		rb = GetComponent<Rigidbody> ();
        rb.useGravity = false;
		rb.constraints = RigidbodyConstraints.FreezeRotation;


	}
	
	void Update() {


		// Look rotation:
		yaw = Input.GetAxis("Mouse X") * mouseSensitivity;
		pitch += Input.GetAxis("Mouse Y") * mouseSensitivity;
		pitch = Mathf.Clamp(pitch,-80,80);

		cameraTransform.localEulerAngles = Vector3.left * pitch;
		rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * yaw));
		
		JetMovement();
		// Calculate movement:
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");
		
		Vector3 moveDir = new Vector3(inputX,0, inputY).normalized;
		Vector3 targetMoveAmount = transform.TransformDirection(moveDir) * walkSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount,targetMoveAmount,ref smoothMoveVelocity,.15f);
		


	}

	private void JetMovement(){
		if (Input.GetKey(KeyCode.Space)){
			flightInputStr += Time.deltaTime * 4f;
			timeSinceLastAction = 0f;
		} else{
			flightInputStr -= Time.deltaTime * 4f;
			timeSinceLastAction += Time.deltaTime;
		}
		flightInputStr = Mathf.Clamp(flightInputStr, 0f, 20f);

		if (timeSinceLastAction > timeToSelfCorrectRotation){
			if (inOrbit) return;
			Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * rb.rotation;
			rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime);
		}


	}

	
	void FixedUpdate() {
		if (planet != null){

			Vector3 forceDir = (planet.position - transform.position).normalized;
			Vector3 acceleration = forceDir * gravity * Time.fixedDeltaTime;
			rb.AddForce(acceleration, ForceMode.Acceleration);

			Vector3 gravityUp = -acceleration.normalized;

			Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * rb.rotation;
			rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * smoothRotation);

		}
		if (Input.GetKey(KeyCode.Space)){
			rb.AddForce(flightForce * flightInputStr * Time.fixedDeltaTime * cameraTransform.forward, ForceMode.Acceleration);
		}		
		rb.MovePosition(rb.position + moveAmount * Time.fixedDeltaTime);



	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Atmosphere"){
			other.gameObject.GetComponentInParent<Planet>().EnterAtmosphere();
			planet = other.gameObject.transform.parent;

			rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.zero, ref smoothMoveVelocity, .2f);
			inOrbit = true;
		}
	}
	void OnTriggerExit(Collider other){
		if (other.tag == "Atmosphere"){
			other.gameObject.GetComponentInParent<Planet>().ExitAtmosphere();
			planet = null;
			inOrbit = false;
		}
	}
}