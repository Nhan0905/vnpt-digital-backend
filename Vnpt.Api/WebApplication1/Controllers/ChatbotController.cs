using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Vnpt.Application.Interfaces;

namespace Vnpt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly string _geminiApiKey = ""; // Để trống hoặc điền Key AI chuẩn (AIzaSy...) 

        public ChatbotController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> AskBot([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                    return BadRequest(new { reply = "Bạn chưa nhập nội dung câu hỏi ạ!" });

                var products = await _productService.GetAllProductsAsync();
                var productList = products.ToList();

                // 1. THỬ GỌI GEMINI AI NẾU CÓ KEY CHUẨN
                if (!string.IsNullOrEmpty(_geminiApiKey) && _geminiApiKey.StartsWith("AIzaSy"))
                {
                    var productData = string.Join("\n", productList.Select(p =>
                        $"- Gói: {p.Name} (Mã: {p.Code}), Giá: {p.Price} VNĐ, Mô tả: {p.Description}"));

                    var prompt = $@"Bạn là nữ nhân viên tư vấn khách hàng thân thiện của VNPT. 
                                   Dữ liệu gói cước:
                                   {productData}

                                   Quy tắc: Xưng 'mình', gọi 'bạn', ngắn gọn 3-4 câu. Khách hỏi: {request.Message}";

                    var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_geminiApiKey}";
                    var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };

                    using var client = new HttpClient();
                    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(jsonResponse);
                        var botReply = doc.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text").GetString();

                        if (!string.IsNullOrEmpty(botReply)) return Ok(new { reply = botReply });
                    }
                }

                // 2. CƠ CHẾ DỰ PHÒNG THÔNG MINH MỞ RỘNG
                string lowerMsg = request.Message.ToLower();
                string fallbackReply = "Dạ VNPT xin chào bạn! Mình có thể hỗ trợ bạn tìm kiếm gói cước, đăng ký dịch vụ hoặc hướng dẫn báo hỏng mạng ạ.";

                if (lowerMsg.Contains("giá") || lowerMsg.Contains("tiền") || lowerMsg.Contains("bao nhiêu") || lowerMsg.Contains("gói"))
                {
                    if (productList.Count > 0)
                    {
                        var sample = productList.First();
                        fallbackReply = $"Bên mình có nhiều gói cước ưu đãi, ví dụ như {sample.Name} ({sample.Code}) với giá chỉ {sample.Price:N0} đ/tháng. Bạn có thể bấm vào mục 'DI ĐỘNG' hoặc 'INTERNET' trên menu để xem chi tiết nhé!";
                    }
                    else
                    {
                        fallbackReply = "Bạn có thể tham khảo các gói cước Di động và Internet trực tiếp trên menu của trang web nhé!";
                    }
                }
                else if (lowerMsg.Contains("hỏng") || lowerMsg.Contains("lỗi") || lowerMsg.Contains("mất mạng") || lowerMsg.Contains("rớt"))
                {
                    fallbackReply = "Rất xin lỗi bạn về sự cố này! Bạn hãy chuyển sang tab 'HỖ TRỢ' trên menu và bấm 'Gửi yêu cầu hỗ trợ / Báo hỏng mạng', kỹ thuật viên sẽ xử lý ngay, hoặc gọi 18001166.";
                }
                else if (lowerMsg.Contains("đăng ký") || lowerMsg.Contains("mua"))
                {
                    fallbackReply = "Để đăng ký, bạn chỉ cần bấm vào nút 'XEM CHI TIẾT' của gói cước, sau đó nhấn 'TIẾN HÀNH ĐĂNG KÝ' và điền thông tin là xong ạ!";
                }
                // BỔ SUNG THÊM NHẬN DIỆN TỪ KHÓA ĐIỂM GIAO DỊCH
                else if (lowerMsg.Contains("điểm giao dịch") || lowerMsg.Contains("cửa hàng") || lowerMsg.Contains("địa chỉ") || lowerMsg.Contains("chi nhánh") || lowerMsg.Contains("store"))
                {
                    fallbackReply = "Để tra cứu điểm giao dịch VNPT gần bạn nhất, bạn hãy chuyển sang tab 'HỖ TRỢ' trên thanh menu và chọn mục 'Tra cứu Điểm giao dịch' để xem bản đồ chi tiết nhé!";
                }
                else if (lowerMsg.Contains("tổng đài") || lowerMsg.Contains("liên hệ") || lowerMsg.Contains("hotline"))
                {
                    fallbackReply = "Tổng đài CSKH VinaPhone: 18001091. Tổng đài hỗ trợ Internet/Truyền hình: 18001166 (Miễn phí cước gọi 24/7).";
                }

                return Ok(new { reply = fallbackReply });
            }
            catch (Exception ex)
            {
                return Ok(new { reply = "Dạ hệ thống đang phản hồi, bạn vui lòng gọi tổng đài 18001091 để được hỗ trợ nhanh nhất nhé!" });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}