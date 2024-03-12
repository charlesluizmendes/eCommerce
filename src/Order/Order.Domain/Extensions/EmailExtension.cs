using Order.Domain.Models;

namespace Order.Domain.Extensions
{
    public static class EmailExtension
    {
        public static string CreateEmailConfirmationAsync(int id, double amount, ICollection<Item> items)
        {
            var email = $"Confirmacao do Pedido #{id}\n";
            email += "\nObrigado por realizar sua compra!\n";
            email += "\nLista de Itens do Pedido:\n";

            foreach (var item in items)
            {
                email += $"{item.Quantity} {item.Name}: {item.Description} R$ {item.Price}\n";
            }

            email += $"\nTotal: R$ {amount}\n";
            email += $"\nVolte Sempre!\n";

            return email;
        }
    }
}
