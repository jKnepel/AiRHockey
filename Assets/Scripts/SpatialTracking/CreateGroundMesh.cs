using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class CreateGroundMesh : MonoBehaviour
{
    [SerializeField]
    ARMeshManager manager;
    // Start is called before the first frame update
    void Awake()
    {
     if(!manager)
        {
            manager = GetComponent<ARMeshManager>();
        }
    }

     void OnEnable()
    {
        manager.meshesChanged += UpdateGroundMesh;
    }

    void OnDisable()
    {
        manager.meshesChanged -= UpdateGroundMesh;
    }
    void UpdateGroundMesh(ARMeshesChangedEventArgs arg)
    {
        foreach(MeshFilter mesh in arg.added)
        {
            if(CheckIfHorizontal(mesh))
            {
                mesh.gameObject.layer = 11;
            }
        }
    }
    private bool CheckIfHorizontal(MeshFilter mesh)
    {
        return true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
