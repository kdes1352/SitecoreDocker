namespace AllinaHealth.Models.MigrationModels
{
    public class EktronMenu
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string MenuType { get; set; }
        public string ParentID { get; set; }
        public string AncestorID { get; set; }
    }
}
