namespace SprintPlanner.Core.Logic
{
    public class DataStorageModel
    {
        public DataStorageModel()
        {
            SprintCrud = new SprintCrudStorageModel();
            Capacity = new CapacityStorageModel();
            ServerModel = new ServerStorageModel();
        }

        public SprintCrudStorageModel SprintCrud { get; set; }

        public CapacityStorageModel Capacity { get; set; }

        public ServerStorageModel ServerModel { get; set; }
    }
}
