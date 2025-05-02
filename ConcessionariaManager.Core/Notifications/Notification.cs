namespace ConcessionariaManager.Core.Notifications
{
    public class Notification(string campo, string mensagem)
    {
        public string Campo { get; } = campo;
        public string Mensagem { get; } = mensagem;
    }

}
