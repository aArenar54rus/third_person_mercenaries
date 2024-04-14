using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;


namespace Arenar.SimpleYandexGames
{
    public class YandexGames : MonoBehaviour
    {
        private const int MIN_TIME_BETWEEN_INT = 60;
        private const string DEFAULT_NAME = "Unknown";


        public event Action<string> onPlayerNameLoaded;
        public event Action<string> onUniqueIdLoaded;
        public event Action<Texture2D> onIconSmallLoaded;
        public event Action<Texture2D> onIconMediumLoaded;
        public event Action<Texture2D> onIconLargeLoaded;
        public event Action<bool> onReviewScreenShown;
        public event Action onShowAdsComplete;

        private event Action<bool> onInterstitialShown;
        private event Action<bool> onRewardedShown;
        private event Action<bool> onBannerShown;


        [SerializeField] private bool isInitOnStart = true;
        [SerializeField] private float timeBeforeInit = 0.1f;
        [SerializeField] private bool isInterstitialOnStart = true;
        
        [Space(5)]
        [SerializeField, Range(MIN_TIME_BETWEEN_INT, float.MaxValue)] private float timeBetweenInterstitials = 60.0f;
        [SerializeField] private bool isDebug = false;

        [Space(5), Header("Language")]
        [SerializeField] private WebPageLocalization defaultLanguage;
        
        [Space(10), Header("Player options")]
        [SerializeField] private bool isLoadingPlayerData = false;
        [SerializeField] private bool isLoadingUniqueId = false;
        [SerializeField] private bool isLoadingName = false;
        [SerializeField] private bool isLoadingIconSmall = false;
        [SerializeField] private bool isLoadingIconMedium = false;
        [SerializeField] private bool isLoadingIconLarge = false;

        private Dictionary<PlayerIconSize, bool> isLoadingIcons
            = new Dictionary<PlayerIconSize, bool>();

        
        private bool isInitialized = false;
        private bool isInShowAdsProcess = false;
        private bool isYandexSdkInitialized = false;
        private bool isYandexPlayerInitialized = false;
        private bool isYandexPlayerLoadedFromWeb = false;
        private bool isIntOnStartShown = false;

        private float interstitialTimer = 0.0f;
        private YandexGamesPlayerData yandexPlayer;
        private Coroutine checkInitializationCoroutine;


        public bool IsInShowAdsProcess =>
            isInShowAdsProcess;

        public bool IsInterstitialReady =>
            interstitialTimer >= timeBetweenInterstitials;
        
        public WebPageLocalization CurrentLanguage
        {
            get;
            private set;
        }

        public bool IsPlayerLoaded =>
            yandexPlayer != null;

        public string UniqueId
        {
            get
            {
                if (!isLoadingPlayerData
                    || !isLoadingUniqueId)
                    return null;
                
                if (!IsPlayerLoaded
                    || yandexPlayer.uniqueId == "")
                    return null;

                return yandexPlayer.uniqueId;
            }
        }

        public string PlayerName
        {
            get
            {
                if (!isLoadingPlayerData
                    || !isLoadingName)
                    return DEFAULT_NAME;
                
                if (!IsPlayerLoaded
                    || yandexPlayer.playerName == "")
                    return DEFAULT_NAME;

                return yandexPlayer.playerName;
            }
        }
        
        
        [DllImport("__Internal")]
        private static extern void SetUniquePath(string path);
        
        [DllImport("__Internal")]
        private static extern void GetLanguageInternal(string methodName);

        [DllImport("__Internal")]
        private static extern void CheckYandexGamesSdkInitializeStatusInternal(string methodName);

        [DllImport("__Internal")]
        private static extern void CheckYandexGamesPlayerWebStatusInternal(string methodName);
        
        [DllImport("__Internal")]
        private static extern void ShowInterstitialInternal(string methodName, string errorMethodName);
        
        [DllImport("__Internal")]
        private static extern void ShowRewardedInternal(string methodNameSuccess, string methodNameFailed, string errorMethodName);
        
        [DllImport("__Internal")]
        private static extern void ShowBannerInternal(string methodName);
        
        [DllImport("__Internal")]
        private static extern void GetUniqueIdInternal(string methodName);
        
        [DllImport("__Internal")]
        private static extern void GetPlayerNameInternal(string methodName);

        [DllImport("__Internal")]
        private static extern void GetPlayerIconSmallInternal(string methodName);
        
        [DllImport("__Internal")]
        private static extern void GetPlayerIconMediumInternal(string methodName);
        
        [DllImport("__Internal")]
        private static extern void GetPlayerIconLargeInternal(string methodName);
        
        [DllImport("__Internal")]
        private static extern void SendReviewInternal(string methodName, string failMethodName);


