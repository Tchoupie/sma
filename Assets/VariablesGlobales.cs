using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablesGlobales : MonoBehaviour
{
    public static int nbPaquetsGlob = 6;
    public static int mouvementGlob = 1;
    public static int nbAgentsGlob = 5;

    public static void reset(){//fonction de remise à zero de l'experience appellee au retour au menu principal
      nbPaquetsGlob = 6;
      mouvementGlob = 1;
      nbAgentsGlob = 5;
    }
}
