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
        if(dt != prevdt) //Pour la modification du deltatime (vitesse de simulation)
        {
            foreach(Agent a in agents)
            {
                a.setDeltaTime(dt);
            }
        }
        prevdt = dt;
    }
}
