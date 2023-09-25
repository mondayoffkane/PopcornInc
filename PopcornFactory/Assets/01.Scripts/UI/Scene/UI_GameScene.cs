using System.Collections;
using System.Collections.Generic;
using MondayOFF;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    enum Images
    {
        Mask,
    }
    enum Texts
    {
        Gem_Text,
        AddParts_Price_Text,
        RV_Income_TimeText,
        UpgradeCountText,
        Money_Text,
        CinemaMoney_Text,
        AddStaff_Price_Text,
        Income_Price_Text,
    }
    enum Buttons
    {
        Setting_Button,
        AddParts_Upgrade_Button,
        AddUpgrade_Button,
        RV_Income_Double,
        BigMoneyButton,
        Recipe_Button,
        Cinema_Button,
        Island_Button,
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
        BaseRoom_Button,
        PremiumRoom_Button,
        CinemaRv_Accept_Button,
    }
    enum GameObjects
    {
        IslandUi_Group,
        CinemaUI_Group,
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
        Recipe_Content,
        RoomUpgrade_Panel,
        CInemaRvPanel,
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
        , Recipe_Button
        , Recipe_Close_Button
        , Cinema_Button
        , Island_Button
        , BaseRoom_Button
        , PremiumRoom_Button
        , CinemaRv_Accept_Button
                ;
    //, NextStageButton;

    public Text Money_Text
        //, AddStaff_Price_Text
        //, Income_Price_Text
        , AddParts_Price_Text, Gem_Text, UpgradeCountText
        , RV_Income_TimeText
        , CinemaMoney_Text
        ;

    public GameObject Setting_Panel, Scroll_Panel
                , Upgrade_Panel
        , Content, RvRail_Panel,
        RvWorker_Panel
        , NoAds_Panel
     , Laboratory_Panel,
        Laboratory_Content
        , Recipe_Panel
        , Recipe_Content
        , IslandUi_Group
        , CinemaUI_Group
        , RoomUpgrade_Panel
        , CInemaRvPanel
        ;

    public Image Mask;

    public GameObject[] ScrollUpgrades;

    //public GameObject[] Laboratory_list;

    // =============================================

    private void Awake()
    {
        Bind<UnityEngine.UI.Text>(typeof(Texts));
        Bind<UnityEngine.UI.Button>(typeof(Buttons));
        Bind<UnityEngine.UI.Image>(typeof(Images));
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
        Recipe_Button = GetButton(Buttons.Recipe_Button);
        Recipe_Close_Button = GetButton(Buttons.Recipe_Close_Button);
        Cinema_Button = GetButton(Buttons.Cinema_Button);
        Island_Button = GetButton(Buttons.Island_Button);

        BaseRoom_Button = GetButton(Buttons.BaseRoom_Button);
        PremiumRoom_Button = GetButton(Buttons.PremiumRoom_Button);
        CinemaRv_Accept_Button = GetButton(Buttons.CinemaRv_Accept_Button);



        Money_Text = GetText(Texts.Money_Text);
        //AddStaff_Price_Text = GetText(Texts.AddStaff_Price_Text);
        //Income_Price_Text = GetText(Texts.Income_Price_Text);
        AddParts_Price_Text = GetText(Texts.AddParts_Price_Text);
        Gem_Text = GetText(Texts.Gem_Text);
        UpgradeCountText = GetText(Texts.UpgradeCountText);
        RV_Income_TimeText = GetText(Texts.RV_Income_TimeText);
        CinemaMoney_Text = GetText(Texts.CinemaMoney_Text);


        Setting_Panel = GetObject(GameObjects.Setting_Panel);
        Scroll_Panel = GetObject(GameObjects.Scroll_Panel);

        Upgrade_Panel = GetObject(GameObjects.Upgrade_Panel);
        Content = GetObject(GameObjects.Content);

        RvRail_Panel = GetObject(GameObjects.RvRail_Panel);
        RvWorker_Panel = GetObject(GameObjects.RvWorker_Panel);

        NoAds_Panel = GetObject(GameObjects.NoAds_Panel);
        Laboratory_Panel = GetObject(GameObjects.Laboratory_Panel);
        Laboratory_Content = GetObject(GameObjects.Laboratory_Content);
        Recipe_Panel = GetObject(GameObjects.Recipe_Panel);
        Recipe_Content = GetObject(GameObjects.Recipe_Content);
        IslandUi_Group = GetObject(GameObjects.IslandUi_Group);
        CinemaUI_Group = GetObject(GameObjects.CinemaUI_Group);
        RoomUpgrade_Panel = GetObject(GameObjects.RoomUpgrade_Panel);
        CInemaRvPanel = GetObject(GameObjects.CInemaRvPanel);


        Mask = GetImage(Images.Mask);
        // ======================================
        Mask.alphaHitTestMinimumThreshold = 0.5f;


        AddParts_Upgrade_Button.AddButtonEvent(() =>
        {
            Managers.Game._stageManager.AddParts();
            if (TutorialManager._instance._tutorialLevel == 6)
            {
                TutorialManager._instance.Tutorial_Comple();
            }
        });

        Sound_Button.AddButtonEvent(() =>
        {
            Managers.Data.UseSound = !Managers.Data.UseSound;
            Debug.Log(Managers.Data.UseSound);
            Sound_Button.transform.GetChild(1).gameObject.SetActive(Managers.Data.UseSound);
            Sound_Button.transform.GetChild(2).gameObject.SetActive(!Managers.Data.UseSound);
            Managers.Game._stageManager._islandCam.GetComponent<AudioSource>().mute = !Managers.Data.UseSound;

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
            if (TutorialManager._instance._tutorialLevel == 1)
            {
                TutorialManager._instance.Tutorial_Comple();
                TutorialManager._instance.Tutorial(false);
            }
        });

        Setting_Button.AddButtonEvent(() =>
        {
            OffPopup();
            Setting_Panel.SetActive(true);
            //Managers.UI.ShowPopupUI<UI_PopupSetting>();
        });
        Close_Setting_Button.AddButtonEvent(() => Setting_Panel.SetActive(false));
        Scroll_Close_Button.AddButtonEvent(() =>
        {
            if (TutorialManager._instance._tutorialLevel == 3)
            {
                TutorialManager._instance.Tutorial_Comple();
                //TutorialManager._instance.Tutorial(false);
            }
            Scroll_Panel.SetActive(false);
        });


        Laboratory_Close_Button.AddButtonEvent(() => Laboratory_Panel.SetActive(false));
        Recipe_Button.AddButtonEvent(() =>
        {
            OffPopup();
            Recipe_Panel.SetActive(!Recipe_Panel.activeSelf);
            Recipe_Button.transform.GetChild(0).gameObject.SetActive(false);
            if (TutorialManager._instance._tutorialLevel == 7)
            {
                TutorialManager._instance.Tutorial_Comple();
            }

        });
        Recipe_Close_Button.AddButtonEvent(() => Recipe_Panel.SetActive(false));

        Cinema_Button.AddButtonEvent(() =>
        {
            OffPopup();
            IslandUi_Group.SetActive(false);
            CinemaUI_Group.SetActive(true);
            Managers.Game._stageManager.isCinemaOn(true);
        });

        Island_Button.AddButtonEvent(() =>
        {
            OffPopup();
            IslandUi_Group.SetActive(true);
            CinemaUI_Group.SetActive(false);
            Managers.Game._stageManager.isCinemaOn(false);

        });

        // == Inapp , No Ads ===========================
        NoAds_Button.AddButtonEvent(() => { OffPopup(); NoAds_Panel.SetActive(true); });
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



        BaseRoom_Button.AddButtonEvent(() =>
        {
            Managers.Game._cinemaManager.RoomUpgrade(1);
            RoomUpgrade_Panel.SetActive(false);
        });
        PremiumRoom_Button.AddButtonEvent(() =>
        {
            Managers.Game._cinemaManager.RoomUpgrade(2);
            RoomUpgrade_Panel.SetActive(false);
        });
        CinemaRv_Accept_Button.AddButtonEvent(() =>
        {
            Managers.Game._cinemaManager.CinemaRv();
            CInemaRvPanel.SetActive(false);
        });


    }/// ========= end Set buttons





    public void OffPopup()
    {
        Setting_Panel.SetActive(false);
        Scroll_Panel.SetActive(false);
        Laboratory_Panel.SetActive(false);
        NoAds_Panel.SetActive(false);
        Recipe_Panel.SetActive(false);
        Upgrade_Panel.SetActive(false);

    }


    public void ShowCinemaRvPanel(int _num)
    {
        CInemaRvPanel.SetActive(true);
        CInemaRvPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        CInemaRvPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        CInemaRvPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);

        CInemaRvPanel.transform.GetChild(0).GetChild(_num).gameObject.SetActive(true);


    }



} 







