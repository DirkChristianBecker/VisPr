namespace VisPrCore.Datamodel.Responses.Object
{
    public class ListObjectObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
    public class ListObjectsResponse
    {
        public List<ListObjectObject>? Objects { get; set; }

        public ListObjectsResponse(List<ListObjectObject>? objects)
        {
            Objects = objects;
        }
    }
}
