using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGameManager : MonoBehaviour
{
    public SnakeHead snake;
    public GameObject FoodPrefab;
    public delegate void VoidFunc();
    public event VoidFunc Lose;
    public float speed = 1.0f;
    public int height, width;
    public bool[,] area;
    public bool endGame;
    void Start()
    {
        snake.EatFood += () => { speed *= 0.95f; };
        snake.EatTail += () => { Debug.Log("StopAllCoroutines"); StopAllCoroutines(); };
        snake.EatTail += () => { Debug.Log("StopCoroutine"); StopCoroutine(CreatFood()); };
        snake.EatTail += () => { endGame = true; };
        area = new bool[height, width];
        for(int i = 0; i < height; i++)
		{
            for(int j = 0; j < width; j++)
			{
                area[i, j] = true;
			}
		}
        StartCoroutine(CreatFood());
    }
	private void OnDrawGizmos()
	{
        
	}
	private void OnDrawGizmosSelected()
	{
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Gizmos.color = Color.white;
                if (area != null) Gizmos.color = area[i, j] ? Color.white : Color.red;
                Gizmos.DrawCube(new Vector3(-width / 4 + 0.5f * j - 0.5f, -height / 4 + 0.5f * i - 0.5f), Vector3.one * 0.4f);
            }
        }
    }

	// Update is called once per frame
	void Update()
    {
        if(snake.transform.position.y > height / 2 * 0.5)
		{
            snake.transform.position = new Vector3(snake.transform.position.x, -(snake.transform.position.y - 0.5f));
        }
        if (snake.transform.position.y < -height / 2 * 0.5)
        {
            snake.transform.position = new Vector3(snake.transform.position.x, -(snake.transform.position.y + 0.5f));
        }
        if (snake.transform.position.x > width / 2 * 0.5)
        {
            snake.transform.position = new Vector3(-(snake.transform.position.x - 0.5f), snake.transform.position.y);
        }
        if (snake.transform.position.x < -width / 2 * 0.5)
        {
            snake.transform.position = new Vector3(-(snake.transform.position.x + 0.5f), snake.transform.position.y);
        }
    }

    

    IEnumerator CreatFood()
	{
        while(!endGame)
		{
            int x = (int)Random.Range(-width / 2, width / 2), y = (int)Random.Range(-height / 2, height / 2);
            Debug.Log(x + " " + y);
            if(area[y + height / 2, x + width / 2])
			{
                Instantiate(FoodPrefab, new Vector3(x, y) * 0.5f, Quaternion.identity);
                area[y + height / 2, x + width / 2] = false;
			}
            yield return new WaitForSeconds(5);
		}
	}
}
