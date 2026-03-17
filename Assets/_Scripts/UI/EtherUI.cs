using TMPro;
using UnityEngine;

public class EtherUI : MonoBehaviour
{
    public TMP_Text redEtherLabel;
    public TMP_Text whiteEtherLabel;
    public TMP_Text purpleEtherLabel;


    private void OnEnable()
    {
        G.OnEtherChanged += UpdateLabel;
        UpdateLabel(EtherType.Red, G.RedEther);
        UpdateLabel(EtherType.White, G.WhiteEther);
        UpdateLabel(EtherType.Purple, G.PurpleEther);
    }

    private void OnDisable()
    {
        G.OnEtherChanged -= UpdateLabel;
    }

    void UpdateLabel(EtherType type, int v)
    {
        switch (type)
        {
            case EtherType.Red:
                redEtherLabel.text = v.ToString();
                break;
            case EtherType.White:
                whiteEtherLabel.text = v.ToString(); 
                break;
            case EtherType.Purple:
                purpleEtherLabel.text = v.ToString();
                break;
        }
    }
}
