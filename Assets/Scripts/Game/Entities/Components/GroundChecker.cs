using Sirenix.OdinInspector;
using UnityEngine;

namespace MHL.Game.Entities.Components
{
	public class GroundChecker : MonoBehaviour
	{
		[Space(10)]
		[Title("Ground Checker", TitleAlignment = TitleAlignments.Centered)]
		[SerializeField] private LayerMask groundLayer;
		[SerializeField] private Transform groundCheckTransform;
		[SerializeField] private Vector2 groundCheckSize = new Vector2(0.1f, 0.1f);
		[SerializeField] private float groundCheckDistance = 0.1f;
		[Space(10)]
		[SerializeField] private bool drawGizmos = true;
		
		public bool IsGrounded { get; private set; }

		private void Update()
		{
			IsGrounded = Physics2D.BoxCast(groundCheckTransform.position, groundCheckSize, 0f, Vector2.down, groundCheckDistance, groundLayer);
		}
		
		private void OnDrawGizmos()
		{
			if (!drawGizmos) return;
			
			Gizmos.color = IsGrounded ? Color.green : Color.red;
			Gizmos.DrawWireCube(groundCheckTransform.position, groundCheckSize);
		}
	}
}