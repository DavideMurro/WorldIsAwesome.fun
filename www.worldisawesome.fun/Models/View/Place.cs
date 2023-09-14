namespace www.worldisawesome.fun.ViewModels
{
    public partial class Place
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string PictureId { get; set; }
        public bool? IsMine { get; set; }
    }
}
