using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SnakeController : MonoBehaviour
{
    public Vector2 Direction = Vector2.right;
    private Vector2 PreviousDirection = Vector2.right;
    public bool shouldGrowSnake = false;
    public int Player = 0;
    private string HorizontalVector { get => "Horizontal_" + Player; }
    private string VerticalVector { get => "Vertical_" + Player; }
    public List<GameObject> SnakeParts = new List<GameObject>();
    public bool ignorePortal = false;
    public int Score = 0;

    public void doMove()
    {

        PreviousDirection = Direction;

        // this.ignorePortal = false;

    }
    public void FixedUpdate()
    {
        // change the direction of the snake based on Horizontal and vertical controls per player.
        // if the objetive direction is the same as the last direction, do nothing.
        // if the objective direction is oposite to the last direction, do nothing.
        // else change the direction to the objective direction.
        if (Input.GetAxis(HorizontalVector) > 0 && PreviousDirection != Vector2.left)
        {
            Direction = Vector2.right;
        }
        else if (Input.GetAxis(HorizontalVector) < 0 && PreviousDirection != Vector2.right)
        {
            Direction = Vector2.left;
        }
        else if (Input.GetAxis(VerticalVector) > 0 && PreviousDirection != Vector2.down)
        {
            Direction = Vector2.up;
        }
        else if (Input.GetAxis(VerticalVector) < 0 && PreviousDirection != Vector2.up)
        {
            Direction = Vector2.down;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Portal")
        {
            this.ignorePortal = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Snake collided with " + collision.gameObject.name);
        if (collision.gameObject.tag == "Collectionable")
        {
            GameObject.Find("GameController").GetComponent<GameController>().EatFood(this.Player, collision.gameObject);
        }
        else if (collision.gameObject.tag == "Portal")
        {
            // if ignore portal is true, do nothing. else, get the portal and go to oposite portal, eg: PortalLeft -> PortalRight
            if (this.ignorePortal)
            {
                return;
            }
            else
            {
                this.ignorePortal = true;
                if (collision.gameObject.name == "PortalLeft")
                {
                    var portal = GameObject.Find("PortalRight");
                    this.transform.position = new Vector3(Mathf.Round(portal.transform.position.x), this.transform.position.y, this.transform.position.z);
                }
                else if (collision.gameObject.name == "PortalRight")
                {
                    var portal = GameObject.Find("PortalLeft");
                    this.transform.position = new Vector3(Mathf.Round(portal.transform.position.x), this.transform.position.y, this.transform.position.z);
                }
                else if (collision.gameObject.name == "PortalUp")
                {
                    var portal = GameObject.Find("PortalDown");
                    this.transform.position = new Vector3(this.transform.position.x, Mathf.Round(portal.transform.position.y), this.transform.position.z);
                }
                else if (collision.gameObject.name == "PortalDown")
                {
                    var portal = GameObject.Find("PortalUp");
                    this.transform.position = new Vector3(this.transform.position.x, Mathf.Round(portal.transform.position.y), this.transform.position.z);
                }
                this.transform.position += (Vector3)PreviousDirection;
            }
        }
        else if (collision.gameObject.tag == "Wall")
        {
            GameObject.Find("GameController").GetComponent<GameController>().Die(this.Player);
        }
        else if (collision.gameObject.name == "SnakePart")
        {
            GameObject.Find("GameController").GetComponent<GameController>().Die(this.Player);
        }
        else if (collision.gameObject.name == "SnakeHead")
        {
            GameObject.Find("GameController").GetComponent<GameController>().Die(-1); //Its a draw
        }
        // debug code.
    }
}
