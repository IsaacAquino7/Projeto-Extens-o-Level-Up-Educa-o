
using Godot;

public partial class VideoManager : Node
{
	private const string ConfigPath = "user://video_settings.cfg";

	public bool TelaCheia { get; private set; } = false;

	public Vector2I Resolucao { get; private set; } =
		new Vector2I(1920, 1080);

	public override void _Ready()
	{
		CarregarConfiguracoes();
	}

	private void CarregarConfiguracoes()
	{
		ConfigFile config = new ConfigFile();

		if (config.Load(ConfigPath) == Error.Ok)
		{
			TelaCheia = (bool)config.GetValue(
				"video", "fullscreen", false
			);

			int largura = (int)config.GetValue(
				"video", "width", 1920
			);

			int altura = (int)config.GetValue(
				"video", "height", 1080
			);

			Resolucao = new Vector2I(largura, altura);
		}

		AplicarConfiguracoes();
	}

	private void AplicarConfiguracoes()
	{
		if (TelaCheia)
		{
			DisplayServer.WindowSetMode(
				DisplayServer.WindowMode.Fullscreen
			);
		}
		else
		{
			DisplayServer.WindowSetMode(
				DisplayServer.WindowMode.Windowed
			);

			DisplayServer.WindowSetSize(Resolucao);

			// Centralizar a janela na tela
			int telaAtual = DisplayServer.WindowGetCurrentScreen();

			Rect2I areaUtil = DisplayServer.ScreenGetUsableRect(
				telaAtual
			);

			Vector2I tamanhoJanela = DisplayServer.WindowGetSize();

			Vector2I posicaoCentral =
				areaUtil.Position +
				(areaUtil.Size - tamanhoJanela) / 2;

			DisplayServer.WindowSetPosition(posicaoCentral);
		}
	}

	private void SalvarConfiguracoes()
	{
		ConfigFile config = new ConfigFile();

		config.SetValue("video", "fullscreen", TelaCheia);
		config.SetValue("video", "width", Resolucao.X);
		config.SetValue("video", "height", Resolucao.Y);

		Error resultado = config.Save(ConfigPath);

		if (resultado != Error.Ok)
		{
			GD.PrintErr(
				"Erro ao salvar configurações de vídeo: ",
				resultado
			);
		}
	}

	public void DefinirTelaCheia(bool ativar)
	{
		TelaCheia = ativar;

		AplicarConfiguracoes();
		SalvarConfiguracoes();
	}

	public void DefinirResolucao(int largura, int altura)
	{
		Resolucao = new Vector2I(largura, altura);

		AplicarConfiguracoes();
		SalvarConfiguracoes();
	}
}
