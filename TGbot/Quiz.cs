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

namespace TGbot
{
    internal class Quiz
    {
        static bool _isChildQuiz;
        static ChatId chatId;
        static List<string> adultQuestions = new List<string>()
        {
            "Взрослый вопрос 1",
            "Взрослый вопрос 2",
            "Взрослый вопрос 3",
            "Взрослый вопрос 4",
            "Взрослый вопрос 5"
        };
        static List<string> childQuestions = new List<string>()
        {
            "Детский вопрос 1",
            "Детский вопрос 2",
            "Детский вопрос 3",
            "Детский вопрос 4",
            "Детский вопрос 5"
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

            if (_isChildQuiz) await botClient.SendTextMessageAsync(chatId, "Вопрос для детского квиза 1:");
            else await botClient.SendTextMessageAsync(chatId, "Вопрос для взрослого квиза 1:");

            Console.ReadLine();

            cts.Cancel();
        }


        async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
                else if (!intAnswers.Contains(update.Message.Text))
                {
                    await botClient.SendTextMessageAsync(chatId, "Неправильный формат ответа");
                }
            }
            questionNumber++;
        }
        
        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
