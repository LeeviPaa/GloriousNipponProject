using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScript1 : MonoBehaviour
{

    EventManager E;

    private void Start()
    {

        E = Toolbox.RegisterComponent<EventManager>();
    }
    private void Update()
    {
        if (Input.GetButton("Jump"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    [AddEditorInvokeButton]
    public void LoadMenuSceneForTest()
    {
        GameManager.ChangeScene("MainMenu");
    }
}
