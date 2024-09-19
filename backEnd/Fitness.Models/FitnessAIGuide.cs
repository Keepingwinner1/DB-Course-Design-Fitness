using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Models
{
    public class FitnessSuggestion
    {
        public int screenshotID { get; set; }

        public int userID { get; set; }

        public string createTime { get; set; }

        public string exerciseName { get; set; }

        public string? suggestions { get; set; }

        public string screenshotUrl { get; set; }
    }

    public class FitnessEquiOperation
    {
        public string equipmentName { get; set; }

        public string equipmentGuide { get; set; }

        public string createTime { get; set; }

        public string lastUpdateTime { get; set; }

        public float? userRatings { get; set; }
    }
    public class CreateScreenshot
    {
        public string exerciseName { get; set; }

        public string screenshotUrl { get; set; }

    }
    public class ScreenshotInfo
    {
        public int userID { get; set; }

        public string exerciseName { get; set; }

        public string screenshotUrl { get; set; }
    }

    public class ScreenshotRes
    {
        public string message { get; set; }

        public int screenshotID { get; set; }

        public DateTime createTime { get; set; }

        public string screenshotUrl { get; set; }
    }

    public class SingleDetailSuggestionRes
    {
        public int screenshotID { get; set; }

        public DateTime createTime { get; set; }

        public string exerciseName { get; set; }

        public string suggestions { get; set; }

        public string screenshotUrl { get; set; }
    }

    public class SingleNoDetailSuggestion
    {
        public int screenshotID { get; set; }

        public DateTime createTime { get; set; }

        public string exerciseName { get; set;}
    }

    public class SuggestionNoDetailRes
    {

        public List<SingleNoDetailSuggestion> suggestions { get; set; } = new();
    
    }

    public class SuggestionDetailRes
    {

        public List<SingleDetailSuggestionRes> suggestions { get; set; } = new();

    }

    public class SingleEquiGuideNoDetail
    {
        public string equipmentName { get; set; }

        public string imgUrl { get; set; }

        public string briefIntr { get; set; }

    }

    public class RandomEquiGuidesRes
    {
        public List<SingleEquiGuideNoDetail> guides { get; set; } = new();
    }

    public class SingleDetailEqui
    {
        public string equipmentName { get; set; }

        public string operationGuide { get; set; }

        public string imgUrl { get; set; }

    }

    public class FitnessEquipment
    {
        public string equipmentName { get; set;}

        public string imgUrl { get; set; }

        public string briefIntr { get; set;}

        public string operationGuide { get; set; }
    }

    public class FitnessEquipmentRes
    {
        public string message { get; set; }

        public DateTime lastUpdateTime { get; set; }
    }

    public class FitnessEquipmentWithTime
    {
        public string equipmentName { get; set; }

        public string imgUrl { get; set; }

        public string briefIntr { get; set; }

        public string operationGuide { get; set; }

        public DateTime lastUpdateTime { get; set; }
    }

    public class AllEquiGuidesRes
    {
        public List<FitnessEquipmentWithTime> guides { get; set; } = new();
    }


    public class AIMessage
    {
        public string role { get; set; }

        public string content { get; set; }
    }


    public class AIInfo
    {
        public string equipmentName { get; set; }

        public List<AIMessage> messages { get; set; }
    }

    public class AIRes
    {
        public string assistant_output { get; set; }
    }


}
