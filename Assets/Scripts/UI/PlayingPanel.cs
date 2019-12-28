using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingPanel : BasePanel
{
    [HideInInspector] public bool inThisPanel;
    private Image _crosshair;
    private Text _ammoText;
    private Text _healthText;
    private Slider _healthSlider;
    private Text _buildingText;
    private Text _dialogText;
    private Text _missionText;
    

    public override void Start()
    {
        base.Start();
        _crosshair = transform.Find("Crosshair").GetComponent<Image>();
        _ammoText = transform.Find("AmmoAmount").GetComponent<Text>();
        _dialogText = transform.Find("DialogText").GetComponent<Text>();
        _healthText = transform.Find("State/Health/numText")
            .GetComponent<Text>();
        _healthSlider = transform.Find("State/Health")
            .GetComponent<Slider>();
        _buildingText = transform.Find("State/Material/numText")
            .GetComponent<Text>();
        _missionText =
            transform.Find("MissionText").GetComponent<Text>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&inThisPanel)
        {
            UIManager.Instance.PushPanel(UIType.PausedMenuUI);
        }
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inThisPanel = true;
    }

    public override void OnPaused()
    {
        inThisPanel = false;
    }

    public override void OnResume()
    {
        inThisPanel = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void SetCrosshair(Sprite crosshair)
    {
        if (_crosshair==null)
        {
            _ammoText = transform.Find("AmmoAmount").GetComponent<Text>();
        }
        _crosshair = transform.Find("Crosshair").GetComponent<Image>();
        
        _crosshair.sprite = crosshair;
    }

    public void SetAmmoAmount(int currentAmmo,int haveAmmo)
    {
        if (_ammoText==null)
        {
            _ammoText = transform.Find("AmmoAmount").GetComponent<Text>();
        }
        _ammoText.text =
            currentAmmo + "/" + haveAmmo;
    }

    public void SetBuildingAmount(int haveAmount)
    {
        if (_buildingText==null)
        {
            _buildingText = transform.Find("State/Material/numText")
                .GetComponent<Text>();
        }

        _buildingText.text =  haveAmount.ToString();
    }

    public void SetDialogText(string text)
    {
        if (_dialogText==null)
        {
            _dialogText = transform.Find("DialogText").GetComponent<Text>();
        }
        _dialogText.text = text;
    }

    public void SetHealthText(int health,int maxHealth)
    {
        if (_healthText==null||_healthSlider==null)
        {
            _healthText = transform.Find("State/Health/numText")
                        .GetComponent<Text>();
            _healthSlider = transform.Find("State/Health")
                .GetComponent<Slider>();
        }
        _healthText.text = health.ToString();
        _healthSlider.value = (float)health / maxHealth;
    }

    public void SetMissionText(string text)
    {
        if (_missionText==null)
        {
            _missionText = transform.Find("MissionText").GetComponent<Text>();
        }

        _missionText.text = text;
    }


}
