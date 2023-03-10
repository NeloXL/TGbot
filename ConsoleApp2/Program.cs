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
                if (update.Message is not { } message)
                    return;
                if (message.Text is not { } messageText)
                    return;
                var chatId = message.Chat.Id;

                if (message.Text == "/start")
                {
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {

                    new []
                    {
                    InlineKeyboardButton.WithCallbackData("Квиз для недовзрослых", "child"),
                    },

                    new []
                    {
                    InlineKeyboardButton.WithCallbackData("Квиз для перемолодых", "adult")
                    },
                    });

                    // отправка сообщения с inline клавиатурой
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Выбери квиз в inline - клавиатуре",
                        replyMarkup: inlineKeyboard
                    );


                    botClient.AnswerCallbackQueryAsync() += (object sender, CallbackQuery e) => {
                        var callbackQuery = e.CallbackQuery;
                        var callbackQueryId = callbackQuery.Id;
                        return Task.CompletedTask;
                    };


                    Start.StartMessage();
                    cts.Cancel();
                }
                else
                {

                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Я тебя не понимаю, напиши '/start'",
                        cancellationToken: cancellationToken);
                }
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
