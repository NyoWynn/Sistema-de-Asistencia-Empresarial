namespace SistemaAsistencia.Services
{
    public class QrService
    {
        public string GenerateQrCode(string data)
        {
            // Generar un QR simple usando una API externa
            string qrUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={Uri.EscapeDataString(data)}";
            return qrUrl;
        }

        public byte[] GenerateQrCodeBytes(string data)
        {
            // Para descarga, usamos la misma URL pero retornamos como bytes
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={Uri.EscapeDataString(data)}").Result;
                return response.Content.ReadAsByteArrayAsync().Result;
            }
        }
    }
}