        public void Initialize()
        {
            #if UNITY_EDITOR
            isDebug = true;
            #endif

            if (timeBetweenInterstitials < MIN_TIME_BETWEEN_INT)
                timeBetweenInterstitials = MIN_TIME_BETWEEN_INT;

            interstitialTimer = timeBetweenInterstitials;
            isLoadingIcons = new Dictionary<PlayerIconSize, bool>();
            isLoadingIcons.Add(PlayerIconSize.SmallIcon, isLoadingIconSmall);
            isLoadingIcons.Add(PlayerIconSize.MediumIcon, isLoadingIconMedium);
            isLoadingIcons.Add(PlayerIconSize.LargeIcon, isLoadingIconLarge);
            
            isInShowAdsProcess = false;
            isInitialized = true;
        }

        public void ShowInterstitial(Action<bool> action = null)
        {
            if (isDebug)
            {
                action?.Invoke(true);
                return;
            }
            
            if (!IsInterstitialReady)
            {
                action?.Invoke(true);
                return;
            }
            
            interstitialTimer = 0;
            isInShowAdsProcess = true;
            onInterstitialShown += action;
            ShowInterstitialInternal("OnInterstitialShown", "OnInterstitialError");
        }
        
        public void ShowRewarded(Action<bool> action = null, string placement = "")
        {
            if (isDebug)
            {
                action?.Invoke(true);
                return;
            }
            
            isInShowAdsProcess = true;
            onRewardedShown += action;
            ShowRewardedInternal("OnRewardedShownSuccess", "OnRewardedShownFailed", "OnRewardedError");
        }

        public void ShowBanner(Action<bool> action = null)
        {
            if (isDebug)
            {
                action?.Invoke(true);
                return;
            }
            
            onBannerShown += action;
            ShowBannerInternal("OnBannerShown");
        }

        public void GetLanguageFromPage()
        {
            GetLanguageInternal("OnLanguageLoaded");
        }

        public void SendReview()
        {
            SendReviewInternal("OnReviewScreenShown", "OnReviewScreenFail");
        }

