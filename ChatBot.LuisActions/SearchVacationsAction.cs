using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.CognitiveServices.LuisActionBinding;

namespace ChatBot.LuisActions
{
    public class SearchVacationsAction : BaseLuisAction
    {
        // https://github.com/Microsoft/BotBuilder-CognitiveServices/blob/master/CSharp/Samples/LuisActions/LuisActions.Samples.Shared/FindHotelsAction.cs
        public string Category { get; set; }

        [Required(ErrorMessage = "Please provide the check-in date")]
        [LuisActionBindingParam(BuiltinType = BuiltInDatetimeV2Types.Date, Order = 2)]
        public DateTime? Checkin { get; set; }
    }
}
