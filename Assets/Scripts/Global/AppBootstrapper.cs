using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AppBootstrapper : MonoBehaviour
{
    [SerializeField] private string primeraEscenaReal = "MainMenu";

    void Start()
    {

        StartCoroutine(CargarMenuPrincipal());
    }

    private IEnumerator CargarMenuPrincipal()
    {
        Debug.Log("Iniciando sistemas... saltando a " + primeraEscenaReal);

        yield return null;

        SceneManager.LoadScene(primeraEscenaReal);
    }
}