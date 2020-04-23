
namespace ThirdEyeSoftware.GameLogic
{
    public enum InputAxis
    {
        Cancel = 1, //used for pausing. We get an exception if we change the name of this input axis to "Pause" in Unity though.
        Horizontal = 2,
    }


    public enum GameState
    {
        InGame = 0,
        Win = 2,
        Lose = 3,
        Pause = 4,
        Ad = 5,
        LoseTransition = 6,
        WinTransition = 7,
    }

    public enum MenuState
    {
        InMenu = 1,
        Loading = 2,
        Purchase = 3,
        OutOfLives = 4,
        ChooseLevel = 5,
        GetMoreLives = 6,
        HowToPlay = 7,
        EULA = 8,
    }

    public enum AppState
    {
        MainMenu = 1,
        Game = 2
    }
}