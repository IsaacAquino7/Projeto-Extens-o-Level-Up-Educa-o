
using Godot;
using System.Collections.Generic;

public partial class MainMenu : Control
{
	private readonly Dictionary<Button, Tween> animacoes = new();

	private AudioManager audioManager;
	private AudioStream somHover;
	private AudioStream somClick;

	public override void _Ready()
	{
		// Localiza o gerenciador de áudio
		audioManager = GetNode<AudioManager>("/root/AudioManager");

		// Carrega os efeitos sonoros
		somHover = GD.Load<AudioStream>("res://audio/hover.ogg");
		somClick = GD.Load<AudioStream>("res://audio/click.ogg");

		VBoxContainer container = GetNode<VBoxContainer>(
			"MarginContainer/CenterContainer/VBoxContainer"
		);

		ConfigurarAnimacao(container.GetNode<Button>("JogarButton"));
		ConfigurarAnimacao(container.GetNode<Button>("ConfiguracoesButton"));
		ConfigurarAnimacao(container.GetNode<Button>("SairButton"));
	}

	private void ConfigurarAnimacao(Button botao)
	{
		botao.PivotOffset = botao.Size / 2;

		botao.MouseEntered += () =>
		{
			AnimarBotao(botao, new Vector2(1.05f, 1.05f));
			audioManager.ReproduzirEfeito(somHover);
		};

		botao.MouseExited += () =>
		{
			AnimarBotao(botao, Vector2.One);
		};

		botao.Pressed += () =>
		{
			audioManager.ReproduzirEfeito(somClick);
		};
	}

	private void AnimarBotao(Button botao, Vector2 escala)
	{
		if (animacoes.TryGetValue(botao, out Tween tween))
		{
			if (tween.IsRunning())
			{
				tween.Kill();
			}
		}

		Tween novaAnimacao = CreateTween();

		novaAnimacao.TweenProperty(
			botao,
			"scale",
			escala,
			0.15
		).SetTrans(Tween.TransitionType.Sine)
		 .SetEase(Tween.EaseType.Out);

		animacoes[botao] = novaAnimacao;
	}

	// Abre a tela de seleção dos minijogos
	private void OnJogarButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://GameSelection.tscn");
	}

	// Abre as configurações
	private void OnConfiguracoesButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://SettingsMenu.tscn");
	}

	// Encerra o jogo
	private void OnSairButtonPressed()
	{
		GetTree().Quit();
	}
}
