using Godot;
using System;
using System.Linq;

public partial class Bunker_park : Node2D
{
    [Export]
    public Sprite2D sprite;
    [Export]
    public Godot.Collections.Array<Texture2D> textures_array;

    private int damage = 0;
    private const int maxdamage = 3;

    [Signal]
    public delegate void BunkerDestroyedEventHandler();
    private Area2D[] bunkerParts;

    public override void _Ready()
    {
        // Obter todas as partes do bunker no grupo "BunkerParts"
        bunkerParts = GetTree().GetNodesInGroup("BunkerParts").OfType<Area2D>().ToArray();

        foreach (var part in bunkerParts)
        {
            GD.Print($"Conectando parte do bunker: {part.Name}");
            part.Connect("area_entered", new Callable(this, nameof(OnPartDamaged)));
        }
    }

    private void OnPartDamaged(Area2D area)
    {
        GD.Print($"Colisão detectada com: {area.Name}");

        if (area is Laser || area is InvaderShoot)
        {
            HandleProjectileCollision(area);
        }
        else if (area is Invader invader)
        {
            HandleInvaderCollision(invader);
        }
    }

    private void HandleProjectileCollision(Area2D projectile)
    {
        if (damage < maxdamage)
        {
            damage++;
            sprite.Texture = textures_array[damage - 1];
            GD.Print($"Bunker atingido! Dano atual: {damage}/{maxdamage}");
        }
        else
        {
            GD.Print("Parte do bunker destruída!");
            RemovePart(projectile.GetParent<Area2D>());
        }

        projectile.QueueFree(); // Remover o tiro após processar a colisão
    }

    private void HandleInvaderCollision(Invader invader)
    {
        invader.DestroyInvader(0);
        GD.Print("Parte do bunker atingida por um invasor!");
        RemovePart(invader.GetParent<Area2D>());
    }

    private void RemovePart(Area2D part)
    {
        if (bunkerParts.Contains(part))
        {
            bunkerParts = bunkerParts.Where(p => p != part).ToArray();
            part.QueueFree();
        }

        // Verificar se todas as partes foram destruídas
        if (bunkerParts.Length == 0)
        {
            GD.Print("Todas as partes do bunker foram destruídas!");
            EmitSignal(nameof(BunkerDestroyed)); // Emite o sinal de destruição do bunker
        }
    }
}
