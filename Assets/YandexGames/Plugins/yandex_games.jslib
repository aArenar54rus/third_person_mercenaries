mergeInto(LibraryManager.library, {

  SetUniquePath: function (path) {
    this.uniquePath = UTF8ToString(path); 
    console.log(this.uniquePath);
  },

  GetLanguageInternal: function (methodName) {
    var methodNameStr = UTF8ToString(methodName);
      
    var data = ysdk.environment;
    myUnityInstance.SendMessage(this.uniquePath, methodNameStr, JSON.stringify(data));
  },

  CheckYandexGamesSdkInitializeStatusInternal: function (methodName) {
    if (isYandexSdkInitialized > 0){
        var methodNameStr = UTF8ToString(methodName);
        myUnityInstance.SendMessage(this.uniquePath, methodNameStr);
    }
  },
  
  CheckYandexGamesPlayerWebStatusInternal: function (methodName) {
     var methodNameStr = UTF8ToString(methodName);
     console.log(this.uniquePath + "   " + methodNameStr + "   result: " + isPlayerInitialized);
     
     myUnityInstance.SendMessage(this.uniquePath, methodNameStr, isPlayerInitialized);
   },

  ShowInterstitialInternal: function (methodNameSuccess, errorMethodName) {
    var methodNameSuccessStr = UTF8ToString(methodNameSuccess);
    var errorMethodNameStr = UTF8ToString(errorMethodName);

    ysdk.adv.showFullscreenAdv({
        callbacks: {
            onClose: function(wasShown) {
              myUnityInstance.SendMessage(this.uniquePath, methodNameSuccessStr);
            },
            onError: function(error) {
              myUnityInstance.SendMessage(this.uniquePath, errorMethodNameStr, error);
            }
        }
    })
  },

  ShowRewardedInternal: function (methodNameSuccess, methodNameFailed, errorMethodName) {
    var methodNameSuccessStr = UTF8ToString(methodNameSuccess);
    var methodNameFailedStr = UTF8ToString(methodNameFailed);
    var errorMethodNameStr = UTF8ToString(errorMethodName);
    var isSuccessInt = 0;
    
    ysdk.adv.showRewardedVideo({
        callbacks: {
            onOpen: () => {
            },
            onRewarded: () => {
              isSuccessInt = 1;
            },
            onClose: () => {           
              if (isSuccessInt > 0) {
                myUnityInstance.SendMessage(this.uniquePath, methodNameSuccessStr);
              }
              else {
                myUnityInstance.SendMessage(this.uniquePath, methodNameFailedStr);
              }
            }, 
            onError: (e) => {
              myUnityInstance.SendMessage(this.uniquePath, errorMethodNameStr, error);
            }
        }
    })
  },

  ShowBannerInternal: function (methodName) {
    var methodNameStr = UTF8ToString(methodName);        
    ysdk.adv.getBannerAdvStatus().then(({ stickyAdvIsShowing , reason }) => {
        if (stickyAdvIsShowing) {
            myUnityInstance.SendMessage(this.uniquePath, methodNameStr);
        } else if(reason) {
            console.log(reason)
        } else {
            ysdk.adv.showBannerAdv()
            myUnityInstance.SendMessage(this.uniquePath, methodNameStr);
        }
    })
  },

  SendReviewInternal: function (methodName, failMethodName) {
    var methodNameStr = UTF8ToString(methodName);
    var failMethodNameStr = UTF8ToString(failMethodName);

    ysdk.feedback.canReview()
        .then(({ value, reason }) => {
            if (value) {
                ysdk.feedback.requestReview()
                    .then(({ feedbackSent }) => {
                        myUnityInstance.SendMessage(this.uniquePath, methodNameStr, feedbackSent);
                    })
            } else {
                myUnityInstance.SendMessage(this.uniquePath, failMethodNameStr, reason);
            }
        })
  },

  GetUniqueIdInternal: function (methodName) {
    var methodNameStr = UTF8ToString(methodName);
    var uniqueId = player.getUniqueID();
    myUnityInstance.SendMessage(this.uniquePath, methodNameStr, uniqueId);
  },

  GetPlayerNameInternal: function (methodName) {
    var methodNameStr = UTF8ToString(methodName);
    var playerName = player.getName();
    myUnityInstance.SendMessage(this.uniquePath, methodNameStr, playerName);
  },
  
  GetPlayerIconSmallInternal: function(methodName){
    var methodNameStr = UTF8ToString(methodName);
    myUnityInstance.SendMessage(this.uniquePath, methodNameStr, player.getPhoto('small'));
  },
  
  GetPlayerIconMediumInternal: function(methodName){
    var methodNameStr = UTF8ToString(methodName);
    myUnityInstance.SendMessage(this.uniquePath, methodNameStr, player.getPhoto('medium'));
  },
  
  GetPlayerIconLargeInternal: function(methodName){
    var methodNameStr = UTF8ToString(methodName);
    myUnityInstance.SendMessage(this.uniquePath, methodNameStr, player.getPhoto('large'));
  },
});