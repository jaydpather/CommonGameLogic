using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThirdEyeSoftware.GameLogic
{
    public class Constants
    {
        public static class URLs
        {
            public const string PrivacyPolicy = "http://rebelsoftware.nl/userDocs/AsteroidFieldNavigation/privacyPolicy.html";
            public const string EULA = "http://rebelsoftware.nl/userDocs/AsteroidFieldNavigation/eula.html";
            public const string Share = "http://rebelsoftware.nl/Games";
        }

        public static class EmailAddresses
        {
            public const string SupportEmail = "fakeEmail@fake.com";
        }
        
        public static class SceneNames
        {
            public const string UIScene = "UIScene";
            public const string GameScene = "GameScene";
            public const string LoadingScene = "LoadingScene";
        }

        public static class ProductNames
        {
            public const string BuyLivesSmall = "buy_lives_small";
            public const string BuyLivesMedium = "buy_lives_medium";
            public const string BuyLivesLarge = "buy_lives_large";
        }

        public static class LivesPerProduct
        {
            public const int Small = 10;
            public const int Medium = 30;
            public const int Large = 55;
        }

        public static class AppStoreNames
        {
            public const string GooglePlay = "GooglePlay";
        }

        public static class SavedDataKeys
        {
            public const string ShouldDisplayAds = "ShouldDisplayAds";
            public const string NumLivesRemaining = "NumLivesRemaining";
            public const string CurLevel = "CurLevel";
            public const string CurSubLevel = "CurSubLevel";
            public const string HasBeatGameStatus = "HasBeatGameStatus";
            public const string IsEULAAccepted = "IsEULAAccepted";
        }

        public static class DefaultDataKeyValues
        {
            public const int NumLivesRemaining = 10;
        }

        public static class Levels
        {
            public const int NumLevels = 5;
            public const int NumSubLevels = 3;
        }

        public static class UIMessages
        {
            public const string BeatLevel = @"You have cleared the asteroid field.";

            public const string ProgressSaved = "Your progress is saved.";

            public const string BeatGame = @"You beat the last level!

Thanks for playing.";
        }
    }
}