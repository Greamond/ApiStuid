using ApiStuid.Models;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ApiStuid.Classes
{
    public class NotificationMobile
    {
        private static bool _initialized = false;
        private static readonly object _lock = new object();

        public static async System.Threading.Tasks.Task SendPushNotification(string fcmToken, Dictionary<string, string> data = null)
        {
            // Инициализация Firebase (потокобезопасная)
            InitializeFirebase();

            var message = new Message()
            {
                Token = fcmToken,
                Data = data ?? new Dictionary<string, string>(),
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "project_notifications",
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"Успешная отправка уведомления: {response}");
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Ошибка Firebase: {ex.ErrorCode} - {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Внутренее исключение: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Главная ошибка: {ex.Message}");
            }
        }

        private static void InitializeFirebase()
        {
            if (!_initialized)
            {
                lock (_lock)
                {
                    if (!_initialized)
                    {
                        string jsonPath = Path.Combine(AppContext.BaseDirectory, "stuid-id-58e59b5e1155.json");

                        if (!File.Exists(jsonPath))
                        {
                            throw new FileNotFoundException($"Firebase credentials файл не найден: {jsonPath}");
                        }

                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.FromFile(jsonPath),
                            ProjectId = "stuid-id" // Укажите ваш Project ID
                        });

                        _initialized = true;
                    }
                }
            }
        }
    }
}
