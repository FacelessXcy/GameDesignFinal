using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingPanel : BasePanel
{
    [HideInInspector] public bool inThisPanel;
    private Image _crosshair;
    private Text _ammoText;

    public override void Start()
    {
        base.Start();
        _crosshair = transform.Find("Crosshair").GetComponent<Image>();
        _ammoText = transform.Find("AmmoAmount").GetComponent<Text>();
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
            currentAmmo.ToString() + "/" + haveAmmo.ToString();
    }



}
