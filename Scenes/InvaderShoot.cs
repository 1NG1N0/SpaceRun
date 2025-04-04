using Godot;
using System;

public partial class InvaderShoot : Area2D
{
    [Export]
    public int Cspeed = 200;
    private VisibleOnScreenNotifier2D visibleNotifier;


    public override void _Ready()
    {
        // Inicializa o nó VisibleOnScreenNotifier2D
        visibleNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        visibleNotifier.ScreenExited += _on_VisibleOnScreenNotifier2D_screen_exited;
        this.AreaEntered += OnAreaEntered;
    }
    public override void _Process(double delta)
    {
        Position += new Vector2(0, Cspeed * (float)delta);
    }

    public void _on_VisibleOnScreenNotifier2D_screen_exited()
    {
        QueueFree();
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area is Player player)
        {
            player.On_player_destroyed();
            QueueFree();
        }
    }
}
