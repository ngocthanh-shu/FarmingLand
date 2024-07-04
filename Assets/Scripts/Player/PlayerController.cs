public class PlayerController
{
    private readonly PlayerView _playerView;
    private readonly PlayerModel _playerModel;

    public PlayerController(PlayerInput playerInput)
    {
        PlayerView playerView = (PlayerView) ViewManager.Instance.GetView(ViewType.PlayerView);
        
        _playerView = playerView;

        _playerView.Initialize(new PlayerViewData(playerInput));

        PlayerDataScriptable playerDataScriptable = (PlayerDataScriptable)
            ScriptableObjectManager.Instance.GetScriptableObject(ScriptableType.PlayerData);
        
        _playerModel = new PlayerModel(playerDataScriptable);
        _playerView.SetPlayerRunSpeed(_playerModel.Speed);
    }

    public void OnFixUpdate()
    {
        _playerView.OnFixUpdate();
    }
}
