
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MemoryGame : Control
{
	// Lista que armazenará as 16 cartas
	private List<Carta> cartas = new();

	// Controla as duas cartas selecionadas
	private int primeiraCarta = -1;
	private int segundaCarta = -1;

	// Impede novos cliques durante comparações e animações
	private bool comparando = false;

	// Conta quantos pares foram encontrados
	private int paresEncontrados = 0;

	// Referência à interface de vitória
	private VBoxContainer vitoriaContainer;

	// Reprodutores dos efeitos sonoros
	private AudioStreamPlayer acertoPlayer;
	private AudioStreamPlayer erroPlayer;

	// Cores utilizadas nas cartas
	private readonly Color corEscondida = new Color("#28649A");
	private readonly Color corRevelada = new Color("#D69B32");
	private readonly Color corEncontrada = new Color("#238C62");

	public override void _Ready()
	{
		// Configura o botão Voltar
		Button voltarButton = GetNode<Button>(
			"MarginContainer/CenterContainer/VBoxContainer/VoltarButton"
		);

		voltarButton.Pressed += OnVoltarButtonPressed;

		// Localiza a interface de vitória
		vitoriaContainer = GetNode<VBoxContainer>(
			"MarginContainer/CenterContainer/VBoxContainer/VitoriaContainer"
		);

		// Mantém a mensagem de vitória escondida
		vitoriaContainer.Hide();

		// Localiza os reprodutores de áudio
		acertoPlayer = GetNode<AudioStreamPlayer>("AcertoPlayer");
		erroPlayer = GetNode<AudioStreamPlayer>("ErroPlayer");

		// Localiza o botão Jogar Novamente
		Button jogarNovamenteButton = vitoriaContainer.GetNode<Button>(
			"JogarNovamenteButton"
		);

		jogarNovamenteButton.Pressed += OnJogarNovamenteButtonPressed;

		// Cria e embaralha as cartas
		CriarCartas();
		EmbaralharCartas();

		// Associa as cartas aos botões
		ConfigurarTabuleiro();

		// Exibe os símbolos no console para testes
		foreach (Carta carta in cartas)
		{
			GD.Print(carta.Simbolo);
		}
	}

	private void CriarCartas()
	{
		// Oito símbolos culturais amazônicos
		string[] simbolos =
		{
			"Curupira",
			"Boto",
			"Iara",
			"Mapinguari",
			"Vitória-Régia",
			"Onça",
			"Tucano",
			"Pirarucu"
		};

		// Cria dois exemplares de cada símbolo
		foreach (string simbolo in simbolos)
		{
			cartas.Add(new Carta(simbolo));
			cartas.Add(new Carta(simbolo));
		}
	}

	private void EmbaralharCartas()
	{
		System.Random random = new System.Random();

		// Embaralhamento Fisher-Yates
		for (int i = cartas.Count - 1; i > 0; i--)
		{
			int j = random.Next(i + 1);

			Carta temporaria = cartas[i];
			cartas[i] = cartas[j];
			cartas[j] = temporaria;
		}
	}

	private void ConfigurarTabuleiro()
	{
		// Localiza o tabuleiro
		GridContainer tabuleiro = GetNode<GridContainer>(
			"MarginContainer/CenterContainer/VBoxContainer/TabuleiroGrid"
		);

		// Configura os 16 botões
		for (int i = 0; i < cartas.Count; i++)
		{
			Button botao = tabuleiro.GetNode<Button>(
				$"Carta{i + 1:D2}"
			);

			// Esconde inicialmente o símbolo
			botao.Text = "?";

			// Define a cor inicial azul
			AtualizarCorCarta(botao, corEscondida);

			// Guarda o índice correto para cada botão
			int indice = i;

			// Conecta o clique ao método
			botao.Pressed += () => OnCartaPressed(indice);
		}
	}

	// Executado quando o jogador clica em uma carta
	private async void OnCartaPressed(int indice)
	{
		// Impede cliques durante animações e comparações
		if (comparando)
		{
			return;
		}

		Carta carta = cartas[indice];

		// Impede selecionar cartas já viradas ou encontradas
		if (carta.Virada || carta.Encontrada)
		{
			return;
		}

		// Marca a carta como virada
		carta.Virada = true;

		// Localiza o tabuleiro
		GridContainer tabuleiro = GetNode<GridContainer>(
			"MarginContainer/CenterContainer/VBoxContainer/TabuleiroGrid"
		);

		// Localiza o botão da carta selecionada
		Button botao = tabuleiro.GetNode<Button>(
			$"Carta{indice + 1:D2}"
		);

		// Bloqueia novos cliques durante a animação
		comparando = true;

		// Anima a revelação da carta
		await AnimarCarta(botao, carta.Simbolo, corRevelada);

		// Interrompe se o jogador saiu da cena
		if (!IsInsideTree())
		{
			return;
		}

		// Libera os cliques após a animação
		comparando = false;

		GD.Print($"Carta {indice + 1} virada: {carta.Simbolo}");

		// Registra a primeira carta
		if (primeiraCarta == -1)
		{
			primeiraCarta = indice;
			return;
		}

		// Registra a segunda carta
		segundaCarta = indice;

		// Bloqueia novos cliques durante a comparação
		comparando = true;

		// Verifica se os símbolos são iguais
		if (cartas[primeiraCarta].Simbolo ==
			cartas[segundaCarta].Simbolo)
		{
			GD.Print("Par encontrado!");

			// Reproduz o efeito sonoro de acerto
			acertoPlayer.Play();

			// Marca as cartas como encontradas
			cartas[primeiraCarta].Encontrada = true;
			cartas[segundaCarta].Encontrada = true;

			// Localiza os botões do par encontrado
			Button botao1 = tabuleiro.GetNode<Button>(
				$"Carta{primeiraCarta + 1:D2}"
			);

			Button botao2 = tabuleiro.GetNode<Button>(
				$"Carta{segundaCarta + 1:D2}"
			);

			// Destaca o par encontrado em verde
			AtualizarCorCarta(botao1, corEncontrada);
			AtualizarCorCarta(botao2, corEncontrada);

			// Atualiza a contagem de pares
			paresEncontrados++;

			GD.Print($"Pares encontrados: {paresEncontrados}/8");

			// Verifica se todos os pares foram encontrados
			if (paresEncontrados == 8)
			{
				GD.Print("PARABÉNS! Você encontrou todos os pares!");
				GD.Print("Jogo da Memória Cultural concluído!");

				// Exibe a interface de vitória
				vitoriaContainer.Show();
			}
		}
		else
		{
			GD.Print("As cartas são diferentes!");

			// Reproduz o efeito sonoro de erro
			erroPlayer.Play();

			// Aguarda 1,5 segundo antes de esconder
			await ToSignal(
				GetTree().CreateTimer(1.5),
				SceneTreeTimer.SignalName.Timeout
			);

			// Interrompe se a cena foi removida
			if (!IsInsideTree())
			{
				return;
			}

			// Localiza os dois botões selecionados
			Button botao1 = tabuleiro.GetNode<Button>(
				$"Carta{primeiraCarta + 1:D2}"
			);

			Button botao2 = tabuleiro.GetNode<Button>(
				$"Carta{segundaCarta + 1:D2}"
			);

			// Anima as duas cartas simultaneamente
			await Task.WhenAll(
				AnimarCarta(botao1, "?", corEscondida),
				AnimarCarta(botao2, "?", corEscondida)
			);

			// Interrompe se a cena foi removida
			if (!IsInsideTree())
			{
				return;
			}

			// Permite virar as cartas novamente
			cartas[primeiraCarta].Virada = false;
			cartas[segundaCarta].Virada = false;
		}

		// Reinicia a seleção para a próxima tentativa
		primeiraCarta = -1;
		segundaCarta = -1;
		comparando = false;
	}

	// Atualiza a aparência visual das cartas
	private void AtualizarCorCarta(Button botao, Color cor)
	{
		// Cria um estilo personalizado
		StyleBoxFlat estilo = new StyleBoxFlat();

		// Define a cor de fundo
		estilo.BgColor = cor;

		// Arredonda os cantos
		estilo.SetCornerRadiusAll(10);

		// Aplica o estilo aos estados do botão
		botao.AddThemeStyleboxOverride("normal", estilo);
		botao.AddThemeStyleboxOverride("hover", estilo);
		botao.AddThemeStyleboxOverride("pressed", estilo);
		botao.AddThemeStyleboxOverride("focus", estilo);
	}

	// Anima a carta ao revelar ou esconder seu símbolo
	private async Task AnimarCarta(
		Button botao,
		string novoTexto,
		Color novaCor)
	{
		// Define o centro da carta como ponto de transformação
		botao.PivotOffset = botao.Size / 2;

		// Garante que a animação comece na escala normal
		botao.Scale = Vector2.One;

		// Primeira metade: estreita a carta
		Tween fechar = CreateTween();

		fechar.TweenProperty(
			botao,
			"scale:x",
			0.05f,
			0.15f
		);

		await ToSignal(
			fechar,
			Tween.SignalName.Finished
		);

		// Interrompe se a cena não estiver mais ativa
		if (!IsInsideTree())
		{
			return;
		}

		// Troca o texto e a cor quando a carta está estreita
		botao.Text = novoTexto;
		AtualizarCorCarta(botao, novaCor);

		// Segunda metade: restaura a largura
		Tween abrir = CreateTween();

		abrir.TweenProperty(
			botao,
			"scale:x",
			1.0f,
			0.15f
		);

		await ToSignal(
			abrir,
			Tween.SignalName.Finished
		);
	}

	// Reinicia o Jogo da Memória Cultural
	private void OnJogarNovamenteButtonPressed()
	{
		GetTree().ReloadCurrentScene();
	}

	// Retorna à seleção de minijogos
	private void OnVoltarButtonPressed()
	{
		GetTree().ChangeSceneToFile(
			"res://GameSelection.tscn"
		);
	}
}
