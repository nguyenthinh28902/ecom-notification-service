using Ecom.Contracts.Requests;
using Ecom.Notification.Application.Interface.System;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Notification.Controller
{
    /// <summary>
    /// API Quản lý và Điều hướng thông báo (Email, Web Push, SMS...)
    /// </summary>
    [Route("api/thong-bao")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationDispatcherService _notificationDispatcherService;

        public NotificationController(ILogger<NotificationController> logger, INotificationDispatcherService notificationDispatcherService)
        {
            _logger = logger;
            _notificationDispatcherService = notificationDispatcherService;
        }

        /// <summary>
        /// Gửi thông báo đến người dùng qua các kênh đã cấu hình
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Notification/send
        ///     {
        ///        "typeCode": "ORDER_SUCCESS",
        ///        "languageCode": "vi",
        ///        "receiverEmail": "thinh@example.com",
        ///        "parameters": { "customer_name": "Thịnh" },
        ///        "items": [ { "product_name": "Laptop", "price": "20tr" } ]
        ///     }
        /// </remarks>
        /// <param name="request">Thông tin yêu cầu gửi thông báo</param>
        /// <returns>Kết quả thực hiện gửi thông báo</returns>
        /// <response code="200">Gửi thông báo thành công hoặc đã đưa vào hàng đợi</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi hệ thống khi xử lý gửi thông báo</response>
        [HttpPost("thong-bao-moi")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequestDto request)
        {
            // Chỉ comment dòng quan trọng: Kiểm tra tính hợp lệ của Request đầu vào
            if (request == null) return BadRequest("Dữ liệu không được để trống");

            try
            {
                // Gọi service điều hướng để xử lý Render và Push
                await _notificationDispatcherService.DispatchNotificationAsync(request);

                return Ok(new { Message = "Thông báo đang được xử lý", SentAt = DateTime.Now });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý gửi thông báo cho: {Email}", request.ReceiverEmail);
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình gửi thông báo");
            }
        }
    }
}