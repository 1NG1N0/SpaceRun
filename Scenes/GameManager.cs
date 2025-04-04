using Godot;
using System;

public partial class GameManager : Node
{
    private SpawnTimer spawnTimer;

    public void SetSpawnTimer(SpawnTimer timer)
    {
        spawnTimer = timer;
    }

    public override void _Ready()
    {
        var spawnTimerScene = GD.Load<PackedScene>("res://Scenes/spawn_timer.tscn");
        spawnTimer = spawnTimerScene.Instantiate<SpawnTimer>();
        AddChild(spawnTimer); // Adiciona o SpawnTimer como filho do GameManager
    }

    public void OnGameOver()
    {
        GD.Print("Game Over! Pausando SpawnTimer.");
        spawnTimer?.OnGameOver(); // Verifica se o spawnTimer não é nulo
    }

    public void OnRestartGame()
    {
        GD.Print("Reiniciando o jogo! Resetando SpawnTimer.");
        spawnTimer?.OnRestartGame(); // Verifica se o spawnTimer não é nulo
    }
}
