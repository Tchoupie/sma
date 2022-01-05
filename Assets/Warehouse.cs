using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warehouse : MonoBehaviour
{
    public GameObject agentPrefab;
    public int nbAgents;
    public List<Agent> agents;
    public int width = 10;
    public int height = 10;
    
    public float time = 0;
    public float dt = 0.002f;
    public Slider sliderSpeed;
    float prevdt = 0f;
    // Start is called before the first frame update
    void Start()
    {
        agents = new List<Agent>();
        for(int i=0; i<nbAgents;i++)
        {
            GameObject a = Instantiate(agentPrefab,new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0),Quaternion.identity);
            a.GetComponent<Agent>().setWitdhHeightWareHouse(width,height); //On donne l'info sur la longueur et la hauteur de la warehouse
            agents.Add(a.GetComponent<Agent>());
        }
        prevdt = dt;
    }

    // Update is called once per frame
    void Update()
    {
        dt = sliderSpeed.value;

        time += dt;

        if(time >= 1.0f)
        {
            time = 0f;
        }
        
        foreach(Agent a in agents)
        {
            if(time == 0)
            {
                a.computeMove(); //On calcule les positions des agens pour t+1 (qui seront dans nextPos)
                resolvePos();
                a.move();
            }
        }
    }

    void resolvePos()
    {
        foreach(Agent a in agents)
        {
            foreach(Agent a1 in agents)
            {
                a1.computeMove();
            }
        }
    }
}
