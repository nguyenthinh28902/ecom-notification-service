using Azure.Core;
using Ecom.Contracts.Requests;
using Ecom.Notification.Application.Interface.System;
using Ecom.Notification.Core.Abstractions.Persistence;
using Ecom.Notification.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecom.Notification.Application.Service.System
{
    public class NotificationDispatcherService : INotificationDispatcherService
    {
        private readonly ILogger<NotificationDispatcherService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationStoreService _notificationStoreService;
        private readonly IEmailService _emailService;
        public NotificationDispatcherService(ILogger<NotificationDispatcherService> logger, IUnitOfWork unitOfWork
            , INotificationStoreService notificationStoreService
            , IEmailService emailService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _notificationStoreService = notificationStoreService;
            _emailService = emailService;
        }
        public async Task DispatchNotificationAsync(NotificationRequestDto request)
        {
            // 1. Gọi hàm Store nãy mình làm để render nội dung và lưu DB
            var entity = await _notificationStoreService.CreateNotificationAsync(request);

            if (entity == null) return;

            // 2. Chỉ comment dòng quan trọng: Dựa vào Channel để gọi hành động tương ứng
            switch (entity.NotificationChannel)
            {
                case "EMAIL":
                    await PushEmailAsync(entity.Id, request);
                    break;
                case "WEB_PUSH":
                    await PushWebNotificationAsync(entity);
                    break;

            }
        }
        private async Task PushEmailAsync(long entityId, NotificationRequestDto request)
        {
            await _unitOfWork.BeginTransactionAsync();
            var entity = await _unitOfWork.Repository<UserNotification>().FirstOrDefaultAsync(x => x.Id == entityId);
            try
            {
                // 3. Chỉ comment dòng quan trọng: Ở đây không cần Replace nữa vì Content đã là HTML hoàn chỉnh
                string emailSubject = entity.Title;
                string emailBody = entity.Content;
                var template = await _unitOfWork.Repository<NotificationTemplate>()
                .GetAll(include: x => x.Include(y => y.Type))
                .FirstOrDefaultAsync(t => t.Type.TypeCode == request.TypeCode
                                          && t.LanguageCode == request.LanguageCode);
                if (template != null)
                {                    
                    emailBody = RenderEmailHtml(template.BodyTemplate, request.Parameters, request.Items);
                    emailBody = emailBody.Replace("{{action_url}}", entity.DeepLink);
                }
                var result = await _emailService.SendHtmlEmailAsync(request.ReceiverEmail, emailSubject, emailBody);
                if (result)
                {
                    entity.IsPushed = true;
                }
                else
                {
                    entity.PushCount++;
                    entity.LastError = "Lỗi khi hành động gữi email";
                }

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                // Ghi log lỗi và tăng PushCount để Worker sau này quét lại
                entity.PushCount++;
                entity.LastError = ex.Message;
                await _unitOfWork.CommitAsync();
                _logger.LogError(ex, "Lỗi khi thực hiện gửi Email cho NotiId: {Id}", entity.Id);
            }
        }
        private async Task PushWebNotificationAsync(UserNotification entity)
        {
            // Tạm thời để trống logic Push Real-time (ví dụ: SignalR Hub)
            // Sau này ông chỉ cần vả code SignalR vào đây là xong.

            // Đối với Web thông báo nội bộ, thường lưu DB xong là coi như đã Pushed
            entity.IsPushed = true;
            await _unitOfWork.SaveChangesAsync();

            await Task.CompletedTask;
        }


        private string RenderEmailHtml(string template, Dictionary<string, string> parameters, List<Dictionary<string, string>> items = null)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var sb = new StringBuilder(template);

            //  Tìm vị trí thẻ LOOP
            const string startTag = "[LOOP]";
            const string endTag = "[/LOOP]";

            int startIdx = template.IndexOf(startTag);
            int endIdx = template.IndexOf(endTag);

            if (startIdx != -1 && endIdx > startIdx)
            {
                // 2.  Nếu không có dữ liệu lặp, xóa toàn bộ khối LOOP
                if (items == null || items.Count == 0)
                {
                    sb.Remove(startIdx, endIdx + endTag.Length - startIdx);
                }
                else
                {
                    //Lấy HTML mẫu và render danh sách sản phẩm
                    string loopTemplate = template.Substring(startIdx + startTag.Length, endIdx - (startIdx + startTag.Length));
                    var itemsBuilder = new StringBuilder();

                    foreach (var item in items)
                    {
                        var rowBuilder = new StringBuilder(loopTemplate);
                        foreach (var prop in item)
                        {
                            rowBuilder.Replace($"{{{{{prop.Key}}}}}", prop.Value);
                        }
                        itemsBuilder.Append(rowBuilder);
                    }

                    // Thay thế khối LOOP bằng nội dung đã render
                    sb.Remove(startIdx, endIdx + endTag.Length - startIdx);
                    sb.Insert(startIdx, itemsBuilder.ToString());
                }
            }

            // Thay thế các tham số đơn (tên khách, ngày đặt...)
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    sb.Replace($"{{{{{param.Key}}}}}", param.Value);
                }
            }

            return sb.ToString();
        }
    }
}
