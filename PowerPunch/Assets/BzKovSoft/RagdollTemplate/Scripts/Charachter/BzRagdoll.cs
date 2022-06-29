using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(IBzRagdollCharacter))]
	public sealed class BzRagdoll : MonoBehaviour, IBzRagdoll
	{
		// animator parameters:
		//readonly int _animatorForward = Animator.StringToHash("Forward");
		//readonly int _animatorTurn = Animator.StringToHash("Turn");

		// animator animations:
		[SerializeField]
		string _animationGetUpFromBelly = "GetUp.GetUpFromBelly";
		[SerializeField]
		string _animationGetUpFromBack = "GetUp.GetUpFromBack";

		[SerializeField] private int id;

		Animator _anim;
		IBzRagdollCharacter _bzRagdollCharacter;

		// parameters for control character moving while it is ragdolled
		private const float AirSpeed = 5f; // determines the max speed of the character while airborne

		// parameters needed to control ragdolling
		RagdollState _state = RagdollState.Animated;
		float _ragdollingEndTime;   //A helper variable to store the time when we transitioned from ragdolled to blendToAnim state
		const float RagdollToMecanimBlendTime = 0.5f;   //How long do we blend when transitioning from ragdolled to animated
		readonly List<RigidComponent> _rigids = new List<RigidComponent>();
		readonly List<TransformComponent> _transforms = new List<TransformComponent>();
		Transform _hipsTransform;
		Rigidbody _hipsTransformRigid;
		Vector3 _storedHipsPosition;
		Vector3 _storedHipsPositionPrivAnim;
		Vector3 _storedHipsPositionPrivBlend;
		public bool onRG;
		public bool onGetUp;

		#region implementation of IBzRagdoll

		public bool Raycast(Ray ray, out RaycastHit hit, float distance)
		{
			var hits = Physics.RaycastAll(ray, distance);
			
			for (int i = 0; i < hits.Length; ++i)
			{
				var h = hits[i];
				if (h.transform != transform && h.transform.root == transform.root)
				{
					hit = h;
					return true;
				}
			}

			// if no hits found, return false
			hit = new RaycastHit();
			return false;
		}

		public bool IsRagdolled
		{
			get
			{
				return
					_state == RagdollState.Ragdolled ||
					_state == RagdollState.WaitStablePosition;
			}
			set
			{
				if (value)
					RagdollIn();
				else
					RagdollOut();
			}
		}

		public void AddExtraMove(Vector3 move)
		{
			if (IsRagdolled)
			{
				Vector3 airMove = new Vector3(move.x * AirSpeed, 0f, move.z * AirSpeed);
				foreach (var rigid in _rigids)
					rigid.RigidBody.AddForce(airMove / 100f, ForceMode.VelocityChange);
			}
		}
		#endregion

		void Start()
		{
			_anim = GetComponent<Animator>();
			_hipsTransform = _anim.GetBoneTransform(HumanBodyBones.Hips);
			//_hipsTransformRigid = _hipsTransform.GetComponent<Rigidbody>();
			_bzRagdollCharacter = GetComponent<IBzRagdollCharacter>();

			
			Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();

			foreach (Rigidbody rigid in rigidBodies)
			{
				if (rigid.transform == transform)
					continue;

				RigidComponent rigidCompontnt = new RigidComponent(rigid);
				_rigids.Add(rigidCompontnt);
			}
			
			ActivateRagdollParts(false);

			foreach (var t in GetComponentsInChildren<Transform>())
			{
				var trComp = new TransformComponent(t);
				_transforms.Add(trComp);
			}
		}

		void FixedUpdate()
		{
			if (_state == RagdollState.WaitStablePosition) //&&
				//_hipsTransformRigid.velocity.magnitude < 0.1f)
			{
				GetUp();
			}

			if (onRG)
			{
				RagdollIn();
			}
			else if (onGetUp)
			{
				GetUp();
			}
			
			/*			if (_state == RagdollState.Animated &&
			                _bzRagdollCharacter.CharacterVelocity.y < -10f)
			{
				RagdollIn();
				RagdollOut();
			}*/
		}

		void LateUpdate()
		{
			if (_state != RagdollState.BlendToAnim)
				return;

			float ragdollBlendAmount = 1f - Mathf.InverseLerp(
				_ragdollingEndTime,
				_ragdollingEndTime + RagdollToMecanimBlendTime,
				Time.time);
			

			if (_storedHipsPositionPrivBlend != _hipsTransform.position)
			{
				_storedHipsPositionPrivAnim = _hipsTransform.position;
			}
			_storedHipsPositionPrivBlend = Vector3.Lerp(_storedHipsPositionPrivAnim, _storedHipsPosition, ragdollBlendAmount);
			_hipsTransform.position = _storedHipsPositionPrivBlend;

			foreach (TransformComponent trComp in _transforms)
			{
				if (trComp.PrivRotation != trComp.Transform.localRotation)
				{
					trComp.PrivRotation = Quaternion.Slerp(trComp.Transform.localRotation, trComp.StoredRotation, ragdollBlendAmount);
					trComp.Transform.localRotation = trComp.PrivRotation;
				}

				if (trComp.PrivPosition != trComp.Transform.localPosition)
				{
					trComp.PrivPosition = Vector3.Slerp(trComp.Transform.localPosition, trComp.StoredPosition, ragdollBlendAmount);
					trComp.Transform.localPosition = trComp.PrivPosition;
				}
			}
			
			if (Mathf.Abs(ragdollBlendAmount) < Mathf.Epsilon)
			{
				_state = RagdollState.Animated;
			}
		}

		public void RagdollIn()
		{
			ActivateRagdollParts(true);
			_anim.enabled = false;
			_state = RagdollState.Ragdolled;
			ApplyVelocity(_bzRagdollCharacter.CharacterVelocity);
		}

		public void RagdollOut()
		{
			if (_state == RagdollState.Ragdolled)
				_state = RagdollState.WaitStablePosition;
		}

		public void GetUp()
		{
			_ragdollingEndTime = Time.time;
			_anim.enabled = true;
			_state = RagdollState.BlendToAnim;
			_storedHipsPositionPrivAnim = Vector3.zero;
			_storedHipsPositionPrivBlend = Vector3.zero;

			_storedHipsPosition = _hipsTransform.position;
			
			Vector3 shiftPos = _hipsTransform.position - transform.position;

			MoveNodeWithoutChildren(shiftPos);
			
			foreach (TransformComponent trComp in _transforms)
			{
				trComp.StoredRotation = trComp.Transform.localRotation;
				trComp.PrivRotation = trComp.Transform.localRotation;

				trComp.StoredPosition = trComp.Transform.localPosition;
				trComp.PrivPosition = trComp.Transform.localPosition;
			}
			
			string getUpAnim = CheckIfLieOnBack() ? _animationGetUpFromBack : _animationGetUpFromBelly;
			_anim.Play(getUpAnim, 0, 0);
			ActivateRagdollParts(false);
		}

		private void MoveNodeWithoutChildren(Vector3 shiftPos)
		{
			Vector3 ragdollDirection = GetRagdollDirection();

			_hipsTransform.position -= shiftPos;
			transform.position += shiftPos;

			Vector3 forward = transform.forward;
			transform.rotation = Quaternion.FromToRotation(forward, ragdollDirection) * transform.rotation;
			_hipsTransform.rotation = Quaternion.FromToRotation(ragdollDirection, forward) * _hipsTransform.rotation;
		}

		private bool CheckIfLieOnBack()
		{
			var left = _anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
			var right = _anim.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
			var hipsPos = _hipsTransform.position;

			left -= hipsPos;
			left.y = 0f;
			right -= hipsPos;
			right.y = 0f;

			var q = Quaternion.FromToRotation(left, Vector3.right);
			var t = q * right;

			return t.z < 0f;
		}

		private Vector3 GetRagdollDirection()
		{
			Vector3 ragdolledFeetPosition = (_anim.GetBoneTransform(HumanBodyBones.Hips).position);
			
			Vector3 ragdolledHeadPosition = _anim.GetBoneTransform(HumanBodyBones.Head).position;
			Vector3 ragdollDirection = ragdolledFeetPosition - ragdolledHeadPosition;
			ragdollDirection.y = 0;
			ragdollDirection = ragdollDirection.normalized;

			if (CheckIfLieOnBack())
				return ragdollDirection;
			else
				return -ragdollDirection;
		}
		
		private void ApplyVelocity(Vector3 predieVelocity)
		{
			foreach (var rigid in _rigids)
				rigid.RigidBody.velocity = predieVelocity;
		}

		private void ActivateRagdollParts(bool activate)
		{
			_bzRagdollCharacter.CharacterEnable(!activate);

			foreach (var rigid in _rigids)
			{
				Collider partColider = rigid.RigidBody.GetComponent<Collider>();
				
				if (partColider == null)
				{
					const string colliderNodeSufix = "_ColliderRotator";
					string childName = rigid.RigidBody.name + colliderNodeSufix;
					Transform transform = rigid.RigidBody.transform.Find(childName);
					partColider = transform.GetComponent<Collider>();
				}

				partColider.isTrigger = !activate;

				if (activate)
				{
					rigid.RigidBody.isKinematic = false;
				}
				else
					rigid.RigidBody.isKinematic = true;
			}
		}

		sealed class TransformComponent
		{
			public readonly Transform Transform;
			public Quaternion PrivRotation;
			public Quaternion StoredRotation;

			public Vector3 PrivPosition;
			public Vector3 StoredPosition;

			public TransformComponent(Transform t)
			{
				Transform = t;
			}
		}

		struct RigidComponent
		{
			public readonly Rigidbody RigidBody;
			public readonly CharacterJoint Joint;
			public readonly Vector3 ConnectedAnchorDefault;

			public RigidComponent(Rigidbody rigid)
			{
				RigidBody = rigid;
				Joint = rigid.GetComponent<CharacterJoint>();
				if (Joint != null)
					ConnectedAnchorDefault = Joint.connectedAnchor;
				else
					ConnectedAnchorDefault = Vector3.zero;
			}
		}
		
		enum RagdollState
		{
			Animated,
			WaitStablePosition,
			Ragdolled,
			BlendToAnim,
		}
	}
}
