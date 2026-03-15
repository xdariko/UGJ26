using TMPro;
using UnityEngine;

public class EtherUI : MonoBehaviour
{
    public TMP_Text label;

    private void OnEnable()
    {
        G.OnEtherChanged += UpdateLabel;
        UpdateLabel(G.Ether);
    }

    private void OnDisable()
    {
        G.OnEtherChanged -= UpdateLabel;
    }

    void UpdateLabel(int v) => label.text = v.ToString();
}
