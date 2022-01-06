using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    float time = 0;
    float dt = 0.002f;
    float widthWarehouse = 10;
    float heightWarehouse = 10;
    public Vector3 nextPos;
    public List<Vector3> possiblePos;

    public Sprite carrySprite;
    Sprite normalSprite;
    SpriteRenderer spriteRenderer;

    public Package packageInHands;

    // Start is called before the first frame update
    void Start()
    {
        possiblePos = new List<Vector3>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;

        packageInHands = null;

    }

    // Update is called once per frame
    void Update()
    {
        time += dt;

        if(time >= 1.0f)
        {
            time = 0f;
        }

        if(time == 0)
        {
            computeMove();
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

    public void computeMove()
    {
        switch (Random.Range(0, possiblePos.Count))
        {
            case 0:
                if (possiblePos.Count == 0){ nextPos = transform.position; }
                else { nextPos = possiblePos[0]; }
                break;
            case 1:
                nextPos = possiblePos[1];
                break;
            case 2:
                nextPos = possiblePos[2];
                break;
            case 3:
                nextPos = possiblePos[3];
                break;
            default:
                nextPos = possiblePos[0];
                break;
        }
    }

    public void move()
    {
        transform.position = nextPos;
    }

    void ChangeForCarrySprite()
    {
        spriteRenderer.sprite = carrySprite; 
    }

    void ChangeForNormalSprite()
    {
        spriteRenderer.sprite = normalSprite;
    }
}
