using Godot;
using System;

public partial class PointsCounter : Node
{
    [Signal]
    public delegate void PointsIncreasedEventHandler(int points);
    private int Points = 0;
    private InvaderSpawn invaderSpawn;

    public override void _Ready()
    {
        invaderSpawn = GetNode<InvaderSpawn>("../InvaderSpawn");
        if (invaderSpawn == null)
        {
            GD.PrintErr("InvaderSpawn não encontrado!");
            return;
        }
        invaderSpawn.InvaderDestroyed += IncreasePoints;
    }

    private void IncreasePoints(int points_to_add)
    {
        Points += points_to_add;
        EmitSignal(nameof(PointsIncreased), Points);
        GD.Print($"Points increased: {Points}");
    }
}
