using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class SettingsController : MonoBehaviour
{

    [SerializeField]
    private RenderPipelineAsset[] qualityLevels;
    private TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.value = QualitySettings.GetQualityLevel();
    }

    // Update is called once per frame
    public void ChangeLevel(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];
    }
}
