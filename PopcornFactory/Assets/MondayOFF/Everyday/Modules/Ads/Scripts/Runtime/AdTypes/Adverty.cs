using System;
using UnityEngine;
using Adverty;

namespace MondayOFF
{
    internal class Adverty : IDisposable
    {
        internal Adverty(in Camera mainCamera)
        {

            EverydayLogger.Info("Initializing Adverty");
            UserData userData = new UserData(AgeSegment.Unknown, Gender.Unknown);
            //Debug.Log("Test1");
            AdvertySDK.Init(EverydaySettings.AdSettings.advertyApiKey, AdvertySettings.Mode.Mobile, !AdsManager.HAS_USER_CONSENT, userData);
            //Debug.Log("Test2");
            AdvertySettings.SetMainCamera(mainCamera);

            // AdvertySettings.SandboxMode = false;
        }

        public void Dispose()
        {
            EverydayLogger.Info("Terminating Adverty");
            AdvertySDK.Terminate();
        }
    }
}