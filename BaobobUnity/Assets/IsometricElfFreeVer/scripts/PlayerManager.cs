using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public Transform ItenPoint;
	public Transform ShotPoint;
	public GameObject ItemPrefab;
	public GameObject ThrowPrefab;
	public GameObject BowPrefab;
	private Rigidbody2D rb;
	private Animator animator;
	public float moveSpeed = 1f;

	[SerializeField]
	private Transform shotPointTransform = null;

	private void Start()
	{
		Application.targetFrameRate = 60;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float y = (x == 0) ? Input.GetAxisRaw("Vertical") : 0.0f;

		if (x != 0 || y != 0)
		{
			animator.SetFloat("x", x);
			animator.SetFloat("y", y);
			animator.SetBool("Walk", true);
		}
		else
		{
			animator.SetBool("Walk", false);
		}

		StartCoroutine(Action());
		StartCoroutine(Shot());
	}

	private IEnumerator Action()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			animator.SetTrigger("Slash");
		}

		if (Input.GetKeyDown(KeyCode.V))
		{
			animator.SetTrigger("Guard");
		}

		if (Input.GetKeyDown(KeyCode.B))
		{
			animator.SetTrigger("Item");
			Instantiate(ItemPrefab, ItenPoint.position, transform.rotation);
		}

		if (Input.GetKeyDown(KeyCode.N))
		{
			animator.SetTrigger("Damage");
		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			animator.SetTrigger("Dead");
			this.transform.position = new Vector2(0f, -0.12f);
			for (var i = 0; i < 64; i++)
			{
				yield return null;
			}
			this.transform.position = Vector2.zero;
		}
	}

	private IEnumerator Shot()//롅뢯븧딇궻멗묖궴?렑
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			animator.SetTrigger("Throw");
			for (var i = 0; i < 30; i++)
			{
				// 긓깑??깛
				yield return null;
			}
			Instantiate(ThrowPrefab, Vector2.zero, Quaternion.identity, shotPointTransform);
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			animator.SetTrigger("Bow");
			for (var i = 0; i < 40; i++)
			{
				yield return null;
			}
			Instantiate(BowPrefab, Vector2.zero, Quaternion.identity, shotPointTransform);
		}
	}
}