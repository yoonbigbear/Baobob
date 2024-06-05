using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerObject : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Animator animator;
	[SerializeField] private Rigidbody2D rigidbody2D;
	[SerializeField] private Transform transform;
	[SerializeField] private Tilemap tilemap;

	private bool isMoving = false;
	private float moveSpeed = 1f;

	[SerializeField] private Vector3Int currentCell;
	[SerializeField] private Vector3 targetPosition;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
	{
		transform = GetComponent<Transform>();
		rigidbody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		animator.speed = 1f;
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void FixedUpdate()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

			if (hit.collider != null && hit.collider.gameObject == tilemap.gameObject)
			{
				Vector3 hitPopint = hit.point;

				Vector3Int tilePosition = tilemap.WorldToCell(hitPopint);
				Debug.Log("Tile clicked at: " + tilemap.CellToWorld(tilePosition));
			}
		}
		KeyboardInputAction();
	}

	// Update is called once per frame
	private void Update()
	{
	}

	private void KeyboardInputAction()
	{
		if (isMoving)
		{
			return;
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			MoveUp();
			return;
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			MoveDown();
			return;
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			MoveLeft();
			return;
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			MoveRight();
			return;
		}
	}

	private IEnumerator Moving(Vector3 dir)
	{
		Vector3Int cell = tilemap.WorldToCell(transform.position + dir);
		targetPosition = tilemap.GetCellCenterWorld(cell);
		isMoving = true;
		animator.SetBool("Walk", true);
		while (transform.position != targetPosition)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
			yield return null;
		}
		animator.SetBool("Walk", false);
		isMoving = false;
	}

	private void MoveUp()
	{
		animator.SetFloat("x", 0);
		animator.SetFloat("y", 1);
		StartCoroutine(Moving(new Vector3(0.5f, 0.25f, 0)));
	}

	private void MoveDown()
	{
		animator.SetFloat("x", 0);
		animator.SetFloat("y", 0);
		StartCoroutine(Moving(new Vector3(-0.5f, -0.25f, 0)));
	}

	private void MoveRight()
	{
		animator.SetFloat("x", 1);
		animator.SetFloat("y", 0);
		StartCoroutine(Moving(new Vector3(0.5f, -0.25f, 0)));
	}

	private void MoveLeft()
	{
		animator.SetFloat("x", -1);
		animator.SetFloat("y", 0);
		StartCoroutine(Moving(new Vector3(-0.5f, .25f, 0)));
	}
}