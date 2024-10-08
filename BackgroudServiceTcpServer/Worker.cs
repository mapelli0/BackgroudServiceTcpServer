using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BackgroudServiceTcpServer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger) {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        try {
            _logger.LogInformation("Starting tcp listener: {time}", DateTimeOffset.Now);
            TcpListener listener = new TcpListener(IPAddress.Any, 8000);
            listener.Start();
            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogInformation("awaiting connection");
                TcpClient client = await listener.AcceptTcpClientAsync(stoppingToken);
                _logger.LogInformation("Client Connected {c}", client.Connected);
                NetworkStream stream = client.GetStream();

                while (!stoppingToken.IsCancellationRequested) {
                    try {
                        byte[] data = new byte[11];
                        _ = await stream.ReadAsync(data, 0, 11, stoppingToken);
                        var str = Encoding.ASCII.GetString(data);
                        _logger.LogInformation("data read: {p}", str);
                    }
                    catch (SocketException se) {
                        _logger.LogError(se, "Connection error");
                        break;
                    }

                }
            }
        }
        catch (Exception e) {
            _logger.LogError(e ,"loops error");
        }
    }
}