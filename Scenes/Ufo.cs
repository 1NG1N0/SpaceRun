using Godot;
using System;
using System.Threading.Tasks;

public partial class Ufo : Area2D
{
    [Export]
    public int uspeed = 300;
    private Sprite2D sprite;
    private VisibleOnScreenNotifier2D visibleNotifier;
    private Node2D Shootingpoint;
    private readonly Texture2D explosion = GD.Load<Texture2D>("res://Assets/UFO/Ufo-explosion.png");

    public override void _Ready()
    {
        visibleNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        Shootingpoint = GetNode<Node2D>("ShootPonit");
        sprite = GetNode<Sprite2D>("Sprite2D");
        visibleNotifier.ScreenExited += exitedvisible;

        this.AreaEntered += EntreredUfo;
    }

    public override void _Process(double delta)
    {
        Position -= new Vector2(uspeed * (float)delta, 0);
    }

    public void exitedvisible()
    {
        QueueFree();
    }

    public async void EntreredUfo(Area2D area)
    {
        if (area is Laser)
        {
            Shootingpoint.QueueFree();
            uspeed = 0;
            sprite.Texture = explosion;
            GD.Print("UFO hit by laser!");
            area.QueueFree();
            var timer = GetTree().CreateTimer(1.5f);
            await ToSignal(timer, "timeout");
            QueueFree();
        }
    }
}
