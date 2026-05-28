using HL7App.Models;

namespace HL7App.Web
{
    public class MessageApiClient(HttpClient httpClient)
    {
        public async Task<HL7Message[]> GetMessagesAsync(int maxItems = 10, CancellationToken cancellationToken = default)
        {
            List<HL7Message>? messages = null;
            await foreach (var message in httpClient.GetFromJsonAsAsyncEnumerable<HL7Message>("/messages", cancellationToken))
            {
                if (messages?.Count >= maxItems)
                {
                    break;
                }
                if (message is not null)
                {
                    messages ??= [];
                    messages.Add(message);
                }
            }
            return messages?.ToArray() ?? [];
        }

        public async Task<HL7Message?> GetMessageByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await httpClient.GetFromJsonAsync<HL7Message>($"/messages/{id}", cancellationToken);
        }

        public async Task<HL7Message?> CreateMessageAsync(String messageContent, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsJsonAsync("/messages", messageContent, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<HL7Message>(cancellationToken: cancellationToken);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
            else
            {
                // Handle error response as needed, for now just return null
                return null;
            }
        }

        public async Task<HL7Message?> UpdateMessageAsync(int id, String messageContent, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PatchAsJsonAsync($"/messages/{id}", messageContent, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<HL7Message>(cancellationToken: cancellationToken);
            }
            else
            {
                // Handle error response as needed, for now just return null
                return null;
            }
        }

        public async Task DeleteMessageAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.DeleteAsync($"/messages/{id}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                // Message deleted successfully, you can handle any post-deletion logic here if needed
            }
            else
            {
                // Handle error response as needed, for now just do nothing
            }
        }
    }
}
