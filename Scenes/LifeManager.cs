using Godot;
using System;

public partial class LifeManager : Node
{
    [Signal]
    public delegate void OnLifeLostEventHandler(int lifesLeft);

    [Export]
    public int lives = 3;

    private Player player;
    private readonly PackedScene playerScene = GD.Load<PackedScene>("res://Scenes/player.tscn");
    private CenterContainer gameoverContainer;
    private Button restartButton;
    private InvaderSpawn invaderSpawn;

    public override void _Ready()
    {
        GD.Print($"Vidas iniciais no LifeManager: {lives}");

        // Obtém o nó Player
        player = GetNode<Player>("../Player");
        gameoverContainer = GetNode<CenterContainer>("../GameBoxContainer");
        restartButton = GetNode<Button>("../GameBoxContainer/GameOverBox/RestartButton");
        invaderSpawn = GetNode<InvaderSpawn>("../InvaderSpawn");

        // Conecta o sinal PlayerDestroyed ao método OnPlayerDestroyed
        player.PlayerDestroyed += OnPlayerDestroyed;
        restartButton.Pressed += OnRestartButtonPressed;
    }

    public void OnPlayerDestroyed()
    {
        lives -= 1;
        GD.Print($"Emitindo sinal OnLifeLost com vidas restantes: {lives}");
        EmitSignal(nameof(OnLifeLost), lives);

        if (lives > 0)
        {
            var player = playerScene.Instantiate<Player>();
            player.GlobalPosition = new Vector2(0, 302);
            player.Connect(nameof(Player.PlayerDestroyed), new Callable(this, nameof(OnPlayerDestroyed)));
            GetTree().Root.GetNode("Node2D").AddChild(player);
        }
        else
        {
            GD.Print("Game Over!");
            
            gameoverContainer.Visible = true;

        }
    }

    public void OnRestartButtonPressed()
    {
        GD.Print("Reiniciando o jogo...");
        GetTree().ReloadCurrentScene();
    }
}
