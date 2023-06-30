using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class LayeredOutline : MonoBehaviour
{
    static FreeMovement freeMovement;
    int[] layerCounts;
    Outline outline;
    void Start()
    {
        if (freeMovement == null)
        {
          freeMovement = GameObject.FindAnyObjectByType<FreeMovement>();
        }
        layerCounts = new int[freeMovement.layerNames.Length];

        outline = GetComponent<Outline>();
    }

  public void AddLayer(string layerName)
  {
    FreeMovement.OutlineLayerColor layerColor = freeMovement.layerDictionary[layerName];
    layerCounts[layerColor.index]++;
    if (layerCounts[layerColor.index] == 1)
    {
      ShowFirstLayer();
    }
  }

  public void SubtractLayer(string layerName)
  {
    FreeMovement.OutlineLayerColor layerColor = freeMovement.layerDictionary[layerName];
    if (layerCounts[layerColor.index] <= 0) throw new System.Exception("Cannot remove Outline layer " + layerName + "; there are none left.");
    layerCounts[layerColor.index]--;
    if (layerCounts[layerColor.index] == 0)
    {
      ShowFirstLayer();
    }
  }

  void ShowFirstLayer()
  {
    for(int i = 0; i < layerCounts.Length; i++)
    {
      if (layerCounts[i] > 0)
      {
        outline.enabled = true;
        outline.OutlineColor = freeMovement.layerArray[i].color;
        return;
      }
    }
    outline.enabled = false;
  }
}
