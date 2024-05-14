using GraphicsCardProject.DAL.DomainClasses;
using System.Text.Json;

namespace GraphicsCardProject.DAL
{
    public class DataUtility
    {
        private readonly AppDbContext _db;

        public DataUtility(AppDbContext context)
        {
            _db = context;
        }

        public async Task<bool> LoadCardInfoFromWebToDb(string stringJson)
        {
            bool brandsLoaded = false;
            bool productsLoaded = false;
            bool categoriesLoaded = false;

            try
            {
                dynamic? objectJson = JsonSerializer.Deserialize<Object>(stringJson);
                brandsLoaded = await LoadBrands(objectJson);
                categoriesLoaded = await LoadCategories(objectJson);
                productsLoaded = await LoadProducts(objectJson);
                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return brandsLoaded && productsLoaded && categoriesLoaded;
        }

        public async Task<bool> LoadBrands(dynamic jsonObjectArray)
        {
            bool loadedBrands = false;

            try
            {
                //clear out old rows
                _db.Brands?.RemoveRange(_db.Brands);
                await _db.SaveChangesAsync();

                List<String> allBrands = new();
                foreach (JsonElement element in jsonObjectArray.EnumerateArray())
                {
                    if (element.TryGetProperty("Brand", out JsonElement prodItemJson))
                    {
                        allBrands.Add(prodItemJson.ToString());
                    }
                }

                IEnumerable<String> brands = allBrands.Distinct<String>();
                foreach (string brandname in brands)
                {
                    Brand br = new();
                    br.BrandName = brandname;
                    await _db.Brands!.AddAsync(br);
                    await _db.SaveChangesAsync();
                }

                loadedBrands = true;
            }
            catch (Exception ex)
            {
                Console.Write("Error - " + ex.Message);
            }

            return loadedBrands;
        }

        public async Task<bool> LoadCategories(dynamic jsonObjectArray)
        {
            bool loadedCategories = false;

            try
            {
                _db.Categories?.RemoveRange(_db.Categories);
                await _db.SaveChangesAsync();

                List<string> allCategories = new();
                foreach (JsonElement element in jsonObjectArray.EnumerateArray())
                {
                    if (element.TryGetProperty("Category", out JsonElement prodItemJson))
                    {
                        allCategories.Add(prodItemJson.ToString());
                    }
                }

                IEnumerable<String> categories = allCategories.Distinct<String>();
                foreach (string category in categories)
                {
                    Category cat = new();
                    cat.CategoryName = category;
                    await _db.Categories!.AddAsync(cat);
                    await _db.SaveChangesAsync();
                }
                loadedCategories = true;
            }
            catch (Exception ex)
            {
                Console.Write("Error - " + ex.Message);
            }

            return loadedCategories;
        }

        public async Task<bool> LoadProducts(dynamic jsonObjectArray)
        {
            bool loadedProducts = false;

            try
            {
                List<Brand> brands = _db.Brands!.ToList();
                List<Category> categories = _db.Categories!.ToList();

                // clear out the old data
                _db.Products?.RemoveRange(_db.Products);
                await _db.SaveChangesAsync();

                foreach (JsonElement element in jsonObjectArray.EnumerateArray())
                {
                    Product item = new ();
                    item.BrandName = element.GetProperty("BrandName").ToString();
                    item.ProductName = element.GetProperty("ProductName").GetString();
                    item.RAM = element.GetProperty("RAM").GetString();
                    item.Description = element.GetProperty("Description").GetString();
                    item.Price = element.GetProperty("Price").GetDouble();
                    item.MSRP = element.GetProperty("MSRP").GetDouble();
                    item.QtyOnHand = element.GetProperty("QtyOnHand").GetInt32();
                    item.QtyOnBackOrder = element.GetProperty("QtyOnBackOrder").GetInt32();
                    item.GraphicName = element.GetProperty("GraphicName").GetString();
                    /** ADDED COLUMNS **/
                    item.Memory = element.GetProperty("Memory").GetString();
                    item.Processor = element.GetProperty("Processor").GetString();
                    item.Graphics = element.GetProperty("Graphics").GetString();
                    item.Storage = element.GetProperty("Storage").GetString();
                    /** ADDED COLUMNS **/

                    string? br = element.GetProperty("Brand").GetString();
                    // add the FK here
                    foreach (Brand brand in brands)
                    {
                        if (brand.BrandName == br)
                        {
                            item.Brand = brand;
                            break;
                        }
                    }

                    string? cat = element.GetProperty("Category").GetString();
                    foreach (Category category in categories)
                    {
                        if (category.CategoryName == cat)
                        {
                            item.Category = category;
                            break;
                        }
                    }

                    await _db.Products!.AddAsync(item);
                    await _db.SaveChangesAsync();
                }

                loadedProducts = true;
            }
            catch (Exception ex)
            {
                Console.Write("Error - " + ex.Message);
            }

            return loadedProducts;
        }
    }
}
