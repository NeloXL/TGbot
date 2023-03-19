using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGbot
{
    internal class Quiz
    {
        static bool _isChildQuiz;
        static ChatId chatId;
        public static string token = "";

        static List<string> adultThemes = new List<string>()
        {
            "Тема взрослого квиза 1",
            "Тема взрослого квиза 2",
            "Тема взрослого квиза 3",
            "Тема взрослого квиза 4",
            "Тема взрослого квиза 5"
        };
        
        static List<string> childThemes = new List<string>()
        {
            "Тема подросткового квиза 1",
            "Тема подросткового квиза 2",
            "Тема подросткового квиза 3",
            "Тема подросткового квиза 4",
            "Тема подросткового квиза 5"
        };
        static List<string> intAnswers = new List<string>()
        {
            "1",
            "1",
            "1",
            "1",
            "1"
        };
        static List<string> answers = new List<string>();

        static int questionNumber = -1;

        public static async void StartQuiz(bool isChildQuiz, string _token, ChatId _chatId)
        {
            _isChildQuiz = isChildQuiz;
            chatId = _chatId;
            token = _token;

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

            if (_isChildQuiz) await ChildQuizThemes(botClient);
            else await AdultQuizThemes(botClient);

            Console.ReadLine();

            cts.Cancel();

            static async Task AdultQuizThemes(ITelegramBotClient botClient)
            {
                await botClient.SendTextMessageAsync(chatId, "Выберите тему квиза:");
                foreach (string str in adultThemes)
                {
                    var keyboard = new InlineKeyboardMarkup(new[]
                    {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Выбрать", $"theme_{str}")
                    }
                });
                    await botClient.SendTextMessageAsync(chatId, str, replyMarkup: keyboard);
                }
            }

            static async Task ChildQuizThemes(ITelegramBotClient botClient)
            {
                await botClient.SendTextMessageAsync(chatId, "Выберите тему квиза:");
                foreach (string str in childThemes)
                {
                    var keyboard = new InlineKeyboardMarkup(new[]
                    {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Выбрать", $"theme_{str}")
                    }
                });
                    await botClient.SendTextMessageAsync(chatId, str, replyMarkup: keyboard);
                }
            }


            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                if (questionNumber >= 0)
                {
                    if (update.Type == UpdateType.Message && intAnswers.Contains(update.Message.Text))
                    {
                        answers[questionNumber] = $"{update.Message.Text}";
                        if (questionNumber == 4)
                        {
                            foreach (string answer in answers)
                            {
                                Console.WriteLine(answer);
                            }
                        }
                    }
                    else if (update.Type == UpdateType.CallbackQuery)
                    {
                        await HandleCallbackQuery(botClient, update.CallbackQuery);
                    }

                    else if (!intAnswers.Contains(update.Message.Text))
                    {
                        await botClient.SendTextMessageAsync(chatId, "Неправильный формат ответа");
                    }
                }
                questionNumber++;
            }

            async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
            {
                if (callbackQuery.Data.StartsWith("theme"))
                {
                    await StartTheme.ThemeStart(token, callbackQuery.Data.Remove(0, 6));
                    Console.WriteLine(callbackQuery.Data.Remove(0, 6));
                    cts.Cancel();
                }
            }

            static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
