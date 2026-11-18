
using Godot;
using System.Collections.Generic;

public partial class GameSelection : Control
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

		// Localiza o contêiner dos botões
		VBoxContainer container = GetNode<VBoxContainer>(
			"MarginContainer/CenterContainer/VBoxContainer"
		);

		// Localiza os quatro botões
		Button memoriaButton = container.GetNode<Button>("MemoriaButton");
		Button contadorButton = container.GetNode<Button>("ContadorButton");
		Button silabasButton = container.GetNode<Button>("SilabasButton");
		Button voltarButton = container.GetNode<Button>("VoltarButton");

		// Configura animações e efeitos sonoros
		ConfigurarBotao(memoriaButton);
		ConfigurarBotao(contadorButton);
		ConfigurarBotao(silabasButton);
		ConfigurarBotao(voltarButton);

		// Conecta os botões às suas funções
		memoriaButton.Pressed += OnMemoriaButtonPressed;
		voltarButton.Pressed += OnVoltarButtonPressed;
	}

	private void ConfigurarBotao(Button botao)
	{
		// Define o centro da animação
		botao.PivotOffset = botao.Size / 2;

		// Quando o mouse entra no botão
		botao.MouseEntered += () =>
		{
			AnimarBotao(botao, new Vector2(1.05f, 1.05f));
			audioManager.ReproduzirEfeito(somHover);
		};

		// Quando o mouse sai do botão
		botao.MouseExited += () =>
		{
			AnimarBotao(botao, Vector2.One);
		};

		// Quando o botão é clicado
		botao.Pressed += () =>
		{
			audioManager.ReproduzirEfeito(somClick);
		};
	}

	private void AnimarBotao(Button botao, Vector2 escala)
	{
		// Interrompe a animação anterior
		if (animacoes.TryGetValue(botao, out Tween tween))
		{
			if (tween.IsRunning())
			{
				tween.Kill();
			}
		}

		// Cria uma nova animação suave
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

	// Abre o Jogo da Memória Cultural
	private void OnMemoriaButtonPressed()
	{
		GetTree().ChangeSceneToFile(
			"res://Minigames/MemoryGame.tscn"
		);
	}

	// Retorna ao menu principal
	private void OnVoltarButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}
}
