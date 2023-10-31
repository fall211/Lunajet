using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class FirstPersonController : MonoBehaviour {
	
	// public vars
	public float mouseSensitivity = 1f;
	public float smoothRotation = 12f;
	public float walkSpeed = 6f;
	public float flightForce = 1f;
	private float flightInputStr = 0f;
	public float maxFlightSpeed = 100f;

	private float timeSinceLastAction = 0f;
	private float timeToSelfCorrectRotation = 2f;
	public LayerMask groundedMask;
	
	// System vars
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	float pitch;
	float yaw;
	Transform cameraTransform;
	Rigidbody rb;
	private Transform planet;
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
		pitch = Mathf.Clamp(pitch,-60,60);

		cameraTransform.localEulerAngles = Vector3.left * pitch;
		if (!rotaterb){
			transform.Rotate(Vector3.up * yaw, Space.Self);
		} else{
			rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * yaw));
		}
		
		if (planet == null){
			JetMovement();
			return;
		}
		// Calculate movement:
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");
		
		Vector3 moveDir = new Vector3(inputX,0, inputY).normalized;
		Vector3 targetMoveAmount = transform.TransformDirection(moveDir) * walkSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount,targetMoveAmount,ref smoothMoveVelocity,.15f);
		
		if (Input.GetKey(KeyCode.Space)){
            rb.AddForce((transform.position-ClosestPlanetPos()).normalized * flightForce);
		}

	}

	private void JetMovement(){
		if (Input.GetKey(KeyCode.Space)){
			flightInputStr += Time.deltaTime * 4f;
			timeSinceLastAction = 0f;
		} else{
			flightInputStr -= Time.deltaTime * 4f;
			timeSinceLastAction += Time.deltaTime;
		}
		rb.AddForce(cameraTransform.forward * flightInputStr * flightForce);
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxFlightSpeed);

		if (timeSinceLastAction > timeToSelfCorrectRotation){
			Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * rb.rotation;
			rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime);
		}


	}

	private Vector3 ClosestPlanetPos(){
		Vector3 closestPlanetPos = Vector3.zero;
		float closestPlanetDist = Mathf.Infinity;
		foreach (Planet planet in FindObjectsOfType<Planet>()){
			float dist = Vector3.Distance(transform.position, planet.transform.position);
			if (dist < closestPlanetDist){
				closestPlanetDist = dist;
				closestPlanetPos = planet.transform.position;
			}
		}
		return closestPlanetPos;
	}
	
	void FixedUpdate() {
		if (planet != null){

			Vector3 forceDir = (planet.position - transform.position).normalized;
			Vector3 acceleration = forceDir * 9.81f;
			rb.AddForce(acceleration, ForceMode.Acceleration);

			Vector3 gravityUp = -acceleration.normalized;

			Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * rb.rotation;
			rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * smoothRotation);

			// rb.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * rb.rotation;
		}


		rb.MovePosition(rb.position + moveAmount * Time.fixedDeltaTime);
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Planet"){
			other.gameObject.GetComponentInParent<Planet>().EnterAtmosphere();
			planet = other.gameObject.transform.parent;

			rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.zero, ref smoothMoveVelocity, .2f);
		}
	}
	void OnTriggerExit(Collider other){
		if (other.tag == "Planet"){
			other.gameObject.GetComponentInParent<Planet>().ExitAtmosphere();
			planet = null;
		}
	}
}