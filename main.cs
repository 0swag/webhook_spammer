using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter webhook URL (or multiple webhook URLs separated by a space): ");
        string[] webhooks = Console.ReadLine().Split(' ');

        Console.WriteLine("Message to spam: ");
        string message = Console.ReadLine();

        Console.WriteLine("How many times to spam: ");
        int repeat = Convert.ToInt32(Console.ReadLine());

        int sent_times = 0;

        var tasks = new List<Task>();

        for (int i = 0; i < repeat; i++)
        {
            tasks.AddRange(webhooks.Select(async webhook_url =>
            {
                using (var http_client = new HttpClient())
                {
                    bool sent = false;
                    while (!sent)
                    {
                        var json_payload = $"{{\"content\": \"{message}\"}}";
                        var http_content = new StringContent(json_payload, Encoding.UTF8, "application/json");

                        var http_response = await http_client.PostAsync(webhook_url, http_content);

                        if (http_response.IsSuccessStatusCode)
                        {
                            sent_times++;
                            sent = true;
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("[+] Sent message.");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("[!] Failed to send message because of rate limiting or the webhook got deleted. Retrying...");
                            await Task.Delay(10000);
                        }
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);
    }
}
