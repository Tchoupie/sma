using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Warehouse : MonoBehaviour
{
    public GameObject agentPrefab;
    public GameObject packagePrefab;
    public GameObject destinationPrefab;
    public int nbAgents;
    public int nbPackages;
    public int nbDestinations;
    public int packagesAffected = 0;

    public int moveType;
    public List<Agent> agents;
    public List<Package> packages;
    public List<Destination> destinations;
    public int width = 10;
    public int height = 10;

    public float time = 0;
    public float dt = 0f;
    public Slider sliderSpeed;
    public GameObject turnText;
    public GameObject TextEnd;
    public GameObject panelExperience;
    public GameObject panelEnd;
    int turn = 0;
    float prevdt = 0f;
    int packageInDestination = 0;
    // Start is called before the first frame update

    public static Vector3Int north = new Vector3Int(0, 1, 0);
    public static Vector3Int south = new Vector3Int(0, -1, 0);
    public static Vector3Int west = new Vector3Int(-1, 0, 0);
    public static Vector3Int east = new Vector3Int(1, 0, 0);

    void Start()
    {
        agents = new List<Agent>();
        packages = new List<Package>();
        destinations = new List<Destination>();
        nbAgents = VariablesGlobales.nbAgentsGlob;
        nbPackages = VariablesGlobales.nbPaquetsGlob;
        nbDestinations = 1;
        moveType = VariablesGlobales.mouvementGlob;
        for(int i=0; i<nbAgents;i++)
        {
            Vector3 posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            while(someoneIsThere(posSpawn) != 0)
            {
                posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            }
            GameObject a = Instantiate(agentPrefab,posSpawn,Quaternion.identity);
            a.GetComponent<Agent>().setWitdhHeightWareHouse(width,height); //On donne l'info sur la longueur et la hauteur de la warehouse
            agents.Add(a.GetComponent<Agent>());
        }

        for(int i=0; i < nbPackages; i++)
        {
            Vector3 posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            while(someoneIsThere(posSpawn) != 0)
            {
                posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            }
            GameObject p = Instantiate(packagePrefab, posSpawn, Quaternion.identity);
            packages.Add(p.GetComponent<Package>());
        }

        for (int i = 0; i < nbDestinations; i++)
        {
            Vector3 posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            while(someoneIsThere(posSpawn) != 0)
            {
                posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            }
            GameObject p = Instantiate(destinationPrefab, posSpawn, Quaternion.identity);
            destinations.Add(p.GetComponent<Destination>());
        }
        prevdt = dt;
        print(dt);
    }

    // Update is called once per frame
    void Update()
    {
      if (nbDestinations != nbPackages) {
        dt = sliderSpeed.value;

        time += dt;

        if(time >= 1.0f)
        {
            time = 0f;
            turn+=1;
            turnText.GetComponent<UnityEngine.UI.Text>().text = "Turn "+turn.ToString();

            switch(moveType)
            {
                case 1 :
                    randomMove();
                    break;
                case 2 :
                    moveClosestAffectationRandom();
                    break;
                default:
                    print ("Incorrect move");
                    break;
            }
        }
      }
      else{
        panelExperience.SetActive(false);
        panelEnd.SetActive(true);
        TextEnd.GetComponent<UnityEngine.UI.Text>().text = "Mission accomplie en "+ turn.ToString() + " tours";
      }
    }
    void moveClosestAffectationRandom()
    {
      foreach(Agent a in agents)
      {
          if(time == 0)
          {
              bool hasPickedUp = false;
              bool deposePackage = false;
              bool swap = false;
              if (a.positionTarget == a.transform.position){
                print("affectation");
                affectPackage(a);
              }
              if (a.packageInHands == null) //Si l'agent n'a rien dans les mains
              {
                  hasPickedUp = resolvePickup(a);
              }
              else // Si il a quelque chose dans les mains
              {
                  deposePackage = resolveDestination(a);
                  if (!deposePackage) {
                    swap = resolveSwap(a);
                  }
              }

              if(!hasPickedUp && !deposePackage && !swap) //Si il n'est pas entrain de pick up ou de déposé
              {
                  if (a.positionTarget != a.transform.position) {
                    resolvePos(a);
                    a.computeMoveToTarget();
                    a.move();
                  }
              }
              else //Si il est entrain de déposer ou pick up
              {
                  if(deposePackage) //Si il est entrain de déposer (il a la destination à côté de lui)
                  {
                      a.changeForNormalSprite();
                      a.packageInHands = null;
                      affectPackage(a);
                      packageInDestination++;
                      print("depose");
                  }

                  if(hasPickedUp)
                  {
                      print("pick");
                      a.positionTarget = destinations[0].transform.position;
                  }
              }

          }
      }
    }
    void randomMove()
    {
        foreach(Agent a in agents)
        {
            bool hasPickedUp = false;
            bool deposePackage = false;
            if (a.packageInHands == null) //Si l'agent n'a rien dans les mains
            {
                hasPickedUp = resolvePickupRandom(a);
            }
            else // Si il a quelque chose dans les mains
            {
                deposePackage = resolveDestination(a);
            }

            if(!hasPickedUp && !deposePackage) //Si il n'est pas entrain de pick up ou de déposé
            {
                resolvePos(a);
                a.computeMoveRandom();
                a.move();
            }
            else //Si il est entrain de déposer ou pick up
            {
                if(deposePackage) //Si il est entrain de déposer (il a la destination à côté de lui)
                {
                    a.changeForNormalSprite();
                    a.packageInHands = null;
                    packageInDestination++;
                    print("depose");

                }

                if(hasPickedUp)
                {
                    print("pick");
                }
            }
        }
    }
    bool resolveDestination(Agent a)
    {
        if (someoneIsThere(a.transform.position + north)==3 || someoneIsThere(a.transform.position + south)==3
        || someoneIsThere(a.transform.position + west)==3 || someoneIsThere(a.transform.position + east)==3)
        {
            return true;
        }
        return false;
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

    bool resolvePickupRandom(Agent a)
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
            a.changeForCarrySprite();
            a.packageInHands = packages[indexP];
            packages[indexP].gameObject.SetActive(false);
            //packages.RemoveAt(indexP);
            return true;
        }
        else { return false; }
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
          if (a.positionTarget == packages[indexP].transform.position) {
            a.changeForCarrySprite();
            a.packageInHands = packages[indexP];
            packages[indexP].gameObject.SetActive(false);
            //packages.RemoveAt(indexP);
            return true;
          }
          else { return false; }
        }
        else { return false; }
    }
    bool resolveSwap(Agent a)
    {
        a.possiblePos.Clear();
        Agent agentcible = null;
        if (someoneIsThere(a.transform.position + north) == 4)
        {
            agentcible = findAgent(a.transform.position + north);
        }
        if (someoneIsThere(a.transform.position + south) == 4)
        {
            if (agentcible == null) {
              agentcible = findAgent(a.transform.position + south);
            }
            else{
              Agent temp = findAgent(a.transform.position + south);
              if (Vector3.Distance(destinations[0].transform.position,temp.transform.position)<Vector3.Distance(destinations[0].transform.position,agentcible.transform.position)) {
                agentcible = temp;
              }
            }
        }
        if (someoneIsThere(a.transform.position + west) == 4)
        {
            if (agentcible == null) {
              agentcible = findAgent(a.transform.position + west);
            }
            else{
              Agent temp = findAgent(a.transform.position + west);
              if (Vector3.Distance(destinations[0].transform.position,temp.transform.position)<Vector3.Distance(destinations[0].transform.position,agentcible.transform.position)) {
                agentcible = temp;
              }
            }
        }
        else if (someoneIsThere(a.transform.position + east) == 4)
        {
          if (agentcible == null) {
            agentcible = findAgent(a.transform.position + east);
          }
          else{
            Agent temp = findAgent(a.transform.position + east);
            if (Vector3.Distance(destinations[0].transform.position,temp.transform.position)<Vector3.Distance(destinations[0].transform.position,agentcible.transform.position)) {
              agentcible = temp;
            }
          }
        }
        if (agentcible != null)
        {
          a.changeForNormalSprite();
          agentcible.changeForCarrySprite();
          agentcible.packageInHands = a.packageInHands;
          a.packageInHands = null;
          agentcible.positionTarget = destinations[0].transform.position;
          affectPackage(a);
          print("echange");
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

    Agent findAgent(Vector3 pos)
    {
        for(int i = 0; i < agents.Count; i++)
        {
            if (agents[i].transform.position == pos)
            {
                return agents[i];
            }
        }
        return null;
    }

    int someoneIsThere(Vector3 pos)
    {
        foreach(Agent a in agents)
        {
            if(a.nextPos == pos)
            {
                if (a.positionTarget == a.transform.position) {
                    return 4;
                }
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
    public void affectPackage(Agent a){
      int index;
      bool done = true;
      for (int i = 0; i<packages.Count; i++) {
        if (!packages[i].affected) {
          done = false;
        }
      }
      if (!done) {
        do {
            print("check");
            index = Random.Range(0, packages.Count);
        } while (packages[index].affected);

        a.positionTarget = packages[index].transform.position;
        packages[index].affected = true;
        packagesAffected = packagesAffected+1;
      }
      else{
        print("plus de paquets");
        a.positionTarget = a.transform.position;
      }
    }
    public void ReturnToMainMenu(){
      VariablesGlobales.reset();
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
