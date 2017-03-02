using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rikka.TelegamClasses.Models;

namespace Rikka.Tsab2.Core.Services
{
    public interface IBotHelper
    {
        ReplyKeyboardMarkupModel SingleReply(string[] commands);
        ReplyKeyboardHideModel Hide();
    }

    public class BotHelper : IBotHelper
    {
        public ReplyKeyboardMarkupModel SingleReply(string[] commands)
        {
            return new ReplyKeyboardMarkupModel()
            {
                Keyboard = commands.Select(s => new[] { new KeyboardButtonModel() { Text = s } }).ToArray()
            };
        }
        public ReplyKeyboardHideModel Hide()
        {
            return new ReplyKeyboardHideModel() {HideKeyboard = true};
        }
    }

    public class TextCheckerService
    {
        
    }
    public class StickersExplorerService
    {
        
    }
}
