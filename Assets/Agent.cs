using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    float widthWarehouse = 10;
    float heightWarehouse = 10;
    public Vector3 nextPos;
    public List<Vector3> possiblePos;

    public Sprite carrySprite;
    Sprite normalSprite;
    SpriteRenderer spriteRenderer;

    public Package packageInHands;
    public Vector3 positionTarget;

    // Start is called before the first frame update
    void Start()
    {
        nextPos = transform.position;
        possiblePos = new List<Vector3>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;

        packageInHands = null;
        positionTarget = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setWitdhHeightWareHouse(int w, int h)
    {
        this.widthWarehouse = w;
        this.heightWarehouse = h;
    }

    public void computeMoveRandom()
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
    public void computeMoveToTarget()
    {
        bool lockVerticale =false;

        float posVerticale = transform.position.y;
        float posHorizontale = transform.position.x;

        float destinatioVerticale = positionTarget.y;
        float destinatioHorizontale = positionTarget.x;

        if (posVerticale<destinatioVerticale) {
            nextPos = nextPos + Warehouse.north;
            if (!possiblePos.Contains(nextPos)) {
              nextPos = transform.position;
              lockVerticale = true;
            }
        }
        else if (posVerticale>destinatioVerticale) {
            nextPos = nextPos +  Warehouse.south;
            if (!possiblePos.Contains(nextPos)) {
              nextPos = transform.position;
              lockVerticale = true;
            }
        }
        else{
          lockVerticale = true;
        }
        if (lockVerticale) {//si on a pas choisi de mouvement en vertical on en choisis un en horizontal
          if (posHorizontale<destinatioHorizontale) {
              nextPos = nextPos +  Warehouse.east;
              if (!possiblePos.Contains(nextPos)) {
                nextPos = transform.position;
              }
          }
          else if (posHorizontale>destinatioHorizontale) {
            nextPos = nextPos +  Warehouse.west;
            if (!possiblePos.Contains(nextPos)) {
              nextPos = transform.position;
            }
          }
        }
    }

    public void move()
    {
        transform.position = nextPos;
    }

    public void changeForCarrySprite()
    {
        spriteRenderer.sprite = carrySprite;
    }

    public void changeForNormalSprite()
    {
        spriteRenderer.sprite = normalSprite;
    }
}