        public Texture2D GetIconBySize(PlayerIconSize size)
        {
            if (!isLoadingPlayerData
                || !isLoadingIcons[size])
                return null;
            
            if (!IsPlayerLoaded
                || yandexPlayer.icons[size] == null)
                return null;

            return yandexPlayer.icons[size];
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnYandexSdkStatusInit()
        {
            isYandexSdkInitialized = true;
            LoadingPlayer();
            ShowBanner();
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnYandexPlayerStatusUpdate(int statusValue)
        {
            isYandexPlayerLoadedFromWeb = statusValue > 0;
            if (!isYandexPlayerLoadedFromWeb)
                return;

            yandexPlayer ??= new YandexGamesPlayerData();
            
            if (isLoadingName)
                GetPlayerNameInternal("OnPlayerNameLoaded");

            if (isLoadingUniqueId)
                GetUniqueIdInternal("OnUniqueIdLoaded");
                
            foreach (var isLoadingIcon in isLoadingIcons)
            {
                if (!isLoadingIcon.Value)
                    continue;

                /*string iconHeight = isLoadingIcon.Key switch
                {
                    PlayerIconSize.SmallIcon => SMALL_ICON_NAME,
                    PlayerIconSize.MediumIcon => MEDIUM_ICON_NAME,
                    PlayerIconSize.LargeIcon => LARGE_ICON_NAME,
                    _ => ""
                };*/

                switch (isLoadingIcon.Key)
                {
                    case PlayerIconSize.SmallIcon:
                        GetPlayerIconSmallInternal("OnPlayerIconSmallLoaded");
                        break;
                    
                    case PlayerIconSize.MediumIcon:
                        GetPlayerIconMediumInternal("OnPlayerIconMediumLoaded");
                        break;
                    
                    case PlayerIconSize.LargeIcon:
                        GetPlayerIconLargeInternal("OnPlayerIconLargeLoaded");
                        break;
                }

                //GetPlayerIconInternal("OnPlayerIconLoaded", iconHeight);
            }
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnInterstitialShown()
        {
            interstitialTimer = 0;
            isInShowAdsProcess = false;
            onShowAdsComplete?.Invoke();
            onInterstitialShown?.Invoke(true);
            onInterstitialShown = null;
        }
        
        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnReviewScreenShown(bool feedback)
        {
            Debug.LogError("OnReviewScreenShown TEST : " + feedback);
            onReviewScreenShown?.Invoke(feedback);
        }
        
        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnReviewScreenFail(string reason)
        {
            Debug.LogError(reason);
        }
        
        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnInterstitialError(string error)
        {
            interstitialTimer = 0;
            isInShowAdsProcess = false;
            onShowAdsComplete?.Invoke();
            onInterstitialShown?.Invoke(false);
            onInterstitialShown = null;
            Debug.LogError($"Interstitial ad error: {error}");
        }
        
        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnRewardedShownSuccess()
        {
            isInShowAdsProcess = false;
            onShowAdsComplete?.Invoke();
            onRewardedShown?.Invoke(true);
            onRewardedShown = null;
        }
        
        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnRewardedShownFailed()
        {
            isInShowAdsProcess = false;
            onShowAdsComplete?.Invoke();
            onRewardedShown?.Invoke(false);
            onRewardedShown = null;
        }
        
        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnRewardedError(string error)
        {
            isInShowAdsProcess = false;
            onShowAdsComplete?.Invoke();
            onRewardedShown?.Invoke(false);
            onRewardedShown = null;
            Debug.LogError($"Rewarded ad error: {error}");
        }
        
        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnBannerShown()
        {
            onBannerShown?.Invoke(true);
            onBannerShown = null;
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnUniqueIdLoaded(string uniqueId)
        {
            yandexPlayer.uniqueId = uniqueId;
            onUniqueIdLoaded?.Invoke(uniqueId);
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnPlayerNameLoaded(string nickname)
        {
            yandexPlayer.playerName = nickname;
            onPlayerNameLoaded?.Invoke(nickname);
        }

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnPlayerIconSmallLoaded(string mediaUrl) =>
            StartCoroutine(CompleteIconLoading(mediaUrl, PlayerIconSize.SmallIcon, onIconSmallLoaded));

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnPlayerIconMediumLoaded(string mediaUrl) =>
            StartCoroutine(CompleteIconLoading(mediaUrl, PlayerIconSize.MediumIcon, onIconMediumLoaded));

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnPlayerIconLargeLoaded(string mediaUrl) =>
            StartCoroutine(CompleteIconLoading(mediaUrl, PlayerIconSize.LargeIcon, onIconLargeLoaded));

        /// <summary>
        /// Callback from index.html
        /// </summary>
        public void OnLanguageLoaded(string languageKey)
        {
            JSONNode json = JSON.Parse(languageKey);
            string resultLanguage = json["i18n"]["lang"];

            switch (resultLanguage)
            {
                case "ru":
                    CurrentLanguage = WebPageLocalization.Russian;
                    break;
                
                case "en":
                    CurrentLanguage = WebPageLocalization.English;
                    break;
                
                case "tr":
                    CurrentLanguage = WebPageLocalization.Turkish;
                    break;
                
                default:
                    CurrentLanguage = defaultLanguage;
                    break;
            }
        }

        private void Start()
        {
            SetFullPathToObject();
            GetLanguageFromPage();
            
            if (isInitOnStart)
                Initialize();
            
            if (timeBeforeYandexSDKInitCurrent < timeBeforeInit)
                timeBeforeYandexSDKInitCurrent = timeBeforeInit;
        }

        float timeBeforeYandexSDKInitCurrent = 0;
        float timeBeforePlayerInitCurrent = 0;

        private void Update()
        {
            if (!isInitialized)
                return;
            
            if (!isYandexSdkInitialized)
            {
                if (timeBeforeYandexSDKInitCurrent >= timeBeforeInit)
                {
                    CheckYandexGamesSdkInitializeStatus();
                    timeBeforeYandexSDKInitCurrent = 0;
                }
                else
                {
                    timeBeforeYandexSDKInitCurrent += Time.deltaTime;
                }

                return;
            }

            if (isLoadingPlayerData && !isYandexPlayerLoadedFromWeb)
            {
                if (timeBeforePlayerInitCurrent >= timeBeforeInit)
                {
                    LoadingPlayer();
                    timeBeforePlayerInitCurrent = 0;
                }
                else
                {
                    timeBeforePlayerInitCurrent += Time.deltaTime;
                }
            }
            
            if (isInterstitialOnStart && !isIntOnStartShown)
            {
                ShowInterstitial();
                isYandexSdkInitialized = true;
            }
            
            interstitialTimer += Time.deltaTime;
        }

        private void LoadingPlayer()
        {
            if (!isLoadingPlayerData)
                return;
            
            CheckYandexGamesPlayerWebStatusInternal("OnYandexPlayerStatusUpdate");
        }

        private void CheckYandexGamesSdkInitializeStatus()
        {
            CheckYandexGamesSdkInitializeStatusInternal("OnYandexSdkStatusInit");
        }

        private IEnumerator CompleteIconLoading(string mediaUrl, PlayerIconSize size, Action<Texture2D> action)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError
                || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Connect error: {request.error}");
            }
            else
            {
                yandexPlayer.icons[size] = ((DownloadHandlerTexture) request.downloadHandler).texture;
            }
            
            action?.Invoke(yandexPlayer.icons[size]);
        }

        private void SetFullPathToObject()
        {
            Transform transform = this.gameObject.transform;
            string path = "/" + transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + path;
                
                if (transform.parent != null)
                    path = "/" + path;
            }
            SetUniquePath(path);
        }

        private void OnDestroy()
        {
            if (checkInitializationCoroutine != null)
                StopCoroutine(checkInitializationCoroutine);
        }
    }
}