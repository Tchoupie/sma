using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warehouse : MonoBehaviour
{
    public GameObject agentPrefab;
    public GameObject packagePrefab;
    public GameObject destinationPrefab;
    public int nbAgents;
    public int nbPackages;
    public int nbDestinations;
    public List<Agent> agents;
    public List<Package> packages;
    public List<Destination> destinations;
    public int width = 10;
    public int height = 10;
    
    public float time = 0;
    public float dt = 0.002f;
    public Slider sliderSpeed;
    float prevdt = 0f;
    // Start is called before the first frame update

    private static Vector3Int north = new Vector3Int(0, 1, 0);
    private static Vector3Int south = new Vector3Int(0, -1, 0);
    private static Vector3Int west = new Vector3Int(-1, 0, 0);
    private static Vector3Int east = new Vector3Int(1, 0, 0);

    void Start()
    {
        agents = new List<Agent>();
        packages = new List<Package>();
        destinations = new List<Destination>();
        nbAgents = VariablesGlobales.nbAgentsGlob;
        nbPackages = VariablesGlobales.nbPaquetsGlob;
        for(int i=0; i<nbAgents;i++)
        {
            GameObject a = Instantiate(agentPrefab,new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0),Quaternion.identity);
            a.GetComponent<Agent>().setWitdhHeightWareHouse(width,height); //On donne l'info sur la longueur et la hauteur de la warehouse
            agents.Add(a.GetComponent<Agent>());
        }

        for(int i=0; i < nbPackages; i++)
        {
            GameObject p = Instantiate(packagePrefab, new Vector3(Random.Range((int)-width / 2, (int)(width / 2) + 1), Random.Range((int)-height / 2, (int)(height / 2) + 1), 0), Quaternion.identity);
            packages.Add(p.GetComponent<Package>());
        }

        for (int i = 0; i < nbDestinations; i++)
        {
            GameObject p = Instantiate(destinationPrefab, new Vector3(Random.Range((int)-width / 2, (int)(width / 2) + 1), Random.Range((int)-height / 2, (int)(height / 2) + 1), 0), Quaternion.identity);
            destinations.Add(p.GetComponent<Destination>());
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
        foreach(Agent a1 in agents)
        {
            a1.possiblePos.Clear();
            if (!someoneIsThere(a1.transform.position + north)){
                a1.possiblePos.Add(a1.transform.position + north);
            }
            if (!someoneIsThere(a1.transform.position + south))
            {
                a1.possiblePos.Add(a1.transform.position + south);
            }
            if (!someoneIsThere(a1.transform.position + west))
            {
                a1.possiblePos.Add(a1.transform.position + west);
            }
            if (!someoneIsThere(a1.transform.position + east))
            {
                a1.possiblePos.Add(a1.transform.position + east);
            }
        }
    }

    bool someoneIsThere(Vector3 pos)
    {
        foreach(Agent a in agents)
        {
            if(a.nextPos == pos)
            {
                return true;
            }
        }

        foreach(Package p in packages)
        {
            if(p.transform.position == pos)
            {
                return true;
            }
        }

        foreach(Destination d in destinations)
        {
            if(d.transform.position == pos)
            {
                return true;
            }
        }

        if(pos.y < -height/2 || pos.y > height/2 || pos.x > width/2 || pos.x < -width/2)
        {
            return true;
        }

        return false;
    }
}
