
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{

	/// <summary>
	///		Provides basic forwards/backwards movement and left/right rotation
	///		controls for a <see cref="CharacterController"/> through Unity's 
	///		<see cref="Input"/> system
	/// </summary>
	[RequireComponent(typeof(CharacterController))]
	public class SimpleMovementController : MonoBehaviour
	{

		public float moveSpeed = 10f;
		public float rotateSpeed = 100f;
		public float gravity = 20f;
		public bool useGravity = false;

		private Animator animator;
		private CharacterController controller;

		private Vector3 moveDirection = Vector3.zero;
		private Vector3 rotateDirection = Vector3.zero;

		void Start()
		{
			controller = GetComponent<CharacterController>();
			animator = GetComponent<Animator>();
		}

		void Update()
		{
			Move();
		}

		/// <summary>
		///		Translate and rotate the <see cref="controller"/> based on vertical
		///		and horizontal movement through the <see cref="Input" /> system
		/// </summary>
		private void Move()
		{
			float vertical = Input.GetAxisRaw("Vertical");
			float horizontal = Input.GetAxisRaw("Horizontal");

			if (animator)
			{
				animator.SetFloat("SpeedY", vertical);
				animator.SetFloat("RotationX", horizontal);
			}

			moveDirection = transform.TransformDirection(Vector3.forward * vertical) * moveSpeed;
			rotateDirection = transform.TransformDirection(Vector3.up * horizontal) * rotateSpeed;

			if (useGravity) { moveDirection.y -= gravity * Time.deltaTime; }
			controller.Move(moveDirection * Time.deltaTime);
			transform.Rotate(rotateDirection * Time.deltaTime);
		}
	}
}