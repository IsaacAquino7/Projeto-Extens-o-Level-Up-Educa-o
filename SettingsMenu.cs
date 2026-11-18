
using Godot;

public partial class SettingsMenu : Control
{
	private Label volumeValueLabel;
	private HSlider volumeSlider;
	private CheckButton muteCheckButton;
	private AudioManager audioManager;
	private VideoManager videoManager;
	private OptionButton resolutionOptionButton;

	// Efeitos sonoros da interface
	private AudioStream somHover;
	private AudioStream somClick;

	public override void _Ready()
	{
		// Configurações de áudio
		volumeValueLabel = GetNode<Label>(
			"CenterContainer/SettingsPanel/VBoxContainer/VolumeValueLabel"
		);

		volumeSlider = GetNode<HSlider>(
			"CenterContainer/SettingsPanel/VBoxContainer/VolumeSlider"
		);

		muteCheckButton = GetNode<CheckButton>(
			"CenterContainer/SettingsPanel/VBoxContainer/MuteCheckButton"
		);

		audioManager = GetNode<AudioManager>("/root/AudioManager");

		volumeSlider.SetValueNoSignal(audioManager.VolumeAtual);
		volumeValueLabel.Text = $"{audioManager.VolumeAtual:0}%";
		muteCheckButton.SetPressedNoSignal(audioManager.Silenciado);

		// Configurações de vídeo
		videoManager = GetNode<VideoManager>("/root/VideoManager");

		CheckButton fullscreenCheckButton = GetNode<CheckButton>(
			"CenterContainer/SettingsPanel/VBoxContainer/FullscreenCheckButton"
		);

		bool estaEmTelaCheia =
			DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen
			|| DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;

		fullscreenCheckButton.SetPressedNoSignal(estaEmTelaCheia);

		// Seletor de resolução
		resolutionOptionButton = GetNode<OptionButton>(
			"CenterContainer/SettingsPanel/VBoxContainer/ResolutionOptionButton"
		);

		Vector2I resolucaoAtual = videoManager.Resolucao;

		if (resolucaoAtual == new Vector2I(1280, 720))
		{
			resolutionOptionButton.Select(0);
		}
		else if (resolucaoAtual == new Vector2I(1600, 900))
		{
			resolutionOptionButton.Select(1);
		}
		else if (resolucaoAtual == new Vector2I(1920, 1080))
		{
			resolutionOptionButton.Select(2);
		}
		else
		{
			resolutionOptionButton.Select(-1);
		}

		// NOVO: carregar os efeitos sonoros
		somHover = GD.Load<AudioStream>("res://audio/hover.ogg");
		somClick = GD.Load<AudioStream>("res://audio/click.ogg");

		// NOVO: configurar os botões
		Button voltarButton = GetNode<Button>(
			"CenterContainer/SettingsPanel/VBoxContainer/VoltarButton"
		);

		Button testarAudioButton = GetNode<Button>(
			"CenterContainer/SettingsPanel/VBoxContainer/TestarAudioButton"
		);

		ConfigurarSomBotao(voltarButton);
		ConfigurarSomBotao(testarAudioButton);
	}

	// NOVO: adiciona sons aos botões
	private void ConfigurarSomBotao(Button botao)
	{
		botao.MouseEntered += () =>
		{
			audioManager.ReproduzirEfeito(somHover);
		};

		botao.Pressed += () =>
		{
			audioManager.ReproduzirEfeito(somClick);
		};
	}

	private void OnVolumeSliderValueChanged(double value)
	{
		audioManager.DefinirVolume(value);
		volumeValueLabel.Text = $"{value:0}%";
	}

	private void OnMuteCheckButtonToggled(bool toggledOn)
	{
		audioManager.DefinirSilenciado(toggledOn);
	}

	private void OnTestarAudioButtonPressed()
	{
		AudioStreamPlayer player = GetNode<AudioStreamPlayer>(
			"TestAudioPlayer"
		);

		player.Play();
	}

	private void OnFullscreenCheckButtonToggled(bool toggledOn)
	{
		videoManager.DefinirTelaCheia(toggledOn);
	}

	private void OnResolutionOptionButtonItemSelected(long index)
	{
		switch (index)
		{
			case 0:
				videoManager.DefinirResolucao(1280, 720);
				break;

			case 1:
				videoManager.DefinirResolucao(1600, 900);
				break;

			case 2:
				videoManager.DefinirResolucao(1920, 1080);
				break;
		}

		GD.Print("Resolução selecionada: ", videoManager.Resolucao);
	}

	private void OnVoltarButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}
}
