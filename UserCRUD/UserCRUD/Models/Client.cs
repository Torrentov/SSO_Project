namespace UserCRUD.Models
{
    public class Client
    {
        public string clientId { get; set; }
        public string clientSecret { get; set; }
    }

    public class ClientsRoot
    {
        public List<Client> clients { get; set; }
    }

    public class ClientRoot
    {
        public Client client { get; set; }
    }
}
