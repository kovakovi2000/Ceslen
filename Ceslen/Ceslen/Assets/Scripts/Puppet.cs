using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puppet : MonoBehaviour
{
    public GameObject TouchedTrigger;
    private PreViewModel.pwModel model;
    private int collectRate;

    public PreViewModel.pwModel Model {
        get => model;
        set {
            model = value;
            switch (model)
            {
                case PreViewModel.pwModel.None:
                    collectRate = 0;
                    break;
                case PreViewModel.pwModel.City:
                    collectRate = 2;
                    break;
                case PreViewModel.pwModel.Town:
                    collectRate = 1;
                    break;
                case PreViewModel.pwModel.Road:
                    collectRate = 0;
                    break;
                default:
                    collectRate = 0;
                    break;
            }
        } }

    public int CollectRate { get => collectRate; }
}
