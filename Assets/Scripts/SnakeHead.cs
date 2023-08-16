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
    private string SnakeDirection;

    public delegate void VoidFunc();
    public event VoidFunc EatTail, EatFood;
    void Start()
    {
        EatTail += () => { Debug.Log("Eat tail"); };
        EatTail += () => { 
            StopCoroutine(Move());
            //tail.Clear();
            
        };
        EatFood += () => { Debug.Log("Eat food"); };
        SnakeDirection = "Up";
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && SnakeDirection != "Down")
        {
            direction = Vector3.up;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            SnakeDirection = "Up";
        }
        else if (Input.GetKeyDown(KeyCode.S) && SnakeDirection != "Up") 
        {
            direction = Vector3.down;
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            SnakeDirection = "Down";
        }
        else if (Input.GetKeyDown(KeyCode.A) && SnakeDirection != "Right")
        {
            direction = Vector3.left;
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            SnakeDirection = "Left";
        }
        else if (Input.GetKeyDown(KeyCode.D) && SnakeDirection != "Left") 
        {
            direction = Vector3.right;
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            SnakeDirection = "Right";
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

        tailNode.prev = tail[0];
        notActiveTail.Add(tailNode);
	}

    IEnumerator Move()
	{
        while(!gameManager.endGame)
		{
            yield return new WaitForSeconds(gameManager.speed);

            Vector3 t = tail[0].transform.position;
            
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

                if (transform.position + direction * step == n.transform.position)
				{
                    EatTail?.Invoke();
                    StopCoroutine(Move());
                }
			}

            if (newTail != null)
            {
                tail.Insert(0, newTail);
                t = newTail.transform.position;
            }

            transform.position += direction * step;

            int x, y;
            x = (int)(t.x / 0.5f + gameManager.width / 2);
            y = (int)(t.y / 0.5f + gameManager.height / 2);
            if (x >= 0 && x < gameManager.width && y >= 0 && y < gameManager.height)
            {
                gameManager.area[y, x] = true;
            }
            x = (int)(transform.position.x / 0.5f + gameManager.width / 2);
            y = (int)(transform.position.y / 0.5f + gameManager.height / 2);
            if (x >= 0 && x < gameManager.width && y >= 0 && y < gameManager.height)
            {
                gameManager.area[y, x] = false;
            }
		}
	}
}
