using Godot;
using System;

public partial class Bunker_park : Area2D
{
    [Export]
    public Sprite2D sprite;
    [Export]
    public Godot.Collections.Array<Texture2D> textures_array;
    private int damage = 0;
    private const int maxdamage = 3;

    public override void _Ready()
    {
        this.AreaEntered += EntreredBunker;
    }

    public void EntreredBunker(Area2D area)
    {
        if (area is Laser || area is InvaderShoot)
        {
            area.QueueFree();
            if (damage < maxdamage)
            {
                damage++;
                sprite.Texture = textures_array[damage - 1];
                //GD.Print("Bunker hit by laser!");
            }
            else
            {
                //GD.Print("Bunker destroyed!");
                QueueFree();
            }
        }

        if (area is Invader)
        {
            area.QueueFree();
            if (area is Invader invader)
            {
                invader.DestroyInvader(0);
            }
            //GD.Print("Bunker hit by Invader!");
        }
    }
}
