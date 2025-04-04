using Godot;
using System;

public partial class UfoSpawner : Node2D
{
    private Timer Spawn_Timer;
    [Export]
    public PackedScene UfoScene;

    public override void _Ready()
    {
        GD.Randomize();
        Spawn_Timer = GetNode<Timer>("SpawnTimer");
        (Spawn_Timer as SpawnTimer)?.setup_timer();
        Spawn_Timer.Timeout += SpawnTimeout;

    }

    public void SpawnTimeout()
    {
        GD.Print($"Timeout! Tempo atual do SpawnTimer: {Spawn_Timer.WaitTime} segundos");
        var ufo = UfoScene.Instantiate<Ufo>();
        ufo.GlobalPosition = Position;
        GetTree().Root.AddChild(ufo);
    }
}
