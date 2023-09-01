using System.Collections;
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
        RV_Income_TimeText,
        UpgradeCountText,
        AddParts_Price_Text,
        AddStaff_Price_Text,
        Income_Price_Text,
    }
    enum Buttons
    {
        AddParts_Upgrade_Button,
        Setting_Button,
        AddUpgrade_Button,
        RV_Income_Double,
        BigMoneyButton,
        NoAds_Button,
        AddStaff_Upgrade_Button,
        Income_Upgrade_Button,
        Close_Setting_Button,
        Sound_Button,
        Vibe_Button,
        Restore_Button,
        Scroll_Close_Button,
        NextStageButton,
        NoAds_Purchase_Button,
        NoAds_Close_Button,
        Laboratory_Close_Button,
        Recipe_Close_Button,
    }
    enum GameObjects
    {
        Upgrade_Panel,
        Setting_Panel,
        Scroll_Panel,
        Content,
        RvRail_Panel,
        RvWorker_Panel,
        NoAds_Panel,
        Laboratory_Panel,
        Laboratory_Content,
        Recipe_Panel,
    }


    public Button
        //AddStaff_Upgrade_Button,
        //Income_Upgrade_Button        ,
        AddParts_Upgrade_Button,
        Sound_Button, Vibe_Button, AddUpgrade_Button, RV_Income_Double
        , BigMoneyButton
        , Setting_Button
        , Scroll_Close_Button
        , Close_Setting_Button
        , NoAds_Purchase_Button
        , NoAds_Close_Button
        , NoAds_Button
        , Restore_Button
        , Laboratory_Close_Button
                ;
    //, NextStageButton;

    public Text Money_Text
        //, AddStaff_Price_Text
        //, Income_Price_Text
        , AddParts_Price_Text, Gem_Text, UpgradeCountText
        , RV_Income_TimeText;

    public GameObject Setting_Panel, Scroll_Panel
                , Upgrade_Panel
        , Content, RvRail_Panel,
        RvWorker_Panel
        , NoAds_Panel
     , Laboratory_Panel,
        Laboratory_Content
        ;


    public GameObject[] ScrollUpgrades;

    public GameObject[] Laboratory_list;

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
        //AddStaff_Upgrade_Button = GetButton(Buttons.AddStaff_Upgrade_Button);
        //Income_Upgrade_Button = GetButton(Buttons.Income_Upgrade_Button);
        AddParts_Upgrade_Button = GetButton(Buttons.AddParts_Upgrade_Button);
        Sound_Button = GetButton(Buttons.Sound_Button);
        Vibe_Button = GetButton(Buttons.Vibe_Button);
        AddUpgrade_Button = GetButton(Buttons.AddUpgrade_Button);
        RV_Income_Double = GetButton(Buttons.RV_Income_Double);
        BigMoneyButton = GetButton(Buttons.BigMoneyButton);
        //NextStageButton = GetButton(Buttons.NextStageButton);
        Setting_Button = GetButton(Buttons.Setting_Button);
        Scroll_Close_Button = GetButton(Buttons.Scroll_Close_Button);
        Close_Setting_Button = GetButton(Buttons.Close_Setting_Button);
        NoAds_Purchase_Button = GetButton(Buttons.NoAds_Purchase_Button);
        NoAds_Close_Button = GetButton(Buttons.NoAds_Close_Button);
        NoAds_Button = GetButton(Buttons.NoAds_Button);
        Restore_Button = GetButton(Buttons.Restore_Button);
        Laboratory_Close_Button = GetButton(Buttons.Laboratory_Close_Button);


        Money_Text = GetText(Texts.Money_Text);
        //AddStaff_Price_Text = GetText(Texts.AddStaff_Price_Text);
        //Income_Price_Text = GetText(Texts.Income_Price_Text);
        AddParts_Price_Text = GetText(Texts.AddParts_Price_Text);
        Gem_Text = GetText(Texts.Gem_Text);
        UpgradeCountText = GetText(Texts.UpgradeCountText);
        RV_Income_TimeText = GetText(Texts.RV_Income_TimeText);



        Setting_Panel = GetObject(GameObjects.Setting_Panel);
        Scroll_Panel = GetObject(GameObjects.Scroll_Panel);

        Upgrade_Panel = GetObject(GameObjects.Upgrade_Panel);
        Content = GetObject(GameObjects.Content);

        RvRail_Panel = GetObject(GameObjects.RvRail_Panel);
        RvWorker_Panel = GetObject(GameObjects.RvWorker_Panel);

        NoAds_Panel = GetObject(GameObjects.NoAds_Panel);
        Laboratory_Panel = GetObject(GameObjects.Laboratory_Panel);
        Laboratory_Content = GetObject(GameObjects.Laboratory_Content);


        // ======================================

        AddParts_Upgrade_Button.AddButtonEvent(() => Managers.Game._stageManager.AddParts());

        Sound_Button.AddButtonEvent(() =>
        {
            Managers.Data.UseSound = !Managers.Data.UseSound;
            Debug.Log(Managers.Data.UseSound);
            Sound_Button.transform.GetChild(1).gameObject.SetActive(Managers.Data.UseSound);
            Sound_Button.transform.GetChild(2).gameObject.SetActive(!Managers.Data.UseSound);
            Managers.Game._stageManager._cam.GetComponent<AudioSource>().mute = !Managers.Data.UseSound;

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

        Setting_Button.AddButtonEvent(() => Setting_Panel.SetActive(true));
        Close_Setting_Button.AddButtonEvent(() => Setting_Panel.SetActive(false));
        Scroll_Close_Button.AddButtonEvent(() => Scroll_Panel.SetActive(false));


        Laboratory_Close_Button.AddButtonEvent(() => Laboratory_Panel.SetActive(false));

        // == Inapp , No Ads ===========================
        NoAds_Button.AddButtonEvent(() => NoAds_Panel.SetActive(true));
        NoAds_Purchase_Button.AddButtonEvent(() =>
        {
            MondayOFF.NoAds.Purchase();

        });
        NoAds.OnNoAds += () => Debug.Log("No Ads 구매 완료 ");
        NoAds.OnNoAds += () =>
        {
            NoAds_Panel.SetActive(false);
            NoAds_Button.gameObject.SetActive(false);
            Managers.Game._stageManager._noAds = 1;
        };

        //NoAds_Close_Button.AddButtonEvent(() => NoAds_Panel.SetActive(false));

        Restore_Button.AddButtonEvent(() => MondayOFF.IAPManager.RestorePurchase());

        // rv

        RV_Income_Double.AddButtonEvent(() => AdsManager.ShowRewarded(() => Managers.Game._stageManager.RV_Income_Double()));
        BigMoneyButton.AddButtonEvent(() => AdsManager.ShowRewarded(() => Managers.Game._stageManager.RV_BigMoney()));
        //RvRail_Panel.transform.Find("Claim_Button").GetComponent<Button>().AddButtonEvent(() => AdsManager.ShowRewarded(() => Managers.Game._stageManager.RV_Rail()));
        //RvWorker_Panel.transform.Find("Claim_Button").GetComponent<Button>().AddButtonEvent(() => AdsManager.ShowRewarded(() => Managers.Game._stageManager.RV_Worker()));

        // ========Laboratory
        Laboratory_list = new GameObject[Laboratory_Content.transform.childCount];
        for (int i = 0; i < /*Laboratory_list.Length*/2; i++)
        {
            Laboratory_list[i] = Laboratory_Content.transform.GetChild(i).gameObject;
            Laboratory_list[i].transform.Find("Slider").GetComponent<Slider>().value = 0;
            Laboratory_list[i].transform.Find("Ratio_Text").GetComponent<Text>().text = "0 %";
        }

        Laboratory_list[0].transform.Find("Slider").GetComponent<Slider>().onValueChanged
                .AddListener(ValueChange0);
        Laboratory_list[1].transform.Find("Slider").GetComponent<Slider>().onValueChanged
                .AddListener(ValueChange1);




    }


    void ValueChange0(float value)
    {
        Managers.Game._labotoryManager.ResourceValue(0, value);
    }
    void ValueChange1(float value)
    {
        Managers.Game._labotoryManager.ResourceValue(1, value);
    }



} /// ========= end Set buttons







