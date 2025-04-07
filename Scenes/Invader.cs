using Godot;
using System;

public partial class Invader : Area2D
{
    private Sprite2D Sprite2D;
    private AnimationPlayer animationPlayer;
    private invader_config config;
    [Signal]
    public delegate void InvaderdestruidoEventHandler(int points);

    public override void _Ready()
    {
        // Inicializa os nós
        Sprite2D = GetNode<Sprite2D>("Sprite2D");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        if (Sprite2D == null)
        {
            GD.PrintErr("Sprite2D node not found in Invader.tscn!");
        }

        if (animationPlayer == null)
        {
            GD.PrintErr("AnimationPlayer node not found in Invader.tscn!");
        }
        this.AreaEntered += OnAreaEntered;
    }

    public void SetConfig(invader_config newConfig)
    {
        config = newConfig;

        if (Sprite2D != null && config != null)
        {
            // Configura o sprite
            Sprite2D.Texture = config.Sprite_1;
            GD.Print($"Invader configured with sprite: {config.Sprite_1}");

            // Reproduz a animação
            if (animationPlayer != null && !string.IsNullOrEmpty(config.animation_name))
            {
                if (animationPlayer.HasAnimation(config.animation_name))
                {
                    animationPlayer.Play(config.animation_name);
                    GD.Print($"Playing animation: {config.animation_name}");
                    animationPlayer.AnimationFinished += OnAnimationFinished;
                }
                else
                {
                    GD.PrintErr($"Animation '{config.animation_name}' not found in AnimationPlayer.");
                }
            }
            else
            {
                GD.PrintErr("AnimationPlayer is null or animation_name is invalid.");
            }
        }
        else
        {
            GD.PrintErr("Failed to configure Invader. Sprite2D or config is null.");
        }
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Laser)
        {
            //GD.Print("Invader hit by bullet!");
            // Aqui você pode adicionar lógica para lidar com a colisão, como remover o invasor
            animationPlayer.Play("destroy");
            area.QueueFree();
            //QueueFree();
        }
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "destroy")
        {
            //GD.Print("Invader destroyed!");
            DestroyInvader(config.points); // Chama o método para destruir o invasor
        }
    }

    public void DestroyInvader(int points)
    {
        GD.Print($"Invasor destruído. Emitindo evento Invaderdestruido com {points} pontos.");
        EmitSignal(nameof(Invaderdestruido), points);
        QueueFree(); // Remove o invasor da cena
    }
}
