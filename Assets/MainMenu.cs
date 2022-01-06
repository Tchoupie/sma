using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void Lancement(){
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void SetPackets(int nb){
      VariablesGlobales.nbPaquetsGlob = nb;
    }
    public void SetMovement(int nb){
      VariablesGlobales.mouvementGlob = nb;
    }
    public void SetAgents(int nb){
      VariablesGlobales.nbAgentsGlob = nb;
    }
}
