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
    public int packageInDestination = 0;
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

        for(int i=0; i<nbAgents;i++) //Initialisation des agents
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

        for(int i=0; i < nbPackages; i++) //Initialisation des packages
        {
            Vector3 posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            while(someoneIsThere(posSpawn) != 0)
            {
                posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            }
            GameObject p = Instantiate(packagePrefab, posSpawn, Quaternion.identity);
            packages.Add(p.GetComponent<Package>());
        }

        for (int i = 0; i < nbDestinations; i++) //Initialisation des destinations
        {
            Vector3 posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            while(someoneIsThere(posSpawn) != 0)
            {
                posSpawn = new Vector3(Random.Range((int)-width/2,(int)(width/2)+1),Random.Range((int)-height/2,(int)(height/2)+1),0);
            }
            GameObject p = Instantiate(destinationPrefab, posSpawn, Quaternion.identity);
            destinations.Add(p.GetComponent<Destination>());
        }
    }

    // Update is called once per frame
    void Update()
    {
      if (packageInDestination != nbPackages) { //Condition de fin de l'expérience
        dt = sliderSpeed.value; //On prend comme dt la valeur du slider

        time += dt; //On rajoute au temps le dt

        if(time >= 1.0f) //Lorsque le temps à 1 (ou au dessus) on passe 1 tour et on execute tout les mouvements et tâches
        {
            time = 0f;
            turn+=1;
            turnText.GetComponent<UnityEngine.UI.Text>().text = "Turn "+turn.ToString(); //Pour savoir le nombre de tour

            //Pour changer de stratégie
            switch(moveType)
            {
                case 1 :
                    randomMove();
                    break;
                case 2 :
                    moveAffectationRandom();
                    break;
                case 3 :
                    moveAffectationFar();
                    break;
                case 4 :
                    moveAffectationClose();
                    break;
                default:
                    print ("Incorrect move");
                    break;
            }
        }
      }
      else{ //Quand tout les packages on été déposé
        print("fin experience");
        panelExperience.SetActive(false);
        panelEnd.SetActive(true);
        TextEnd.GetComponent<UnityEngine.UI.Text>().text = "Mission accomplie en "+ turn.ToString() + " tours";
      }
    }
    void moveAffectationRandom()
    {
      foreach(Agent a in agents)
      {
          if(time == 0)
          {
              bool hasPickedUp = false;
              bool deposePackage = false;
              Agent swap = null;
              if (a.positionTarget == a.transform.position){ //Si l'agent n'as rien à faire on essaie de lui affecté un package
                print("affectation");
                affectPackageRandom(a);
              }
              if (a.packageInHands == null) //Si l'agent n'a rien dans les mains
              {
                  hasPickedUp = resolvePickup(a);
              }
              else // Si il a quelque chose dans les mains
              {
                  deposePackage = resolveDestination(a); //On essaie de voir si il peut déposer le package a la destination
                  if (!deposePackage) { //Si il ne peut pas déposer le package (pas à côté de la destination)
                    swap = resolveSwap(a); //On vois si il y a quelqu'un de disponible pour prendre le colis (quelqu'un qui n'as rien a faire)
                  }
              }

              if(!hasPickedUp && !deposePackage) //Si il n'est pas entrain de pick up ou de déposé
              {
                  if (a.positionTarget != a.transform.position) { //Si l'agent doit faire quelque chose (bouger ou faire une tâche)
                    resolvePos(a); //L'agent regarde où est-ce qu'il peut éventuellement aller
                    a.computeMoveToTarget(); //On calcule ensuite la meilleure position pour aller a l'objectif
                    if (swap != null) { //Si l'agent peux swap
                    //On vérifie également si c'est intéressant de faire un swap ou pas (en fonction de la distance à la destination)
                      if (Vector3.Distance(destinations[0].transform.position,a.nextPos)>Vector3.Distance(destinations[0].transform.position,swap.transform.position)) {
                        a.changeForNormalSprite();

                        swap.changeForCarrySprite();
                        swap.packageInHands = a.packageInHands;

                        a.packageInHands = null;
                        swap.positionTarget = destinations[0].transform.position;
                        a.nextPos = a.transform.position;

                        affectPackageRandom(a);
                        print("echange");
                      }
                      else{ //Si c'est pas intéressant on bouge
                        a.move();
                      }
                    }
                    else{ //Si l'agent ne peux pas faire de swap
                      a.move();
                    }
                  }
              }
              else //Si il est entrain de déposer ou pick up
              {
                  if(deposePackage) //Si il est entrain de déposer (il a la destination à côté de lui)
                  {
                      a.changeForNormalSprite();
                      a.packageInHands = null;
                      affectPackageRandom(a);
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
    void moveAffectationFar()
    {
      foreach(Agent a in agents)
      {
          if(time == 0)
          {
              bool hasPickedUp = false;
              bool deposePackage = false;
              Agent swap = null;
              if (a.positionTarget == a.transform.position){ //Si l'agent n'as rien à faire on essaie de lui affecté un package
                print("affectation");
                affectPackageFar(a);
              }
              if (a.packageInHands == null) //Si l'agent n'a rien dans les mains
              {
                  hasPickedUp = resolvePickup(a);
              }
              else // Si il a quelque chose dans les mains
              {
                  deposePackage = resolveDestination(a); //On essaie de voir si il peut déposer le package a la destination
                  if (!deposePackage) { //Si il ne peut pas déposer le package (pas à côté de la destination)
                    swap = resolveSwap(a); //On vois si il y a quelqu'un de disponible pour prendre le colis (quelqu'un qui n'as rien a faire)
                  }
              }

              if(!hasPickedUp && !deposePackage) //Si il n'est pas entrain de pick up ou de déposé
              {
                  if (a.positionTarget != a.transform.position) { //Si l'agent doit faire quelque chose (bouger ou faire une tâche)
                    resolvePos(a); //L'agent regarde où est-ce qu'il peut éventuellement aller
                    a.computeMoveToTarget(); //On calcule ensuite la meilleure position pour aller a l'objectif
                    if (swap != null) { //Si l'agent peux swap
                    //On vérifie également si c'est intéressant de faire un swap ou pas (en fonction de la distance à la destination)
                      if (Vector3.Distance(destinations[0].transform.position,a.nextPos)>Vector3.Distance(destinations[0].transform.position,swap.transform.position)) {
                        a.changeForNormalSprite();

                        swap.changeForCarrySprite();
                        swap.packageInHands = a.packageInHands;

                        a.packageInHands = null;
                        swap.positionTarget = destinations[0].transform.position;
                        a.nextPos = a.transform.position;

                        affectPackageFar(a);
                        print("echange");
                      }
                      else{ //Si c'est pas intéressant on bouge
                        a.move();
                      }
                    }
                    else{ //Si l'agent ne peux pas faire de swap
                      a.move();
                    }
                  }
              }
              else //Si il est entrain de déposer ou pick up
              {
                  if(deposePackage) //Si il est entrain de déposer (il a la destination à côté de lui)
                  {
                      a.changeForNormalSprite();
                      a.packageInHands = null;
                      affectPackageFar(a);
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
    void moveAffectationClose()
    {
      foreach(Agent a in agents)
      {
          if(time == 0)
          {
              bool hasPickedUp = false;
              bool deposePackage = false;
              Agent swap = null;
              if (a.positionTarget == a.transform.position){ //Si l'agent n'as rien à faire on essaie de lui affecté un package
                print("affectation");
                affectPackageClose(a);
              }
              if (a.packageInHands == null) //Si l'agent n'a rien dans les mains
              {
                  hasPickedUp = resolvePickup(a);
              }
              else // Si il a quelque chose dans les mains
              {
                  deposePackage = resolveDestination(a); //On essaie de voir si il peut déposer le package a la destination
                  if (!deposePackage) { //Si il ne peut pas déposer le package (pas à côté de la destination)
                    swap = resolveSwap(a); //On vois si il y a quelqu'un de disponible pour prendre le colis (quelqu'un qui n'as rien a faire)
                  }
              }

              if(!hasPickedUp && !deposePackage) //Si il n'est pas entrain de pick up ou de déposé
              {
                  if (a.positionTarget != a.transform.position) { //Si l'agent doit faire quelque chose (bouger ou faire une tâche)
                    resolvePos(a); //L'agent regarde où est-ce qu'il peut éventuellement aller
                    a.computeMoveToTarget(); //On calcule ensuite la meilleure position pour aller a l'objectif
                    if (swap != null) { //Si l'agent peux swap
                    //On vérifie également si c'est intéressant de faire un swap ou pas (en fonction de la distance à la destination)
                      if (Vector3.Distance(destinations[0].transform.position,a.nextPos)>=Vector3.Distance(destinations[0].transform.position,swap.transform.position)) {
                        a.changeForNormalSprite();

                        swap.changeForCarrySprite();
                        swap.packageInHands = a.packageInHands;

                        a.packageInHands = null;
                        swap.positionTarget = destinations[0].transform.position;
                        a.nextPos = a.transform.position;

                        affectPackageClose(a);
                        print("echange");
                      }
                      else{ //Si c'est pas intéressant on bouge
                        a.move();
                      }
                    }
                    else{ //Si l'agent ne peux pas faire de swap
                      a.move();
                    }
                  }
              }
              else //Si il est entrain de déposer ou pick up
              {
                  if(deposePackage) //Si il est entrain de déposer (il a la destination à côté de lui)
                  {
                      a.changeForNormalSprite();
                      a.packageInHands = null;
                      affectPackageClose(a);
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
    bool resolveDestination(Agent a) //On regarde si la destination est une case à côté
    {
        if (someoneIsThere(a.transform.position + north)==3 || someoneIsThere(a.transform.position + south)==3
        || someoneIsThere(a.transform.position + west)==3 || someoneIsThere(a.transform.position + east)==3)
        {
            return true;
        }
        return false;
    }
    void resolvePos(Agent a) //On regarde les positions possibles
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

    bool resolvePickupRandom(Agent a) //On regarde les position possible (dans la méthode de déplacement Random)
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
            packages.RemoveAt(indexP);
            return true;
        }
        else { return false; }
    }
    bool resolvePickup(Agent a) //Pour le cas où les packages sont attribués
    {
        a.possiblePos.Clear();
        int indexP = -1;
        List<int> indexPs = new List<int>();
        if (someoneIsThere(a.transform.position + north) == 2)
        {
            indexP = findPackage(a.transform.position + north);
            indexPs.Add(indexP);
        }
        if (someoneIsThere(a.transform.position + south) == 2)
        {
            indexP = findPackage(a.transform.position + south);
            indexPs.Add(indexP);
        }
        if (someoneIsThere(a.transform.position + west) == 2)
        {
            indexP = findPackage(a.transform.position + west);
            indexPs.Add(indexP);
        }
        if (someoneIsThere(a.transform.position + east) == 2)
        {
            indexP = findPackage(a.transform.position + east);
            indexPs.Add(indexP);
        }

        if (indexPs.Count > 0)
        {
            foreach(int i in indexPs)
            {
                if (a.positionTarget == packages[i].transform.position)
                { //On vérifie que le package que l'agent essaie de prendre est le sien
                    a.changeForCarrySprite();
                    a.packageInHands = packages[i];
                    packages[i].gameObject.SetActive(false);
                    packages.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        return false;
    }
    Agent resolveSwap(Agent a) //On regarde si il est possible de swap (on vérifie si il y a un ou plusieurs agents proche, dans le cas où il y en a plusieurs
    // on prend celui le plus proche de la destination). Renvoe null si swap pas dispo
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
        return agentcible;
    }

    int findPackage(Vector3 pos) //Méthode pour retrouver l'index (dans la liste packages) d'un package à partir de sa position
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

    Agent findAgent(Vector3 pos) //Méthode pour retrouver un agent dans la liste agents à partir de sa position
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

    int someoneIsThere(Vector3 pos) //Méthode pour vérifier ce qu'il y a à une position donnée.
    // Renvoie 1 si il y a un agent, 2 si il y a un package, 3 si il y a la destination et 4 si il y a un agent qui n'as rien à faire
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
    public void affectPackageRandom(Agent a){ //Affecter package
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
    public void affectPackageClose(Agent a){ //Affecter package
      int index = 0;
      bool done = true;
      for (int i = 0; i<packages.Count; i++) {
        if (!packages[i].affected) {
          done = false;
        }
      }
      if (!done) {
        float distMin = 9999;
        for (int i = 0; i<packages.Count; i++) {
          if (!packages[i].affected) {
            float distance = Vector3.Distance(packages[i].transform.position,a.transform.position);
            if (distance<distMin) {
              distMin = distance;
              index = i;
            }
          }
        }
        a.positionTarget = packages[index].transform.position;
        packages[index].affected = true;
        packagesAffected = packagesAffected+1;
      }
      else{
        print("plus de paquets");
        a.positionTarget = a.transform.position;
      }
    }
    public void affectPackageFar(Agent a){ //Affecter package
      int index = 0;
      bool done = true;
      for (int i = 0; i<packages.Count; i++) {
        if (!packages[i].affected) {
          done = false;
        }
      }
      if (!done) {
        float distMax = 0;
        for (int i = 0; i<packages.Count; i++) {
          if (!packages[i].affected) {
            float distance = Vector3.Distance(packages[i].transform.position,a.transform.position);
            if (distance>distMax) {
              distMax = distance;
              index = i;
            }
          }
        }
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
