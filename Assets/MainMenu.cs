using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void Lancement(){// change de scene pour lancer l'experience
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void SetPackets(int nb){
      VariablesGlobales.nbPaquetsGlob = nb +1;
    }
    public void SetMovement(int nb){
      VariablesGlobales.mouvementGlob = nb +1;
    }
    public void SetAgents(int nb){
      VariablesGlobales.nbAgentsGlob = nb +1;
    }
}
