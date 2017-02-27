using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rikka.TelegamClasses.Models;
using Rikka.TelegramBotCore;
using Rikka.TelegramBotCore.Models;
using Rikka.Tsab2.Core.Services;
using Rikka.Tsab2.Database.Repositories;

namespace Rikka.Tsab2.Core.BotActions
{
    public class SearchAction : IBotAction
    {
        private readonly IChatRepository _chatRepository;
        private readonly ISearchService _searchService;
        private readonly ISearchEngineRepository _searchEngineRepository;
        private readonly IBotApi _botApi;

        public SearchAction(IChatRepository chatRepository, ISearchService searchService, ISearchEngineRepository searchEngineRepository, IBotApi botApi)
        {
            _chatRepository = chatRepository;
            _searchService = searchService;
            _searchEngineRepository = searchEngineRepository;
            _botApi = botApi;
            _tasks = new List<Task>();
        }
        public async Task<MessageFlow> Command(string command, TelegamClasses.Models.MessageModel message)
        {
            var tags = new[] { "#opm", "#one_punch_man", "#onepunchman", "#saitama", "#genos" };
            var keyboad = tags.Select(tag => new KeyboardButtonModel() { Text = tag });
            var reply = new ReplyKeyboardMarkupModel(keyboad);
            await _chatRepository.SetState(message.Chat.Id, "search-choose-tag");
            return new MessageFlow(new SendMessageModel(message.Chat.Id, "����� ������, ������ ����� �� ������ ����?", reply));
        }

        public async Task<MessageFlow> Message(string text, string state, TelegamClasses.Models.MessageModel message)
        {
            switch (state)
            {
                case "search-choose-tag":
                    return await _chooseTag(text, message);
                case "search-pick-results":                    
                    return await _steps(text,message);
                default:
                    throw new ArgumentException(nameof(state));
            }
        }

        private List<Task> _tasks;

        private async Task<MessageFlow> _steps(string text, MessageModel message)
        {
            if (!_searchService.Results.ContainsKey(message.Chat.Id))
            {
                await _chatRepository.SetState(message.Chat.Id, "NoState");
                return new MessageFlow(MessageFlowItem.GetMessage("� �� ������ ��� ���������� �����? ���� ���, ������ /search ��� �����..."));
            }
            var show = new[] { "������", "����", "��������", "����", "�������� ����", "�������� ����", "��������" };
            var post = new[] { "��������", "��������", "� �������", "��", "� ��", "������", "� ������", "������������ ����", "������������" };
            var next = new[] { "������", "�����", "���", "���������", "���������", "�������� ���������" };
            var current = _searchService.Results[message.Chat.Id].Current;
            if (current == null)
            {
                await _chatRepository.SetState(message.Chat.Id, "NoState");
                return new MessageFlow(MessageFlowItem.GetMessage("�� ���� ���"));
            }
            if (show.Any(a => a == text))
            {
                return new MessageFlow()
                {
                    MessageFlowItem.GetMessage(current.ItemUrl),
                    MessageFlowItem.GetMessage(current.Description)
                };
            } else if (post.Any(a => a == text))
            {
                throw new NotImplementedException();
            } else if (next.Any(a => a == text))
            {
                return await _pickResults(message);
            }
            else
            {
                return new MessageFlow(MessageFlowItem.GetMessage("���-�� � ���� �� �����"));
            }
        }
        private async Task<MessageFlow> _pickResults(MessageModel message)
        {                           
            var search = _searchService.Results[message.Chat.Id];

            var item = search.Next();
            if (item == null)
            {                
                await _chatRepository.SetState(message.Chat.Id, "NoState");
                return new MessageFlow(MessageFlowItem.GetMessage("��� ���! ������������� ���. ������ ���� ��������"));
            }
            var image = await _searchService.Download(item.ImageUrl);
            var markup = _getMarkup();
            return new MessageFlow()
            {
                new MessageFlowItem(new SendPhotoModel(message.Chat.Id, image, reply: markup)),
                MessageFlowItem.GetMessage($"������� � '{item.Engine}', ������� {item.Score}")

            };
        }

        private async Task<MessageFlow> _chooseTag(string tag, MessageModel message)
        {
            if (tag.StartsWith("#"))
                tag = tag.Substring(1);
            if (tag.Contains(" "))
            {
                return new MessageFlow(MessageFlowItem.GetMessage("���-�� �� ������ �� ���..."));
            }
            await _chatRepository.SetState(message.Chat.Id, "NoState");            

            var task = _search(message, tag);
            _tasks.Add(task);

            var reply = new ReplyKeyboardHideModel() {HideKeyboard = true};
            return new MessageFlow()
            {
                new MessageFlowItem(new SendMessageModel(message.Chat.Id, "������� ��������� �����...",replyMarkup:reply)),
                MessageFlowItem.GetSticker("BQADBAADTAUAAqKYZgABfaNgr6BIuFIC")
            };
        }

        private async Task _search(MessageModel message, string tag)
        {
            var search = await _searchService.Search(message.Chat.Id,tag, 20);
            var first = search.Next();
            if (first==null)
            {
                await _botApi.BotMethod(new SendMessageModel(message.Chat.Id, "������ �� ������ ������� ���� ������..."));
                return;
            }
            await _botApi.BotMethod(new SendMessageModel(message.Chat.Id, "����, ��� ��� ��� ������� ����� �� ������� #" + tag));
            Thread.Sleep(300);
            var image = await _searchService.Download(first.ImageUrl);
            var markup = _getMarkup();
            await _chatRepository.SetState(message.Chat.Id, "NoState");
            await _botApi.BotMethod(new SendPhotoModel(message.Chat.Id,image,reply: markup));            
        }

        private ReplyKeyboardMarkupModel _getMarkup()
        {
            var commands = new[] { "������", "��������", "/cansel" };
            return new ReplyKeyboardMarkupModel()
            {
                Keyboard = commands.Select(s => new[] { new KeyboardButtonModel() { Text = s } }).ToArray()
            };
        }

        public bool Private { get; } = false;
        public bool Chat { get; } = true;
        public string[] States => new [] { "search-choose-tag", "search-pick-results" };
        public string CommandName { get; } = "/search";
        public string Description { get; } = "����� ��������";
    }
}