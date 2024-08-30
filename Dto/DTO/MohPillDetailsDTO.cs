namespace MrPill.DTOs.DTOs;

public class MohPillDetailsDTO
{
    public string Barcode { get; }
    public string EnglishName { get; }
    public string HebrewName { get; }
    public string? EnglishDescription { get; }
    public string? HebrewDescription { get; }
    public string? ImagePath { get; }
    public int NumberOfPills { get; }

    private MohPillDetailsDTO(Builder builder)
    {
        Barcode = builder.Barcode;
        EnglishName = builder.DrugEnglishName;
        HebrewName = builder.DrugHebrewName;
        EnglishDescription = builder.EnglishDescription;
        HebrewDescription = builder.HebrewDescription;
        ImagePath = builder.ImagePath;
        NumberOfPills = builder.PackageSize;
    }
    public class Builder
    {
        public string Barcode { get; set; } = string.Empty;
        public string DrugEnglishName { get; set; } = string.Empty;
        public string DrugHebrewName { get; set; } = string.Empty;
        public string? EnglishDescription { get; set; }
        public string? HebrewDescription { get; set; }
        public string? ImagePath { get; set; }
         public int PackageSize { get; set;}

        public Builder SetBarcode(string barcode)
        {
            Barcode = barcode;
            return this;
        }

        public Builder SetDrugEnglishName(string name)
        {
            DrugEnglishName = name;
            return this;
        }public Builder SetPackageSize(int size)
        {
            PackageSize = size;
            return this;
        }

        public Builder SetDrugHebrewName(string name)
        {
            DrugHebrewName = name;
            return this;
        }

        public Builder SetEnglishDescription(string? description)
        {
            EnglishDescription = description;
            return this;
        }

        public Builder SetHebrewDescription(string? description)
        {
            HebrewDescription = description;
            return this;
        }

        public Builder SetImagePath(string? imagePath)
        {
            ImagePath = imagePath;
            return this;
        }

        public MohPillDetailsDTO Build()
        {
            return new MohPillDetailsDTO(this);
        }
    }
}