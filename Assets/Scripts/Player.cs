﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    public GameController gameController;

    bool eat = false;

    bool dead = false;

    public GameObject bodyPrefab;

    public static float moveDistance = 0.279f; // specific to the size of the snake blocks
   
    private enum Directions
    {
        right,
        down,
        left,
        up,
        stop
    } Directions direction = Directions.stop;

    bool righting = false;
    bool downing = false;
    bool lefting = false;
    bool uping = false;

    public static List<Transform> tail = new List<Transform>();

    public float speed = 0.3f;

    //private float[] validXLocs = {2.511f, 2.232f, 1.953f, 1.674f, 1.395f, 1.116f, 0.837f, 0.558f, 0.279f, 0,
    //                                -2.511f, -2.232f, -1.953f, -1.674f, -1.395f, -1.116f, -0.837f, -0.558f, -0.279f};
    //private float[] validYLocs = {3.431f, 3.152f, 2.873f, 2.315f, 2.036f, 1.757f, 1.478f, 1.199f, 0.92f, 0.641f, 0.362f, 0.083f,
    //                                -0.196f, -0.475f, -0.754f, -1.033f, -1.312f, -1.591f, -1.87f, -2.149f, -2.428f, -2.707f, -2.986f,
    //                                -3.265f, -3.544f, -3.823f, -4.102f, -4.381f, -4.66f};
    private float[] validXLocs = new float[19];
    private float[] validYLocs = new float[30];

    private int xCounter = 9;
    private int yCounter = 15;

    void Start()
    {

        //Debug.Log("Res: " + Screen.currentResolution + " DPI: " + Screen.dpi);
        //Renderer rend = gameObject.GetComponent<Renderer>();
        //Debug.Log(rend.bounds.max);
        //Debug.Log(rend.bounds.min);

        float minX = -2.511f;
        validXLocs[0] = minX;
        for (int i = 1; i < validXLocs.Length; i++)
        {
            minX += moveDistance;
            validXLocs[i] = minX;
        }

        float minY = -4.66f;
        validYLocs[0] = minY;
        for (int i = 1; i < validYLocs.Length; i++)
        {
            minY += moveDistance;
            validYLocs[i] = minY;
        }

        transform.position = new Vector3(validXLocs[xCounter], validYLocs[yCounter]);

        InvokeRepeating("Move", 1, speed);   
    }

    void Update()
    {
        if (!dead)
        {
            if (Input.GetKey(KeyCode.RightArrow) && !lefting)
            {
                direction = Directions.right;
            }
            else if (Input.GetKey(KeyCode.DownArrow) && !uping)
            { 
                direction = Directions.down;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !righting)
            {
                direction = Directions.left;
            }
            else if (Input.GetKey(KeyCode.UpArrow) && !downing)
            {
                direction = Directions.up;
            }
            

        }
        else // if (dead)
        {
            gameController.ResetScore();

            foreach (Transform t in tail)
            {
                Destroy(t.gameObject);
            }
            tail.Clear();

            Destroy(GameObject.FindGameObjectWithTag("Food"));

            direction = Directions.stop;
            xCounter = 9;
            yCounter = 15;
            transform.position = new Vector3(validXLocs[xCounter], validYLocs[yCounter]);

            setDirBools();
            dead = false;
        }
        
    }

    private void setDirBools()
    {
        if (direction == Directions.right)
        {
            righting = true;
            downing = false;
            lefting = false;
            uping = false;
        }
        else if (direction == Directions.down)
        {
            righting = false;
            downing = true;
            lefting = false;
            uping = false;
        }
        else if (direction == Directions.left)
        {
            righting = false;
            downing = false;
            lefting = true;
            uping = false;
        }
        else if (direction == Directions.up)
        {
            righting = false;
            downing = false;
            lefting = false;
            uping = true;
        }
        else if (dead)
        {
            righting = false;
            downing = false;
            lefting = false;
            uping = false;
        }
    }

    private void Move()
    {
        if (!dead)
        {
            // Save current position
            Vector2 pos = new Vector2(validXLocs[xCounter], validYLocs[yCounter]);

            setDirBools();

            if (direction == Directions.right)
            {
                xCounter++;
            }
            else if (direction == Directions.down)
            {
                yCounter--;
            }
            else if (direction == Directions.left)
            {
                xCounter--;
            }
            else if (direction == Directions.up)
            {
                yCounter++;
            }

            if (xCounter >= 0 && xCounter <= 18 && yCounter >= 0 && yCounter <= 29)
            {
                transform.position = new Vector3(validXLocs[xCounter], validYLocs[yCounter]);
            }
            else
            {
                dead = true;
            }


            if (eat)
            {
                GameObject bodyPiece = Instantiate(bodyPrefab, pos, Quaternion.identity);

                tail.Insert(0, bodyPiece.transform);

                eat = false;

                gameController.AddScore();
            }
            else if (tail.Count > 0)
            {
                // Move last tail element to where the head was ...
                // basically rotating the last body piece to behind the head *after* the head moves forward
                // so the rest stay in place and snake maintains shape
                tail.Last().position = pos;

                // Add to front of the list, remove from back
                tail.Insert(0, tail.Last());
                tail.RemoveAt(tail.Count - 1);

            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        //if (col.name.StartsWith("Food"))
        if (col.tag == "Food")
        {
            eat = true;

            Destroy(col.gameObject);
        }
        else if (col.tag == "Body")
        {

            dead = true;
        }
    }

}
