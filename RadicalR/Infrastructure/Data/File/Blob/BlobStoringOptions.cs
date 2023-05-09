namespace RadicalR
{
    public class BlobStoringOptions
    {
        public BlobContainerConfigurations Containers { get; }

        public BlobStoringOptions()
        {
            Containers = new BlobContainerConfigurations();
        }
    }
}