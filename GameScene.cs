
using Godot;

public partial class GameScene : Control
{
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

		// Localiza o botão Voltar
		Button voltarButton = FindChild(
			"VoltarButton",
			true,
			false
		) as Button;

		if (voltarButton == null)
		{
			GD.PrintErr("Botão VoltarButton não encontrado!");
			return;
		}

		// Som ao passar o mouse
		voltarButton.MouseEntered += () =>
		{
			audioManager.ReproduzirEfeito(somHover);
		};

		// Som ao clicar
		voltarButton.Pressed += () =>
		{
			audioManager.ReproduzirEfeito(somClick);
		};
	}

	private void OnVoltarButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}
}
