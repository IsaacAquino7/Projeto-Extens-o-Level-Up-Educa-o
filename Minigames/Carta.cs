
public class Carta
{
    // Identifica o símbolo da carta
    public string Simbolo { get; set; }

    // Indica se a carta está virada
    public bool Virada { get; set; }

    // Indica se o par foi encontrado
    public bool Encontrada { get; set; }

    public Carta(string simbolo)
    {
        Simbolo = simbolo;
        Virada = false;
        Encontrada = false;
    }
}
