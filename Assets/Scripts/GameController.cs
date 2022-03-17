using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // snake prefab
    public GameObject SnakePrefab;
    // food prefab
    public GameObject FoodPrefab;
    // snake controllers
    public List<SnakeController> SnakeControllers = new List<SnakeController>();

    //list of spawn points
    public List<Transform> SpawnPoints = new List<Transform>();

    // time to wait for the next move
    public float InitialMoveDelay = 0.5f;
    public float MinimumMoveDelay = 0.1f;
    public float MoveDelay = 0.1f;
    // accumulated time since last move
    private float MoveTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.MoveDelay = this.InitialMoveDelay;
        this.InitSnake();
        // create the food
        CreateFood();
    }
    private void InitSnake()
    {
        var players = PlayerPrefs.GetInt("PlayerCount", 2);
        // create the snake
        for (int i = 0; i < players; i++)
        {
            CreateSnake(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // move the snake
        MoveTimer += Time.deltaTime;
        if (MoveTimer > MoveDelay)
        {
            MoveTimer = 0.0f;
            for (int i = 0; i < SnakeControllers.Count; i++)
            {
                MoveSnake(i);
            }

            // make the game slightly faster
            MoveDelay = Mathf.Max(MoveDelay - 0.0001f, this.MinimumMoveDelay);
        }


    }

    // create the snake
    private void CreateSnake(int player)
    {
        // create the snake head at spawn point
        var SnakeHead = Instantiate(this.SnakePrefab, this.SpawnPoints[player].position, Quaternion.identity);

        SnakeHead.name = "SnakeHead";
        SnakeController SnakeController = SnakeHead.GetComponent<SnakeController>();
        SnakeController.SnakeParts = new List<GameObject>();
        SnakeController.SnakeParts.Add(SnakeHead);
        SnakeController.Player = player;
        SnakeControllers.Add(SnakeController);

    }

    // create the food
    private void CreateFood()
    {
        // create the food in a random position
        GameObject Food = Instantiate(FoodPrefab, new Vector3(Random.Range(-10, 10), Random.Range(-8, 8), 0), Quaternion.identity);
        Food.name = "Food";
        // if the food is created in the same position as the snake, destroy it and create a new one.
        for (int i = 0; i < SnakeControllers.Count; i++)
        {
            if (SnakeControllers[i].transform.position == Food.transform.position)
            {
                Destroy(Food);
                CreateFood();
                return;
            }
            // if the food is colliding with the snake, destroy it and create a new one.
            for (int j = 0; j < SnakeControllers[i].SnakeParts.Count; j++)
            {
                if (SnakeControllers[i].SnakeParts[j].transform.position == Food.transform.position)
                {
                    Destroy(Food);
                    CreateFood();
                    return;
                }
            }
        }

    }

    // move the snake
    private void MoveSnake(int player)
    {
        SnakeControllers[player].doMove();

        // if shouldGrowSnake is true, grow the snake
        if (SnakeControllers[player].shouldGrowSnake)
        {
            GrowSnake(player);
            SnakeControllers[player].shouldGrowSnake = false;
        }



        // move the snake body
        for (int i = SnakeControllers[player].SnakeParts.Count - 1; i > 0; i--)
        {
            SnakeControllers[player].SnakeParts[i].transform.position = SnakeControllers[player].SnakeParts[i - 1].transform.position;
        }

        // move the snake head
        GameObject SnakeHead = SnakeControllers[player].SnakeParts[0];
        SnakeHead.transform.position += (Vector3)SnakeControllers[player].Direction;


    }

    // grow the snake
    public void GrowSnake(int player)
    {
        var parts = SnakeControllers[player].SnakeParts;
        // create a new snake part at the position that the snake was before
        GameObject SnakePart = Instantiate(SnakePrefab, parts[parts.Count - 1].transform.position, Quaternion.identity);
        SnakePart.name = "SnakePart";
        parts.Add(SnakePart);
        // remove the SnakeController component from the new snake part
        SnakeController SnakeController = SnakePart.GetComponent<SnakeController>();
        SnakeController.enabled = false;
        Destroy(SnakeController);


    }

    // eat the food
    public void EatFood(int player, GameObject food)
    {
        // destroy the food
        Destroy(food);
        // grow the snake
        SnakeControllers[player].shouldGrowSnake = true;
        // create a new food
        CreateFood();

        Debug.Log("Snake ate the food");
    }

    public void Die(int player)
    {
        //loop throught all controllers and save the score
        for (int i = 0; i < SnakeControllers.Count; i++)
        {
            PlayerPrefs.SetInt("Score" + (i + 1).ToString(), (SnakeControllers[i].Score - 1) * 10);
        }

        SceneManager.LoadScene("EndMenu");
    }

}
