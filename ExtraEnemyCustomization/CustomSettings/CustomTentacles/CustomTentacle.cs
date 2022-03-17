namespace EEC.CustomSettings.CustomTentacles
{
    public class CustomTentacle
    {
        public string DebugName { get; set; } = string.Empty;
        public int ID { get; set; } = 15;
        public GPUCurvyType BodyPrefab { get; set; } = GPUCurvyType.Striker;
        public GPUCurvyType BodyMaterial { get; set; } = GPUCurvyType.Striker;
        public GPUCurvyType HeadPrefab { get; set; } = GPUCurvyType.Striker;
        public GPUCurvyType HeadMaterial { get; set; } = GPUCurvyType.Striker;
        public GPUCurvyType Shape { get; set; } = GPUCurvyType.Striker;
        public int MaxCount { get; set; } = 50;
    }
}