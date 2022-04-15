using System;
using System.Collections;
using System.Collections.Generic;
using Core.Character;
using Core.CoreEnums;
using Core.Enums;
using Core.Events;
using Core.Interfaces;
using RiptideNetworking;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private CharacterEntity _characterEntity;
	[SerializeField] private CharacterCombat _characterCombat;
	[SerializeField, SerializeReference] private Player _player;
	[SerializeField] private Transform _cameraProxy;
	[SerializeField] private Rigidbody _rb;

	[SerializeField] private float _lerpValue = 0.01f;
	[SerializeField] private float _snapTolerance = 5f;

	[SerializeField] private bool _toReadInput = true;

	public Action<Message> OnMovementDataChange;
	private Message _movementData;
	private bool[] _inputs;
	private bool _attackReleased = true;

	//private float MoveSpeed => _characterEntity.Stats[BasedStat.Speed];
	//private float SprintSpeed => MoveSpeed*2;
	//private float JumpSpeed => Mathf.Sqrt(MoveSpeed/2 * -2f * Physics.gravity.magnitude);

	private Vector3 _moveDir;
	public Vector3 MoveDir { get => _moveDir; set => _moveDir = value; }


	private void Start()
	{
		_inputs = new bool[7];
	}

	private void OnValidate()
	{
		if (_rb == null)
		{
			_rb = GetComponent<Rigidbody>();
		}

		if (_player == null)
		{
			_player = GetComponent<Player>();
		}

		if (_characterEntity == null)
		{
			_characterEntity = GetComponentInParent<CharacterEntity>();
		}
	}

	private void FixedUpdate()
	{
		if (_toReadInput)
		{
			ReadInput();
		}
	}

	public void SetInput(bool[] inputs, Vector3 forward)
	{
		inputs.CopyTo(_inputs, 0);
		_cameraProxy.forward = forward;
	}

	private void ReadInput()
	{
		Vector2 inpDir = Vector2.zero;
		if (_inputs[0])
			inpDir.y += 1;

		if (_inputs[1])
			inpDir.y -= 1;

		if (_inputs[2])
			inpDir.x -= 1;

		if (_inputs[3])
			inpDir.x += 1;

		Move(inpDir, _inputs[4], _inputs[5], _inputs[6]);
	}

	
	private void Move(Vector2 inpDir, bool jump, bool sprint, bool attack)
	{
		Vector3 dir = Vector3.Normalize(_cameraProxy.right * inpDir.x + _cameraProxy.forward * inpDir.y);
		float realMS = sprint ? _characterEntity.Stats[BasedStat.Speed] * 2 : _characterEntity.Stats[BasedStat.Speed];
		dir *= realMS;

		dir.y = _rb.velocity.y;
		if (jump && Grounded())
		{
			dir.y += Mathf.Sqrt(_characterEntity.Stats[BasedStat.Speed] / 2 * -2f * Physics.gravity.y);
		}

		_rb.velocity = dir;
		_moveDir = new Vector3(dir.x, 0, dir.z);
		if (!attack)
		{
			_attackReleased = true;
		}
		else if (_attackReleased)
		{
			_attackReleased = false;
			EventTrigger.I[_characterEntity, ActionType.OnAttackStarted].Invoke(new EmptyEventArgs());
		}

		RecordMovementData();
	}

	private bool Grounded()
	{
		float colliderYsize = GetComponent<Collider>().bounds.extents.y;
		return Physics.Raycast(transform.position, Vector3.down, colliderYsize + 0.1f);
	}


	private void RecordMovementData()
	{
		Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.playerMovement);
		message.AddUShort(_player.Id);
		message.AddUShort(Ticker.CurrentTick);
		message.AddVector3(transform.position);
		message.AddVector3(_moveDir);
		_movementData = message;
		OnMovementDataChange?.Invoke(_movementData);
	}
}