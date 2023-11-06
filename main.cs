using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter webhook: ");
        string webhook_url = Console.ReadLine();
        Console.WriteLine("Message to spam: ");
        string message = Console.ReadLine();
        Console.WriteLine("How many times to spam: ");
        int repeat = Convert.ToInt32(Console.ReadLine());

        var tasks = Enumerable.Range(0, repeat)
            .Select(async i =>
            {
                using (var http_client = new HttpClient())
                {
                    var json_payload = $"{{ \"content\": \"{message}\" }}";
                    var http_content = new StringContent(json_payload, Encoding.UTF8, "application/json");
                    HttpResponseMessage http_response = await http_client.PostAsync(webhook_url, http_content);
                    if (http_response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("[+] Message Sent!");
                    }
                    else
                    {
                        Console.WriteLine("[!] Failed to Send Message. Retrying..");
                    }
                }
            })
            .ToArray();

        await Task.WhenAll(tasks);
    }
}
