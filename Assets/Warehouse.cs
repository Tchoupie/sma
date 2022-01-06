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
                bool hasPickedUp = false;
                if (a.packageInHands == null)
                {
                    hasPickedUp = resolvePickup(a);
                }
                if(!hasPickedUp)
                {
                    resolvePos(a);
                    a.computeMove();
                    a.move();
                }
                else
                {
                    print("pick");
                }
                
            }
        }
    }

    void resolvePos(Agent a)
    {
        a.possiblePos.Clear();
        if (someoneIsThere(a.transform.position + north)==0){
            a.possiblePos.Add(a.transform.position + north);
        }
        if (someoneIsThere(a.transform.position + south)==0)
        {
            a.possiblePos.Add(a.transform.position + south);
        }
        if (someoneIsThere(a.transform.position + west)==0)
        {
            a.possiblePos.Add(a.transform.position + west);
        }
        if (someoneIsThere(a.transform.position + east)==0)
        {
            a.possiblePos.Add(a.transform.position + east);
        }
    }

    bool resolvePickup(Agent a)
    {
        a.possiblePos.Clear();
        int indexP = -1;
        if (someoneIsThere(a.transform.position + north) == 2)
        {
            indexP = findPackage(a.transform.position + north);
        }
        else if (someoneIsThere(a.transform.position + south) == 2)
        {
            indexP = findPackage(a.transform.position + south);
        }
        else if (someoneIsThere(a.transform.position + west) == 2)
        {
            indexP = findPackage(a.transform.position + west);
        }
        else if (someoneIsThere(a.transform.position + east) == 2)
        {
            indexP = findPackage(a.transform.position + east);
        }
        if (indexP != -1)
        {
            a.packageInHands = packages[indexP];
            packages[indexP].gameObject.SetActive(false);
            packages.RemoveAt(indexP);
            return true;
        }
        else { return false; }
    }

    int findPackage(Vector3 pos)
    {
        for(int i = 0; i < packages.Count; i++)
        {
            if (packages[i].transform.position == pos)
            {
                return i;
            }
        }
        return -1;
    }

    int someoneIsThere(Vector3 pos)
    {
        foreach(Agent a in agents)
        {
            if(a.nextPos == pos)
            {
                return 1;
            }
        }

        foreach(Package p in packages)
        {
            if(p.transform.position == pos)
            {
                return 2;
            }
        }

        foreach(Destination d in destinations)
        {
            if(d.transform.position == pos)
            {
                return 3;
            }
        }

        if(pos.y < -height/2 || pos.y > height/2 || pos.x > width/2 || pos.x < -width/2)
        {
            return 100;
        }

        return 0;
    }
}
