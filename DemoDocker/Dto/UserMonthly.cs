namespace DemoDocker.Dto
{
	public class UserMonthly
	{
		public int Year { get; set; }
		public int Month { get; set; }
		public int NewUsers { get; set; }
		public int PreviousMonthUsers { get; set; }
		public float GrowthRate { get; set; }
	}
}
