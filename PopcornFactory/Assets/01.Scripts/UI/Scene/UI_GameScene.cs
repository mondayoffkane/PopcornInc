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
        AddStaff_Upgrade_Button,
        Income_Upgrade_Button,
        Sound_Button,
        Vibe_Button,
    }
    enum GameObjects
    {
        Setting_Panel,
        Scroll_Panel,
        Content,
        Worker_Hire,
        Worker_Speed,
        Upg1_Income,
        Upg1_Speed,
        Upg2_Income,
        Upg2_Speed,
        Upg3_Income,
        Upg3_Speed,
    }


    public Button AddStaff_Upgrade_Button, Income_Upgrade_Button, AddParts_Upgrade_Button,
        Sound_Button, Vibe_Button, AddUpgrade_Button, RV_Income_Double;

    public Text Money_Text
       , AddStaff_Price_Text, Income_Price_Text, AddParts_Price_Text, Gem_Text, UpgradeCountText;

    public GameObject Setting_Panel, Scroll_Panel,
        Worker_Hire,
        Worker_Speed,
        Upg1_Income,
        Upg1_Speed,
        Upg2_Income,
        Upg2_Speed,
        Upg3_Income,
        Upg3_Speed;


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




        Money_Text = GetText(Texts.Money_Text);
        AddStaff_Price_Text = GetText(Texts.AddStaff_Price_Text);
        Income_Price_Text = GetText(Texts.Income_Price_Text);
        AddParts_Price_Text = GetText(Texts.AddParts_Price_Text);
        Gem_Text = GetText(Texts.Gem_Text);
        UpgradeCountText = GetText(Texts.UpgradeCountText);




        Setting_Panel = GetObject(GameObjects.Setting_Panel);
        Scroll_Panel = GetObject(GameObjects.Scroll_Panel);

        Worker_Hire = GetObject(GameObjects.Worker_Hire);
        Worker_Speed = GetObject(GameObjects.Worker_Speed);
        Upg1_Income = GetObject(GameObjects.Upg1_Income);
        Upg1_Speed = GetObject(GameObjects.Upg1_Speed);
        Upg2_Income = GetObject(GameObjects.Upg2_Income);
        Upg2_Speed = GetObject(GameObjects.Upg2_Speed);
        Upg3_Income = GetObject(GameObjects.Upg3_Income);
        Upg3_Speed = GetObject(GameObjects.Upg3_Speed);



        // ======================================
        //AddStaff_Upgrade_Button.AddButtonEvent(() => Managers.Game._stageManager.AddStaff());
        //Income_Upgrade_Button.AddButtonEvent(() => Managers.Game._stageManager.AddIncome());
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

        AddUpgrade_Button.AddButtonEvent(() => Scroll_Panel.SetActive(!Scroll_Panel.activeSelf));

        Worker_Hire.transform.GetChild(3).GetComponent<Button>().AddButtonEvent(() => Managers.Game._stageManager.UpgradeType(0));
        Worker_Speed.transform.GetChild(3).GetComponent<Button>().AddButtonEvent(() => Managers.Game._stageManager.UpgradeType(1));
        Upg1_Income.transform.GetChild(3).GetComponent<Button>().AddButtonEvent(() => Managers.Game._stageManager.UpgradeType(2));
        Upg1_Speed.transform.GetChild(3).GetComponent<Button>().AddButtonEvent(() => Managers.Game._stageManager.UpgradeType(3));
        Upg2_Income.transform.GetChild(3).GetComponent<Button>().AddButtonEvent(() => Managers.Game._stageManager.UpgradeType(4));
        Upg2_Speed.transform.GetChild(3).GetComponent<Button>().AddButtonEvent(() => Managers.Game._stageManager.UpgradeType(5));
        Upg3_Income.transform.GetChild(3).GetComponent<Button>().AddButtonEvent(() => Managers.Game._stageManager.UpgradeType(6));
        Upg3_Speed.transform.GetChild(3).GetComponent<Button>().AddButtonEvent(() => Managers.Game._stageManager.UpgradeType(7));



        // rv

        RV_Income_Double.AddButtonEvent(() => AdsManager.ShowRewarded(() => Managers.Game._stageManager.RV_Income_Double()));

    } /// ========= end Set buttons







}
