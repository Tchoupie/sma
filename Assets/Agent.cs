using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    float widthWarehouse = 10; //largeur de la warehouse
    float heightWarehouse = 10; // hauteur de la warehouse
    public Vector3 nextPos; //prochaine position vers laquelle l'agent veut se déplacer
    public List<Vector3> possiblePos; //ensemble des positions vers lesquels l'agent peut se déplacer

    public Sprite carrySprite; //sprite d'un agent qui porte un package
    Sprite normalSprite; //sprite d'un agent sans package
    SpriteRenderer spriteRenderer;

    public Package packageInHands; //Package que notre agent est actuellement en train de porter (peut être null)
    public Vector3 positionTarget; // position où se trouve la cible actuelle de l'agent 

    // Start is called before the first frame update
    void Start()
    {
        nextPos = transform.position; //On commence par prétendre que l'agent restera sur place
        possiblePos = new List<Vector3>(); //On prétend aussi qu'il ne pourra effectuer aucun déplacement

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite; //on lui affecte le sprite normal

        packageInHands = null; //Il n'a aucun package au début
        positionTarget = transform.position; // N'a pas encore de cible

    }

    // Update is called once per frame
    void Update()
    {
    }

    /*
     * setWitdhHeightWareHouse :
     * Permet d'indiquer à l'agent les limites qu'il ne doit pas franchir
     * @params :
     * int w : La largeur de la warehouse
     * int h : la hauteur de la warehouse
     * @return :
     * void
     */
    public void setWitdhHeightWareHouse(int w, int h)
    {
        this.widthWarehouse = w;
        this.heightWarehouse = h;
    }

    /*
     * computeMoveRandom :
     * Permet de choisir un mouvement aléatoire à effectuer par l'agent
     * parmis ceux qui sont possibles.
     * @params :
     * void
     * @return :
     * void
     */
    public void computeMoveRandom()
    {
        //On ne peut pas avoir plus de 4 mouvements différents
        switch (Random.Range(0, possiblePos.Count)) //On tire un nombre aléatoire parmis tous les 
                                                    //mouvement possibles
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

    /*
     * computeMoveToTarget :
     * Permet de choisir le mouvement qui rapproche le plus l'agent de son objectif
     * parmis ceux qui sont possibles.
     * @params :
     * void
     * @return :
     * void
     */
    public void computeMoveToTarget()
    {
      if (possiblePos.Count>0) {//Si l'agent peut bouger
        if (possiblePos.Count == 1) { //Si il n'a pas le choix
          nextPos = possiblePos[0];
        }
        else{//Si il doit choisir parmis les possibilités
          Vector3 bestMove = possiblePos[0];
          for (int i = 1; i<possiblePos.Count; i++) {//On regarde la case qui minimise la distance avec l'objectif
            if (Vector3.Distance(positionTarget,bestMove)>Vector3.Distance(positionTarget,possiblePos[i])) {
              bestMove = possiblePos[i];
            }
          }
          nextPos = bestMove;
        }
      }
    }
    /*
     * move :
     * Permet de déplacer l'agent vers l'endroit qu'il vise
     * @params :
     * void
     * @return :
     * void
     */
    public void move()
    {
        transform.position = nextPos;
    }

    /*
     * changeForCarrySprite :
     * Permet de changer l'apparence de l'agent pour afficher qu'il PORTE un package
     * @params :
     * void
     * @return :
     * void
     */
    public void changeForCarrySprite()
    {
        spriteRenderer.sprite = carrySprite;
    }

    /*
     * changeForCarrySprite :
     * Permet de changer l'apparence de l'agent pour afficher qu'il NE PORTE PAS de package
     * @params :
     * void
     * @return :
     * void
     */
    public void changeForNormalSprite()
    {
        spriteRenderer.sprite = normalSprite;
    }
}
