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
      if (possiblePos.Count>0) {
        if (possiblePos.Count == 1) {
          nextPos = possiblePos[0];
        }
        else{
          Vector3 bestMove = possiblePos[0];
          for (int i = 1; i<possiblePos.Count; i++) {
            if (Vector3.Distance(positionTarget,bestMove)>Vector3.Distance(positionTarget,possiblePos[i])) {
              bestMove = possiblePos[i];
            }
          }
          nextPos = bestMove;
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
