using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rikka.TelegamClasses.Models;
using Rikka.TelegramBotCore;
using Rikka.TelegramBotCore.Models;
using Rikka.Tsab2.Core.Services;
using Rikka.Tsab2.Database.Context.Entities;
using Rikka.Tsab2.Database.Repositories;
using Rikka.Tsab2.Endpoint.App.Filters;
using System;

namespace Rikka.Tsab2.Endpoint.Controllers
{
    [ServiceFilter(typeof(ExceptionFilter))]
    public class TelegramController:Controller
    {
        private readonly IChatRepository _chatRepository;
        private readonly IBotService _botService;
        private readonly IBotApi _botApi;

        public TelegramController(IChatRepository chatRepository,IBotService botService,IBotApi botApi)
        {
            _chatRepository = chatRepository;
            _botService = botService;
            _botApi = botApi;
        }

        public const string NoState = "NoState";

        
        [HttpPost("Telegram{token}")]
        public async Task<bool> Webhook(string token, [FromBody] UpdateModel model)
        {
            try
            {

                MessageFlow response = null;
                if (!string.IsNullOrEmpty(model.Message?.Text) && _botApi.CheckToken(token))
                {
                    var text = model.Message.Text;
                    var msg = model.Message;
                    var chat = await _chatRepository.GetyChatId(model.Message.Chat.Id);
                    if (chat == null)
                    {
                        var type = model.Message.From.Id == model.Message.Chat.Id ? ChatType.Private : ChatType.Group;
                        chat = new Chat(model.Message.Chat.Id, NoState, type);
                        await _chatRepository.Insert(chat);
                    }
                    var state = chat.State;
                    if (string.IsNullOrEmpty(state))
                    {
                        state = NoState;
                        await _chatRepository.SetState(chat.ChatId, state);
                    }
                    if (text != null)
                    {
                        if (text.StartsWith("/"))
                            response = await _botService.Command(text, msg);
                        else
                            response = await _botService.Message(text, state, msg);
                    }
                    await _botService.Send(response, model.Message.Chat.Id);
                }
            }
            catch(Exception exception)
            {

            }
            return true;
        }
    }
}
