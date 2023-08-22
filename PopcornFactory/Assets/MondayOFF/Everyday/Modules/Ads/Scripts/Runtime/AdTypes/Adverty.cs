using System;
using UnityEngine;
using Adverty;

namespace MondayOFF
{
    internal class Adverty : IDisposable
    {
        internal Adverty(in Camera mainCamera)
        {
            Debug.Log("test adverty 1");
            EverydayLogger.Info("Initializing Adverty");
            UserData userData = new UserData(AgeSegment.Unknown, Gender.Unknown);
            Debug.Log("test adverty 2");
            AdvertySDK.Init(EverydaySettings.AdSettings.advertyApiKey, AdvertySettings.Mode.Mobile, !AdsManager.HAS_USER_CONSENT, userData);
            Debug.Log("test adverty 3");
            AdvertySettings.SetMainCamera(mainCamera);
            Debug.Log("test adverty 4");
            // AdvertySettings.SandboxMode = false;
        }

        public void Dispose()
        {
            EverydayLogger.Info("Terminating Adverty");
            AdvertySDK.Terminate();
        }
    }
}