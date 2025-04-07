using Godot;
using System;

public partial class Ui : CanvasLayer
{
    private Texture2D Life_texture = GD.Load<Texture2D>("res://Assets/Player/Player.png");

    private HBoxContainer lifeContainer;
    private Label pointsLabel;
    private PointsCounter pointsCounter;
    private LifeManager lifeManager;
    private InvaderSpawn invaderSpawn;
    private Label gameoverLabel;
    private Button restartButton;
    private CenterContainer gameoverContainer;


    public override void _Ready()
    {
        lifeContainer = GetNode<HBoxContainer>("MarginContainer/HBoxContainer");
        pointsLabel = GetNode<Label>("MarginContainer/Points");
        gameoverContainer = GetNode<CenterContainer>("MarginContainer/GameBoxContainer");
        if (gameoverContainer == null)
        {
            GD.PrintErr("gameoverContainer não encontrado!");
        }
        else
        {
            GD.Print("gameoverContainer encontrado com sucesso!");
        }
        gameoverLabel = GetNode<Label>("%GameOverLabel");
        restartButton = GetNode<Button>("%RestartButton");
        pointsCounter = GetNode<PointsCounter>("../PointsCounter");
        if (pointsCounter == null)
        {
            GD.PrintErr("PointsCounter não encontrado!");
            return;
        }

        lifeManager = GetNode<LifeManager>("../LifeManager");
        if (lifeManager == null)
        {
            GD.PrintErr("LifeManager não encontrado!");
            return;
        }
        invaderSpawn = GetNode<InvaderSpawn>("../InvaderSpawn");

        pointsLabel.Text = string.Format("SCORE: {0}", 0);

        pointsCounter.Connect(nameof(PointsCounter.PointsIncreased), new Callable(this, nameof(UpdatePointsLabel)));
        if (invaderSpawn.Connect(nameof(InvaderSpawn.game_lost), new Callable(this, nameof(Ganhoufoipika))) == Error.Ok)
        {
            GD.Print("Sinal game_lost conectado com sucesso!");
        }
        else
        {
            GD.PrintErr("Falha ao conectar o sinal game_lost!");
        }
        invaderSpawn.Connect(nameof(InvaderSpawn.game_won), new Callable(this, nameof(naoganhopika)));
        restartButton.Pressed += OnRestartButtonPressed;

        if (lifeManager.Connect(nameof(LifeManager.OnLifeLost), new Callable(this, nameof(UpdateLifeIcons))) == Error.Ok)
        {
            GD.Print("Sinal OnLifeLost conectado com sucesso!");
        }
        else
        {
            GD.PrintErr("Falha ao conectar o sinal OnLifeLost!");
        }

        for (int i = 0; i < lifeManager.lives; i++)
        {
            GD.Print($"Adicionando ícone de vida {i + 1}");
            var life_texture_rect = new TextureRect();
            life_texture_rect.ExpandMode = TextureRect.ExpandModeEnum.KeepSize; // Configura o modo de expansão
            life_texture_rect.CustomMinimumSize = new Vector2(40, 25); // Define o tamanho mínimo
            life_texture_rect.TextureFilter = CanvasItem.TextureFilterEnum.Nearest; // Define o filtro de textura
            life_texture_rect.Texture = Life_texture; // Define a textura
            lifeContainer.AddChild(life_texture_rect); // Adiciona o TextureRect ao container
        }
        
    }

    private void UpdatePointsLabel(int points)
    {
        GD.Print("era pra aumentar aqui");
        pointsLabel.Text = string.Format("SCORE: {0}", points);
    }
    private void Ganhoufoipika()
    {
        GD.Print("Chamando Ganhoufoipika()");
        gameoverContainer.Visible = true;
        GD.Print($"gameoverContainer.Visible: {gameoverContainer.Visible}");
    }
    private void naoganhopika()
    {
        gameoverContainer.Visible = true;
        gameoverLabel.Text = "YOU WON!";
        gameoverLabel.AddThemeColorOverride("font_color", new Color(0, 1, 0)); // Verde
        restartButton.Text = "RESTART";
        invaderSpawn.GetNode<Timer>("MovementTimer").Stop();
        invaderSpawn.GetNode<Timer>("ShootTimer").Stop();
    }
    private void OnRestartButtonPressed()
    {
        // Reinicia o jogo
        GetTree().ReloadCurrentScene();
    }
    private void UpdateLifeIcons(int lifesLeft)
    {
        GD.Print($"UpdateLifeIcons chamado com lifesLeft: {lifesLeft}");
        GD.Print($"lifeContainer tem {lifeContainer.GetChildCount()} filhos.");

        if (lifeContainer.GetChildCount() > 0)
        {
            var lifeTextureRect = lifeContainer.GetChild(lifeContainer.GetChildCount() - 1) as TextureRect;
            GD.Print($"Removendo ícone no índice: {lifeContainer.GetChildCount() - 1}");
            lifeTextureRect?.QueueFree();
        }
        else
        {
            GD.Print("Nenhuma vida restante. Chamando Ganhoufoipika().");
            Ganhoufoipika();
        }
    }
        
}

