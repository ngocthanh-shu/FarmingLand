public class PlayerModel
{
    public float Speed { get; set; }

    public PlayerModel(PlayerDataScriptable data)
    {
        Speed = data.speed;
    }
}
