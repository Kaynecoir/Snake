using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : SnakeNode
{
    public UGameManager gameManager;
    public Vector3 direction;
    public float step;
    public List<SnakeNode> tail = new List<SnakeNode>();
    public List<SnakeNode> notActiveTail = new List<SnakeNode>();
    public GameObject TailPrefab;

    public delegate void VoidFunc();
    public event VoidFunc EatTail, EatFood;
    void Start()
    {
        StartCoroutine(Move());
        EatTail += () => { Debug.Log("Eat tail"); };
        EatFood += () => { Debug.Log("Eat food"); };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Vector3.up;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (Input.GetKeyDown(KeyCode.S)) 
        {
            direction = Vector3.down;
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = Vector3.left;
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (Input.GetKeyDown(KeyCode.D)) 
        {
            direction = Vector3.right;
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        }
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.tag == "Food")
        {
            EatFood?.Invoke();
            Destroy(collision.gameObject);
            AddTail();
        }
    }
	void AddTail()
	{
        GameObject go = Instantiate(TailPrefab, transform.position, Quaternion.identity);
        go.SetActive(false);
        SnakeNode tailNode = go.GetComponent<SnakeNode>();
        Debug.Log(tail.Count);

        tailNode.prev = tail[0];
        notActiveTail.Add(tailNode);
	}

    IEnumerator Move()
	{
        while(true)
		{
            SnakeNode newTail = null;
            if (notActiveTail.Count > 0 && notActiveTail[0].transform.position == tail[0].transform.position)
			{
                newTail = notActiveTail[0];
                notActiveTail.Remove(newTail);
                newTail.gameObject.SetActive(true);
			}
            foreach(SnakeNode n in tail)
			{
                n.transform.position = n.prev.transform.position;
                if(transform.position + direction * step == n.transform.position)
				{
                    EatTail?.Invoke();
                    StopCoroutine(Move());
                }
			}

            if (newTail != null) tail.Insert(0, newTail);
            transform.position += direction * step;
            yield return new WaitForSeconds(gameManager.speed);
		}
	}
}
