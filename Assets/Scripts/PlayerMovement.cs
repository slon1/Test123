using UnityEngine;
using UnityEngine.EventSystems;

namespace Tes123 {
	public class PlayerMovement : MonoBehaviour {
		private Rigidbody playerRigidbody;
		public float playerMovingSpeed = 10f;
		public float strafeSpeed = 5f;
		private float moving;
		private float strafe;
		public GameObject cam;
		private float xRot = 0.0f;
		private float yRot = 0.0f;
		public float horizontalSensitivity = 2.0f;
		public float verticalSensitivity = 2.0f;
		public float rotationLimit;		
		public FixedJoystick Joystick;
		public float TouchDistance = 2;
		void Start() {
			playerRigidbody = GetComponent<Rigidbody>();
		}
		void ParseMouse() {		

			if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
				RayCast();
			}
			if (Input.GetMouseButton(1) && Joystick.Vertical == 0 && Joystick.Horizontal == 0) {
				RotateCam();
			}
		}

		void ParseTouches() {			

			if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject()) {
				RayCast();
			}
			if (Input.touchCount == 2 && (Input.touches[0].phase == TouchPhase.Moved
					|| Input.touches[1].phase == TouchPhase.Moved) && Joystick.Vertical == 0 && Joystick.Horizontal == 0) {
				RotateCam();
			}

		}

		void RayCast() {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit, TouchDistance)) {
				hit.transform.GetComponentInParent<IInteractable>()?.Interact();
			}
		}

		void RotateCam() {
			yRot = Input.GetAxis("Mouse X") * horizontalSensitivity;
			xRot += Input.GetAxis("Mouse Y") * verticalSensitivity;
			xRot = Mathf.Clamp(xRot, -rotationLimit, rotationLimit);
			cam.transform.localEulerAngles = new Vector3(-xRot, 0, 0);
			transform.Rotate(0, yRot, 0);
		}

		void Update() {
#if UNITY_EDITOR
			ParseMouse();
#else
			ParseTouches();
#endif			
		}

		void FixedUpdate() {
			moving = Joystick.Vertical * -playerMovingSpeed;
			strafe = Joystick.Horizontal * -strafeSpeed;
			playerRigidbody.AddRelativeForce(-strafe, 0, -moving);
			//playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, 2 * runSpeed);
		}
	}
}