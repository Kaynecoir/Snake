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
    void Start()
    {
        snake.EatFood += () => { speed *= 0.95f; };
        StartCoroutine(CreatFood());
    }

    // Update is called once per frame
    void Update()
    {
        if(snake.transform.position.y > 5)
		{
            snake.transform.position = new Vector3(snake.transform.position.x, -(snake.transform.position.y - 0.5f));
        }
        if (snake.transform.position.y < -5)
        {
            snake.transform.position = new Vector3(snake.transform.position.x, -(snake.transform.position.y + 0.5f));
        }
        if (snake.transform.position.x > 10)
        {
            snake.transform.position = new Vector3(-(snake.transform.position.x - 0.5f), snake.transform.position.y);
        }
        if (snake.transform.position.x < -10)
        {
            snake.transform.position = new Vector3(-(snake.transform.position.x + 0.5f), snake.transform.position.y);
        }
    }

    IEnumerator CreatFood()
	{
        while(true)
		{
            Instantiate(FoodPrefab, new Vector3((int)Random.Range(-20, 20) * 0.5f, (int)Random.Range(-10, 10) * 0.5f), Quaternion.identity);
            yield return new WaitForSeconds(5 * speed);
		}
	}
}
