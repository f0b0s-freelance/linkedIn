namespace LinkedInApplication.Core
{
    public class FacetItemInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
