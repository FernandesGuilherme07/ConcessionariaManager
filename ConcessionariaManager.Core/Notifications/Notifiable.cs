namespace ConcessionariaManager.Core.Notifications
{
    public abstract class Notifiable
    {
        private readonly List<Notification> _notificacoes = [];
        public IReadOnlyCollection<Notification> Notificacoes => _notificacoes;

        public bool Invalido => _notificacoes.Count != 0;
        public bool Valido => _notificacoes.Count == 0;

        public void AdicionarNotificacao(string campo, string mensagem)
        {
            _notificacoes.Add(new Notification(campo, mensagem));
        }

        public void AdicionarNotificacoes(IEnumerable<Notification> notificacoes)
        {
            _notificacoes.AddRange(notificacoes);
        }

        public void LimparNotificacoes()
        {
            _notificacoes.Clear();
        }
    }

}
