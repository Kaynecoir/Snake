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
    void Start()
    {
        snake.EatFood += () => { speed *= 0.95f; };
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
        while(true)
		{
            int x = (int)Random.Range(-width / 4, width / 4), y = (int)Random.Range(-height / 4, height / 4);
            if(area[y + height / 4, x + width / 4])
			{
                Instantiate(FoodPrefab, new Vector3(x, y), Quaternion.identity);
                area[y + height / 4, x + width / 4] = false;
			}
            yield return new WaitForSeconds(5);
		}
	}
}
