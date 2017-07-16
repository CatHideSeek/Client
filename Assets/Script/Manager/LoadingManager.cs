using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour {

    public void Start() {
        SceneLoadManager.instance.sceneMethod.Invoke();
    }


}
