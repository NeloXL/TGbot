using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections;

namespace ConsoleApp2
{
    internal class Program
    {
        public static string _token = "5659281240:AAHXUxFnZqKYqwWB_p7-8Al299w5N3vx6rc";

        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient(_token);

            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                if (update.Type == UpdateType.Message) await HandleMessage(botClient, update.Message);
            }

            async Task HandleMessage(ITelegramBotClient botClient, Message message)
            {
                ChatId chatId = message.Chat.Id;

                switch (message.Text)
                {
                    case "/start":
                        await StartMessage(botClient, message, chatId);
                        break;
                    default:
                        await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Я вас не понимаю. Напишите /start");
                        break;
                }
            }

            async Task StartMessage(ITelegramBotClient botClient, Message message, ChatId chatId)
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[]
                        {
                            new KeyboardButton("Квиз для взрослых")
                        },
                        new KeyboardButton[]
                        {
                            new KeyboardButton("Квиз для подростков")
                        }
                    });
                keyboard.ResizeKeyboard = true;
                keyboard.OneTimeKeyboard = true;

                await botClient.SendTextMessageAsync(chatId: chatId, text: "Выберите категорию квиза:", replyMarkup: keyboard);
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }
        }
    }
}
