using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AlineadorUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public RectTransform referenciaUIPunto;
    public Canvas canvasPrincipal;

    [Header("Configuración")]
    public bool alinearAhora = false;

    void Update()
    {
        if (!Application.isPlaying && alinearAhora)
        {
            AlinearObjetoAMatchUI();
        }
    }

    public void AlinearObjetoAMatchUI()
    {
        if (referenciaUIPunto == null || canvasPrincipal == null) return;


        Vector3[] esquinasUI = new Vector3[4];
        referenciaUIPunto.GetWorldCorners(esquinasUI);

        Vector3 centroUIWorld = (esquinasUI[0] + esquinasUI[2]) * 0.5f;


        transform.position = new Vector3(centroUIWorld.x, centroUIWorld.y, transform.position.z);


        float anchoUIWorld = Vector3.Distance(esquinasUI[0], esquinasUI[3]);
        float altoUIWorld = Vector3.Distance(esquinasUI[0], esquinasUI[1]);

        float factorDeEscala = anchoUIWorld;

        transform.localScale = new Vector3(factorDeEscala, factorDeEscala, 1f);

        Debug.Log("Objeto alineado con la UI.");
    }
}