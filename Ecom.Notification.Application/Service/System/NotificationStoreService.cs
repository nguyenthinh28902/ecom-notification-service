using Ecom.Contracts.Requests;
using Ecom.Notification.Application.Interface.System;
using Ecom.Notification.Core.Abstractions.Persistence;
using Ecom.Notification.Core.Entities;
using Ecom.Notification.Core.Models.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecom.Notification.Application.Service.System
{
    public class NotificationStoreService : INotificationStoreService
    {
        private readonly ILogger<NotificationStoreService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public NotificationStoreService(ILogger<NotificationStoreService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserNotification?> CreateNotificationAsync(NotificationRequestDto request)
        {
            // 1. Chỉ comment dòng quan trọng: Lấy Template từ DB dựa trên TypeCode
            var template = await _unitOfWork.Repository<NotificationTemplate>()
                .GetAll(include: x => x.Include(y => y.Type))
                .FirstOrDefaultAsync(t => t.Type.TypeCode == request.TypeCode
                                          && t.LanguageCode == request.LanguageCode);

            string finalTitle = string.Empty;
            string finalBody =  string.Empty;
            string actionUrl = string.Empty;
            if (template != null)
            {
                finalTitle = template.TitleTemplate;
                finalBody = template.BodyTemplate;
                actionUrl = template.ActionUrl ?? string.Empty;
                //actionUrl = template.ActionUrl ?? string.Empty;
                foreach (var param in request.Parameters)
                {
                    finalTitle = finalTitle.Replace($"{{{{{param.Key}}}}}", param.Value);
                    actionUrl = actionUrl.Replace($"{{{{{param.Key}}}}}", param.Value);
                    
                }
                if (request.Channel != NotificationChannel.EMAIL.ToString())
                {
                    foreach (var param in request.Parameters)
                    {
                        finalBody = finalBody.Replace($"{{{{{param.Key}}}}}", param.Value);
                    }
                }
                else
                {
                    finalBody = request.Message ?? finalTitle; // không lưu nội dung email nếu là gửi email
                }
            }
            else
            {
               finalTitle = "Bạn có một thông báo mới.";
               finalBody = request.Message ?? "Hãy kiểm tra thông báo."; 
            }
           
            try
            {
                await _unitOfWork.BeginTransactionAsync();
               
                var userNoti = new UserNotification
                {
                    ReceiverId = request.ReceiverId,
                    ReceiverRole = request.ReceiverRole,
                    ReceiverEmail = request.ReceiverEmail,
                    TemplateId = template?.Id ?? null,
                    Title = finalTitle,
                    Content = finalBody,
                    DeepLink = actionUrl,
                    IsPushed = false,
                    PushCount = 0,
                    IsRead = false,
                    NotificationChannel = request.Channel,
                };
                await _unitOfWork.Repository<UserNotification>().AddAsync(userNoti);
                await _unitOfWork.CommitAsync();
                return userNoti;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email cho {Email}", request.ReceiverEmail);
                return null;
            }
        }
    }
}
