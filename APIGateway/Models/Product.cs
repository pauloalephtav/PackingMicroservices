namespace APIGateway.Models
{
    public class Product
    {
        public Product()
        {

        }

        public Product(string productId, Dimension dimension)
        {
            ProductId = productId;
            Dimension = dimension;
        }

        public string ProductId { get; set; }
        public Dimension Dimension { get; set; }
    }
    public class Dimension
    {
        public Dimension()
        {

        }

        public Dimension(int height, int width, int length)
        {
            Height = height;
            Width = width;
            Length = length;
        }

        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
    }
}
