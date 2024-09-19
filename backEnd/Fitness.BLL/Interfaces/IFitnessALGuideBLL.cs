using Fitness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.BLL.Interfaces
{
    public interface IFitnessALGuideBLL
    {
        public ScreenshotRes Create(ScreenshotInfo screenshotInfo);

        public MessageRes DeleteFitnessSuggestion(int screenshotID);

        public RandomEquiGuidesRes GetRandomEquiGuides();

        public AllEquiGuidesRes GetALLEquiGuides();

        public SingleDetailEqui GetOneEquiGuide(string equipmentName);

        public MessageRes GetAISuggestion(int screenshotID);

        public SuggestionDetailRes GetAllDetails(int userID);

        public FitnessEquipmentRes InsertEquipment(FitnessEquipment fitnessEquipment);

        public MessageRes DeleteFitnessEquipment(string equipmentName);

        public MessageRes UpdateFitnessEquipment(FitnessEquipment fitnessEquipment);

        public IAsyncEnumerable<string> EquipmentMultiAIDialogStream(AIInfo info);
    }
}
