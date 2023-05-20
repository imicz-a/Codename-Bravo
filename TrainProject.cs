using System;
namespace Train
{
    [Serializable]
    public struct TrainProject
	{
		public float projectVersion { get; set; } = 0;
		public List<string> sourceFiles { get; set; }
		public TrainProject() { }
	}
}

