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
        PlayTime_Guage,
    }
    enum Texts
    {
        AddParts_Price_Text,
        RV_Income_TimeText,
        UpgradeCountText,
        Money_Text,
        CinemaMoney_Text,
        AddStaff_Price_Text,
        Income_Price_Text,
        CinemaRvMoney_Text,
        CurrentPlayTime_Text,
        Gem_Text,
    }
    enum Buttons
    {
        Setting_Button,
        NoAds_Button,
        PlayTimeReward_Button,
        AddParts_Upgrade_Button,
        AddUpgrade_Button,
        RV_Income_Double,
        BigMoneyButton,
        Recipe_Button,
        Cinema_Button,
        Island_Button,
        Shop_Button,
        StarterPack_Button,
        CleanPack_Button,
        UpgradePack_Button,
        GemPack_1_Button,
        GemPack_2_Button,
        GemPack_3_Button,
        Shop_Close_Button,
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
        CinemaGem_Accept_Button,
        CinemaRv_Accept_Button,
        NoThanks_button,
        PlayTimeReward_Accept_Button,
        PlayTimeReward_Close_Button,
    }
    enum GameObjects
    {
        IslandUi_Group,
        CinemaUI_Group,
        Shop_Panel,
        StarterPack,
        CleanPack,
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
        PlayTimeReward_Panel,
        Loading_Panel,
        AdBreak_Panel,
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
        , Shop_Button
        , CinemaGem_Accept_Button
        , StarterPack_Button
        , CleanPack_Button,
        GemPack_1_Button,
        GemPack_2_Button,
        GemPack_3_Button
        , UpgradePack_Button
        , Shop_Close_Button
        , PlayTimeReward_Button
        , PlayTimeReward_Accept_Button
        , NoThanks_button
        , PlayTimeReward_Close_Button
                ;
    //, NextStageButton;

    public Text Money_Text
        //, AddStaff_Price_Text
        //, Income_Price_Text
        , AddParts_Price_Text, Gem_Text, UpgradeCountText
        , RV_Income_TimeText
        , CinemaMoney_Text
        , CurrentPlayTime_Text
        , CinemaRvMoney_Text
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
        , Shop_Panel
        , StarterPack,
        CleanPack
        , PlayTimeReward_Panel
        , Loading_Panel
        , AdBreak_Panel
        ;

    public Image Mask, PlayTime_Guage;

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
        Shop_Button = GetButton(Buttons.Shop_Button);
        CinemaGem_Accept_Button = GetButton(Buttons.CinemaGem_Accept_Button);

        StarterPack_Button = GetButton(Buttons.StarterPack_Button);
        CleanPack_Button = GetButton(Buttons.CleanPack_Button);
        GemPack_1_Button = GetButton(Buttons.GemPack_1_Button);
        GemPack_2_Button = GetButton(Buttons.GemPack_2_Button);
        GemPack_3_Button = GetButton(Buttons.GemPack_3_Button);
        UpgradePack_Button = GetButton(Buttons.UpgradePack_Button);
        Shop_Close_Button = GetButton(Buttons.Shop_Close_Button);

        PlayTimeReward_Button = GetButton(Buttons.PlayTimeReward_Button);
        PlayTimeReward_Accept_Button = GetButton(Buttons.PlayTimeReward_Accept_Button);
        NoThanks_button = GetButton(Buttons.NoThanks_button);
        PlayTimeReward_Close_Button = GetButton(Buttons.PlayTimeReward_Close_Button);


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
        Shop_Panel = GetObject(GameObjects.Shop_Panel);

        StarterPack = GetObject(GameObjects.StarterPack);
        CleanPack = GetObject(GameObjects.CleanPack);

        PlayTimeReward_Panel = GetObject(GameObjects.PlayTimeReward_Panel);

        Loading_Panel = GetObject(GameObjects.Loading_Panel);
        AdBreak_Panel = GetObject(GameObjects.AdBreak_Panel);


        CurrentPlayTime_Text = GetText(Texts.CurrentPlayTime_Text);
        PlayTime_Guage = GetImage(Images.PlayTime_Guage);

        CinemaRvMoney_Text = GetText(Texts.CinemaRvMoney_Text);


        Mask = GetImage(Images.Mask);
        // ======================================

        // ===================================
        Mask.alphaHitTestMinimumThreshold = 0.5f;

        Loading_Panel.SetActive(true);


        AddParts_Upgrade_Button.AddButtonEvent(() =>
        {
            Managers.Game._stageManager.AddParts();
            if (TutorialManager._instance._tutorialLevel == 6)
            {
                TutorialManager._instance.Tutorial_Comple();
                //TutorialManager._instance.Tutorial();
                //Managers.GameUI.Cinema_Button.interactable = true;
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
                if (Managers.Game.Money < 5)
                {
                    Managers.Game.CalcMoney(5);
                }
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
            Cinema_Button.transform.GetChild(1).gameObject.SetActive(false);
            OffPopup();
            IslandUi_Group.SetActive(false);
            CinemaUI_Group.SetActive(true);
            Managers.Game._stageManager.isCinemaOn(true);
            if (TutorialManager._instance._tutorialLevel == 7)
            {
                TutorialManager._instance.Tutorial();
                //TutorialManager._instance.Tutorial_Comple();
                //StartCoroutine(Cor_Func());
            }

            //IEnumerator Cor_Func()
            //{
            //    yield return new WaitForSeconds(2f);
            //    TutorialManager._instance.Tutorial();

            //    //yield return new WaitForSeconds(5f);
            //    //TutorialManager._instance.Tutorial();
            //}


            EventTracker.LogCustomEvent("Cinema", new Dictionary<string, string> { { "Change", "To Cinema" } });
        });

        Island_Button.AddButtonEvent(() =>
        {
            OffPopup();
            IslandUi_Group.SetActive(true);
            CinemaUI_Group.SetActive(false);
            Managers.Game._stageManager.isCinemaOn(false);
            EventTracker.LogCustomEvent("Cinema", new Dictionary<string, string> { { "Change", "To Island" } });
        });

        Shop_Button.AddButtonEvent(() =>
        {

            // add max data check
            Shop_Panel.SetActive(true);
            Managers.Game._cinemaManager._joystick.isFix = true;
            if (Managers.Game._stageManager._noAds == 0)
            {
                CleanPack.SetActive(false);
                StarterPack.SetActive(true);
            }

            if (Managers.Game._cinemaManager._player._isBuyCleanPack)
            {
                CleanPack.SetActive(false);
                StarterPack.SetActive(false);
            }

            EventTracker.LogCustomEvent("RV_ShowCount", new Dictionary<string, string> { { "Rv_ShowPanel", "ShopPanel" } });

        });

        Shop_Close_Button.AddButtonEvent(() =>
        {
            Managers.Game._cinemaManager._joystick.isFix = false;
            Shop_Panel.SetActive(false);
        });
        PlayTimeReward_Button.AddButtonEvent(()
            => PlayTimeReward_Panel.SetActive(!PlayTimeReward_Panel.activeSelf));


        NoThanks_button.AddButtonEvent(() =>
        {
            Managers.Game._cinemaManager._joystick.isFix = false;
            CInemaRvPanel.SetActive(false);
        });

        PlayTimeReward_Close_Button.AddButtonEvent(() =>
        {
            Managers.Game._cinemaManager._joystick.isFix = false;
            PlayTimeReward_Panel.SetActive(false);
        });


        // rv

        RV_Income_Double.AddButtonEvent(() => AdsManager.ShowRewarded(() => Managers.Game._stageManager.RV_Income_Double()));
        BigMoneyButton.AddButtonEvent(() => AdsManager.ShowRewarded(() => Managers.Game._stageManager.RV_BigMoney()));



        BaseRoom_Button.AddButtonEvent(() =>
        {
            Managers.Game._cinemaManager.RoomUpgrade(1);
            RoomUpgrade_Panel.SetActive(false);

            EventTracker.LogCustomEvent("Cinema", new Dictionary<string, string> { { "RoomUpgradeType", "Basic" } });
        });
        PremiumRoom_Button.AddButtonEvent(() => AdsManager.ShowRewarded(() =>
        {
            Managers.Game._cinemaManager.RoomUpgrade(2);
            RoomUpgrade_Panel.SetActive(false);
            EventTracker.LogCustomEvent("Cinema", new Dictionary<string, string> { { "RoomUpgradeType", "Premium" } });
        }));


        CinemaGem_Accept_Button.AddButtonEvent(() =>
        {
            if (Managers.Game.CanUseGem(3))
            {
                Managers.Game._cinemaManager.CinemaRv();
                CInemaRvPanel.SetActive(false);
                EventTracker.LogCustomEvent("Cinema", new Dictionary<string, string> { { "RvPay", "Gem" } });
            }
            else
            {
                AdsManager.ShowRewarded(() =>
                {
                    Managers.Game._cinemaManager.CinemaRv();
                    CInemaRvPanel.SetActive(false);
                    EventTracker.LogCustomEvent("Cinema", new Dictionary<string, string> { { "RvPay", "Video" } });
                });
            }
        });
        CinemaRv_Accept_Button.AddButtonEvent(() => AdsManager.ShowRewarded(() =>
        {
            Managers.Game._cinemaManager.CinemaRv();
            CInemaRvPanel.SetActive(false);
        }));

        PlayTimeReward_Accept_Button.AddButtonEvent(() => Managers.Game._stageManager.GetPlayTimeReward());


        // == IAP , No Ads ===========================
        Restore_Button.AddButtonEvent(() => MondayOFF.IAPManager.RestorePurchase());

        NoAds_Button.AddButtonEvent(() => { OffPopup(); NoAds_Panel.SetActive(true); });


        NoAds_Purchase_Button.AddButtonEvent(() =>
        {
            MondayOFF.NoAds.Purchase();

        });
        NoAds.OnNoAds += () =>
        {
            Debug.Log("No Ads 구매 완료 ");
            NoAds_Panel.SetActive(false);
            NoAds_Button.gameObject.SetActive(false);
            Managers.Game._stageManager._noAds = 1;
        };



        StarterPack_Button.AddButtonEvent(() => MondayOFF.IAPManager.PurchaseProduct("popcorninc_starterpack"));

        CleanPack_Button.AddButtonEvent(() => MondayOFF.IAPManager.PurchaseProduct("popcorninc_cleanpack"));

        GemPack_1_Button.AddButtonEvent(() => MondayOFF.IAPManager.PurchaseProduct("popcorninc_gempack_1"));

        GemPack_2_Button.AddButtonEvent(() => MondayOFF.IAPManager.PurchaseProduct("popcorninc_gempack_2"));

        GemPack_3_Button.AddButtonEvent(() => MondayOFF.IAPManager.PurchaseProduct("popcorninc_gempack_3"));

        UpgradePack_Button.AddButtonEvent(() => MondayOFF.IAPManager.PurchaseProduct("popcorninc_upgradepack"));










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
        Managers.Game._cinemaManager._joystick.isFix = true;

        for (int i = 0; i < CInemaRvPanel.transform.GetChild(1).childCount; i++)
        {
            CInemaRvPanel.transform.GetChild(1).GetChild(i).gameObject.SetActive(false);

        }

        CInemaRvPanel.transform.GetChild(1).GetChild(_num).gameObject.SetActive(true);


        EventTracker.LogCustomEvent("RV_ShowCount", new Dictionary<string, string> { { "Rv_ShowPanel", ((RvInteract.RvType)_num).ToString() } });




    }





}







