using System;
using Game.Input;
using Game.Maths;
using KBCore.Refs;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class PlayerController : MonoBehaviour
	{
		[Header("Input")]
		[SerializeField] private InputReader inputReader;

		[Header("Movement Settings")]
		[SerializeField]private float movementSpeed = 5f;

		[SerializeField, Self, HideInInspector] 
		private Rigidbody2D rb;
		
		private Vector2 _movementInput;

		private void Start()
		{
			Debug.Log($"Input Reader: {inputReader}");
			inputReader.EnableGameplayInput();
		}

		private void OnEnable()
		{
			inputReader.MoveEvent += OnMove;
		}

		private void OnDisable()
		{
			inputReader.MoveEvent -= OnMove;
		}

		private void FixedUpdate()
		{
			Vector2 movement = _movementInput.normalized * movementSpeed;
			Vector2 newPosition = rb.position + movement * Time.fixedDeltaTime;
			
			rb.MovePosition(newPosition);
		}

		/// <summary>
		/// Callback of <see cref="inputReader"/> when movement occurs.
		/// </summary>
		/// <param name="movement">The raw movement vector.</param>
		private void OnMove(Vector2 movement) => _movementInput = movement;

		private void OnValidate()
		{
			this.ValidateRefs();
		}
	}
}