using cst_back.Protos;

namespace cst_back.Models
{
    public class InstanceDetails
    {
        private Instance Instance;
        private Leaderboard? Leaderboard;

        public InstanceDetails(Instance instance, Leaderboard? leaderboard)
        {
            Instance = instance;
            Leaderboard = leaderboard;
        }

        public InstancesDetailsResponse InstancesDetailsResponse()
        {
            InstancesDetailsResponse details = new()
            {
                Id = Instance.Id,
                Civilization = Instance.Civilization ?? "",
                Map = Instance.Map ?? "",
                Goal = Instance.Goal ?? "",
                Creator = Instance.Creator ?? ""
            };

            if (Instance.Mods != null)
            {
                foreach (var mod in Instance.Mods!)
                {
                    details.Mods.Add(mod);
                }
            }

            if (Leaderboard != null)
            {
                details.Leaderboard = new();
                foreach (var result in Leaderboard.Results!)
                {
                    details.Leaderboard.Results.Add(new Protos.Result()
                    {
                        Positon = (int)result.Position!,
                        Username = result.Username,
                        Value = result.Value
                    });
                }
            }

            return details;
        }
    }
}
