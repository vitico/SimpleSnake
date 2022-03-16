using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // snake prefab
    public GameObject SnakePrefab;
    // food prefab
    public GameObject FoodPrefab;
    // snake controllers
    public List<SnakeController> SnakeControllers = new List<SnakeController>();

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
        // create the snake head at a random position
        Vector2 position = new Vector2(Mathf.Round(Random.Range(0, 10)), Mathf.Round(Random.Range(0, 10)));
        GameObject SnakeHead = Instantiate(SnakePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
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
        Debug.Log("Snake(" + player + ") died");

        // destroy all the snake parts (except the head) of all the snakes
        for (int i = 0; i < SnakeControllers.Count; i++)
        {
            for (int j = 1; j < SnakeControllers[i].SnakeParts.Count; j++)
            {
                Destroy(SnakeControllers[i].SnakeParts[j]);
            }
            Vector2 position = new Vector2(Mathf.Round(Random.Range(0, 10)), Mathf.Round(Random.Range(0, 10)));
            SnakeControllers[i].transform.position = new Vector3(position.x, position.y, 0);
            SnakeControllers[i].SnakeParts = new List<GameObject>();
            SnakeControllers[i].SnakeParts.Add(SnakeControllers[i].gameObject);
        }


        // destroy all collectionable objects
        GameObject[] collectionables = GameObject.FindGameObjectsWithTag("Collectionable");
        for (int i = 0; i < collectionables.Length; i++)
        {
            Destroy(collectionables[i]);
        }

        // create a new food
        CreateFood();

        // give the other player a point (if player 0 dies, player 1 gets a point, if player 1 dies, player 0 gets a point)
        if (player < 0)
        {
            // its a draw
            SnakeControllers[0].Score++;
            SnakeControllers[1].Score++;


        }
        else if (player == 0)
        {
            SnakeControllers[1].Score++;
        }
        else
        {
            SnakeControllers[0].Score++;
        }

        //show the score and tell if it was a draw
        Debug.Log("Player 0: " + SnakeControllers[0].Score + " Player 1: " + SnakeControllers[1].Score);
        this.MoveDelay = this.InitialMoveDelay;


    }

}
