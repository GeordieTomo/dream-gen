using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ColourPicker : MonoBehaviour
{
    public float currentHue, currentSat, currentVal;

    [SerializeField]
    private RawImage hueImage, satValImage, outputImage;

    [SerializeField]
    private Image pickerImage;

    [SerializeField]
    private Slider hueSlider;

    [SerializeField]
    private TMP_InputField hexInputField;

    private Texture2D hueTexture, svTexture;

    private RectTransform svTransform, pickerTransform, bgTransform;

    private bool clickWithinSVWindow = false;

    public ColourPickerButton colourPickerButton;

    public delegate void ValueChangedHandler();
    public event ValueChangedHandler OnValueChanged;

    private bool updatingVisually = false;

    private void Awake()
    {
        svTransform = satValImage.GetComponent<RectTransform>();
        pickerTransform = pickerImage.GetComponent<RectTransform>();
        pickerTransform.position = new Vector2(-(svTransform.sizeDelta.x * 0.5f), -(svTransform.sizeDelta.y * 0.5f));


        CreateHueImage();

        CreateSVImage();

        UpdateOutputImage();

        hueSlider.onValueChanged.AddListener(UpdateSVImage);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            OnMouseDown();
        }
        if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }

    }

    public void CompleteInputButtonPress()
    {
        colourPickerButton.SetColour(outputImage.color);
        OnValueChanged?.Invoke();
        gameObject.SetActive(false);
    }

    public void CancelInputButtonPress()
    {
        gameObject.SetActive(false);
    }

    void UpdateColour(bool click)
    {
        float deltaX = svTransform.sizeDelta.x * 0.5f;
        float deltaY = svTransform.sizeDelta.y * 0.5f;

        Vector2 mousePos = (Input.mousePosition);
        Vector2 pos = (mousePos - new Vector2(Screen.width / 2, Screen.height / 2)) * new Vector2(1080f / Screen.height, 1080f / Screen.height) - svTransform.anchoredPosition;

        float x = (pos.x + deltaX) / svTransform.sizeDelta.x;
        float y = (pos.y + deltaY) / svTransform.sizeDelta.y;

        float xNorm = Mathf.Clamp(x, 0, 1);
        float yNorm = Mathf.Clamp(y, 0, 1);

        if (click && (x != xNorm || y != yNorm))
        {
            clickWithinSVWindow = false;
        } else if (click)
        {
            clickWithinSVWindow = true;
        }

        if (clickWithinSVWindow)
        {
            pos = new Vector2(deltaX * (xNorm * 2 - 1), deltaY * (yNorm * 2 - 1));


            pickerTransform.anchoredPosition = pos;
            pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);

            SetSV(xNorm, yNorm);
        }
        
    }

    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for (int i = 0; i < hueTexture.height; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 1f));
        }

        hueTexture.Apply();
        currentHue = 0;

        hueImage.texture = hueTexture;
    }

    private void CreateSVImage()
    {
        svTexture = new Texture2D (16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for (int y = 0; y < svTexture.height; y++)
        {
            for (int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();
        currentSat = 0;
        currentVal = 0;

        satValImage.texture = svTexture;
    }

    private void UpdateOutputImage()
    {
        updatingVisually = true;

        Color currentColour = Color.HSVToRGB(currentHue,currentSat,currentVal);

        outputImage.color = currentColour;

        hexInputField.text = ColorUtility.ToHtmlStringRGB(currentColour);

        updatingVisually = false;

    }

    public void SetSV(float s, float v)
    {
        currentSat = s;
        currentVal = v;
        UpdateOutputImage();
    }

    public void UpdateSVImage(float value)
    {

        currentHue = value;

        for (int y= 0; y < svTexture.height;y++)
        {
            for (int x= 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply ();
        UpdateOutputImage ();

    }

    private void OnMouseDrag()
    {
        UpdateColour(false);
    }

    private void OnMouseDown()
    {
        UpdateColour(true);
    }

    public void OnTextInput()
    {
        Debug.Log("visualupdt" + updatingVisually);
        if (hexInputField.text.Length < 6 || updatingVisually)
        {
            return;
        }

        Color newCol;

        if (ColorUtility.TryParseHtmlString("#" + hexInputField.text, out newCol))
        {
            SetColour(newCol);
        }
    }

    public void SetColour(Color color)
    {
        Color.RGBToHSV(color, out currentHue, out currentSat, out currentVal);

        hueSlider.value = currentHue;

        SetXYPos(currentSat, currentVal);

        hexInputField.text = "";
        UpdateOutputImage();
    }

    public void SetXYPos(float x, float y)
    {

        float deltaX = svTransform.sizeDelta.x * 0.5f;
        float deltaY = svTransform.sizeDelta.y * 0.5f;

        Vector2 pos = new Vector2(deltaX * (x * 2 - 1), deltaY * (y * 2 - 1));

        pickerTransform.anchoredPosition = pos;

    }
}
