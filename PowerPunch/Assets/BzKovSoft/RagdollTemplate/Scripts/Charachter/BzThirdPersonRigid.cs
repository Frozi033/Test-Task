using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	//[RequireComponent(typeof(Rigidbody))]
	public sealed class BzThirdPersonRigid : BzThirdPersonBase
	{
		private CharacterController _characterController;
		private Rigidbody _rigidbody;
		private BoxCollider _boxCollider;

		protected override void Awake()
		{
			base.Awake();
			
			_characterController = GetComponent<CharacterController>();
			_rigidbody = GetComponent<Rigidbody>();
			_boxCollider = GetComponent<BoxCollider>();
		}

		public override void CharacterEnable(bool enable)
		{
			base.CharacterEnable(enable);
			
			if (_characterController != null)
			{
				_characterController.enabled = enable;
			}
			else
			{
				_boxCollider.enabled = enable;
			}
			
			if (enable)
				_firstAnimatorFrame = true;
		}

		protected override Vector3 PlayerVelocity { get { return _rigidbody.velocity; } }

		protected override void ApplyCapsuleHeight()
		{
			/*float capsuleY = _animator.GetFloat(_animatorCapsuleY);
			_capsuleCollider.height = capsuleY;
			var c = _capsuleCollider.center;
			c.y = capsuleY / 2f;
			_capsuleCollider.center = c;*/
		}

#region Ground Check

		/// <summary>
		/// every FixedUpdate _groundChecker resets to false,
		/// and if collision with ground found till next FixedUpdate,
		/// the character on a ground
		/// </summary>
		bool _groundChecker;
		float _jumpStartedTime;


		protected override bool? PlayerTouchGound()
		{
			bool gb = false;
			/*bool grounded = _groundChecker;
			_groundChecker = false;
			// if the character is on the ground and
			// half of second was passed, return true
			return grounded & (_jumpStartedTime + 0.5f < Time.time );*/
			return gb;
		}

#endregion
		protected override void UpdatePlayerPosition(Vector3 deltaPos)
		{
			Vector3 finalVelocity = deltaPos / Time.deltaTime;
			/*if (!_jumpPressed)
			{
				finalVelocity.y = _rigidbody.velocity.y;
			}
			else
			{
				_jumpStartedTime = Time.time;
			}*/
			_airVelocity = finalVelocity;		// i need this to correctly detect player velocity in air mode
			_rigidbody.velocity = finalVelocity;
		}
	}
}