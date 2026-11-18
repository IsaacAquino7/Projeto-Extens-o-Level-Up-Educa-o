
using Godot;

public partial class AudioManager : Node
{
	private const string ConfigPath = "user://settings.cfg";

	public double VolumeAtual { get; private set; } = 50.0;
	public bool Silenciado { get; private set; } = false;

	public override void _Ready()
	{
		CarregarConfiguracoes();
	}

	private void CarregarConfiguracoes()
	{
		ConfigFile config = new ConfigFile();

		Error resultado = config.Load(ConfigPath);

		if (resultado == Error.Ok)
		{
			VolumeAtual = (double)config.GetValue(
				"audio", "volume", 50.0
			);

			Silenciado = (bool)config.GetValue(
				"audio", "muted", false
			);
		}

		VolumeAtual = Mathf.Clamp(VolumeAtual, 0.0, 100.0);

		AplicarAudio();
	}

	private void AplicarAudio()
	{
		int masterBus = AudioServer.GetBusIndex("Master");

		AudioServer.SetBusVolumeLinear(
			masterBus,
			(float)(VolumeAtual / 100.0)
		);

		AudioServer.SetBusMute(masterBus, Silenciado);
	}

	public void DefinirVolume(double valor)
	{
		VolumeAtual = Mathf.Clamp(valor, 0.0, 100.0);

		AplicarAudio();
		SalvarConfiguracoes();
	}

	public void DefinirSilenciado(bool silenciar)
	{
		Silenciado = silenciar;

		AplicarAudio();
		SalvarConfiguracoes();
	}

	// NOVO: reproduz um efeito sonoro
	public void ReproduzirEfeito(AudioStream som)
	{
		if (som == null)
		{
			return;
		}

		AudioStreamPlayer player = new AudioStreamPlayer();
		player.Stream = som;
		player.Bus = "Master";

		AddChild(player);
		player.Finished += player.QueueFree;

		player.Play();
	}

	private void SalvarConfiguracoes()
	{
		ConfigFile config = new ConfigFile();

		config.SetValue("audio", "volume", VolumeAtual);
		config.SetValue("audio", "muted", Silenciado);

		Error resultado = config.Save(ConfigPath);

		if (resultado != Error.Ok)
		{
			GD.PrintErr("Erro ao salvar configurações: ", resultado);
		}
	}
}
