namespace _14548_employes_managment.Models
{
    //nome dos instrumentos em ingles porque embora no form teja a dizer em pt, a aplicacao ta em ingles e pediu para segui a aplicacao
    public class Instrument
    {
        public int Id { get; set; }
        public string InstrumentType { get; set; } = string.Empty;
        public string InstrumentName { get; set; } = string.Empty;
        public bool UseStrings { get; set; } = false;
    }
}
