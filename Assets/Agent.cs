using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    float time = 0;
    float dt = 0.002f;
    float widthWarehouse = 10;
    float heightWarehouse = 10;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        move();
        time += dt;
        if(time >= 1.0f)
        {
            time = 0f;
        }
    }

    public void setDeltaTime(float dt)
    {
        this.dt = dt;
    }

    public void setWitdhHeightWareHouse(int w, int h)
    {
        this.widthWarehouse = w;
        this.heightWarehouse = h;
    }

    void move()
    {
        if(time == 0)
        {   

            int x = 0;
            int y = 0;

            if(Random.value<0.5f)
            {
                if(Random.value<0.5f)
                {
                    x = -1;
                }
                else
                {
                    x = 1;
                }
            }
            else
            {
                if(Random.value<0.5f)
                {
                    y = -1;
                }
                else
                {
                    y = 1;
                } 
            }

            if(x==0) //On se deplace en y
            {
                //print(heightWarehouse/2);
                if(transform.position.y + y > heightWarehouse/2)
                {
                    y = -1;
                }

                if(transform.position.y - y < -heightWarehouse/2)
                {
                    y = 1;
                }

            }
            else //On se deplace en x
            {   
                //On verifie les bords
                if(transform.position.x + x > widthWarehouse/2)
                {
                    x = -1;
                }

                if(transform.position.x - x < -widthWarehouse/2)
                {
                    x = 1;
                }
            }
            transform.position += new Vector3(x,y,0);
        }
    }
}
