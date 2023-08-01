﻿using System.Collections;
using System.Collections.Generic;
using MondayOFF;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    enum Texts
    {
        Money_Text,
        Gem_Text,
        AddParts_Price_Text,
        RV_Income_TimeText,
        UpgradeCountText,
        AddStaff_Price_Text,
        Income_Price_Text,
    }
    enum Buttons
    {
        AddParts_Upgrade_Button,
        AddUpgrade_Button,
        RV_Income_Double,
        BigMoneyButton,
        AddStaff_Upgrade_Button,
        Income_Upgrade_Button,
        Sound_Button,
        Vibe_Button,
    }
    enum GameObjects
    {
        Upgrade_Panel,
        Setting_Panel,
        Scroll_Panel,
        Content,

    }


    public Button AddStaff_Upgrade_Button, Income_Upgrade_Button, AddParts_Upgrade_Button,
        Sound_Button, Vibe_Button, AddUpgrade_Button, RV_Income_Double
        , BigMoneyButton;

    public Text Money_Text
       , AddStaff_Price_Text, Income_Price_Text, AddParts_Price_Text, Gem_Text, UpgradeCountText
        , RV_Income_TimeText;

    public GameObject Setting_Panel, Scroll_Panel

        , Upgrade_Panel
        , Content;


    public GameObject[] ScrollUpgrades;

    // =============================================

    private void Awake()
    {
        Bind<UnityEngine.UI.Text>(typeof(Texts));
        Bind<UnityEngine.UI.Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        base.Init();

        SetButton();
    }

    void SetButton()
    {
        AddStaff_Upgrade_Button = GetButton(Buttons.AddStaff_Upgrade_Button);
        Income_Upgrade_Button = GetButton(Buttons.Income_Upgrade_Button);
        AddParts_Upgrade_Button = GetButton(Buttons.AddParts_Upgrade_Button);
        Sound_Button = GetButton(Buttons.Sound_Button);
        Vibe_Button = GetButton(Buttons.Vibe_Button);
        AddUpgrade_Button = GetButton(Buttons.AddUpgrade_Button);
        RV_Income_Double = GetButton(Buttons.RV_Income_Double);
        BigMoneyButton = GetButton(Buttons.BigMoneyButton);



        Money_Text = GetText(Texts.Money_Text);
        AddStaff_Price_Text = GetText(Texts.AddStaff_Price_Text);
        Income_Price_Text = GetText(Texts.Income_Price_Text);
        AddParts_Price_Text = GetText(Texts.AddParts_Price_Text);
        Gem_Text = GetText(Texts.Gem_Text);
        UpgradeCountText = GetText(Texts.UpgradeCountText);
        RV_Income_TimeText = GetText(Texts.RV_Income_TimeText);



        Setting_Panel = GetObject(GameObjects.Setting_Panel);
        Scroll_Panel = GetObject(GameObjects.Scroll_Panel);

        Upgrade_Panel = GetObject(GameObjects.Upgrade_Panel);
        Content = GetObject(GameObjects.Content);

        // ======================================

        AddParts_Upgrade_Button.AddButtonEvent(() => Managers.Game._stageManager.AddParts());

        Sound_Button.AddButtonEvent(() =>
        {
            Managers.Data.UseSound = !Managers.Data.UseSound;
            Debug.Log(Managers.Data.UseSound);
            Sound_Button.transform.GetChild(1).gameObject.SetActive(Managers.Data.UseSound);
            Sound_Button.transform.GetChild(2).gameObject.SetActive(!Managers.Data.UseSound);

        });

        Vibe_Button.AddButtonEvent(() =>
        {
            Managers.Data.UseHaptic = !Managers.Data.UseHaptic;
            Debug.Log(Managers.Data.UseHaptic);
            Vibe_Button.transform.GetChild(1).gameObject.SetActive(Managers.Data.UseHaptic);
            Vibe_Button.transform.GetChild(2).gameObject.SetActive(!Managers.Data.UseHaptic);
        });

        AddUpgrade_Button.AddButtonEvent(() =>
        {
            Scroll_Panel.SetActive(!Scroll_Panel.activeSelf);
            Managers.Game._stageManager.ScrollButtonCheck();
        });



        // rv

        RV_Income_Double.AddButtonEvent(() => AdsManager.ShowRewarded(() => Managers.Game._stageManager.RV_Income_Double()));
        BigMoneyButton.AddButtonEvent(() => AdsManager.ShowRewarded(() => Managers.Game._stageManager.RV_BigMoney()));

    } /// ========= end Set buttons







}
